using System;
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesMatematicas
{
    /// <summary>
    /// FUnción que convierte radianes a grados.
    /// </summary>
    public static double RadiansToDegrees(double radians)
    {
        return (180 / Math.PI) * radians;
    }

    /// <summary>
    /// Función que convierte grados a radianes.
    /// </summary>
    public static double DegreesToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    public static bool PuntosCerca(Vector2 puntoA, Vector2 puntoB, float distancia)
    {
        // Distancia al cuadrado entre el punto y el jugador
        float distSq = puntoA.DistanceSquaredTo(puntoB);

        // Distancia mínima al cuadrado
        float minDistSq = distancia * distancia;

        return distSq < minDistSq;
    }
}
