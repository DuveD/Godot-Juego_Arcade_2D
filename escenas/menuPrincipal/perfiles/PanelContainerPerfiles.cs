using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfiles;

public partial class PanelContainerPerfiles : ContenedorMenu
{
    [Signal]
    public delegate void OnSlotSeleccionadoPressedEventHandler(SlotPerfil slotPerfilSeleccionado);

    [Signal]
    public delegate void OnButtonBorrarToggledEventHandler(bool toggled);

    private SlotPerfil _slotPerfil1;
    private SlotPerfil _slotPerfil2;
    private SlotPerfil _slotPerfil3;

    private List<SlotPerfil> _slotPerfiles => new List<SlotPerfil> { _slotPerfil1, _slotPerfil2, _slotPerfil3 };

    private ButtonPersonalizado _buttonAtras;
    public ButtonPersonalizado ButtonAtras => _buttonAtras;

    private ButtonPersonalizado _buttonBorrar;
    public ButtonPersonalizado ButtonBorrar => _buttonBorrar;

    private HBoxContainer _containerBotones;
    public HBoxContainer ContainerBotones => _containerBotones;

    private bool _modoBorrarActivo = false;

    public override void _Ready()
    {
        _slotPerfil1 = GetNode<SlotPerfil>("VBoxContainer/ContainerPerfiles/SlotPerfil1");
        _slotPerfil1.NumeroSlot = 1;
        _slotPerfil2 = GetNode<SlotPerfil>("VBoxContainer/ContainerPerfiles/SlotPerfil2");
        _slotPerfil2.NumeroSlot = 2;
        _slotPerfil3 = GetNode<SlotPerfil>("VBoxContainer/ContainerPerfiles/SlotPerfil3");
        _slotPerfil3.NumeroSlot = 3;

        _slotPerfil1.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil1);
        _slotPerfil2.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil2);
        _slotPerfil3.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil3);

        _buttonAtras = GetNode<ButtonPersonalizado>("VBoxContainer/ContainerBotones/ButtonAtras");
        _buttonBorrar = GetNode<ButtonPersonalizado>("VBoxContainer/ContainerBotones/ButtonBorrar");
        _buttonBorrar.Toggled += ButtonBorrarToggled;

        _containerBotones = GetNode<HBoxContainer>("VBoxContainer/ContainerBotones");

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void ButtonBorrarToggled(bool toggled)
    {
        _modoBorrarActivo = toggled;
        if (toggled)
        {
            _buttonAtras.Visible = false;
            _slotPerfil3.FocusNeighborBottom = this._slotPerfil3.GetPathTo(_buttonBorrar);
        }
        else
        {
            _buttonAtras.Visible = true;
            _slotPerfil3.FocusNeighborBottom = this._slotPerfil3.GetPathTo(_buttonAtras);
        }

        ConfigurarFocusSlots();

        EmitSignal(SignalName.OnButtonBorrarToggled, toggled);
    }

    private void ConfigurarFocusSlots()
    {
        foreach (var slot in _slotPerfiles)
        {
            if (slot.Vacio)
            {
                slot.Disabled = _modoBorrarActivo;
            }
            else if (slot.Activo)
            {
                slot.Disabled = !_modoBorrarActivo;
            }
            else
            {
                slot.Disabled = false;
            }
        }
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotones)
    {
        this.Show(seleccionarPrimerElemento);
        this._buttonBorrar.Visible = !ocultarBotones;

        // Aseguramos que el botón de borrar esté desactivado al mostrar el panel.
        ButtonBorrar.ButtonPressed = false;

        ConfigurarFocusSlots();
    }

    public void ConfigurarSlots(List<Perfil> perfiles)
    {
        if (perfiles == null || perfiles.Count != 3)
        {
            LoggerJuego.Error("La lista de perfiles debe contener exactamente 3 elementos.");
            return;
        }

        for (int i = 0; i < _slotPerfiles.Count; i++)
        {
            Perfil perfil = i < perfiles.Count ? perfiles[i] : null;
            _slotPerfiles[i].Perfil = perfil;
            bool activo = Ajustes.IdPerfilActivo == perfil?.Id;
            _slotPerfiles[i].SetActivo(activo);
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [_slotPerfil1, _slotPerfil2, _slotPerfil3, _buttonAtras, _buttonBorrar];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _modoBorrarActivo ? _slotPerfil1 : _slotPerfiles.FirstOrDefault(s => !s.Activo) ?? _slotPerfil1;
    }

    public bool HaySlotPerfilActivo => _slotPerfiles.Any(p => p.Activo);
}
