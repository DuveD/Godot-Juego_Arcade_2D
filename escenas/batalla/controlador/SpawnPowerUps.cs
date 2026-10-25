
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class SpawnPowerUps : Control
{
    // Intentos máximos para encontrar una posición libre; así nunca se bloquea el juego si no queda hueco.
    private const int MAX_INTENTOS_POSICION = 30;

    [Export]
    long _TiempoSpawn = 6;

    [Export]
    public int ProbabilidadSpawnPowerUps = 50;

    [Export]
    public PackedScene PowerUpImanMonedaPackedScene;

    [Export]
    public PackedScene PowerUpInvulnerabilidadPackedScene;

    [Export]
    public PackedScene BotiquinPackedScene;

    [Export]
    public int DistanciaMinima = 200;

    private Jugador Jugador { get; set; }

    private Timer _TimerSpawn;

    public Node NodoContenedorPowerUps => this.GetParent().GetParent();

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public void IniciarSpawnLoop(Jugador jugador)
    {
        this.Jugador = jugador;
        InicializarTimer();
    }

    public void InicializarTimer()
    {
        this._TimerSpawn = new Timer();
        this._TimerSpawn.WaitTime = _TiempoSpawn;

        this._TimerSpawn.Timeout += SpawnPowerUp;

        this.AddChild(this._TimerSpawn);
        this._TimerSpawn.Start();
    }

    public void ReiniciarTimer()
    {
        if (_TimerSpawn == null)
            return;

        this._TimerSpawn.Stop();
        this._TimerSpawn.WaitTime = _TiempoSpawn;
        this._TimerSpawn.Start();
    }

    private void SpawnPowerUp()
    {
        List<PackedScene> posibles = new();

        // Siempre permitido
        posibles.Add(PowerUpImanMonedaPackedScene);
        posibles.Add(PowerUpInvulnerabilidadPackedScene);

        // Solo si NO tiene la vida al máximo
        if (this.Jugador.Vida < this.Jugador.VidaMaxima)
            posibles.Add(BotiquinPackedScene);

        // Seguridad: si por algún motivo no hay ninguno
        if (posibles.Count == 0)
            return;

        int index = Randomizador.GetRandomInt(0, posibles.Count - 1);
        SpawnPowerUp(posibles[index]);

        ReiniciarTimer();
    }

    private void SpawnPowerUp(PackedScene powerUpPackedScene)
    {
        // Si no hay hueco, este power-up no se coloca: el timer volverá a intentarlo en el siguiente ciclo.
        if (!ObtenerPosicionAleatoriaSegura(out Vector2 posicion))
        {
            LoggerJuego.Warn("No hay posición libre para el PowerUp; se intentará en el siguiente ciclo.");
            return;
        }

        LoggerJuego.Trace("Spawneamos un nuevo PowerUp.");

        // Lo hacemos Consumible para los powerup cómo la vida, que no tienen efecto continuo, sólo al recogerse.
        Consumible powerUp = powerUpPackedScene.Instantiate<Consumible>();
        powerUp.Position = posicion;

        NodoContenedorPowerUps.AddChild(powerUp);
    }

    private bool ObtenerPosicionAleatoriaSegura(out Vector2 posicion)
    {
        Vector2 centroJugador = Jugador.GlobalPosition;
        List<Consumible> consumibles = UtilidadesNodos.ObtenerNodosDeTipo<Consumible>(GetTree().CurrentScene);
        Rect2 areaSpawn = new(GlobalPosition, Size);

        for (int intento = 0; intento < MAX_INTENTOS_POSICION; intento++)
        {
            Vector2 nuevaPos = Randomizador.ObtenerPosicionAleatoria(areaSpawn);

            bool cercaJugador = UtilidadesMatematicas.PuntosCerca(centroJugador, nuevaPos, this.DistanciaMinima);
            bool cercaOtroConsumible = consumibles.Any(consumible => UtilidadesMatematicas.PuntosCerca(consumible.Position, nuevaPos, consumible.ObtenerRadioCollisionShape2D() * 2));

            if (!cercaJugador && !cercaOtroConsumible)
            {
                posicion = nuevaPos;
                return true;
            }
        }

        posicion = Vector2.Zero;
        return false;
    }
}
