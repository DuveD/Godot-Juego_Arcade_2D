using Godot;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo;

public partial class Fondo : Node2D
{
    [Export]
    private Sprite2D _SpriteFondo;
    private Sprite2D SpriteFondo => _SpriteFondo ??= GetNode<Sprite2D>("SpriteFondo");

    [Export]
    private GpuParticles2D _GpuParticles2D;
    private GpuParticles2D GpuParticles2D => _GpuParticles2D ??= GetNode<GpuParticles2D>("GpuParticles2D");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.FONDO);

        InicializarSpriteFondo();
        InicializarParticulas();
    }

    private void InicializarSpriteFondo()
    {
        // Creamos una textura 1x1 para inicializar el fondo.
        var image = Image.CreateEmpty(1, 1, false, Image.Format.Rgba8);
        image.Fill(Colors.White);

        // Creamos una textura a partir de la imagen.
        var texture = ImageTexture.CreateFromImage(image);
        this.SpriteFondo.Texture = texture;

        // Aplicamos el color de fondo desde el gestor de colores.
        this.SpriteFondo.Modulate = Global.GestorColor.ColorFondo;

        Vector2 viewportSize = GetViewportRect().Size;

        // Fondo
        this.SpriteFondo.Position = viewportSize / 2;
        this.SpriteFondo.Scale = viewportSize;
    }

    private void InicializarParticulas()
    {
        Vector2 viewportSize = GetViewportRect().Size;

        // Ajustamos la posición de inicio de generación de las partículas.
        this.GpuParticles2D.Position = new Vector2(0, viewportSize.Y / 2);

        // Si el material de proceso es ParticleProcessMaterial, ajustamos el área de emisión.
        if (this.GpuParticles2D.ProcessMaterial is ParticleProcessMaterial particleProcessMaterial)
            particleProcessMaterial.EmissionBoxExtents = new Vector3(1, viewportSize.Y / 2, 1);

        this.GpuParticles2D.Restart();
    }
}
