using System.Collections.Generic;
using Godot;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.sistema.logros;

public partial class GestorNotificacionLogros : Node
{
    [Export]
    public PackedScene EscenaContenedorLogro;

    [Export]
    int TiempoVisible = 1;

    private CanvasLayer _canvasLayer;

    private readonly Queue<Logro> _cola = new();
    private bool _procesando;
    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
        UtilidadesLayer.AjustarLayerNodo(_canvasLayer, ConstantesLayer.NOTIFICACIONES);
    }

    public void MostrarLogros(IEnumerable<Logro> logros)
    {
        foreach (var logro in logros)
            _cola.Enqueue(logro);

        IntentarProcesar();
    }

    private void IntentarProcesar()
    {
        if (_procesando || _cola.Count == 0)
            return;

        ProcesarCola();
    }

    private async void ProcesarCola()
    {
        _procesando = true;

        while (_cola.Count > 0)
        {
            var logro = _cola.Dequeue();

            ContenedorLogroNotificacion instancia = EscenaContenedorLogro.Instantiate<ContenedorLogroNotificacion>();
            _canvasLayer.AddChild(instancia);

            instancia.Inicializar(logro, TiempoVisible);

            await instancia.MostrarAsync();
            instancia.QueueFree();
        }

        _procesando = false;
    }
}
