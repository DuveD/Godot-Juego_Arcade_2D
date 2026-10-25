using Godot;

namespace Primerjuego2D.escenas.ui.overlays;

public partial class IndicadorCarga : CanvasLayer
{
    [Export]
    public float VelocidadRotacion = 180f; // grados por segundo

    [Export]
    public float TiempoMinimoVisible = 1.0f; // segundos

    private TextureRect _icono;
    private int _numeroCargas = 0;
    private double _momentoMostrado = 0;

    public override void _Ready()
    {
        _icono = GetNode<TextureRect>("MarginContainer/Control/TextureRect");
        EsconderInmediato();
    }

    public override void _Process(double delta)
    {
        if (_icono != null && Visible)
        {
            _icono.RotationDegrees += VelocidadRotacion * (float)delta;
        }
    }

    public void Mostrar()
    {
        _numeroCargas++;

        if (_numeroCargas == 1)
        {
            Visible = true;
            SetProcess(true);
            _momentoMostrado = Time.GetTicksMsec() / 1000.0;
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
            {
                await ToSignal(GetTree().CreateTimer(tiempoRestante), "timeout");
            }

            // Si durante la espera ha empezado otra carga, lo dejamos visible.
            if (_numeroCargas > 0)
                return;

            EsconderInmediato();
        }
    }

    private void EsconderInmediato()
    {
        Visible = false;
        SetProcess(false);
    }
}
