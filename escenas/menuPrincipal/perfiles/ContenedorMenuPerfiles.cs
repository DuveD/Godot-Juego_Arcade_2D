using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfiles;

public partial class ContenedorMenuPerfiles : CenterContainer
{
    [Signal]
    public delegate void OnCrearPrimerPerfilEventHandler();

    public Label Titulo;

    public PanelContainerPerfiles PanelContainerPerfiles;
    public PanelContainerNuevoPerfil PanelNuevoPerfil;

    private SlotPerfil _slotSeleccionado;

    private bool _ocultarBotones;

    private bool _modoBorrarActivo = false;

    private List<Perfil> _perfiles;

    public override void _Ready()
    {
        Titulo = GetNode<Label>("PanelContainerPerfiles/VBoxContainer/Titulo");
        PanelContainerPerfiles = GetNode<PanelContainerPerfiles>("PanelContainerPerfiles");
        PanelNuevoPerfil = GetNode<PanelContainerNuevoPerfil>("PanelContainerNuevoPerfil");

        PanelContainerPerfiles.OnSlotSeleccionadoPressed += OnSlotPerfilPressed;
        PanelContainerPerfiles.OnButtonBorrarToggled += OnButtonBorrarToggled;
        PanelNuevoPerfil.OnButtonConfirmarPressed += OnCrearNuevoPerfil;
        PanelNuevoPerfil.OnButtonCancelarPressed += OnCancelarNuevoPerfil;

        // Inicializa UI
        InicializarSlots();
        ModoBorrarActivo(false);

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private async void InicializarSlots()
    {
        Global.IndicadorCarga.Mostrar();

        _perfiles = await Task.Run(() => new List<Perfil>
        {
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot1),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot2),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot3)
        });

        Global.IndicadorCarga.Esconder();

        PanelContainerPerfiles.ConfigurarSlots(_perfiles);
    }

    private void OnButtonBorrarToggled(bool toggled)
    {
        ModoBorrarActivo(toggled);
    }

    private void ModoBorrarActivo(bool toggled)
    {
        _modoBorrarActivo = toggled;
        InformarTitulo();
    }

    private void InformarTitulo()
    {
        if (_modoBorrarActivo)
        {
            Titulo.Text = "MenuPrincipal.perfil.tituloBorrar";
        }
        else
        {
            Titulo.Text = "MenuPrincipal.perfil.titulo";
        }
    }

    private void OnSlotPerfilPressed(SlotPerfil slotPerfil)
    {
        // Borrar perfil
        if (_modoBorrarActivo)
        {
            OnSlotPerfilBorrarPressed(slotPerfil);
        }
        // Cambiar perfil activo
        else if (!slotPerfil.Vacio)
        {
            if (!slotPerfil.Activo)
            {
                OnSlotPerfilCambiarActivoPressed(slotPerfil);
            }
            else
            {
                // El perfil seleccionado ya está activo.
            }
        }
        // Crear nuevo perfil
        else if (slotPerfil.Vacio)
        {
            OnSlotVacioPressed(slotPerfil);
        }
    }

    private void OnSlotPerfilBorrarPressed(SlotPerfil slotPerfil)
    {
        PanelContainerPerfiles.Hide();

        // Mostrar confirmación de cambio
        ContenedorConfirmacion.Instanciar(
            this,
            "MenuPrincipal.perfil.confirmarEliminarPerfil",
            "General.si",
            "General.no",
            () => EliminarPerfil(slotPerfil),
            () => PanelContainerPerfiles.Show(false, false)
        );
    }

    private void OnSlotPerfilCambiarActivoPressed(SlotPerfil slotPerfil)
    {
        string textoMensaje;
        if (PanelContainerPerfiles.HaySlotPerfilActivo)
            textoMensaje = "MenuPrincipal.perfil.confirmarCambiarPerfil";
        else
            textoMensaje = "MenuPrincipal.perfil.confirmarCargarPerfil";

        PanelContainerPerfiles.Hide();

        // Mostrar confirmación de cambio
        ContenedorConfirmacion.Instanciar(
            this,
            textoMensaje,
            "General.si",
            "General.no",
            () => CambiarPerfilActivo(slotPerfil),
            () => PanelContainerPerfiles.Show(false, false)
        );
    }

    private void OnSlotVacioPressed(SlotPerfil slotVacio)
    {
        _slotSeleccionado = slotVacio;
        PanelContainerPerfiles.Hide();
        PanelNuevoPerfil.Show(true);
    }

    private async void OnCrearNuevoPerfil(string nombrePerfil)
    {
        if (_slotSeleccionado == null) return;

        Global.IndicadorGuardado.Mostrar();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Perfil nuevoPerfil = await Task.Run(() =>
        {
            string idPerfil = GestorPerfiles.GenerarIdPerfil();
            Perfil nuevoPerfil = new(idPerfil, nombrePerfil, DateTime.UtcNow, null);
            GestorPerfiles.InicializarPerfil(nuevoPerfil);
            GestorPerfiles.GuardarPerfil(nuevoPerfil);

            return nuevoPerfil;
        });

        _slotSeleccionado.Perfil = nuevoPerfil;
        _perfiles[_slotSeleccionado.NumeroSlot - 1] = nuevoPerfil;
        Ajustes.SetIdPerfil(_slotSeleccionado.NumeroSlot, nuevoPerfil.Id);

        CambiarPerfilActivo(_slotSeleccionado);

        _slotSeleccionado = null;

        Global.IndicadorGuardado.Esconder();

        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(false);

        if (_ocultarBotones)
            EmitSignal(SignalName.OnCrearPrimerPerfil);
    }
    private async void EliminarPerfil(SlotPerfil slotPerfil)
    {
        var perfil = slotPerfil.Perfil;
        if (perfil == null) return;

        Global.IndicadorGuardado.Mostrar();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // El estado compartido (lista de perfiles y ajustes) se modifica en el hilo principal;
        // en segundo plano solo borramos el archivo.
        _perfiles[slotPerfil.NumeroSlot - 1] = null;
        Ajustes.SetIdPerfil(slotPerfil.NumeroSlot, null);

        await Task.Run(() => GestorPerfiles.EliminarPerfil(perfil));

        Global.IndicadorGuardado.Esconder();

        slotPerfil.Perfil = null;

        if (Ajustes.IdPerfilActivo == perfil?.Id)
            Global.CambiarPerfilActivo(null);

        PanelContainerPerfiles.Show(true, _ocultarBotones);
    }

    private void CambiarPerfilActivo(SlotPerfil slotPerfil)
    {
        var perfil = slotPerfil.Perfil;
        Global.CambiarPerfilActivo(perfil);
        PanelContainerPerfiles.ConfigurarSlots(_perfiles);

        PanelContainerPerfiles.Show(false, false);
    }

    private void OnCancelarNuevoPerfil()
    {
        _slotSeleccionado = null;
        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(false, _ocultarBotones);
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotones)
    {
        _ocultarBotones = ocultarBotones;
        PanelContainerPerfiles.Show(seleccionarPrimerElemento, ocultarBotones);
        PanelNuevoPerfil.Hide();
        this.Show();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (PanelContainerPerfiles.Visible)
            {
                if (_modoBorrarActivo)
                {
                    PanelContainerPerfiles.ButtonBorrar.GrabFocus();
                    UtilidadesNodos.PulsarBoton(PanelContainerPerfiles.ButtonBorrar);
                }
                else if (!PanelContainerPerfiles.ButtonAtras.HasFocus())
                {
                    PanelContainerPerfiles.ButtonAtras.GrabFocus();
                }
                else
                {
                    UtilidadesNodos.PulsarBoton(PanelContainerPerfiles.ButtonAtras);
                }
            }
            else if (PanelNuevoPerfil.Visible)
            {
                UtilidadesNodos.PulsarBoton(PanelNuevoPerfil.ButtonCancelar);
            }

            AcceptEvent();
        }
    }
}
