using Godot;
using Primerjuego2D.escenas.modelos.interfaces;

namespace Primerjuego2D.escenas.ui.controles;

public partial class HSliderPersonalizado : HSlider, IFocusSilencioso
{
    [Export]
    public string NombreSonidoOnFocus { get; set; }
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
        if (this._reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnFocus))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnFocus);
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
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnMouseEntered);
    }
}
