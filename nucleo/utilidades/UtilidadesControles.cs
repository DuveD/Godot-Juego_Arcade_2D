
using System.Linq;
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesControles
{
    /// <summary>
    /// Comprueba si el evento recibido corresponde a alguna de las acciones indicadas.
    /// </summary>
    public static bool IsActionPressed(InputEvent inputEvent, params string[] acciones)
    {
        return acciones.Any(accion => inputEvent.IsActionPressed(accion));
    }

    public static bool IsMouseMovementEvent(InputEvent inputEvent)
    {
        return inputEvent is InputEventMouseMotion inputEventMouseMotion && inputEventMouseMotion.Relative.Length() > 0;
    }
}
