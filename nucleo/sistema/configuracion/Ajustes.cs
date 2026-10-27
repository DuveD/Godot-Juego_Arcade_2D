using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.nucleo.sistema.configuracion;

public static class Ajustes
{
    private static ConfigFile ArchivoAjustes { get; } = new ConfigFile();

    private const int VERSION_AJUSTES = 1;

    // ================= SECCIONES =================

    public const string SECCION_GENERAL = "general";
    public const string SECCION_SONIDO = "sonido";
    public const string SECCION_INTERFAZ = "interfaz";
    public const string SECCION_DESARROLLO = "desarrollo";
    public const string SECCION_PERFILES = "perfiles";

    // ================= INTERNOS =================

    public static string NombreJuego { get; } = (string)ProjectSettings.GetSetting("application/config/name");

    /// <summary>
    /// Carpeta base de los datos del juego (ajustes, perfiles y logs).
    /// En Windows, en Documentos, para que el jugador la encuentre fácilmente; en el resto de plataformas, en user://.
    /// </summary>
    public static string RutaJuego { get; } = OS.GetName() == "Windows"
        ? $"{OS.GetSystemDir(OS.SystemDir.Documents)}/{NombreJuego}"
        : ProjectSettings.GlobalizePath("user://");

    public static string RutaLogs { get; } = $"{RutaJuego}/logs";

    public static string NombreArchivoAjustes { get; } = "ajustes.cfg";
    public static string RutaArchivoAjustes { get; } = $"{RutaJuego}/{NombreArchivoAjustes}";

    public static string RutaPerfiles { get; } = $"{RutaJuego}/perfiles";

    public static string Version { get; } = (string)ProjectSettings.GetSetting("application/config/version");

    // ================= GENERAL =================

    public static int VersionArchivoAjustes
    {
        get => (int)ArchivoAjustes.GetValue(SECCION_GENERAL, "version_archivo", -1);
        set => GuardarValor(SECCION_GENERAL, "version_archivo", value);
    }

    // ================= SONIDO =================
    public static int VolumenGeneral
    {
        get => (int)ArchivoAjustes.GetValue(SECCION_SONIDO, "volumen_general", 100);
        set => GuardarValor(SECCION_SONIDO, "volumen_general", value);
    }

    public static int VolumenMusica
    {
        get => (int)ArchivoAjustes.GetValue(SECCION_SONIDO, "volumen_musica", 100);
        set => GuardarValor(SECCION_SONIDO, "volumen_musica", value);
    }

    public static int VolumenSonidos
    {
        get => (int)ArchivoAjustes.GetValue(SECCION_SONIDO, "volumen_sonidos", 100);
        set => GuardarValor(SECCION_SONIDO, "volumen_sonidos", value);
    }

    // ================= INTERFAZ =================

    public static Idioma Idioma
    {
        get
        {
            string codigoIdioma = (string)ArchivoAjustes.GetValue(SECCION_INTERFAZ, "idioma", Idioma.ES.Codigo);
            var idioma = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
            return idioma ?? Idioma.ES;
        }

        set
        {
            string codigoIdioma = value.Codigo;
            GuardarValor(SECCION_INTERFAZ, "idioma", codigoIdioma);
        }
    }

    // ================= DESARROLLO =================

    public static NivelLog NivelLog
    {
        get
        {
            string nivelLogStr = (string)ArchivoAjustes.GetValue(SECCION_DESARROLLO, "nivel_log", NivelLog.Info.ToString());
            if (Enum.TryParse(typeof(NivelLog), nivelLogStr, ignoreCase: true, out var enumRes))
                return (NivelLog)enumRes;
            else
                return NivelLog.Info;
        }

        set
        {
            string nivelLogStr = value.ToString().ToLower();
            GuardarValor(SECCION_DESARROLLO, "nivel_log", nivelLogStr);
        }
    }

    public static bool EscribirLogEnFichero
    {
        get => (bool)ArchivoAjustes.GetValue(SECCION_DESARROLLO, "escribir_log_en_fichero", false);
        set => GuardarValor(SECCION_DESARROLLO, "escribir_log_en_fichero", value);
    }

    public static bool VerColisiones
    {
        get => (bool)ArchivoAjustes.GetValue(SECCION_DESARROLLO, "ver_colisiones", false);
        set => GuardarValor(SECCION_DESARROLLO, "ver_colisiones", value);
    }

    // ================= PERFILES =================

    public static string IdPerfilActivo
    {
        get => (string)ArchivoAjustes.GetValue(SECCION_PERFILES, "id_perfil_activo", "");
        set => GuardarValor(SECCION_PERFILES, "id_perfil_activo", value);
    }

    public static string IdPerfilSlot1
    {
        get => (string)ArchivoAjustes.GetValue(SECCION_PERFILES, "id_perfil_slot_1", "");
        set => GuardarValor(SECCION_PERFILES, "id_perfil_slot_1", value);
    }

    public static string IdPerfilSlot2
    {
        get => (string)ArchivoAjustes.GetValue(SECCION_PERFILES, "id_perfil_slot_2", "");
        set => GuardarValor(SECCION_PERFILES, "id_perfil_slot_2", value);
    }

    public static string IdPerfilSlot3
    {
        get => (string)ArchivoAjustes.GetValue(SECCION_PERFILES, "id_perfil_slot_3", "");
        set => GuardarValor(SECCION_PERFILES, "id_perfil_slot_3", value);
    }

    public static void SetIdPerfil(int numeroSlot, string value)
    {
        switch (numeroSlot)
        {
            case 1: IdPerfilSlot1 = value; break;
            case 2: IdPerfilSlot2 = value; break;
            case 3: IdPerfilSlot3 = value; break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public static string GetIdPerfil(int numeroSlot)
    {
        return numeroSlot switch
        {
            1 => IdPerfilSlot1,
            2 => IdPerfilSlot2,
            3 => IdPerfilSlot3,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    // ================= CARGA Y GUARDADO =================

    public static bool GuardarAjustesAlGuardarPropiedad = true;

    public static void CargarAjustes()
    {
        if (!FileAccess.FileExists(RutaArchivoAjustes))
        {
            GuardarAjustesAlGuardarPropiedad = false;
            InicializarValoresPorDefecto();
            GuardarAjustesAlGuardarPropiedad = true;

            Guardar();

            LoggerJuego.Info($"Creado archivo '{NombreArchivoAjustes}' con la configuración por defecto.");
        }
        else
        {
            Cargar();

            if (VersionArchivoAjustes < VERSION_AJUSTES)
                MigrarArchivoAjustes();

            LoggerJuego.Info("Ajustes cargados.");
        }
    }
    private static void InicializarValoresPorDefecto()
    {
        // GENERAL
        VersionArchivoAjustes = VERSION_AJUSTES;

        // SONIDO
        VolumenGeneral = 100;
        VolumenMusica = 100;
        VolumenSonidos = 100;

        // INTERFAZ
        Idioma = Idioma.ES;

        // DESARROLLO
        NivelLog = NivelLog.Trace;
        EscribirLogEnFichero = false;
        VerColisiones = false;

        // PERFILES
        IdPerfilActivo = null;
        IdPerfilSlot1 = null;
        IdPerfilSlot2 = null;
        IdPerfilSlot3 = null;
    }

    private static void MigrarArchivoAjustes()
    {
        GuardarAjustesAlGuardarPropiedad = false;

        if (VersionArchivoAjustes < 2)
            MigrarArchivoAjustesVersion2();

        VersionArchivoAjustes = VERSION_AJUSTES;

        GuardarAjustesAlGuardarPropiedad = true;

        Guardar();
    }

    private static void MigrarArchivoAjustesVersion2()
    {
        // GENERAL
        VersionArchivoAjustes = 2;

        // Añadir los nuevos valores aquí.

        LoggerJuego.Info("Archivo de ajustes migrado a la versión 2.");
    }

    /// <summary>
    /// Aplica cambios de ajustes solo en memoria, sin escribir en disco. Después hay que llamar a Guardar().
    /// </summary>
    public static void CambiarSinGuardar(Action cambios)
    {
        GuardarAjustesAlGuardarPropiedad = false;
        try
        {
            cambios();
        }
        finally
        {
            GuardarAjustesAlGuardarPropiedad = true;
        }
    }

    public static void GuardarValor(string seccion, string clave, Variant valor)
    {
        ArchivoAjustes.SetValue(seccion, clave, valor);
        if (GuardarAjustesAlGuardarPropiedad)
            Guardar(clave);
    }

    private static void Cargar()
    {
        if (!FileAccess.FileExists(RutaArchivoAjustes))
        {
            LoggerJuego.EscribirLog("ERROR", $"No existe el archivo de ajustes: {RutaArchivoAjustes}", null, "red");
            return;
        }

        var error = ArchivoAjustes.Load(RutaArchivoAjustes);
        if (error != Godot.Error.Ok)
            LoggerJuego.EscribirLog("ERROR", $"No se pudo cargar el archivo de ajustes: {error}", null, "red");
    }


    public static void Guardar(String clave = null)
    {
        // Crea la carpeta si no existe (si ya existe, no hace nada).
        DirAccess.MakeDirRecursiveAbsolute(RutaJuego);

        var error = ArchivoAjustes.Save(RutaArchivoAjustes);
        if (error != Godot.Error.Ok)
            LoggerJuego.EscribirLog("ERROR", $"No se pudo guardar el archivo de ajustes: {error}", null, "red");

        if (clave == null)
            LoggerJuego.Info("Ajustes guardados.");
        else
            LoggerJuego.Info($"Ajuste '{clave}' guardado.");
    }
}
