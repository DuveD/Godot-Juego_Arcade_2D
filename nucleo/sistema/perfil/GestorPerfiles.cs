
using System;
using System.IO;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.perfil;

public static class GestorPerfiles
{
    private const int VERSION_PERFIL = 1;

    private const string SECCION_DATOS_PERFIL = "datos_perfil";

    public static string GenerarIdPerfil()
    {
        return "perfil_" + Guid.NewGuid().ToString("N")[..8];
    }

    private static bool IdPerfilValido(string idPerfil)
    {
        return !string.IsNullOrWhiteSpace(idPerfil)
            && idPerfil.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }

    public static void InicializarPerfil(Perfil perfil)
    {
        if (perfil == null)
            return;

        // Inicializamos los logros del perfil.
        GestorLogros.InicializarLogros(perfil);
    }

    public static Perfil CargarPerfil(string idPerfil)
    {
        if (!IdPerfilValido(idPerfil))
            return null;

        ConfigFile archivoPerfil = CargarArchivoPerfil(idPerfil);
        if (archivoPerfil == null)
            return null;

        // Cargamos la informacion básica perfil.
        Perfil perfil = CargarInformacionBasica(idPerfil, archivoPerfil);

        // Cargamos las estadísticas globales del perfil.
        GestorEstadisticas.CargarGlobales(perfil, archivoPerfil);

        // Cargamos los logros del perfil.
        GestorLogros.CargarLogros(perfil, archivoPerfil);

        LoggerJuego.Trace($"Perfil '{perfil.Nombre}' ({perfil.Id}) cargado correctamente.");

        return perfil;
    }

    private static Perfil CargarInformacionBasica(string idPerfil, ConfigFile archivoPerfil)
    {
        int version = (int)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "version", 1);
        if (version < VERSION_PERFIL)
            MigrarArchivoPerfil(version, archivoPerfil);

        string nombre = (string)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "nombre", "");

        long fechaCreacionTicks = (long)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "fecha_creacion", DateTime.Now.Ticks);
        DateTime fechaCreacion = new(fechaCreacionTicks);

        long fechaUltimaPartidaTicks = (long)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "fecha_ultima_partida", -1);
        DateTime? fechaUltimaPartida = (fechaUltimaPartidaTicks > 0) ? new(fechaUltimaPartidaTicks) : null;

        Perfil perfil = new(idPerfil, nombre, fechaCreacion, fechaUltimaPartida);
        return perfil;
    }

    private static void MigrarArchivoPerfil(int version, ConfigFile archivoPerfil)
    {
        if (version < 2)
            MigrarArchivoPerfilVersion2(archivoPerfil);
    }

    private static void MigrarArchivoPerfilVersion2(ConfigFile archivoPerfil)
    {
        archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "version", 2);

        // Añadir los nuevos valores aquí.

        LoggerJuego.Info("Archivo de perfil migrado a la versión 2.");
    }

    public static void GuardarPerfil(Perfil perfil)
    {
        if (perfil == null)
            return;

        GuardarArchivoPerfil(perfil.Id, CrearArchivoPerfil(perfil));
    }

    /// <summary>
    /// Crea un archivo con una copia de los datos del perfil. Se debe llamar desde el hilo principal
    /// para que el perfil no cambie mientras se copia; el archivo resultante se puede guardar en otro hilo.
    /// </summary>
    public static ConfigFile CrearArchivoPerfil(Perfil perfil)
    {
        ConfigFile archivoPerfil = new();

        // Guardamos la informacion básica perfil.
        GuardarInformacionBasica(perfil, archivoPerfil);

        // Guardamos las estadísticas globales del perfil.
        GestorEstadisticas.GuardarGlobales(perfil, archivoPerfil);

        // Guardamos los logros del perfil.
        GestorLogros.GuardarLogros(perfil, archivoPerfil);

        return archivoPerfil;
    }

    private static void GuardarInformacionBasica(Perfil perfil, ConfigFile archivoPerfil)
    {
        archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "version", VERSION_PERFIL);

        archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "nombre", perfil.Nombre);

        archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "fecha_creacion", perfil.FechaCreacion.Ticks);

        long fechaUltimaPartidaTicks = perfil.FechaUltimaPartida?.Ticks ?? -1L;
        archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "fecha_ultima_partida", fechaUltimaPartidaTicks);
    }

    private static ConfigFile CargarArchivoPerfil(string idPerfil)
    {
        if (!IdPerfilValido(idPerfil))
        {
            LoggerJuego.Error($"El id del perfil no es válido: '{idPerfil}'.");
            return null;
        }

        string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, idPerfil + ".save");
        if (!File.Exists(rutaArchivoPerfil))
        {
            LoggerJuego.Warn($"No se ha encontrado el archivo del perfil '{idPerfil}'.");
            return null;
        }

        ConfigFile archivoPerfil = new();

        var error = archivoPerfil.Load(rutaArchivoPerfil);
        if (error != Error.Ok)
        {
            LoggerJuego.Error($"No se ha podido cargar el perfil '{idPerfil}': {error}");
            return null;
        }

        return archivoPerfil;
    }

    public static bool GuardarArchivoPerfil(string idPerfil, ConfigFile archivoPerfil)
    {
        if (!IdPerfilValido(idPerfil))
        {
            LoggerJuego.Error($"El id del perfil no es válido: '{idPerfil}'.");
            return false;
        }

        string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, idPerfil);
        string rutaArchivoPerfilTemporal = rutaArchivoPerfil + ".tmp";
        string rutaArchivoPerfilSave = rutaArchivoPerfil + ".save";

        if (!Directory.Exists(Ajustes.RutaPerfiles))
            Directory.CreateDirectory(Ajustes.RutaPerfiles);

        var error = archivoPerfil.Save(rutaArchivoPerfilTemporal);
        if (error != Error.Ok)
        {
            LoggerJuego.Error($"No se ha podido guardar el perfil '{idPerfil}': {error}");
            return false;
        }

        try
        {
            File.Move(rutaArchivoPerfilTemporal, rutaArchivoPerfilSave, true);
            LoggerJuego.Info($"Perfil '{idPerfil}' guardado correctamente.");
            return true;
        }
        catch (Exception e)
        {
            LoggerJuego.Error($"Error al guardar el perfil '{idPerfil}': {e.Message}");
            return false;
        }
    }

    public static bool EliminarPerfil(Perfil perfil)
    {
        if (perfil == null)
            return false;

        return EliminarArchivoPerfil(perfil.Id);
    }

    private static bool EliminarArchivoPerfil(string idPerfil)
    {
        if (!IdPerfilValido(idPerfil))
        {
            LoggerJuego.Error($"El id del perfil no es válido: '{idPerfil}'.");
            return false;
        }

        string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, idPerfil + ".save");
        if (!File.Exists(rutaArchivoPerfil))
        {
            LoggerJuego.Warn($"No se puede eliminar el perfil '{idPerfil}': archivo no encontrado.");
            return false;
        }

        try
        {
            File.Delete(rutaArchivoPerfil);
            LoggerJuego.Info($"Perfil '{idPerfil}' eliminado correctamente.");
            return true;
        }
        catch (Exception e)
        {
            LoggerJuego.Error($"Error al eliminar el perfil '{idPerfil}': {e.Message}");
            return false;
        }
    }
}
