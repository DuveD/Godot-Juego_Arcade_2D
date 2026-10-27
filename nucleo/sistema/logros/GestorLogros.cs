using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.logros;

public static class GestorLogros
{
    // Formato de fecha de la versión 1 del perfil (texto en hora local). Solo se usa para migrar.
    private const string FORMATO_FECHA_VERSION_1 = "yyyy-MM-dd HH:mm:ss";

    private const string SECCION_LOGROS = "logros";

    public static void InicializarLogros(Perfil perfil)
    {
        CargarLogros(perfil, null);
    }

    public static void CargarLogros(Perfil perfil, ConfigFile archivoPerfil)
    {
        List<Logro> logros = DefinicionLogros.ObtenerLogros().ToList();

        foreach (Logro logro in logros)
        {
            Godot.Collections.Dictionary datosLogro = archivoPerfil != null ?
                (Godot.Collections.Dictionary)archivoPerfil.GetValue(SECCION_LOGROS, logro.Id,
                    new Godot.Collections.Dictionary()) : null;

            // Si hay datos guardados, los aplicamos.
            if (datosLogro != null && datosLogro.Count > 0)
                InformarDatosLogro(logro, datosLogro);

            perfil.AnadirOActualizarLogro(logro);
        }

        LoggerJuego.Trace($"Logros del perfil '{perfil.Id}' cargados.");
    }

    private static void InformarDatosLogro(Logro logro, Godot.Collections.Dictionary datosLogro)
    {
        logro.Desbloqueado = (bool)datosLogro.GetValueOrDefault("desbloqueado", false);

        string fechaDesbloqueadoTexto = (string)datosLogro.GetValueOrDefault("fecha_desbloqueado", "");
        logro.FechaDesbloqueado = fechaDesbloqueadoTexto.DesdeTextoIso8601();

        if (logro is LogroContador logroContador)
            logroContador.Progreso = (int)datosLogro.GetValueOrDefault("progreso", 0);
    }

    public static bool EmitirEvento(Perfil perfil, string evento, object datos = null)
    {
        if (perfil?.Logros == null || string.IsNullOrWhiteSpace(evento))
            return false;

        List<Logro> logrosDesbloqueados = [];

        // Los logros viven en el perfil recibido, así que el evento solo afecta a ese perfil.
        List<Logro> logrosEvento = perfil.Logros.Where(l => l.Evento == evento).ToList();

        foreach (var logro in logrosEvento)
        {
            bool desbloqueado = logro.ProcesarEvento(evento, datos);

            if (desbloqueado)
                logrosDesbloqueados.Add(logro);
        }

        if (logrosDesbloqueados.Any())
        {
            LogrosDesbloqueados?.Invoke(logrosDesbloqueados);
            return true;
        }

        return false;
    }

    public static event Action<List<Logro>> LogrosDesbloqueados;

    public static void GuardarLogros(Perfil perfil, ConfigFile archivoPerfil)
    {
        IReadOnlyList<Logro> logrosPerfil = perfil.Logros;
        foreach (var logro in logrosPerfil)
        {
            GuardarLogro(logro, archivoPerfil);
        }
    }

    private static void GuardarLogro(Logro logro, ConfigFile archivoPerfil)
    {
        Godot.Collections.Dictionary datosLogro = [];

        datosLogro.Add("desbloqueado", logro.Desbloqueado);

        datosLogro.Add("fecha_desbloqueado", logro.FechaDesbloqueado.ATextoIso8601Utc() ?? "");

        if (logro is LogroContador logroContador)
            datosLogro.Add("progreso", logroContador.Progreso);

        archivoPerfil.SetValue(SECCION_LOGROS, logro.Id, datosLogro);
    }

    /// <summary>
    /// Migración del perfil a la versión 2: las fechas de desbloqueo pasan de texto en hora local a ISO 8601 en UTC.
    /// </summary>
    internal static void MigrarFechasAVersion2(ConfigFile archivoPerfil)
    {
        if (!archivoPerfil.HasSection(SECCION_LOGROS))
            return;

        foreach (string idLogro in archivoPerfil.GetSectionKeys(SECCION_LOGROS))
        {
            var datosLogro = (Godot.Collections.Dictionary)archivoPerfil.GetValue(SECCION_LOGROS, idLogro);

            string fechaVersion1 = (string)datosLogro.GetValueOrDefault("fecha_desbloqueado", "");
            DateTime? fecha = fechaVersion1.StringToDateTime(FORMATO_FECHA_VERSION_1);

            // La fecha antigua estaba en hora local, sin indicar zona horaria.
            DateTime? fechaLocal = fecha.HasValue ? DateTime.SpecifyKind(fecha.Value, DateTimeKind.Local) : null;
            datosLogro["fecha_desbloqueado"] = fechaLocal.ATextoIso8601Utc() ?? "";

            archivoPerfil.SetValue(SECCION_LOGROS, idLogro, datosLogro);
        }
    }
}
