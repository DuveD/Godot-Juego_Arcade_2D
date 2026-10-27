

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesFechas
{
    public static string DateTimeToString(this DateTime? dateTime, string formato)
    {
        if (!dateTime.HasValue || String.IsNullOrWhiteSpace(formato))
            return null;

        return dateTime.Value.ToString(formato, CultureInfo.InvariantCulture);
    }

    public static DateTime? StringToDateTime(this string dateTimeStr, string formato)
    {
        if (String.IsNullOrWhiteSpace(dateTimeStr) || String.IsNullOrWhiteSpace(formato))
            return null;

        if (DateTime.TryParseExact(dateTimeStr, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            return dateTime;

        return null;
    }

    /// <summary>
    /// Convierte la fecha a texto ISO 8601 en UTC (por ejemplo "2026-10-27T18:30:00.0000000Z").
    /// Es el formato estándar para guardar fechas: no depende del idioma ni de la zona horaria del equipo.
    /// </summary>
    public static string ATextoIso8601Utc(this DateTime? fecha)
    {
        if (!fecha.HasValue)
            return null;

        return fecha.Value.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Lee una fecha guardada en texto ISO 8601 y la devuelve en UTC, o null si el texto está vacío o no es válido.
    /// </summary>
    public static DateTime? DesdeTextoIso8601(this string texto)
    {
        if (String.IsNullOrWhiteSpace(texto))
            return null;

        if (DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime fecha))
            return fecha.ToUniversalTime();

        return null;
    }

    /// <summary>
    /// Formatea la fecha en hora local y en el idioma actual, sin el día de la semana.
    /// </summary>
    public static string FormatearFechaSinDiaSemana(DateTime fecha)
    {
        // Las fechas se guardan en UTC; para mostrarlas se pasan a la hora local del jugador.
        fecha = fecha.ToLocalTime();

        var culture = CultureInfo.CurrentCulture;

        // Patrón largo dependiente del idioma
        var pattern = culture.DateTimeFormat.LongDatePattern;

        // Eliminamos el día de la semana (dddd)
        pattern = Regex.Replace(
            pattern,
            @"(^dddd,\s*|\s*,?\s*dddd)",
            "",
            RegexOptions.IgnoreCase
        );

        return fecha.ToString(pattern, culture);
    }

    public static string FormatearFechaSinDiaSemana(DateTime? fecha)
    {
        return fecha.HasValue
            ? FormatearFechaSinDiaSemana(fecha.Value)
            : null;
    }
}
