using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.sistema.camara;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Juego : Control
{
    [Export]
    public CamaraPrincipal NodoCamara { get; set; }    // Nodo de la escena
    public static CamaraPrincipal Camara => Instancia.NodoCamara;

    public static Juego Instancia;

    private Control _contenedorEscena;

    public Juego()
    {
        Juego.Instancia = this;
    }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this._contenedorEscena = GetNode<Control>("ContenedorEscena");

        AjustaViewPortYCamara();

        // Cargamos el menú principal de forma diferida para asegurarnos de que la escena esté completamente lista.
        CallDeferred(nameof(CargarMenuPrincipal));
    }

    private void AjustaViewPortYCamara()
    {
        // Ajustamos el tamaño del juego al tamaño de la pantalla.
        this.Size = GetViewportRect().Size;

        // Ajustamos el tamaño de la cámara al tamaño del juego.
        Juego.Camara.AjustarCamara(this.Size);
        GetViewport().SizeChanged += () => Juego.Camara.AjustarCamara(this.Size);
    }

    public void CargarMenuPrincipal()
    {
        LoggerJuego.Trace("Cargando menú principal.");

        Global.GestorAudio.ReproducirMusica("retro_song.mp3");

        string rutaMenuprincipal = UtilidadesNodos.ObtenerRutaEscena<MenuPrincipal>();
        MenuPrincipal menuPrincipal = (MenuPrincipal)CambiarPantalla(rutaMenuprincipal);

        menuPrincipal.ContenedorMenuPrincipal.BotonEmpezarPartidaPulsado += CargarBatalla;
        menuPrincipal.ContenedorMenuPerfiles.OnCrearPrimerPerfil += CargarBatalla;
    }

    public void CargarBatalla()
    {
        LoggerJuego.Trace("Cargando batalla.");

        Global.GestorAudio.ReproducirMusica("Tronicles-Sirius_Beat.mp3", 1f);

        string rutaBatalla = UtilidadesNodos.ObtenerRutaEscena<Batalla>();
        Batalla batalla = (Batalla)CambiarPantalla(rutaBatalla);

        batalla.GameOverFinalizado += CargarMenuPrincipal;
    }

    public Node CambiarPantalla(string rutaEscena)
    {
        // Cargamos la escena desde la ruta proporcionada.
        PackedScene pantalla = ResourceLoader.Load<PackedScene>(rutaEscena);

        if (pantalla == null)
        {
            LoggerJuego.Error($"No se pudo cargar la escena en la ruta: {rutaEscena}");
            return null;
        }

        // Instanciamos la escena cargada.
        Node instanciaEscena = pantalla.Instantiate();
        if (instanciaEscena == null)
        {
            LoggerJuego.Error($"No se pudo instanciar la escena desde la ruta: {rutaEscena}");
            return null;
        }

        // Limpiamos el contenedor de escenas actual.
        UtilidadesNodos.BorrarHijos(this._contenedorEscena);

        // Añadimos la nueva escena al contenedor.
        this._contenedorEscena.AddChild(instanciaEscena);

        return instanciaEscena;
    }
}
