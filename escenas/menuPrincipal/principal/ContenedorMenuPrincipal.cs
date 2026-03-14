using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.miscelaneo.animaciones;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.principal;

public partial class ContenedorMenuPrincipal : ContenedorMenu
{
    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    private ButtonPersonalizado _ButtonEmpezarPartidaSinPerfil;
    private ButtonPersonalizado _ButtonPerfil;
    private ButtonPersonalizado _ButtonAjustes;
    private ButtonPersonalizado _ButtonLogros;
    private ButtonPersonalizado _ButtonEstadisticas;
    private ButtonPersonalizado _ButtonSalir;

    private AnimacionCrtShutdown _AnimacionCrtShutdown;

    public ButtonPersonalizado UltimoBotonPulsado;

    public override void _Ready()
    {
        _ButtonEmpezarPartida = UtilidadesNodos.ObtenerNodoPorNombre<ButtonEmpezarPartida>(this, "ButtonEmpezarPartida");
        _ButtonEmpezarPartidaSinPerfil = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonEmpezarPartidaSinPerfil");
        _ButtonPerfil = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonPerfil");
        _ButtonAjustes = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAjustes");
        _ButtonLogros = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonLogros");
        _ButtonEstadisticas = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonEstadisticas");
        _ButtonSalir = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonSalir");
        _AnimacionCrtShutdown = GetNode<AnimacionCrtShutdown>("../AnimacionCrtShutdown");

        this.VisibilityChanged += OnVisibilityChanged;
        Global.Instancia.OnCambioPerfilActivo += CalcularEstadosBotonesSinPerfil;

        CalcularEstadosBotonesSinPerfil();

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void OnVisibilityChanged()
    {
        if (this.IsVisibleInTree())
            CalcularEstadosBotonesSinPerfil();
    }

    public void CalcularEstadosBotonesSinPerfil()
    {
        bool desactivarBotonesDeperfil = Global.PerfilActivo == null;

        _ButtonEmpezarPartida.Visible = !desactivarBotonesDeperfil;
        _ButtonEmpezarPartida.Disabled = false;
        _ButtonEmpezarPartidaSinPerfil.Visible = desactivarBotonesDeperfil;
        _ButtonEmpezarPartidaSinPerfil.Disabled = false;
        _ButtonLogros.Disabled = desactivarBotonesDeperfil;
        _ButtonEstadisticas.Disabled = desactivarBotonesDeperfil;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [.. UtilidadesNodos.ObtenerNodosDeTipo<ButtonPersonalizado>(this)];
    }

    protected override void PostReady()
    {
        base.PostReady();

        ConfigurarBotonesMenu();

        LoggerJuego.Trace(this.Name + " PostReady.");
    }

    private void ConfigurarBotonesMenu()
    {
        LoggerJuego.Trace("Configuramos el focus de los botones del menú.");

        var elementosConFoco = ObtenerElementosConFoco();
        foreach (var boton in elementosConFoco.OfType<ButtonPersonalizado>())
            boton.Pressed += () => this.UltimoBotonPulsado = boton;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (!_ButtonSalir.HasFocus())
                _ButtonSalir.GrabFocus();
            else
                UtilidadesNodos.PulsarBoton(_ButtonSalir);

            AcceptEvent();
        }
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return (this.UltimoBotonPulsado != null && this.UltimoBotonPulsado.IsVisibleInTree()) ? this.UltimoBotonPulsado : (_ButtonEmpezarPartida.IsVisibleInTree() ? _ButtonEmpezarPartida : _ButtonEmpezarPartidaSinPerfil);
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.PausarMusica(0.5f);

        _AnimacionCrtShutdown.Reproducir();

        await ToSignal(_AnimacionCrtShutdown, AnimacionCrtShutdown.SignalName.AnimacionFinalizada);
        await Task.Delay(300);

        this.GetTree().Quit();
    }

    override public void _ExitTree()
    {
        base._ExitTree();

        this.VisibilityChanged -= OnVisibilityChanged;
        Global.Instancia.OnCambioPerfilActivo -= CalcularEstadosBotonesSinPerfil;
    }
}
