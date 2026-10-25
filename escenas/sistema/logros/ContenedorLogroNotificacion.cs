using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.sistema.logros;

namespace Primerjuego2D.escenas.sistema.logros;

public partial class ContenedorLogroNotificacion : ContenedorLogro
{
    int _tiempoVisible = 1;

    public void Inicializar(Logro logro, int tiempoVisible = 1)
    {
        base.Inicializar(logro);
        this._tiempoVisible = tiempoVisible;
    }

    public async Task MostrarAsync()
    {
        var viewport = GetViewportRect();
        var margen = 20f;

        // Posición final (visible)
        Vector2 posicionFinal = new(
            viewport.Size.X - Size.X - margen,
            margen
        );

        // Posición inicial (fuera por arriba)
        Vector2 posicionInicial = new(
            posicionFinal.X,
            -Size.Y
        );

        Position = posicionInicial;

        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Cubic);

        // Desplazar hacia abajo para entrar en la pantalla.
        tween.TweenProperty(this, "position", posicionFinal, 0.4f);

        await ToSignal(tween, Tween.SignalName.Finished);

        // Esperar el tiempo configurado.
        await ToSignal(GetTree().CreateTimer(_tiempoVisible), SceneTreeTimer.SignalName.Timeout);

        // Desplazar hacia arriba para salir de la pantalla.
        var tweenSalida = CreateTween();
        tweenSalida.SetEase(Tween.EaseType.In);
        tweenSalida.SetTrans(Tween.TransitionType.Cubic);

        tweenSalida.TweenProperty(this, "position", posicionInicial, 0.4f);

        await ToSignal(tweenSalida, Tween.SignalName.Finished);
    }
}
