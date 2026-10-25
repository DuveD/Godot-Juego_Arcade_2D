using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public partial class ControlSeleccion : HBoxContainer
{
    [Signal]
    public delegate void ValorCambiadoEventHandler(Variant valor);

    [Export]
    public string TextoLabel
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    private Variant _valor;

    [Export]
    public Variant Valor
    {
        get => _valor;
        set => SetValorInterno(value);
    }

    private Label _label;
    public Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

    private OptionButtonPersonalizado _optionButton;
    public OptionButtonPersonalizado OptionButton =>
        _optionButton ??= UtilidadesNodos.ObtenerNodoDeTipo<OptionButtonPersonalizado>(this);

    public override void _Ready()
    {
        LoggerJuego.Trace($"{Name} Ready.");

        OptionButton.ItemSelected += OnOptionButtonItemSelected;

        // Aplicamos el valor exportado si existe
        if (_valor.VariantType != Variant.Type.Nil)
            SetValorInterno(_valor, emitirSenal: false);
    }

    public void AgregarOpciones(Dictionary<Variant, string> opciones)
    {
        foreach (var kvp in opciones)
            AgregarOpcion(kvp.Key, kvp.Value);
    }

    public void AgregarOpcion(Variant valor, string texto)
    {
        OptionButton.AddItem(texto);
        int index = OptionButton.GetItemCount() - 1;
        OptionButton.SetItemMetadata(index, valor);
    }

    private void SetValorInterno(Variant nuevoValor, bool emitirSenal = true)
    {
        if (_valor.EqualsByType(nuevoValor))
            return;

        _valor = nuevoValor;

        // Intentamos sincronizar el OptionButton
        for (int i = 0; i < OptionButton.GetItemCount(); i++)
        {
            if (OptionButton.GetItemMetadata(i).EqualsByType(nuevoValor))
            {
                if (OptionButton.Selected != i)
                    OptionButton.Select(i);

                if (emitirSenal)
                    EmitSignal(SignalName.ValorCambiado, _valor);

                return;
            }
        }

        // No lanzamos excepción en runtime normal
        LoggerJuego.Warn($"Valor '{nuevoValor}' no encontrado en ControlSeleccion '{Name}'.");
    }

    private void OnOptionButtonItemSelected(long index)
    {
        var seleccionado = OptionButton.GetItemMetadata((int)index);
        SetValorInterno(seleccionado);
    }
}
