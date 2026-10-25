using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.sistema.logros;

public partial class ContenedorLogro : PanelContainer
{
    [Export]
    public StyleBox StyleNormal;

    [Export]
    public StyleBox StyleFocus;

    private Label _LabelNombre;
    public Label LabelNombre => _LabelNombre ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Nombre");

    private Label _LabelDescripcion;
    public Label LabelDescripcion => _LabelDescripcion ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Descripcion");

    public override void _Ready()
    {
        AplicarEstiloNormal();
        this.FocusEntered += OnFocusEntered;
        this.FocusExited += OnFocusExited;
    }

    public void OnFocusEntered()
    {
        AplicarEstiloFocus();
        EnsureVisible();
    }

    public void OnFocusExited()
    {
        AplicarEstiloNormal();
    }

    public virtual void Inicializar(Logro logro)
    {
        LabelNombre.Text = logro.Nombre;
        LabelDescripcion.Text = logro.Descripcion;

        Desbloqueado(logro.Desbloqueado);
    }

    public void Desbloqueado(bool desbloqueado)
    {
        this.Modulate = desbloqueado
             ? Colors.White
             : new Color(1, 1, 1, 0.4f);
    }

    private void AplicarEstiloNormal()
    {
        if (StyleNormal != null)
            AddThemeStyleboxOverride("panel", StyleNormal);
    }

    private void AplicarEstiloFocus()
    {
        if (StyleFocus != null)
            AddThemeStyleboxOverride("panel", StyleFocus);
    }

    private void EnsureVisible()
    {
        var scroll = GetParent()?.GetParent()?.GetParent() as ScrollContainer;
        scroll?.EnsureControlVisible(this);
    }
}
