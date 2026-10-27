using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.batalla.HUD;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.consumible;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaControlador : Control
{
    [Signal]
    public delegate void PreparandoBatallaEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    [Signal]
    public delegate void PausarBatallaEventHandler();

    [Signal]
    public delegate void RenaudarBatallaEventHandler();

    [Signal]
    public delegate void PuntuacionActualizadaEventHandler(int Puntuacion);

    public int Puntuacion { get; private set; } = 0;

    public bool BatallaEnCurso { get; private set; } = false;

    public bool _JuegoPausado = false;

    public bool JuegoPausado
    {
        get => _JuegoPausado;
        set
        {
            _JuegoPausado = value;
            UtilidadesNodos.PausarNodo(this, value);

            if (value)
            {
                LoggerJuego.Trace("Juego pausado.");
                EmitSignal(SignalName.PausarBatalla);
            }
            else
            {
                LoggerJuego.Trace("Juego renaudado.");
                EmitSignal(SignalName.RenaudarBatalla);
            }
        }
    }

    private BatallaHUD BatallaHUD;

    private SpawnEnemigos SpawnEnemigos;

    private SpawnMonedas SpawnMonedas;

    private SpawnPowerUps SpawnPowerUps;

    private Jugador Jugador;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.BatallaHUD = GetNode<BatallaHUD>("../BatallaHUD");
        this.SpawnEnemigos = GetNode<SpawnEnemigos>("SpawnEnemigos");
        this.SpawnMonedas = GetNode<SpawnMonedas>("SpawnMonedas");
        this.SpawnPowerUps = GetNode<SpawnPowerUps>("SpawnPowerUps");
        this.Jugador = GetNode<Jugador>("../Jugador");

        GestorEstadisticas.InicializarPartida();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            PausarJuego();
            AcceptEvent();
        }
    }

    public void PausarJuego()
    {
        if (!this.BatallaEnCurso)
            return;

        this.JuegoPausado = true;
    }

    public void RenaudarJuego()
    {
        if (!this.BatallaEnCurso)
            return;

        this.JuegoPausado = false;
    }

    public async void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        LoggerJuego.Info("Preparando la batalla.");
        EmitSignal(SignalName.PreparandoBatalla);

        this.Puntuacion = 0;

        EmitSignal(SignalName.PuntuacionActualizada, Puntuacion);

        SpawnMonedas.IniciarSpawnLoop(this.Jugador);
        SpawnPowerUps.IniciarSpawnLoop(this.Jugador);

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        LoggerJuego.Info("Batalla iniciada.");
        EmitSignal(SignalName.BatallaIniciada);

        this.BatallaEnCurso = true;
    }

    public void FinalizarBatalla()
    {
        if (!this.BatallaEnCurso)
            return;

        BatallaEnCurso = false;

        GestorEstadisticas.PartidaActual.RegistrarPuntuacion(this.Puntuacion);
        GestorEstadisticas.FinalizarPartida(Global.PerfilActivo);
        Global.PerfilActivo.FechaUltimaPartida = DateTime.UtcNow;

        // Procesamos los logros antes de guardar para que un único guardado incluya todo.
        GestorLogros.EmitirEvento(Global.PerfilActivo, DefinicionLogros.EVENTO_LOGRO_PRIMERA_PARTIDA);
        Global.GuardarPerfilActivo();

        LoggerJuego.Info("Batalla finalizada.");
        EmitSignal(SignalName.BatallaFinalizada);
    }

    public void SumarPuntuacion(Moneda moneda)
    {
        if (moneda == null)
        {
            LoggerJuego.Error("La moneda recogida es nula.");
            return;
        }

        if (GestorEstadisticas.PartidaActual == null)
        {
            LoggerJuego.Error("Moneda recogida después de terminar la partida..");
            return;
        }

        this.Puntuacion += moneda.Valor;
        GestorEstadisticas.PartidaActual.RegistrarMoneda(moneda is MonedaEspecial);

        EmitSignal(SignalName.PuntuacionActualizada, this.Puntuacion);
    }
}
