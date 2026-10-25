
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesLayer
{
    public static void AjustarLayerNodo(Node nodo, int layer)
    {
        if (nodo is CanvasLayer canvasLayer)
        {
            canvasLayer.Layer = layer;
        }

        foreach (Node child in nodo.GetChildren())
        {
            AjustarLayerNodo(child, layer);
        }
    }
}
