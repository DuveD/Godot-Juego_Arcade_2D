using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo.animaciones;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.entidades.atributo;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.powerup;

public partial class Invulnerabilidad : PowerUp
{
    [Export]
    private float AlphaMinJudaor = 0.1f;

    [Export]
    private float AlphaMaxJudador = 0.8f;

    [Export]
    private float VelocidadParpadeoFinalJugador = 15f; // Oscilación muy rápida al final

    [Export]
    private double DuracionParpadeoJugador = 1;       // Último tramo donde parpadea

    private EfectoEstelaSprite2D _estelaJugador;

    private ModificadorAtributo<long> _modificadorVelocidad = new ModificadorAtributo<long>(nameof(Jugador.Velocidad), 100, (valor) => valor + 100);

    private ModificadorAtributo<bool> _modificadorVInvulnerable = new ModificadorAtributo<bool>(nameof(Jugador.Invulnerable), false, (_) => true);

    public override List<ModificadorAtributo<T>> ObtenerModificadoresAtributos<T>()
    {
        var todos = new List<object>
        {
            _modificadorVelocidad,
            _modificadorVInvulnerable
        };

        return todos.OfType<ModificadorAtributo<T>>().ToList();
    }

    public override void _Ready()
    {
        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
        InvalidarAtributosModificados(jugador);

        // Al principio solo azul fijo
        PonerColorAzurJugador(jugador);

        PonerEstelaJugador(jugador);
    }

    private void PonerEstelaJugador(Jugador jugador)
    {
        _estelaJugador = new EfectoEstelaSprite2D();
        _estelaJugador.Inicializar(jugador.AnimatedSprite2D, 0.05f);

        jugador.AddChild(_estelaJugador);
        _estelaJugador.Activar();
    }

    private static void InvalidarAtributosModificados(Jugador jugador)
    {
        jugador.Velocidad.InvalidarValor();
        jugador.Invulnerable.InvalidarValor();
    }

    private static void PonerColorAzurJugador(Jugador jugador)
    {
        jugador?.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(ConstantesColores.AZUL_CLARO));
        jugador?.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
    }
    private static void RestaurarColorJugador(Jugador jugador)
    {
        jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
        jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(1, 1, 1));
    }

    public override void ProcessPowerUp(double delta, Jugador jugador)
    {
        // Si es permanente o no hay timer, efecto fijo
        if (this.TimerDuracionPowerUp == null || TiempoDuracion <= 0)
        {
            PonerColorAzurJugador(jugador);
        }
        else
        {
            float timeLeft = (float)this.TimerDuracionPowerUp.TimeLeft;

            // Fase final
            if (timeLeft <= DuracionParpadeoJugador)
            {
                double tiempoFinal = DuracionParpadeoJugador - timeLeft;

                // Onda rápida
                float onda =
                    (Mathf.Sin((float)tiempoFinal * VelocidadParpadeoFinalJugador * Mathf.Pi * 2f) + 1f) * 0.5f;

                Color color = new Color(ConstantesColores.AZUL_CLARO)
                    .Lerp(new Color(1, 1, 1), onda);

                jugador.CallDeferred(nameof(Jugador.SetSpriteColor), color);

                float alpha = Mathf.Lerp(AlphaMinJudaor, AlphaMaxJudador, onda);
                jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), alpha);
            }
            else
            {
                // Fase estable
                PonerColorAzurJugador(jugador);
            }
        }
    }

    public override void EfectoPowerUpExpirado(Jugador jugador)
    {
        // Restauramos visibilidad total y color normal
        RestaurarColorJugador(jugador);

        InvalidarAtributosModificados(jugador);

        // Quitamos estela
        QuitarEstelaJugador(jugador);
    }

    private void QuitarEstelaJugador(Jugador jugador)
    {
        if (_estelaJugador != null)
        {
            _estelaJugador.Desactivar();
            _estelaJugador.QueueFree();
            jugador.RemoveChild(_estelaJugador);
            _estelaJugador = null;
        }
    }
}
