using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesNodos
{
    /// <summary>
    /// Pausa o reanuda el nodo y todo su árbol.
    /// </summary>
    public static void PausarNodo(Node node, bool pausar)
    {
        node.GetTree().Paused = pausar;
    }

    /// <summary>
    /// Devuelve si el nodo está pausado.
    /// </summary>
    public static bool NodoPausado(Node node)
    {
        return node.GetTree().Paused;
    }

    /// <summary>
    /// Espera una cantidad de segundos respetando o no la pausa del juego.
    /// </summary>
    public static async Task EsperarSegundos(Node node, double segundos, bool respetarPausa = true)
    {
        await node.ToSignal(node.GetTree().CreateTimer(segundos, respetarPausa), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>
    /// Esconde todos los nodos hijos de un nodo padre, excepto los nodos indicados.
    /// </summary>
    public static void EsconderMenos(Node padre, params CanvasItem[] nodosExcluidos)
    {
        // Usamos HashSet para búsquedas O(1) en lugar de Array.IndexOf (O(n))
        var excluidos = new HashSet<CanvasItem>(nodosExcluidos);

        foreach (var hijo in padre.GetChildren())
        {
            if (hijo is CanvasItem canvasItem && !excluidos.Contains(canvasItem))
                canvasItem.Hide();
        }
    }

    public static void EsconderTodo(Node padre)
    {
        EsconderMenos(padre);
    }

    /// <summary>
    /// Muestra todos los nodos hijos de un nodo padre, excepto los nodos indicados.
    /// </summary>
    public static void MostrarMenos(Node padre, params CanvasItem[] nodosExcluidos)
    {
        // Usamos HashSet para búsquedas O(1) en lugar de Array.IndexOf (O(n))
        var excluidos = new HashSet<CanvasItem>(nodosExcluidos);

        foreach (var hijo in padre.GetChildren())
        {
            if (hijo is CanvasItem canvasItem && !excluidos.Contains(canvasItem))
                canvasItem.Show();
        }
    }

    /// <summary>
    /// Muestra todos los nodos hijos de un nodo padre.
    /// </summary>
    public static void MostrarTodo(Node padre)
    {
        MostrarMenos(padre);
    }

    /// <summary>
    /// Espera hasta que el nodo se reanude si está pausado.
    /// </summary>
    public static async Task EsperarRenaudar(Node node)
    {
        while (node.GetTree().Paused)
            await node.ToSignal(node.GetTree(), "process_frame");
    }


    /// <summary>
    /// Marca el ítem del PopupMenu correspondiente al ID dado y desmarca los demás.
    /// </summary>
    public static void CheckItemPorId(PopupMenu popupMenu, long id)
    {
        int index = popupMenu.GetItemIndex((int)id);

        for (int i = 0; i < popupMenu.ItemCount; i++)
        {
            if (i != index)
                popupMenu.SetItemChecked(i, false);
            else
                popupMenu.SetItemChecked(i, true);
        }
    }

    /// <summary>
    /// Obtiene la ruta de la escena correspondiente a la clase dada.
    /// </summary>
    public static string ObtenerRutaEscena<T>(string root = "res://") where T : Node
    {
        Type type = typeof(T);

        string path = "";

        if (!string.IsNullOrEmpty(type.Namespace))
        {
            // Dividimos el namespace por puntos
            var parts = type.Namespace.Split('.');

            // Eliminamos la primera parte (base del namespace)
            if (parts.Length > 1)
            {
                path = string.Join("/", parts, 1, parts.Length - 1) + "/";
            }
        }

        // Añadimos el nombre de la clase y la extensión
        path += type.Name + ".tscn";

        // Devolvemos la ruta completa respecto a res://
        return root + path;
    }

    /// <summary>
    /// Borra todos los nodos hijos del nodo dado.
    /// </summary>
    public static void BorrarHijos(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            node.RemoveChild(child);
            child.QueueFree();
        }

    }

    /// <summary>
    /// Obtiene un nodo hijo por su nombre.
    /// </summary>
    public static T ObtenerNodoPorNombre<T>(Node nodoPadre, string nombre, int nivel = 0) where T : Node
    {
        if (nodoPadre == null)
            return null;

        if (nodoPadre.Name == nombre && nodoPadre is T match)
        {
            return match;
        }
        else if (nodoPadre.GetChildren().Count > 0)
        {
            foreach (Node child in nodoPadre.GetChildren())
            {
                var result = ObtenerNodoPorNombre<T>(child, nombre, nivel + 1);
                if (result != null)
                    return result;
            }
        }

        if (nivel == 0)
            LoggerJuego.Warn("No se ha encontrado ningún nodo con el nombre '" + nombre + "' del tipo '" +
                             typeof(T).Name + "'.");

        return null;
    }

    /// <summary>
    /// Obtiene el primer nodo hijo del tipo indicado.
    /// </summary>
    public static T ObtenerNodoDeTipo<T>(Node nodoPadre, int nivel = 0) where T : Node
    {
        if (nodoPadre == null)
            return null;

        if (nodoPadre is T nodoDelTipo)
        {
            return nodoDelTipo;
        }

        foreach (Node hijo in nodoPadre.GetChildren())
        {
            var resultado = ObtenerNodoDeTipo<T>(hijo, nivel + 1);
            if (resultado != null)
                return resultado;
        }

        if (nivel == 0)
            LoggerJuego.Warn("No se ha encontrado ningún nodo del tipo '" + typeof(T).Name + "'.");

        return null;
    }

    /// <summary>
    /// Obtiene todos los nodos hijos del tipo indicado.
    /// </summary>
    public static List<T> ObtenerNodosDeTipo<T>(Node nodoPadre) where T : Node
    {
        var resultado = new List<T>();

        if (nodoPadre == null)
            return resultado;

        if (nodoPadre is T nodoDelTipo)
        {
            resultado.Add(nodoDelTipo);
        }

        foreach (Node hijo in nodoPadre.GetChildren())
        {
            resultado.AddRange(ObtenerNodosDeTipo<T>(hijo));
        }

        return resultado;
    }

    public static void PulsarBoton(Button boton, bool soloSiNoEstaDesactivado = true)
    {
        if (boton == null)
        {
            LoggerJuego.Warn("Intentando pulsar un botón nulo.");
            return;
        }

        if (soloSiNoEstaDesactivado && boton.Disabled)
        {
            LoggerJuego.Warn("Intentando pulsar el botón '" + boton.Name + "' pero está desactivado.");
            return;
        }

        if (boton.ToggleMode)
        {
            // Cambiar ButtonPressed ya emite la señal Toggled con el nuevo estado.
            boton.ButtonPressed = !boton.ButtonPressed;
        }
        else
        {
            boton.EmitSignal(BaseButton.SignalName.Pressed);
        }
    }
}
