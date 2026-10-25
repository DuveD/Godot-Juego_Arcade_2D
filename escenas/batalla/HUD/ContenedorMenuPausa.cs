using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla.HUD;

public partial class ContenedorMenuPausa : ContenedorMenu
{
    public ButtonPersonalizado ButtonRenaudar;
    public ButtonPersonalizado ButtonAjustes;
    public ButtonPersonalizado ButtonSalir;

    public override void _Ready()
    {
        ButtonRenaudar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonRenaudar");
        ButtonAjustes = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAjustes");
        ButtonSalir = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonSalir");

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.IsVisibleInTree())
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(ButtonRenaudar);
            AcceptEvent();
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ButtonRenaudar, ButtonAjustes, ButtonSalir];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ButtonRenaudar;
    }
}
