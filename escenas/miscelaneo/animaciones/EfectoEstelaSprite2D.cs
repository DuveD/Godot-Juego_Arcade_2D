using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo.animaciones;

public partial class EfectoEstelaSprite2D : Node2D
{
    [Export]
    public float DuracionEstela = 0.3f;

    public Node2D Sprite { get; private set; }

    public float Intervalo { get; set; } = 0.05f;

    private List<Node2D> _spritesEstela = new();

    public bool Activo
    {
        get => _activo;
        set
        {
            _activo = value;
            SetProcess(value);
        }
    }
    private bool _activo = false;

    private double _tiempo;

    public override void _Ready()
    {
        SetProcess(false);
    }

    public void Inicializar(Node2D sprite, float intervalo = 0.05f)
    {
        Sprite = sprite;
        Intervalo = intervalo;
    }

    public override void _Process(double delta)
    {
        if (!Activo || Sprite == null || !Sprite.Visible)
            return;

        _tiempo += delta;
        if (_tiempo >= Intervalo)
        {
            _tiempo = 0;
            EmitirEstela();
        }
    }

    public override void _ExitTree()
    {
        EliminarEstela();
    }

    private void EmitirEstela()
    {
        if (Sprite is AnimatedSprite2D animatedSprite2D)
            EmitirEstelaAnimatedSprite2D(animatedSprite2D);
        else if (Sprite is Sprite2D sprite2D)
            EmitirEstelaSprite2D(sprite2D);
        else
            LoggerJuego.Warn("Tipo de nodo no soportado para emitir estela: " + Sprite.GetClass());
    }

    private void EmitirEstelaAnimatedSprite2D(AnimatedSprite2D animatedSprite2D)
    {
        AnimatedSprite2D estelaAnimatedSprite2D = new()
        {
            TopLevel = true,

            GlobalPosition = animatedSprite2D.GlobalPosition,
            GlobalRotation = animatedSprite2D.GlobalRotation,
            Scale = animatedSprite2D.Scale,

            SpriteFrames = animatedSprite2D.SpriteFrames,
            Animation = animatedSprite2D.Animation,

            Frame = animatedSprite2D.Frame,
            FlipH = animatedSprite2D.FlipH,
            FlipV = animatedSprite2D.FlipV,
            Modulate = animatedSprite2D.Modulate
        };

        AddChild(estelaAnimatedSprite2D);
        _spritesEstela.Add(estelaAnimatedSprite2D);
        AplicarTween(estelaAnimatedSprite2D);
    }

    private void EmitirEstelaSprite2D(Sprite2D sprite2D)
    {
        Sprite2D estelaSprite2D = new()
        {
            TopLevel = true,

            Texture = sprite2D.Texture,

            GlobalPosition = sprite2D.GlobalPosition,
            GlobalRotation = sprite2D.GlobalRotation,
            Scale = sprite2D.Scale,

            Frame = sprite2D.Frame,
            FlipH = sprite2D.FlipH,
            FlipV = sprite2D.FlipV,
            Modulate = sprite2D.Modulate
        };

        AddChild(estelaSprite2D);
        _spritesEstela.Add(estelaSprite2D);
        AplicarTween(estelaSprite2D);
    }

    private void AplicarTween(Node2D estela)
    {
        var tween = CreateTween();
        tween.TweenProperty(estela, "modulate:a", 0f, DuracionEstela);
        tween.TweenCallback(Callable.From(() => LiberarSpriteEstela(estela)));
    }

    private void LiberarSpriteEstela(Node2D estela)
    {
        _spritesEstela.Remove(estela);
        if (IsInstanceValid(estela))
            estela.QueueFree();
    }

    public void Activar()
    {
        _tiempo = 0;
        Activo = true;
    }

    public void Desactivar()
    {
        Activo = false;
        EliminarEstela();
    }

    public void EliminarEstela()
    {
        foreach (var estela in _spritesEstela.ToList())
            LiberarSpriteEstela(estela);
    }
}
