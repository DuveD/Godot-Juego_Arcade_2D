using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public partial class ControlCheckButton : HBoxContainer
{
    [Signal]
    public delegate void ValorCambiadoEventHandler(bool valor);

    [Export]
    public string TextoLabel
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    private bool _valor;

    [Export]
    public bool Valor
    {
        get => _valor;
        set => SetValorInterno(value);
    }

    private Label _label;
    public Label Label =>
        _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

    private CheckButtonPersonalizado _checkButton;
    public CheckButtonPersonalizado CheckButton =>
        _checkButton ??= UtilidadesNodos.ObtenerNodoDeTipo<CheckButtonPersonalizado>(this);

    public override void _Ready()
    {
        LoggerJuego.Trace($"{Name} Ready.");

        CheckButton.Toggled += OnCheckButtonToggled;

        // Aplicar valor exportado sin emitir señal
        SetValorInterno(_valor, emitirSenal: false);
    }

    private void OnCheckButtonToggled(bool toggledOn)
    {
        SetValorInterno(toggledOn);
    }

    private void SetValorInterno(bool nuevoValor, bool emitirSenal = true)
    {
        if (_valor == nuevoValor)
            return;

        _valor = nuevoValor;

        // Sincronizar estado visual del botón
        if (CheckButton.ButtonPressed != nuevoValor)
            CheckButton.ButtonPressed = nuevoValor;

        if (emitirSenal)
            EmitSignal(SignalName.ValorCambiado, _valor);
    }
}
