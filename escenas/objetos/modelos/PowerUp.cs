using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.entidades.atributo;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.modelos;

public abstract partial class PowerUp : Consumible
{
    public enum TipoAcumulacionPowerUp
    {
        NoAcumulable,
        RenuevaDuracion,
        AcumulaEfecto
    }

    // Si es -1 es permanente.
    [Export]
    public float TiempoDuracion { get; set; } = -1f;

    [Export]
    public TipoAcumulacionPowerUp TipoAcumulacion;

    [Export]
    public bool PermiteDuplicados = false;

    public Timer TimerDuracionPowerUp;

    private bool _anadidoAJugador = false;

    public virtual List<ModificadorAtributo<T>> ObtenerModificadoresAtributos<T>()
    {
        return [];
    }

    public override bool OnRecogida(Jugador jugador)
    {
        LoggerJuego.Info("PowerUp " + this.Name + " recogido.");

        if (jugador.TienePowerUpDeTipo(this.GetType()))
        {
            switch (TipoAcumulacion)
            {
                default:
                case TipoAcumulacionPowerUp.NoAcumulable:
                    OnRecogidaNoAcumulable(jugador);
                    break;
                case TipoAcumulacionPowerUp.RenuevaDuracion:
                    OnRecogidaRenuevaDuracion(jugador);
                    break;
                case TipoAcumulacionPowerUp.AcumulaEfecto:
                    OnRecogidaAcumulaEfecto(jugador);
                    break;
            }
        }
        else
        {
            AnadirPowerUpAJugador(jugador);
        }

        // Evitamos que se vuelva a disparar la señal.
        CollisionShape2D?.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        this.SetDeferred(Area2D.PropertyName.Monitoring, false);

        // Ocultamos el Sprite del consumible.
        Sprite2D.SetDeferred(CanvasItem.PropertyName.Visible, false);

        // Si este nodo pasa a ser el power-up activo del jugador, no se elimina; si solo ha
        // renovado o acumulado sobre el que ya estaba activo, ya no sirve y se elimina.
        return !_anadidoAJugador;
    }

    private void AnadirPowerUpAJugador(Jugador jugador)
    {
        _anadidoAJugador = true;
        InicializarTimerDuracion(jugador);
        jugador.CallDeferred(nameof(Jugador.AnadirPowerUp), this);
        AplicarEfectoPowerUp(jugador);
    }

    public virtual void OnRecogidaNoAcumulable(Jugador jugador)
    {
    }

    public virtual void OnRecogidaRenuevaDuracion(Jugador jugador)
    {
        LoggerJuego.Info("PowerUp " + this.Name + " recogido: renovando duración.");
        PowerUp powerUp = jugador.ObtenerPowerUp(this.GetType());
        powerUp?.ReiniciarTimer();
    }

    public virtual void OnRecogidaAcumulaEfecto(Jugador jugador)
    {
        if (this.PermiteDuplicados)
        {
            LoggerJuego.Info("PowerUp " + this.Name + " recogido: duplicando efecto.");
            AnadirPowerUpAJugador(jugador);
        }
        else
        {
            LoggerJuego.Info("PowerUp " + this.Name + " recogido: acumulando efecto.");
            // El tiempo se suma al power-up que ya está activo, no a este (que no tiene timer).
            PowerUp powerUp = jugador.ObtenerPowerUp(this.GetType());
            powerUp?.SumarTiempoTimer(this.TiempoDuracion);
        }
    }

    private void InicializarTimerDuracion(Jugador jugador)
    {
        if (this.TiempoDuracion > 0)
        {
            this.TimerDuracionPowerUp = new Timer
            {
                WaitTime = this.TiempoDuracion,
                OneShot = true,
                Autostart = true,
            };

            this.TimerDuracionPowerUp.Timeout += () => OnTimerDuracionTimeout(jugador);
            AddChild(this.TimerDuracionPowerUp);
        }
    }

    public void ReiniciarTimer()
    {
        if (TimerDuracionPowerUp == null || !IsInstanceValid(TimerDuracionPowerUp))
            return;

        this.TimerDuracionPowerUp.Stop();
        this.TimerDuracionPowerUp.WaitTime = TiempoDuracion;
        this.TimerDuracionPowerUp.Start();
    }

    public void SumarTiempoTimer(float tiempo)
    {
        if (TimerDuracionPowerUp == null || !IsInstanceValid(TimerDuracionPowerUp))
            return;

        // Sumamos al tiempo que queda, no a la duración total.
        double tiempoRestante = this.TimerDuracionPowerUp.TimeLeft;
        this.TimerDuracionPowerUp.Stop();
        this.TimerDuracionPowerUp.Start(tiempoRestante + tiempo);
    }

    public abstract void AplicarEfectoPowerUp(Jugador jugador);

    public abstract void ProcessPowerUp(double delta, Jugador jugador);

    public void OnTimerDuracionTimeout(Jugador jugador)
    {
        LoggerJuego.Info("PowerUp " + this.Name + " expirado.");
        jugador.QuitarPowerUp(this);
        EfectoPowerUpExpirado(jugador);
    }

    public abstract void EfectoPowerUpExpirado(Jugador jugador);
}