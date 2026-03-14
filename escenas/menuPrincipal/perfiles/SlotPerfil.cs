using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfiles;

public partial class SlotPerfil : ButtonPersonalizado
{
    public int NumeroSlot { get; set; }

    public bool Vacio => this.Perfil == null;

    public Perfil Perfil
    {
        set
        {
            field = value;
            if (field == null)
                VaciarPerfil();
            else
                InformarPerfil(field);
        }

        get;
    }

    private bool _activo;

    public bool Activo => _activo;

    private Label _labelNombre;

    private Label _labelFechaUltimaPartidaFecha;

    private Label _labelPartidasJugadasNumero;

    private Label _labelMonedasRecogidas;

    private PanelContainer _contenedorInformacion;

    private PanelContainer _contenedorVacio;

    #region Estilos slot con partida
    private static readonly StyleBox _estiloNormal = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfil_normal.tres");
    private static readonly StyleBox _estiloNormalActivo = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfilActivo_normal.tres");
    private static readonly StyleBox _estiloPressed = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfil_pressed.tres");
    private static readonly StyleBox _estiloHover = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfil_hover.tres");
    private static readonly StyleBox _estiloFocus = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfil_focus.tres");
    #endregion

    #region Estilos slot vacío
    private static readonly StyleBox _estiloNormalVacio = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfilVacio_normal.tres");
    private static readonly StyleBox _estiloPressedVacio = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfilVacio_pressed.tres");
    private static readonly StyleBox _estiloHoverVacio = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfilVacio_hover.tres");
    private static readonly StyleBox _estiloFocusVacio = GD.Load<StyleBox>("res://recursos/temas/perfil/SlotPerfilVacio_focus.tres");
    #endregion

    private static StyleBox _estiloDesactivado;

    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        _labelNombre = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelNombre");
        _labelFechaUltimaPartidaFecha = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelFechaUltimaPartidaFecha");
        _labelPartidasJugadasNumero = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelPartidasJugadasNumero");
        _labelMonedasRecogidas = UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMonedasRecogidas");
        _contenedorInformacion = GetNode<PanelContainer>("ContenedorInformacion");
        _contenedorVacio = GetNode<PanelContainer>("ContenedorVacio");
        _animationPlayer = GetNode<AnimationPlayer>("ContenedorInformacion/VBoxContainer/HBoxContainerDatos/Control/TextureRectMonedaNormal/AnimationPlayer");

        _estiloDesactivado = GetThemeStylebox("disabled");

        VaciarPerfil();

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void InformarPerfil(Perfil perfil)
    {
        _labelNombre.Text = perfil.Nombre;
        _labelFechaUltimaPartidaFecha.Text = (perfil.FechaUltimaPartida != null) ? UtilidadesFechas.FormatearFechaSinDiaSemana(perfil.FechaUltimaPartida) : "-";
        _labelPartidasJugadasNumero.Text = perfil.EstadisticasGlobales.PartidasJugadas.ToString();
        _labelMonedasRecogidas.Text = perfil.EstadisticasGlobales.MonedasRecogidas.ToString();
        _activo = false;

        _contenedorVacio.Hide();
        _contenedorInformacion.Show();

        AplicarEstilos(_estiloNormal, _estiloPressed, _estiloHover, _estiloFocus);
    }

    public void VaciarPerfil()
    {
        _labelNombre.Text = this.Name;
        _labelFechaUltimaPartidaFecha.Text = "-";
        _labelPartidasJugadasNumero.Text = "0";
        _labelMonedasRecogidas.Text = "0";
        _activo = false;

        _contenedorInformacion.Hide();
        _contenedorVacio.Show();

        AplicarEstilos(_estiloNormalVacio, _estiloPressedVacio, _estiloHoverVacio, _estiloFocusVacio);
    }

    private void AplicarEstilos(StyleBox normal, StyleBox pressed, StyleBox hover, StyleBox focus)
    {
        AddThemeStyleboxOverride("normal", normal);
        AddThemeStyleboxOverride("pressed", pressed);
        AddThemeStyleboxOverride("hover", hover);
        AddThemeStyleboxOverride("focus", focus);
    }

    public void SetActivo(bool activo)
    {
        if (this.Perfil == null)
            return;

        _activo = activo;

        if (activo)
        {
            _animationPlayer.Play("Rotar");
            AddThemeStyleboxOverride("normal", _estiloNormalActivo);
            AddThemeStyleboxOverride("disabled", _estiloNormalActivo);
        }
        else
        {
            _animationPlayer.Stop();
            AddThemeStyleboxOverride("normal", _estiloNormal);
            AddThemeStyleboxOverride("disabled", _estiloDesactivado);
        }
    }
}
