using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;

namespace Primerjuego2D.nucleo.utilidades.log;

public static class LoggerJuego
{
    public enum NivelLog : long
    {
        Trace = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        None = 4
    }

    public const string FORMATO_FECHA_LOG = "yyyy-MM-dd HH:mm:ss";


    public static bool EscribirLogEnFichero = Ajustes.EscribirLogEnFichero;

    private const int ContextWidth = 60;

    private static readonly string PathLog;

    private static readonly object _bloqueoFichero = new();

    public static NivelLog NivelLogJuego { get; set; } = Ajustes.NivelLog;

    static LoggerJuego()
    {
        string pathCarpetaLogs = Ajustes.RutaLogs;
        if (!Directory.Exists(pathCarpetaLogs))
            Directory.CreateDirectory(pathCarpetaLogs);

        PathLog = Path.Combine(pathCarpetaLogs, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        Info("Log iniciado.", "Logger");
    }

    public static void Trace(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Trace) return;
        EscribirLog("TRAZA", message, context, "gray");
    }

    public static void Info(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Info) return;
        EscribirLog("INFO", message, context);
    }

    public static void Warn(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Warning) return;
        EscribirLog("WARN", message, context, "yellow");
    }

    public static void Error(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Error) return;
        EscribirLog("ERROR", message, context, "red");
    }

    // ================== Internals ==================

    private static NivelLog ObtenerNivelLog()
    {
        var frame = new StackTrace().GetFrame(2);
        var metodo = frame?.GetMethod();
        Type tipoLlamador = metodo?.DeclaringType;

        return ObtenerNivelLog(tipoLlamador);
    }

    private static NivelLog ObtenerNivelLog(Type tipoLlamador)
    {
        var atributoLogLevel = tipoLlamador?.GetCustomAttributes(typeof(AtributoNivelLog), inherit: true).FirstOrDefault() as AtributoNivelLog;
        return atributoLogLevel?.NivelLog ?? NivelLogJuego; // si no tiene atributo, usa NivelLog global
    }

    private static string ObtenerContexto()
    {
        var stack = new StackTrace(true);
        MethodBase metodo = null;
        Type tipoClase = null;
        int linea = 0;

        foreach (var frame in stack.GetFrames())
        {
            var frameMethod = frame.GetMethod();
            if (frameMethod == null) continue;

            var tipo = frameMethod.DeclaringType;
            if (tipo == null) continue;

            string nombreMetodo = frameMethod.Name;

            // Ignoramos LoggerJuego y métodos generados por async/await
            if (tipo != typeof(LoggerJuego) &&
                !nombreMetodo.Contains(">d__") &&
                !nombreMetodo.Contains("MoveNext") &&
                !nombreMetodo.Contains("b__") &&              // ignora lambdas
                !tipo.Name.Contains("<>c__DisplayClass") &&   // ignora clases de cierre
                tipo.Namespace != "System.Runtime.CompilerServices" &&
                tipo.Namespace != "Godot")                    // ignora clases internas de Godor
            {
                metodo = frameMethod;
                tipoClase = tipo;
                linea = frame.GetFileLineNumber();
                break;
            }
        }

        // fallback si no encontramos frame válido
        if (metodo == null)
        {
            var frame = stack.GetFrame(2);
            metodo = frame?.GetMethod();
            tipoClase = metodo?.DeclaringType;
            linea = frame?.GetFileLineNumber() ?? 0;
        }

        string clase = tipoClase?.Name ?? "UnknownClass";
        string nombre = metodo?.Name ?? "UnknownMethodName";

        return linea > 0 ? $"{clase}.{nombre}:{linea}" : $"{clase}.{nombre}";
    }

    public static void EscribirLog(string level, string message, string context, string color = "white")
    {
        if (string.IsNullOrEmpty(context))
            context = ObtenerContexto();

        string mensajeLog = FormatearMensajeLog(level, context, message);

        // Aplicamos color en la consola
        string mensajeColoreado = $"[color={color}]{mensajeLog}[/color]";

        // Muestra en consola con color
        GD.PrintRich(mensajeColoreado);

        if (EscribirLogEnFichero)
        {
            // Se puede loguear desde hilos de fondo (guardados); dos escrituras a la vez en el fichero fallarían.
            lock (_bloqueoFichero)
                File.AppendAllText(PathLog, mensajeLog + "\n"); // el archivo no tiene color
        }
    }

    private static string FormatearMensajeLog(string level, string context, string message)
    {
        string time = DateTime.Now.ToString(FORMATO_FECHA_LOG);

        string padingLevel = new string(' ', 5 - level.Length);

        // Base del mensaje
        string mensajeBase = $"{time} [{level}]{padingLevel} [{context}]: ";

        // Calculamos el ancho total deseado
        int widthContext = ContextWidth - mensajeBase.Length;

        // Creamos la variable para el padding (solo espacios)
        string padingMensaje = new string(' ', Math.Max(widthContext, 0));

        // Retornamos el mensaje final
        return $"{mensajeBase}{padingMensaje}{message}";
    }
}
