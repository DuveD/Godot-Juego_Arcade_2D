using System;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesString
{
    public static string PascalToSnake(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new System.Text.StringBuilder();

        foreach (char c in input)
        {
            if (char.IsUpper(c) && sb.Length > 0)
                sb.Append('_');

            sb.Append(char.ToLower(c));
        }

        return sb.ToString();
    }

    public static string SnakeToPascal(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var sb = new System.Text.StringBuilder();

        foreach (var part in parts)
        {
            if (part.Length == 0) continue;

            sb.Append(char.ToUpper(part[0]));
            if (part.Length > 1)
                sb.Append(part.Substring(1).ToLower());
        }

        return sb.ToString();
    }
}
