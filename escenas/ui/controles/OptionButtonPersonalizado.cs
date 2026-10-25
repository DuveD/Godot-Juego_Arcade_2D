using Godot;
using Primerjuego2D.escenas.modelos.interfaces;

namespace Primerjuego2D.escenas.ui.controles;

public partial class OptionButtonPersonalizado : OptionButton, IFocusSilencioso
{
    [Export]
    public string NombreSonidoOnFocusEntered { get; set; }
    [Export]
    public string NombreSonidoOnMouseEntered { get; set; }

    private bool _reproducirSonido = true;

    public override void _Ready()
    {
        this.FocusEntered += OnFocusedEntered;
        this.MouseEntered += OnMouseEntered;
    }

    public void OnFocusedEntered()
    {
        if (this._reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnFocusEntered))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnFocusEntered);
    }

    public void GrabFocusSilencioso()
    {
        this._reproducirSonido = false;
        this.GrabFocus();
        this._reproducirSonido = true;
    }

    public void OnMouseEntered()
    {
        if (!string.IsNullOrEmpty(NombreSonidoOnMouseEntered))
        {
            if (!this.Disabled)
                Global.GestorAudio.ReproducirSonido(NombreSonidoOnMouseEntered);
        }
    }
}
