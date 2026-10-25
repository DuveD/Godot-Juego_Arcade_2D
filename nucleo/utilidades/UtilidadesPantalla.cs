using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesPantalla
{
    public static Vector2 ObtenerTamanoPantalla(CanvasItem canvasItem)
    {
        return canvasItem.GetViewportRect().Size;
    }
}
