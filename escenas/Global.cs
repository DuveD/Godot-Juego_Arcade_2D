using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.escenas.sistema.audio;
using Primerjuego2D.escenas.sistema.logros;
using Primerjuego2D.escenas.ui.overlays;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    [Signal]
    public delegate void OnCambioPerfilActivoEventHandler();

    [Signal]
    public delegate void OnNavegacionTecladoCambiadoEventHandler(bool nuevoValor);

    public static bool NavegacionTeclado
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                Instancia.EmitSignal(SignalName.OnNavegacionTecladoCambiado, value);
            }
        }
    }

    public static Global Instancia;

    private static readonly SemaphoreSlim _semaforoGuardado = new(1, 1);

    private GestorColor _gestorColor;
    public static GestorColor GestorColor => Global.Instancia._gestorColor;

    private GestorAudio _gestorAudio;
    public static GestorAudio GestorAudio => Global.Instancia._gestorAudio;

    private GestorEfectosAudio _gestorEfectosAudio;
    public static GestorEfectosAudio GestorEfectosAudio => Global.Instancia._gestorEfectosAudio;

    private GestorNotificacionLogros _gestorNotificacionLogros;
    public static GestorNotificacionLogros GestorNotificacionLogros => Global.Instancia._gestorNotificacionLogros;

    private Perfil _perfilActivo;
    public static Perfil PerfilActivo => Global.Instancia._perfilActivo;

    private IndicadorCarga _indicadorCarga;
    public static IndicadorCarga IndicadorCarga => Global.Instancia._indicadorCarga;

    private IndicadorGuardado _indicadorGuardado;
    public static IndicadorGuardado IndicadorGuardado => Global.Instancia._indicadorGuardado;

    public Global()
    {
        Global.Instancia = this;
        Ajustes.CargarAjustes();
    }

    public override void _Ready()
    {
        // Informar idioma.
        Idioma idioma = Ajustes.Idioma;
        GestorIdioma.CambiarIdioma(idioma);

        _gestorColor = GetNode<GestorColor>("GestorColor");
        _gestorAudio = GetNode<GestorAudio>("GestorAudio");
        _gestorEfectosAudio = GetNode<GestorEfectosAudio>("GestorEfectosAudio");
        _gestorNotificacionLogros = GetNode<GestorNotificacionLogros>("GestorNotificacionLogros");
        _indicadorCarga = GetNode<IndicadorCarga>("IndicadorCarga");
        _indicadorGuardado = GetNode<IndicadorGuardado>("IndicadorGuardado");

        // Mostramos colisiones.
        bool verColisiones = Ajustes.VerColisiones;
        GetTree().DebugCollisionsHint = verColisiones;

        CargarPerfilActivo();

        NavegacionTeclado = false;

        ConfigurarGestorNotificacionLogros();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void ConfigurarGestorNotificacionLogros()
    {
        GestorLogros.LogrosDesbloqueados += GestorNotificacionLogros.MostrarLogros;
    }

    private async void CargarPerfilActivo()
    {
        if (String.IsNullOrWhiteSpace(Ajustes.IdPerfilActivo))
        {
            LoggerJuego.Info("No hay perfil activo configurado en ajustes.");
            return;
        }

        IndicadorCarga.Mostrar();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Perfil perfilActivo = await Task.Run(() => GestorPerfiles.CargarPerfil(Ajustes.IdPerfilActivo));

        if (perfilActivo != null)
        {
            Global.Instancia._perfilActivo = perfilActivo;
            LoggerJuego.Info($"Perfil '{Ajustes.IdPerfilActivo}' activo cargado.");
            Global.Instancia.EmitSignal(SignalName.OnCambioPerfilActivo);
        }
        else
        {
            LoggerJuego.Warn($"No se pudo cargar el perfil ({Ajustes.IdPerfilActivo}) activo configurado en ajustes.");
        }

        IndicadorCarga.Esconder();
    }

    public static async void CambiarPerfilActivo(Perfil perfil)
    {
        Global.Instancia._perfilActivo = perfil;

        // El id se cambia en memoria al momento, porque los menús lo leen enseguida;
        // la escritura en disco se hace con el indicador de guardado ya visible.
        Ajustes.CambiarSinGuardar(() => Ajustes.IdPerfilActivo = perfil?.Id);

        Global.Instancia.EmitSignal(SignalName.OnCambioPerfilActivo);

        IndicadorGuardado.Mostrar();

        await Global.Instancia.ToSignal(Global.Instancia.GetTree(), SceneTree.SignalName.ProcessFrame);

        Ajustes.Guardar();

        IndicadorGuardado.Esconder();
    }

    public static async void GuardarPerfilActivo()
    {
        Perfil perfil = Global.PerfilActivo;
        if (perfil == null)
            return;

        // Copiamos los datos en el hilo principal: el hilo de fondo solo escribe esta copia,
        // así no lee el perfil mientras el juego lo modifica.
        ConfigFile archivoPerfil = GestorPerfiles.CrearArchivoPerfil(perfil);

        IndicadorGuardado.Mostrar();

        await Global.Instancia.ToSignal(Global.Instancia.GetTree(), SceneTree.SignalName.ProcessFrame);

        // Un guardado cada vez: dos escrituras a la vez usarían el mismo archivo temporal.
        await _semaforoGuardado.WaitAsync();
        try
        {
            await Task.Run(() => GestorPerfiles.GuardarArchivoPerfil(perfil.Id, archivoPerfil));
        }
        finally
        {
            _semaforoGuardado.Release();
        }

        IndicadorGuardado.Esconder();
    }
}
