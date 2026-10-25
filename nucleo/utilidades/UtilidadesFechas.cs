

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

    public static string FormatearFechaSinDiaSemana(DateTime fecha)
    {
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
