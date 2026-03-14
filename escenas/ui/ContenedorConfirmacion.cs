using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui;

public partial class ContenedorConfirmacion : ContenedorMenu
{
    public Label LabelMensaje;
    public ButtonPersonalizado ButtonConfirmar;
    public ButtonPersonalizado ButtonCancelar;

    private Action _onConfirmar;
    private Action _onCancelar;

    private static readonly PackedScene _escena = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<ContenedorConfirmacion>());

    public static ContenedorConfirmacion Instanciar(Node nodoPadre, string textoMensaje, string textoConfirmar, string textoCancelar, Action onConfirmar, Action onCancelar)
    {
        ArgumentNullException.ThrowIfNull(nodoPadre);

        var instancia = _escena.Instantiate<ContenedorConfirmacion>();
        instancia.Visible = false;
        nodoPadre.GetParent().AddChild(instancia);
        instancia.Inicializar(textoMensaje, textoConfirmar, textoCancelar, onConfirmar, onCancelar);
        return instancia;
    }

    public override void _Ready()
    {
        SetProcessUnhandledInput(true);

        LabelMensaje = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "MensajeConfirmacion");
        ButtonConfirmar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonConfirmar");
        ButtonCancelar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonCancelar");

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public void Inicializar(string textoMensaje, string textoConfirmar, string textoCancelar, Action onConfirmar, Action onCancelar)
    {
        LabelMensaje.Text = textoMensaje;
        ButtonConfirmar.Text = textoConfirmar;
        ButtonCancelar.Text = textoCancelar;
        _onConfirmar = onConfirmar;
        _onCancelar = onCancelar;

        ButtonConfirmar.Pressed += OnButtonConfirmarPressed;
        ButtonCancelar.Pressed += OnButtonCancelarPressed;

        Show(true);
    }

    private void OnButtonConfirmarPressed()
    {
        _onConfirmar?.Invoke();
        this.QueueFree();
    }

    private void OnButtonCancelarPressed()
    {
        _onCancelar?.Invoke();
        this.QueueFree();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(ButtonCancelar);
            AcceptEvent();
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ButtonConfirmar, ButtonCancelar];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ButtonCancelar;
    }

}
