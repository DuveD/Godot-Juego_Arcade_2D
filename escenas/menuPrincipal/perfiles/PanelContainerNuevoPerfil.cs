using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfiles;

public partial class PanelContainerNuevoPerfil : ContenedorMenu
{
    [Signal]
    public delegate void OnButtonConfirmarPressedEventHandler(string nombrePerfil);

    [Signal]
    public delegate void OnButtonCancelarPressedEventHandler();

    private LineEdit _lineEdit;
    private ButtonPersonalizado _buttonConfirmar;
    public ButtonPersonalizado ButtonConfirmar => _buttonConfirmar;

    private ButtonPersonalizado _buttonCancelar;
    public ButtonPersonalizado ButtonCancelar => _buttonCancelar;

    public override void _Ready()
    {
        _lineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
        _lineEdit.TextChanged += OnLineEditTextChanged;
        _lineEdit.GuiInput += OnLineEditGuiInput;
        _lineEdit.FocusEntered += OnLineEditFocusEntered;

        _buttonConfirmar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilConfirmar");
        _buttonConfirmar.Pressed += () => EmitSignal(SignalName.OnButtonConfirmarPressed, _lineEdit.Text);

        _buttonCancelar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilCancelar");
        _buttonCancelar.Pressed += () => EmitSignal(SignalName.OnButtonCancelarPressed);

        this.VisibilityChanged += OnVisibilityChanged;

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void OnLineEditFocusEntered()
    {
        this._lineEdit.Edit();
    }

    private void OnLineEditGuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.UP))
        {
            NodePath path = _lineEdit.FocusNeighborTop;
            if (!path.IsEmpty && _lineEdit.HasNode(path))
            {
                Control neighbor = _lineEdit.GetNode<Control>(path);
                neighbor.GrabFocus();
            }
            AcceptEvent();
        }
        if (@event.IsActionPressed(ConstantesAcciones.DOWN))
        {
            NodePath path = _lineEdit.FocusNeighborBottom;
            if (!path.IsEmpty && _lineEdit.HasNode(path))
            {
                Control neighbor = _lineEdit.GetNode<Control>(path);
                neighbor.GrabFocus();
            }
            AcceptEvent();
        }
        else if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(_buttonCancelar);
            AcceptEvent();
        }
        else if (@event.IsActionPressed(ConstantesAcciones.ENTER))
        {
            if (!_buttonConfirmar.Disabled)
            {
                UtilidadesNodos.PulsarBoton(_buttonConfirmar);
                AcceptEvent();
            }
        }
    }

    private void OnLineEditTextChanged(string newText)
    {
        bool desactivarButtonConfirmar = string.IsNullOrWhiteSpace(newText) /*|| newText.Length > ConstantesGenerales.LONGITUD_MAXIMA_NOMBRE_PERFIL*/;
        _buttonConfirmar.Disabled = desactivarButtonConfirmar;
        _lineEdit.FocusNeighborBottom = desactivarButtonConfirmar ? _lineEdit.GetPathTo(_buttonCancelar) : _lineEdit.GetPathTo(_buttonConfirmar);
    }

    public override List<Control> ObtenerElementosConFoco() =>
        new List<Control> { _lineEdit, _buttonConfirmar, _buttonCancelar };

    public override Control ObtenerPrimerElementoConFoco() => _lineEdit;

    private void OnVisibilityChanged()
    {
        if (this.Visible)
            Limpiar();
    }

    private void Limpiar()
    {
        _lineEdit.Text = "";
        OnLineEditTextChanged(_lineEdit.Text);
    }
}
