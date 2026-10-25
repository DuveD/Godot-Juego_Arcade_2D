using System.IO;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.estadisticas;

public static class GestorEstadisticas
{
    private const string SECCION_ESTADISTICAS = "estadisticas";

    public static EstadisticasPartida PartidaActual { get; private set; }

    public static void CargarGlobales(Perfil perfil, ConfigFile archivoPerfil)
    {
        if (perfil?.EstadisticasGlobales == null || archivoPerfil == null)
            return;

        perfil.EstadisticasGlobales.PartidasJugadas = (int)archivoPerfil.GetValue(SECCION_ESTADISTICAS, "partidas_jugadas", 0);
        perfil.EstadisticasGlobales.MejorPuntuacion = (int)archivoPerfil.GetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", 0);
        perfil.EstadisticasGlobales.MonedasRecogidas = (int)archivoPerfil.GetValue(SECCION_ESTADISTICAS, "monedas_recogidas", 0);
        perfil.EstadisticasGlobales.MonedasEspecialesRecogidas =
                (int)archivoPerfil.GetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", 0);
        perfil.EstadisticasGlobales.EnemigosDerrotados = (int)archivoPerfil.GetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", 0);

        LoggerJuego.Trace($"Estadísticas del perfil '{perfil.Id}' cargadas.");
    }

    public static void InicializarPartida()
    {
        if (PartidaActual != null)
            LoggerJuego.Info("Ya existen unas estadísticas de una partida vigente. Se sobreescribe.");

        PartidaActual = new EstadisticasPartida();
    }

    public static void FinalizarPartida(Perfil perfil)
    {
        ActualizarGlobales(perfil);
        PartidaActual = null;
    }

    private static void ActualizarGlobales(Perfil perfil)
    {
        if (perfil?.EstadisticasGlobales == null)
            return;

        perfil.EstadisticasGlobales.PartidasJugadas++;
        perfil.EstadisticasGlobales.MonedasRecogidas += PartidaActual.MonedasRecogidas;
        perfil.EstadisticasGlobales.MonedasEspecialesRecogidas += PartidaActual.MonedasEspecialesRecogidas;
        perfil.EstadisticasGlobales.EnemigosDerrotados += PartidaActual.EnemigosDerrotados;

        if (PartidaActual.PuntuacionFinal > perfil.EstadisticasGlobales.MejorPuntuacion)
            perfil.EstadisticasGlobales.MejorPuntuacion = PartidaActual.PuntuacionFinal;
    }

    public static void GuardarGlobales(Perfil perfil, ConfigFile archivoPerfil)
    {
        if (perfil?.EstadisticasGlobales == null || archivoPerfil == null)
            return;

        archivoPerfil.SetValue(SECCION_ESTADISTICAS, "partidas_jugadas", perfil.EstadisticasGlobales.PartidasJugadas);
        archivoPerfil.SetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", perfil.EstadisticasGlobales.MejorPuntuacion);
        archivoPerfil.SetValue(SECCION_ESTADISTICAS, "monedas_recogidas", perfil.EstadisticasGlobales.MonedasRecogidas);
        archivoPerfil.SetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", perfil.EstadisticasGlobales.MonedasEspecialesRecogidas);
        archivoPerfil.SetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", perfil.EstadisticasGlobales.EnemigosDerrotados);
    }
}
