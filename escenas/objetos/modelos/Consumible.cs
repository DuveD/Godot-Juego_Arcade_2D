using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.modelos;

public abstract partial class Consumible : Area2D
{
    public Sprite2D Sprite2D;

    public CollisionShape2D CollisionShape2D;

    // Si es -1, no se autodestruye. Si >0, se destruye automáticamente después de ese tiempo.
    [Export]
    public long TiempoDestruccion { get; set; } = -1;

    [Export]
    public float AlphaMin = 0.1f;

    [Export]
    public float AlphaMax = 0.8f;

    [Export]
    public float VelocidadParpadeoFinal = 8f; // Oscilación

    public double DuracionParpadeo = 1;       // Últimos segundos donde parpadea

    public Timer TimerDestruccion;

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>("Sprite2D");
        CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

        BodyEntered += OnBodyEntered;

        ConfigurarTimerDestruccion();
    }

    private void ConfigurarTimerDestruccion()
    {       // Configuramos timer de autodestrucción

        if (TiempoDestruccion > 0)
        {
            TimerDestruccion = new Timer
            {
                WaitTime = TiempoDestruccion,
                OneShot = true,
                Autostart = true
            };
            TimerDestruccion.Timeout += OnTimerDestruccionTimeout;
            AddChild(TimerDestruccion);
        }
    }

    public override void _Process(double delta)
    {
        if (this.TimerDestruccion == null)
            return;

        double timeLeft = this.TimerDestruccion.TimeLeft;

        // Fase final
        if (timeLeft <= 1)
        {
            double tiempoFinal = DuracionParpadeo - timeLeft;

            // Onda rápida
            float onda =
                (Mathf.Sin((float)tiempoFinal * VelocidadParpadeoFinal * Mathf.Pi * 2f) + 1f) * 0.5f;

            float alpha = Mathf.Lerp(AlphaMin, AlphaMax, onda);

            // Aplica transparencia al Sprite2D
            Color color = this.Sprite2D.Modulate;
            color.A = alpha;
            this.Sprite2D.Modulate = color;
        }
    }

    private void OnTimerDestruccionTimeout()
    {
        LoggerJuego.Trace(this.Name + " autodestruido tras " + TiempoDestruccion + " segundos.");

        // Usamos CallDeferred para que no choque con signals o procesamiento actual.

        CallDeferred(Node.MethodName.QueueFree);
    }
    private void DetenerYEliminarTimer()
    {
        if (TimerDestruccion == null)
            return;

        TimerDestruccion.Stop();
        TimerDestruccion.QueueFree();
        TimerDestruccion = null;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Jugador jugador)
        {
            DetenerYEliminarTimer();
            if (jugador.Muerto)
            {
                LoggerJuego.Warn("Consumible " + this.Name + " recogido con el jugador muerto.");
            }
            else
            {
                OnRecogidaInterno(jugador);
            }
        }
    }

    public virtual void OnRecogidaInterno(Jugador jugador)
    {
        bool eliminarNodo = OnRecogida(jugador);

        // Cancelamos el timer si estaba activo.
        TimerDestruccion?.Stop();

        if (eliminarNodo)
        {
            // Usamos CallDeferred para evitar conflictos si el spawn ocurre durante la señal.
            CallDeferred(Node.MethodName.QueueFree);
        }
    }

    public abstract bool OnRecogida(Jugador jugador);

    public float ObtenerRadioCollisionShape2D()
    {
        return ((CircleShape2D)CollisionShape2D?.Shape)!.Radius;
    }
}
