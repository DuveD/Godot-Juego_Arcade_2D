using System;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.consumible;

public partial class Moneda : Consumible
{
    [Signal]
    public delegate void RecogidaEventHandler(Moneda moneda);

    [Export]
    public int Valor { get; set; } = 1;

    [Export]
    public float VelocidadAnimacion { get; set; } = 1.0f;

    [Export]
    public Color ColorTextoFlotante { get; set; } = Colors.Gold;

    public PackedScene TextoFlotanteScene;

    public AnimationPlayer AnimationPlayerRotacion;

    public override void _Ready()
    {
        this.AnimationPlayerRotacion = GetNode<AnimationPlayer>("AnimationPlayerRotacion");

        UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.OBJETOS);

        this.AnimationPlayerRotacion.SpeedScale = this.VelocidadAnimacion;

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override bool OnRecogida(Jugador jugador)
    {
        LoggerJuego.Info("Moneda (" + this.Valor + ") recogida.");

        EmitSignal(SignalName.Recogida, this);

        Global.GestorAudio.ReproducirSonido("retro_coin.mp3");

        MostrarTextoFlotante();

        return true;
    }

    public virtual void MostrarTextoFlotante()
    {
        var texto = TextoFlotanteScene.Instantiate<TextoFlotante>();

        texto.Texto = " +" + this.Valor.ToString();
        texto.Color = ColorTextoFlotante;
        texto.PosicionGlobal = GlobalPosition;

        GetTree().CurrentScene.AddChild(texto);
    }
}
