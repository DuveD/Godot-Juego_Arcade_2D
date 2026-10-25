using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.controles;

public partial class ButtonPersonalizado : Button, IFocusSilencioso
{
    [Export]
    public string NombreSonidoOnFocusEntered { get; set; }
    [Export]
    public string NombreSonidoOnMouseEntered { get; set; }
    [Export]
    public string NombreSonidoOnPressed { get; set; }

    public new bool Disabled
    {
        get => base.Disabled;
        set
        {
            if (base.Disabled == value)
                return;

            base.Disabled = value;

            OnDisabledChanged(value);
        }
    }

    private bool _reproducirSonido = true;

    private bool _tieneFocus;
    private bool _tieneHover;
    private bool _estaResaltado;

    private Tween _tweenAumentar;
    private Vector2 _escalaNormal = Vector2.One;
    private Vector2 _escalaHover = new Vector2(1.1f, 1.1f);

    public override void _Ready()
    {
        this.FocusEntered += OnFocusedEntered;
        this.FocusExited += OnFocusExited;
        this.MouseEntered += OnMouseEntered;
        this.MouseExited += OnMouseExited;
        this.Pressed += OnPressed;    // Centrar pivote después de que el layout esté calculado

        OnNotificationResized();
        OnDisabledChanged(this.Disabled);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            OnNotificationResized();
        }
    }

    private void OnNotificationResized()
    {
        CentrarPivote();
    }

    private void CentrarPivote()
    {
        PivotOffset = Size / 2f;
    }


    private void OnDisabledChanged(bool disabled)
    {
        if (disabled)
        {
            FocusMode = FocusModeEnum.None;
            MouseFilter = MouseFilterEnum.Ignore;
        }
        else
        {
            FocusMode = FocusModeEnum.All;
            MouseFilter = MouseFilterEnum.Pass;
        }
    }

    public void OnFocusedEntered()
    {
        _tieneFocus = true;

        if (this._reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnFocusEntered))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnFocusEntered);

        CambiarEstadoVisual();
    }

    private void OnFocusExited()
    {
        _tieneFocus = false;

        CambiarEstadoVisual();
    }

    public void OnMouseEntered()
    {
        _tieneHover = true;

        if (!string.IsNullOrEmpty(NombreSonidoOnMouseEntered))
        {
            if (!this.Disabled)
                Global.GestorAudio.ReproducirSonido(NombreSonidoOnMouseEntered);
        }

        CambiarEstadoVisual();
    }

    public void OnMouseExited()
    {
        _tieneHover = false;

        CambiarEstadoVisual();
    }

    private void OnPressed()
    {
        LoggerJuego.Trace("Botón '" + this.Name + "' pulsado.");

        _tweenAumentar?.Kill();

        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(0.95f, 0.95f), 0.05f);

        Vector2 escalaFinal = (_tieneFocus || _tieneHover)
            ? _escalaHover
            : _escalaNormal;

        tween.TweenProperty(this, "scale", escalaFinal, 0.08f);

        if (_reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnPressed))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnPressed);
    }

    public void GrabFocusSilencioso()
    {
        this._reproducirSonido = false;
        this.GrabFocus();
        this._reproducirSonido = true;
    }

    private void CambiarEstadoVisual()
    {
        if (Disabled)
            return;

        bool debeResaltarse = _tieneFocus || _tieneHover;

        if (debeResaltarse == _estaResaltado)
            return;

        _estaResaltado = debeResaltarse;

        if (_estaResaltado)
            AnimacionAumentar();
        else
            AnimacionReducir();
    }

    private void AnimacionAumentar()
    {
        _tweenAumentar?.Kill();
        _tweenAumentar = CreateTween();
        _tweenAumentar.TweenProperty(this, "scale", _escalaHover, 0.12f)
              .SetTrans(Tween.TransitionType.Cubic)
              .SetEase(Tween.EaseType.Out);
    }

    private void AnimacionReducir()
    {
        _tweenAumentar?.Kill();
        _tweenAumentar = CreateTween();
        _tweenAumentar.TweenProperty(this, "scale", _escalaNormal, 0.12f)
              .SetTrans(Tween.TransitionType.Cubic)
              .SetEase(Tween.EaseType.Out);
    }

    public override void _ExitTree()
    {
        _tweenAumentar?.Kill();
        Scale = _escalaNormal;
    }
}
