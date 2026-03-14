using Godot;
using Primerjuego2D.nucleo.utilidades.log;
using ButtonPersonalizado = Primerjuego2D.escenas.ui.controles.ButtonPersonalizado;

namespace Primerjuego2D.escenas.menuPrincipal.principal;

public partial class ButtonEmpezarPartida : ButtonPersonalizado
{
	[Signal]
	public delegate void PressedAnimationEndEventHandler();

	[Export]
	public int NumeroParpadeos = 4;

	[Export]
	public float IntervaloEntreParpadeos = 0.10f;

	private Tween _tween;
	private bool _blinking = false;

	private StyleBox _focusStyle;
	private StyleBox _hoverStyle;
	private StyleBox _normalStyle;
	private StyleBox _pressedStyle;

	public override void _Ready()
	{
		this.Pressed += StartBlink;

		_focusStyle = GetThemeStylebox("focus")?.Duplicate() as StyleBox;
		_hoverStyle = GetThemeStylebox("hover")?.Duplicate() as StyleBox;
		_normalStyle = GetThemeStylebox("normal")?.Duplicate() as StyleBox;
		_pressedStyle = GetThemeStylebox("pressed")?.Duplicate() as StyleBox;

		base._Ready();

		LoggerJuego.Trace(this.Name + " Ready.");
	}

	private void StartBlink()
	{
		if (_blinking) return;
		_blinking = true;

		Global.GestorAudio.ReproducirSonido("digital_click.mp3");

		// Limpiar Tween anterior
		_tween?.Kill();

		_tween = CreateTween();
		_tween.SetLoops(NumeroParpadeos);

		// Alterna focus → pressed → focus → pressed...
		_tween.TweenCallback(Callable.From(() =>
			{
				AddThemeStyleboxOverride("focus", _pressedStyle);
				AddThemeStyleboxOverride("hover", _pressedStyle);
				AddThemeStyleboxOverride("normal", _pressedStyle);
				AddThemeStyleboxOverride("pressed", _pressedStyle);
			}))
			.SetDelay(IntervaloEntreParpadeos);

		_tween.TweenCallback(Callable.From(() =>
			{
				AddThemeStyleboxOverride("focus", _focusStyle);
				AddThemeStyleboxOverride("hover", _focusStyle);
				AddThemeStyleboxOverride("normal", _focusStyle);
				AddThemeStyleboxOverride("pressed", _focusStyle);
			}))
			.SetDelay(IntervaloEntreParpadeos);

		// Cuando acaba el tween, restauramos todo
		_tween.Finished += () =>
		{
			AddThemeStyleboxOverride("focus", _focusStyle);
			AddThemeStyleboxOverride("hover", _hoverStyle);
			AddThemeStyleboxOverride("normal", _normalStyle);
			AddThemeStyleboxOverride("pressed", _pressedStyle);

			_blinking = false;

			EmitSignal(SignalName.PressedAnimationEnd);
			this.Disabled = false;
		};
	}
}