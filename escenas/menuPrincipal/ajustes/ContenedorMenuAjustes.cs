using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;
using ButtonPersonalizado = Primerjuego2D.escenas.ui.controles.ButtonPersonalizado;

namespace Primerjuego2D.escenas.menuPrincipal.ajustes;

public partial class ContenedorMenuAjustes : ContenedorMenu
{
    private ControlSlider _ControlVolumenGeneral;
    private ControlSlider _ControlVolumenMusica;
    private ControlSlider _ControlVolumenSonido;

    private ControlSeleccion _ControlLenguaje;
    private ControlSeleccion _ControlNivelLog;

    private ControlCheckButton _ControlEscribirLogEnFichero;
    private ControlCheckButton _ControlVerColisiones;

    private ButtonPersonalizado _ButtonAtras;
    private ButtonPersonalizado _ButtonGuardar;

    #region Valor ajustes
    public int VolumenGeneral;
    public int VolumenMusica;
    public int VolumenSonidos;
    public Idioma Lenguaje;
    public NivelLog NivelLog;
    public bool EscribirLogEnFichero;
    public bool VerColisiones;
    #endregion

    // Los cambios se aplican al momento pero se escriben en disco una sola vez, al cerrar el menú.
    private bool _hayCambiosSinGuardar = false;

    public override void _Ready()
    {
        _ControlVolumenGeneral = UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenGeneral");
        _ControlVolumenMusica = UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenMusica");
        _ControlVolumenSonido = UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenSonido");
        _ControlLenguaje = UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlLenguaje");
        _ControlNivelLog = UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlNivelLog");
        _ControlEscribirLogEnFichero = UtilidadesNodos.ObtenerNodoPorNombre<ControlCheckButton>(this, "ControlEscribirLogEnFichero");
        _ControlVerColisiones = UtilidadesNodos.ObtenerNodoPorNombre<ControlCheckButton>(this, "ControlVerColisiones");
        _ButtonAtras = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");
        _ButtonGuardar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonGuardar");

        CargarOpcionesLenguaje();
        CargarOpcionesNivelLog();

        // Cargar ajustes actuales.
        CargarValoresDeAjustes();

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _ControlVolumenGeneral.SliderVolumen;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        List<Control> elementosConFoco;
        elementosConFoco = [.. UtilidadesNodos.ObtenerNodosDeTipo<Button>(this)];
        elementosConFoco.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<SpinBox>(this));
        elementosConFoco.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<HSlider>(this));

        return elementosConFoco;
    }

    private void CargarOpcionesLenguaje()
    {
        var opcionesLenguajes = GestorIdioma.IdiomasDisponibles.Values.ToDictionary(
            idioma => (Variant)idioma.Codigo,
            idioma => idioma.TagNombre
        );
        _ControlLenguaje.AgregarOpciones(opcionesLenguajes);
    }

    private void CargarOpcionesNivelLog()
    {
        var opcionesNivelLog = Enum.GetValues<NivelLog>().ToDictionary(
            nivel => (Variant)(int)nivel,
            nivel => nivel.ToString()
        );
        _ControlNivelLog.AgregarOpciones(opcionesNivelLog);
    }

    private void CargarValoresDeAjustes()
    {
        _ControlVolumenGeneral.ValorCambiado -= OnControlVolumenGeneralValorCambiado;
        _ControlVolumenMusica.ValorCambiado -= OnControlVolumenMusicaValorCambiado;
        _ControlVolumenSonido.ValorCambiado -= OnControlVolumenSonidosValorCambiado;
        _ControlLenguaje.ValorCambiado -= OnControlLenguajeValorCambiado;
        _ControlNivelLog.ValorCambiado -= OnControlNivelLogValorCambiado;
        _ControlEscribirLogEnFichero.ValorCambiado -= OnControlEscribirLogEnFicheroValorCambiado;
        _ControlVerColisiones.ValorCambiado -= OnControlVerColisionesValorCambiado;

        VolumenGeneral = Ajustes.VolumenGeneral;
        _ControlVolumenGeneral.Valor = VolumenGeneral;

        VolumenMusica = Ajustes.VolumenMusica;
        _ControlVolumenMusica.Valor = VolumenMusica;

        VolumenSonidos = Ajustes.VolumenSonidos;
        _ControlVolumenSonido.Valor = VolumenSonidos;

        Lenguaje = Ajustes.Idioma;
        _ControlLenguaje.Valor = Ajustes.Idioma.Codigo;

        NivelLog = Ajustes.NivelLog;
        _ControlNivelLog.Valor = (int)Ajustes.NivelLog;

        EscribirLogEnFichero = Ajustes.EscribirLogEnFichero;
        _ControlEscribirLogEnFichero.Valor = EscribirLogEnFichero;

        VerColisiones = Ajustes.VerColisiones;
        _ControlVerColisiones.Valor = VerColisiones;

        _ControlVolumenGeneral.ValorCambiado += OnControlVolumenGeneralValorCambiado;
        _ControlVolumenMusica.ValorCambiado += OnControlVolumenMusicaValorCambiado;
        _ControlVolumenSonido.ValorCambiado += OnControlVolumenSonidosValorCambiado;
        _ControlLenguaje.ValorCambiado += OnControlLenguajeValorCambiado;
        _ControlNivelLog.ValorCambiado += OnControlNivelLogValorCambiado;
        _ControlEscribirLogEnFichero.ValorCambiado += OnControlEscribirLogEnFicheroValorCambiado;
        _ControlVerColisiones.ValorCambiado += OnControlVerColisionesValorCambiado;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (!_ButtonAtras.HasFocus())
                _ButtonAtras.GrabFocus();
            else
                UtilidadesNodos.PulsarBoton(_ButtonAtras);

            AcceptEvent();
        }
    }

    private void ActivarBotonGuardarSiCambio()
    {
        bool hayCambios =
            !VolumenGeneral.Equals((int)_ControlVolumenGeneral.Valor) ||
            !VolumenMusica.Equals((int)_ControlVolumenMusica.Valor) ||
            !VolumenSonidos.Equals((int)_ControlVolumenSonido.Valor) ||
            !Lenguaje.Codigo.Equals(_ControlLenguaje.Valor.AsString()) ||
            !NivelLog.Equals((NivelLog)(int)_ControlNivelLog.Valor);
        if (hayCambios)
        {
            ActivarNavegacionButtonGuardar();
        }
        else
        {
            DesactivarNavegacionButtonGuardar();
        }
    }

    private void ActivarNavegacionButtonGuardar()
    {
        _ButtonGuardar.Disabled = false;
        _ButtonGuardar.FocusMode = FocusModeEnum.All;

        // Informamos al ControlNivelLog que su vecino de abajo es el botón Guardar

        _ControlNivelLog.OptionButton.FocusNeighborBottom = _ControlNivelLog.OptionButton.GetPathTo(_ButtonGuardar);
        // Informamos al botón Atrás que su vecino a la derecha es el botón Guardar

        _ButtonAtras.FocusNeighborRight = _ButtonAtras.GetPathTo(_ButtonGuardar);
    }

    private void DesactivarNavegacionButtonGuardar()
    {
        _ButtonGuardar.Disabled = true;
        _ButtonGuardar.FocusMode = FocusModeEnum.None;

        // Informamos al ControlNivelLog que su vecino de abajo es el botón Atrás

        _ControlNivelLog.OptionButton.FocusNeighborBottom = _ControlNivelLog.OptionButton.GetPathTo(_ButtonAtras);
        // Informamos al botón Atrás que su vecino a la derecha es el ControlNivelLog

        _ButtonAtras.FocusNeighborRight = _ButtonAtras.GetPathTo(_ControlNivelLog.OptionButton);
    }

    private void OnControlVolumenGeneralValorCambiado(double valor)
    {
        Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
        //ActivarBotonGuardarSiCambio();

        CambiarAjusteSinGuardar(() => Ajustes.VolumenGeneral = (int)valor);
    }

    private void OnControlVolumenMusicaValorCambiado(double valor)
    {
        Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
        //ActivarBotonGuardarSiCambio();

        CambiarAjusteSinGuardar(() => Ajustes.VolumenMusica = (int)valor);
    }

    private void OnControlVolumenSonidosValorCambiado(double valor)
    {
        Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
        //ActivarBotonGuardarSiCambio();

        CambiarAjusteSinGuardar(() => Ajustes.VolumenSonidos = (int)valor);
    }

    private void OnControlLenguajeValorCambiado(Variant valor)
    {
        string codigoIdioma = (string)valor;
        Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
        GestorIdioma.CambiarIdioma(idiomaSeleccionado);
        //ActivarBotonGuardarSiCambio();

        CambiarAjusteSinGuardar(() => Ajustes.Idioma = idiomaSeleccionado);
    }

    private void OnControlNivelLogValorCambiado(Variant valor)
    {
        NivelLog nivelLogSeleccionado = (NivelLog)(int)valor;
        LoggerJuego.NivelLogJuego = nivelLogSeleccionado;
        //ActivarBotonGuardarSiCambio();

        CambiarAjusteSinGuardar(() => Ajustes.NivelLog = nivelLogSeleccionado);
    }

    private void OnControlEscribirLogEnFicheroValorCambiado(bool valor)
    {
        LoggerJuego.EscribirLogEnFichero = valor;

        CambiarAjusteSinGuardar(() => Ajustes.EscribirLogEnFichero = valor);
    }

    private void OnControlVerColisionesValorCambiado(bool valor)
    {
        CambiarAjusteSinGuardar(() => Ajustes.VerColisiones = valor);

        GetTree().DebugCollisionsHint = valor;
    }

    private void CambiarAjusteSinGuardar(Action cambiarAjuste)
    {
        Ajustes.CambiarSinGuardar(cambiarAjuste);
        _hayCambiosSinGuardar = true;
    }

    private async void GuardarCambiosPendientesConIndicador()
    {
        if (!_hayCambiosSinGuardar)
            return;

        _hayCambiosSinGuardar = false;

        Global.IndicadorGuardado.Mostrar();

        // Esperamos un frame para que el indicador se dibuje antes de escribir en disco.
        // Usamos Global porque este menú puede liberarse antes (por ejemplo, al empezar partida).
        await Global.Instancia.ToSignal(Global.Instancia.GetTree(), SceneTree.SignalName.ProcessFrame);

        Ajustes.Guardar();

        Global.IndicadorGuardado.Esconder();
    }

    public override void OnMenuInvisible()
    {
        base.OnMenuInvisible();

        GuardarCambiosPendientesConIndicador();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        // Por si el menú desaparece estando abierto (por ejemplo, al cerrar el juego).
        // Aquí se guarda directamente: puede que ya no haya más frames para mostrar el indicador.
        if (_hayCambiosSinGuardar)
        {
            Ajustes.Guardar();
            _hayCambiosSinGuardar = false;
        }
    }

    public void OnButtonGuardarPressed()
    {
        LoggerJuego.Trace("Botón Ajustes 'Guardar' pulsado.");

        Ajustes.GuardarAjustesAlGuardarPropiedad = false;
        Ajustes.VolumenGeneral = (int)_ControlVolumenGeneral.Valor;
        Ajustes.VolumenMusica = (int)_ControlVolumenMusica.Valor;
        Ajustes.VolumenSonidos = (int)_ControlVolumenSonido.Valor;
        Ajustes.Idioma = GestorIdioma.ObtenerIdiomaDeCodigo((string)_ControlLenguaje.Valor);
        Ajustes.NivelLog = (NivelLog)(int)_ControlNivelLog.Valor;
        Ajustes.GuardarAjustesAlGuardarPropiedad = true;
        Ajustes.Guardar();

        CargarValoresDeAjustes();

        DesactivarNavegacionButtonGuardar();

        _ButtonAtras.GrabFocusSilencioso();
    }
}
