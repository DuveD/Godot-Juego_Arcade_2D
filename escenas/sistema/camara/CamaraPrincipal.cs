using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.sistema.camara;

[AtributoNivelLog(NivelLog.Info)]
public partial class CamaraPrincipal : Camera2D
{
    [Export]
    public float Decay = 0.8f; // Tiempo para volver a 0 trauma

    [Export]
    public Vector2 MaxOffset = new Vector2(100, 75); // Límite desplazamiento

    [Export]
    public float MaxRoll = 0.1f; // Rotación máxima

    [Export]
    public Node2D FollowNode; // A quién seguir (jugador)

    private float Trauma = 0f;
    private int TraumaPower = 2;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public void AjustarCamara(Vector2 viewportSize)
    {
        this.GlobalPosition = viewportSize;

        // Sin zoom, para que 1 unidad = 1 píxel
        this.Zoom = Vector2.One;

        // Opcional: limita la cámara dentro de los bordes del mapa
        this.LimitLeft = 0;
        this.LimitTop = 0;
        this.LimitRight = Mathf.RoundToInt(viewportSize.X);
        this.LimitBottom = Mathf.RoundToInt(viewportSize.Y);
    }

    public override void _Process(double delta)
    {
        // Si tenemos nodo a seguir, nos pegamos a él
        if (FollowNode != null)
            this.GlobalPosition = FollowNode.GlobalPosition;

        if (this.Trauma > 0f)
        {
            this.Trauma = Mathf.Max(this.Trauma - Decay * (float)delta, 0f);
            Shake();
        }
    }

    /// <summary>
    /// Añade trauma al sistema (entre 0 y 1). 0.5 es un buen valor para golpes fuertes.
    /// </summary>
    public void AddTrauma(float amount)
    {
        this.Trauma = Mathf.Clamp(this.Trauma + amount, 0f, 1f);
        LoggerJuego.Trace("Trauma de la cámara a: " + this.Trauma);
    }

    private void Shake()
    {
        LoggerJuego.Trace("Shacke de Camara.");

        float amount = Mathf.Pow(this.Trauma, TraumaPower);

        Rotation = MaxRoll * amount * Randomizador.GetRandomFloatGodot(-1f, 1f);

        Offset = new Vector2(
            MaxOffset.X * amount * Randomizador.GetRandomFloatGodot(-1f, 1f),
            MaxOffset.Y * amount * Randomizador.GetRandomFloatGodot(-1f, 1f)
        );
    }
}
