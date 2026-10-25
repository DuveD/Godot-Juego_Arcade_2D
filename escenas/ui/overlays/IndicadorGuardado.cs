using Godot;

namespace Primerjuego2D.escenas.ui.overlays;

public partial class IndicadorGuardado : CanvasLayer
{
    [Export] public float Intensidad = 3f;          // amplitud movimiento
    [Export] public float Frecuencia = 8f;          // velocidad oscilación
    [Export] public float Squash = 0.05f;           // compresión vertical (0.05 = 5%)
    [Export] public float TiempoMinimoVisible = 1.0f;

    private TextureRect _icono;
    private Vector2 _posicionOriginal;
    private Vector2 _escalaOriginal;

    private int _numeroCargas = 0;
    private double _momentoMostrado = 0;
    private double _tiempo = 0;

    public override void _Ready()
    {
        _icono = GetNode<TextureRect>("MarginContainer/Control/TextureRect");

        _posicionOriginal = _icono.Position;
        _escalaOriginal = _icono.Scale;

        EsconderInmediato();
    }

    public override void _Process(double delta)
    {
        if (!Visible)
            return;

        _tiempo += delta * Frecuencia;

        float offsetX = Mathf.Sin((float)_tiempo) * Intensidad;
        float offsetY = Mathf.Cos((float)(_tiempo * 1.3f)) * Intensidad * 0.7f;

        _icono.Position = _posicionOriginal + new Vector2(offsetX, offsetY);

        // Squash suave vertical
        float squashFactor = 1f - (Mathf.Sin((float)_tiempo) * Squash);
        _icono.Scale = new Vector2(_escalaOriginal.X, _escalaOriginal.Y * squashFactor);
    }

    public void Mostrar()
    {
        _numeroCargas++;

        if (_numeroCargas == 1)
        {
            Visible = true;
            SetProcess(true);
            _momentoMostrado = Time.GetTicksMsec() / 1000.0;
            _tiempo = 0;
        }
    }

    public async void Esconder()
    {
        _numeroCargas--;

        if (_numeroCargas <= 0)
        {
            _numeroCargas = 0;

            double tiempoTranscurrido = (Time.GetTicksMsec() / 1000.0) - _momentoMostrado;
            double tiempoRestante = TiempoMinimoVisible - tiempoTranscurrido;

            if (tiempoRestante > 0)
                await ToSignal(GetTree().CreateTimer(tiempoRestante), "timeout");

            // Si durante la espera ha empezado otro guardado, lo dejamos visible.
            if (_numeroCargas > 0)
                return;

            EsconderInmediato();
        }
    }

    private void EsconderInmediato()
    {
        Visible = false;
        SetProcess(false);

        if (_icono != null)
        {
            _icono.Position = _posicionOriginal;
            _icono.Scale = _escalaOriginal;
        }
    }
}
