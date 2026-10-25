using Godot;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.sistema.logros;

public partial class ContenedorLogroContador : ContenedorLogro
{
    private ProgressBar _ProgressBar;
    public ProgressBar ProgressBar => _ProgressBar ??= UtilidadesNodos.ObtenerNodoPorNombre<ProgressBar>(this, "ProgressBar");

    public override void Inicializar(Logro logro)
    {
        base.Inicializar(logro);

        ProgressBar.MinValue = 0;
        ProgressBar.MaxValue = 100;

        if (logro is LogroContador logroContador)
            ProgressBar.Value = logroContador.PorcentajeProgreso;
    }
}
