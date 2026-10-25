using System;
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class Randomizador
{
    // Instancia global compartida (para uso rápido y general)
    private static readonly Random GlobalRandom = new();

    // ---- MÉTODOS GENERALES ----

    /// <summary>
    /// Devuelve un número entero aleatorio entre minValue (incluido) y maxValue (incluido).
    /// Usa una instancia global compartida.
    /// </summary>
    public static int GetRandomInt(int minValue, int maxValue)
    {
        return GlobalRandom.Next(minValue, maxValue + 1);
    }

    /// <summary>
    /// Devuelve un número aleatorio entre minValue y maxValue como double.
    /// Usa la instancia global.
    /// </summary>
    public static double GetRandomDouble(double minValue, double maxValue)
    {
        return minValue + GlobalRandom.NextDouble() * (maxValue - minValue);
    }

    /// <summary>
    /// Devuelve un número aleatorio entre 0.0 y 1.0.
    /// </summary>
    public static float GetRandomFloat()
    {
        return (float)GlobalRandom.NextDouble();
    }

    /// <summary>
    /// Devuelve una posición aleatoria dentro del área indicada.
    /// </summary>
    public static Vector2 ObtenerPosicionAleatoria(Rect2 area)
    {
        float x = (float)GetRandomDouble(area.Position.X, area.End.X);
        float y = (float)GetRandomDouble(area.Position.Y, area.End.Y);

        return new Vector2(x, y);
    }

    // ---- MÉTODOS GODOT ----

    /// <summary>
    /// Devuelve un número entero aleatorio entre minValue y maxValue usando el RNG de Godot.
    /// </summary>
    public static int GetRandomIntGodot(int minValue, int maxValue)
    {
        return (int)GetRandomDoubleGodot(minValue, maxValue);
    }

    /// <summary>
    /// Devuelve un número aleatorio entre minValue y maxValue usando el RNG de Godot.
    /// </summary>
    public static double GetRandomDoubleGodot(double minValue, double maxValue)
    {
        return GD.RandRange(minValue, maxValue);
    }

    /// <summary>
    /// Devuelve un número flotante aleatorio entre 0.0 y 1.0 usando el RNG de Godot.
    /// </summary>
    public static float GetRandomFloatGodot()
    {
        return GD.Randf();
    }

    /// <summary>
    /// Devuelve un número flotante aleatorio entre minValue y maxValue usando el RNG de Godot.
    /// </summary>
    public static float GetRandomFloatGodot(float minValue, float maxValue)
    {
        return (float)GetRandomDoubleGodot(minValue, maxValue);
    }
}
