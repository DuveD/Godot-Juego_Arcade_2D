using System;

namespace Primerjuego2D.nucleo.entidades.atributo;

public class ModificadorAtributo<T>
{
    public static class Prioridades
    {
        public const int MUY_BAJO = -2;
        public const int BAJO = -1;
        public const int NORMAL = 0;
        public const int ALTO = 1;
        public const int MUY_ALTO = 2;
    }

    public string NombreAtributo { get; internal set; }

    public T Valor { get; set; }  // opcional, depende de cómo quieras usarlo
    public int Prioridad { get; set; }

    // Función que aplica el modificador
    private readonly Func<T, T> _aplicar;

    // Constructor
    public ModificadorAtributo(string nombreAtributo, T valor, Func<T, T> aplicar, int prioridad = Prioridades.NORMAL)
    {
        NombreAtributo = nombreAtributo;
        Valor = valor;
        _aplicar = aplicar;
        Prioridad = prioridad;
    }

    // Método que se llama desde Atributo
    public T AplicarModificador(T valorActual)
    {
        if (_aplicar != null)
            return _aplicar(valorActual);
        return valorActual;
    }
}
