
using System;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.consumible;

public partial class Botiquin : Consumible
{
    public override bool OnRecogida(Jugador jugador)
    {
        LoggerJuego.Info("Botiquin recogido.");

        jugador.Vida += 1;

        return true;
    }
}
