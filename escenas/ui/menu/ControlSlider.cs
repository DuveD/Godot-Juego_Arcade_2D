using System;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public partial class ControlSlider : VBoxContainer
{
    [Signal]
    public delegate void ValorCambiadoEventHandler(double valor);

    private string _textoLabel = "";
    [Export]
    public string TextoLabel
    {
        get => _textoLabel;
        set
        {
            _textoLabel = value;
            Label?.Text = _textoLabel;
        }
    }

    private bool _mostrarValorNumerico = true;
    [Export]
    public bool MostrarValorNumerico
    {
        get => _mostrarValorNumerico;
        set
        {
            _mostrarValorNumerico = value;
            SpinBox?.Visible = _mostrarValorNumerico;
        }
    }

    public bool _ModoEntero = true;
    [Export]
    public bool ModoEntero
    {
        get => _ModoEntero;
        set
        {
            _ModoEntero = value;
            AplicarRango();
            SetValorInterno(_valor, emitirSenal: false);
        }
    }

    private double _valor;

    [Export]
    public double Valor
    {
        get => _valor;
        set => SetValorInterno(value);
    }

    private double _MinValor = 0;

    [Export]
    public double MinValor
    {
        get => _MinValor;
        set
        {
            _MinValor = value;
            AplicarRango();
        }
    }

    private double _maxValor = 100;

    [Export]
    public double MaxValor
    {
        get => _maxValor;
        set
        {
            _maxValor = value;
            AplicarRango();
        }
    }

    private double _step = 1;

    [Export]
    public double Step
    {
        get => _step;
        set
        {
            _step = ModoEntero ? Math.Max(1, Math.Truncate(value)) : value;
            AplicarRango();
        }
    }

    private Label Label;

    private SpinBox SpinBox;

    public HSliderPersonalizado SliderVolumen;

    public override void _Ready()
    {
        LoggerJuego.Trace($"{Name} Ready.");

        this.Label = UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);
        this.Label.Text = _textoLabel;

        this.SpinBox = UtilidadesNodos.ObtenerNodoDeTipo<SpinBox>(this);
        this.SpinBox.Visible = _mostrarValorNumerico;

        this.SliderVolumen = UtilidadesNodos.ObtenerNodoDeTipo<HSliderPersonalizado>(this);

        AplicarRango();
        SetValorInterno(_valor, emitirSenal: false);

        this.SpinBox.ValueChanged += OnSpinBoxValueChanged;
        this.SliderVolumen.ValueChanged += OnSliderValueChanged;
    }

    private void OnSpinBoxValueChanged(double value)
    {
        SetValorInterno(value);
    }

    private void OnSliderValueChanged(double value)
    {
        SetValorInterno(value);
    }

    public void SetValorInterno(double value, bool emitirSenal = true)
    {
        if (NearlyEqual(_valor, value))
            return;

        _valor = this.ModoEntero ? Math.Truncate(value) : value;

        // Sincronizamos controles sin provocar bucles

        if (!NearlyEqual(SpinBox.Value, _valor))
            SpinBox.Value = _valor;

        if (!NearlyEqual(SliderVolumen.Value, _valor))
            SliderVolumen.Value = _valor;

        if (emitirSenal)
            EmitSignal(SignalName.ValorCambiado, _valor);
    }

    private void AplicarRango()
    {
        if (SpinBox == null || SliderVolumen == null)
            return;

        SpinBox.MinValue = _MinValor;
        SpinBox.MaxValue = _maxValor;
        SpinBox.Step = _step;

        SliderVolumen.MinValue = _MinValor;
        SliderVolumen.MaxValue = _maxValor;
        SliderVolumen.Step = _step;

        // Aseguramos que el valor actual sigue siendo válido

        SetValorInterno(Math.Clamp(_valor, _MinValor, _maxValor), emitirSenal: false);
    }

    private static bool NearlyEqual(double a, double b, double epsilon = 0.0001)
    {
        return Math.Abs(a - b) < epsilon;
    }
}
