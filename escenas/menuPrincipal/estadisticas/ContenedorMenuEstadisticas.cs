using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.estadisticas;

public partial class ContenedorMenuEstadisticas : ContenedorMenu
{
    private Label _LabelPartidasJugadasValor;
    private Label _LabelMejorPuntuacionValor;
    private Label _LabelMonedasRecogidasValor;
    private Label _LabelMonedasEspecialesRecogidasValor;
    private Label _LabelEnemigosDerrotadosValor;

    private ButtonPersonalizado _ButtonAtras;

    public override void _Ready()
    {
        _LabelPartidasJugadasValor = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelPartidasJugadasValor");
        _LabelMejorPuntuacionValor = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMejorPuntuacionValor");
        _LabelMonedasRecogidasValor = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMonedasRecogidasValor");
        _LabelMonedasEspecialesRecogidasValor = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMonedasEspecialesRecogidasValor");
        _LabelEnemigosDerrotadosValor = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelEnemigosDerrotadosValor");
        _ButtonAtras = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override void OnMenuVisible()
    {
        base.OnMenuVisible();

        CargarEstadisticas();
    }

    public void CargarEstadisticas()
    {
        if (Global.PerfilActivo == null)
            return;

        EstadisticasGlobales estadisticasGlobalesPartida = Global.PerfilActivo.EstadisticasGlobales;

        _LabelPartidasJugadasValor.Text = estadisticasGlobalesPartida.PartidasJugadas.ToString();
        _LabelMejorPuntuacionValor.Text = estadisticasGlobalesPartida.MejorPuntuacion.ToString();
        _LabelMonedasRecogidasValor.Text = estadisticasGlobalesPartida.MonedasRecogidas.ToString();
        _LabelMonedasEspecialesRecogidasValor.Text = estadisticasGlobalesPartida.MonedasEspecialesRecogidas.ToString();
        _LabelEnemigosDerrotadosValor.Text = estadisticasGlobalesPartida.EnemigosDerrotados.ToString();
    }


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (!_ButtonAtras.HasFocus())
                _ButtonAtras.GrabFocus();
            else
                UtilidadesNodos.PulsarBoton(_ButtonAtras);

            AcceptEvent();
        }
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _ButtonAtras;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [_ButtonAtras];
    }
}