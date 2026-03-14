using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public abstract partial class ContenedorMenu : Container
{
    public bool EjecutarAccion => IsInstanceValid(this) && this.IsVisibleInTree();

    public Control _UltimoElementoConFoco;
    public Control UltimoElementoConFoco
    {
        get => _UltimoElementoConFoco;
        set
        {
            if (_UltimoElementoConFoco == value)
                return;

            _UltimoElementoConFoco = value;
            if (value != null)
                LoggerJuego.Trace($"({this.Name}) Último elemento con focus actualizado a '{value.Name}'.");
            else
                LoggerJuego.Trace($"({this.Name}) Último elemento con focus limpiado.");
        }
    }

    public abstract Control ObtenerPrimerElementoConFoco();

    public abstract List<Control> ObtenerElementosConFoco();

    public override void _Ready()
    {
        CallDeferred(nameof(PostReady));

        this.VisibilityChanged += OnVisibilityChanged;
        OnVisibilityChanged();
        ConfigurarElementosConFoco();
    }

    public override void _ExitTree()
    {
        this.VisibilityChanged -= OnVisibilityChanged;
    }

    // PostReady: se ejecuta tras layout inicial para evitar problemas de focus.
    protected virtual void PostReady()
    {
        if (this.IsVisibleInTree())
        {
            CallDeferred(nameof(GrabFocusPrimerElemento));
        }
    }

    public new void Show()
    {
        Show(true);
    }

    public void Show(bool seleccionarPrimerElemento)
    {
        if (Visible)
            return;

        if (seleccionarPrimerElemento)
            _UltimoElementoConFoco = ObtenerPrimerElementoConFoco();

        base.Show();

        OnNavegacionTecladoCambiado(Global.NavegacionTeclado);

        // Asegúrate que los elementos de foco están listos
        CallDeferred(nameof(GrabFocusUltimoElementoConFoco));
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
            OnMenuVisible();
        else
            OnMenuInvisible();
    }

    public virtual void OnMenuVisible()
    {
        this.SetProcessInput(true);
    }

    public virtual void OnMenuInvisible()
    {
        this.SetProcessInput(false);
    }

    private void OnNavegacionTecladoCambiado(bool navegacionTeclado)
    {
        if (!EjecutarAccion)
            return;

        if (navegacionTeclado)
            OnActivarNavegacionTeclado();
        else
            OnDesactivarNavegacionTeclado();
    }

    public void GrabFocusUltimoElementoConFoco()
    {
        if (!EjecutarAccion || !Global.NavegacionTeclado)
            return;

        if (UltimoElementoConFoco == null || !IsInstanceValid(UltimoElementoConFoco))
        {
            LoggerJuego.Trace($"({this.Name}) Último elemento nulo. Seleccionamos el primer elemento.");
            GrabFocusPrimerElemento();
            return;
        }

        if (!UltimoElementoConFoco.IsVisibleInTree() || !UltimoElementoConFoco.Visible)
        {
            LoggerJuego.Trace($"({this.Name}) Último elemento con foco está no visible. Seleccionamos el primer elemento.");
            GrabFocusPrimerElemento();
            return;
        }

        bool desactivado = UltimoElementoConFoco is BaseButton button && button.Disabled;
        if (desactivado)
        {
            LoggerJuego.Trace($"({this.Name}) Último elemento con foco está desactivado. Seleccionamos el primer elemento.");
            GrabFocusPrimerElemento();
            return;
        }

        LoggerJuego.Trace($"({this.Name}) Cogemos el foco del último elemento con foco: '" + UltimoElementoConFoco.Name + "'.");

        if (this.UltimoElementoConFoco is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            this.UltimoElementoConFoco.GrabFocus();
    }

    private void GrabFocusPrimerElemento()
    {
        if (!EjecutarAccion || !Global.NavegacionTeclado)
            return;

        Control elementoASeleccionar = ObtenerPrimerElementoConFoco();
        if (elementoASeleccionar == null || !IsInstanceValid(elementoASeleccionar))
            return;

        LoggerJuego.Trace($"({this.Name}) Cogemos el foco del primer elemento con foco: '" + elementoASeleccionar.Name + "'.");

        if (elementoASeleccionar is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            elementoASeleccionar.GrabFocus();
    }

    private void ConfigurarElementosConFoco()
    {
        if (!IsInstanceValid(this))
            return;

        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Configuramos el focus de los elementos con foco del contenedor.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            ConfigurarElementoConFoco(elementoConFoco);
        }
    }

    public void ConfigurarElementoConFoco(Control elementoConFoco)
    {
        if (elementoConFoco == null || !IsInstanceValid(elementoConFoco))
            return;

        Action onElementoConFocoFocusEntered = () => OnElementoConFocoFocusEntered(elementoConFoco);
        elementoConFoco.FocusEntered += onElementoConFocoFocusEntered;

        GuiInputEventHandler onClickOnElement = @event =>
        {
            if (@event is InputEventMouseButton iem && iem.Pressed)
                OnElementoConFocoFocusEntered(elementoConFoco);
        };
        elementoConFoco.GuiInput += onClickOnElement;
    }

    public void OnElementoConFocoFocusEntered(Control control)
    {
        this.UltimoElementoConFoco = control;
    }

    public override void _Input(InputEvent @event)
    {
        if (!EjecutarAccion)
            return;

        DetectarMetodoDeEntrada(@event);
    }

    private void DetectarMetodoDeEntrada(InputEvent @event)
    {
        // if (@event is InputEventKey keyEvent && UtilidadesControles.IsActionPressed(keyEvent, ConstantesAcciones.ACCIONES_NAVEGACION_MENU))
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!Global.NavegacionTeclado)
            {
                Global.NavegacionTeclado = true;
                OnNavegacionTecladoCambiado(Global.NavegacionTeclado);

                // Esto lo hacemos por si ya existe algún elemento con foco "especial" cómo un LineEdit, que puede tener foco aunque naveguemos con teclado.
                var focoActual = GetViewport().GuiGetFocusOwner();
                if (focoActual == null)
                {
                    // Filtramos escape para que el contenedor recoja la ación.
                    if (!UtilidadesControles.IsActionPressed(keyEvent, ConstantesAcciones.ESCAPE))
                    {
                        GrabFocusUltimoElementoConFoco();
                        AcceptEvent();
                    }
                }
            }
        }
        else if (@event is InputEventMouse inputEventMouse)
        {
            if (Global.NavegacionTeclado)
            {
                Global.NavegacionTeclado = false;
                OnNavegacionTecladoCambiado(Global.NavegacionTeclado);
            }

            // Esto lo hacemos para, si es un evento de clic, sólo quitar el foco si el click es fuera del control para poder hacer clicks en botones.
            if (!UtilidadesControles.IsMouseMovementEvent(inputEventMouse))
                QuitarFocoSiClickFuera(inputEventMouse);
        }
    }

    private void QuitarFocoSiClickFuera(InputEventMouse inputEventMouse)
    {
        var focoActual = GetViewport().GuiGetFocusOwner();

        if (focoActual == null)
            return;

        // Rectángulo global del control
        Rect2 rectGlobal = focoActual.GetGlobalRect();

        // Si el click está fuera del rectángulo
        if (!rectGlobal.HasPoint(inputEventMouse.GlobalPosition))
        {
            GetViewport().GuiReleaseFocus();

            LoggerJuego.Trace($"({Name}) Foco quitado por click fuera.");
        }
    }

    private void OnActivarNavegacionTeclado()
    {
        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Activamos la navegación por teclado.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco is BaseButton button && button.Disabled)
                continue;

            elementoConFoco.FocusMode = FocusModeEnum.All;
        }
    }

    private void OnDesactivarNavegacionTeclado()
    {
        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        GetViewport().GuiReleaseFocus();

        LoggerJuego.Trace($"({this.Name}) Desactivamos la navegación por teclado.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco is LineEdit)
                continue;
            else
                elementoConFoco.FocusMode = FocusModeEnum.None;
        }
    }
}