using Godot;
using Primerjuego2D.escenas.menuPrincipal.ajustes;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla.HUD;

public partial class PanelMenuPausa : Control
{
    [Signal]
    public delegate void OnButtonSalirConfirmarPressedEventHandler();

    private ContenedorMenuPausa ContenedorMenuPausa;

    private ContenedorMenuAjustes ContenedorMenuAjustes;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.ContenedorMenuPausa = GetNode<ContenedorMenuPausa>("ContenedorMenuPausa");
        this.ContenedorMenuAjustes = GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");

        this.VisibilityChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
        {
            this.ContenedorMenuPausa.Show(true);
            this.ContenedorMenuAjustes.Hide();
        }
        else
        {
            this.ContenedorMenuPausa.Hide();
            this.ContenedorMenuAjustes.Hide();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        // Solo respondemos si el menú es visible.
        if (!this.IsVisibleInTree())
            return;

        // Bloqueamos todos los eventos hacia abajo.
        AcceptEvent();
    }

    public void OnButtonAjustesPressed()
    {
        this.ContenedorMenuPausa.Hide();
        this.ContenedorMenuAjustes.Show(true);
    }

    public void OnButtonAjustesAtrasPressed()
    {
        this.ContenedorMenuPausa.Show(false);
        this.ContenedorMenuAjustes.Hide();
    }

    public void OnButtonSalirPressed()
    {
        this.ContenedorMenuPausa.Hide();
        this.ContenedorMenuAjustes.Hide();
        ContenedorConfirmacion.Instanciar(this,
            "BatallaHUD.mensaje.preguntaSalir",
            "BatallaHUD.boton.salir",
            "BatallaHUD.boton.cancelar",
            () => EmitSignal(SignalName.OnButtonSalirConfirmarPressed),
            OnButtonSalirCancelarPressed);
    }

    public void OnButtonSalirCancelarPressed()
    {
        this.ContenedorMenuPausa.Show(false);
        this.ContenedorMenuAjustes.Hide();
    }
}
