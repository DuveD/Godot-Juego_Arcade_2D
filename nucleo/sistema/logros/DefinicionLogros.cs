
using System.Collections.Generic;

namespace Primerjuego2D.nucleo.sistema.logros;

public static class DefinicionLogros
{
    public static string EVENTO_LOGRO_PRIMERA_PARTIDA = "primera_partida";

    public static string EVENTO_LOGRO_ENEMIGO_DERROTADO = "enemigo_derrotado";

    public static string EVENTO_LOGRO_MONEDA_OBTENIDA = "moneda_obtenida";

    public static IEnumerable<Logro> ObtenerLogros()
    {
        return DefinirLogros();
    }

    private static List<Logro> DefinirLogros()
    {
        List<Logro> logros = [];

        logros.Add(new LogroUnico(
            "primeraPartida",
            "Logro.primeraPartida.nombre",
            "Logro.primeraPartida.descripcion",
            EVENTO_LOGRO_PRIMERA_PARTIDA
        ));

        // Comentamos para no añadir estos logros hasta que tengamos implementada la lógica de enemigos derrotados.
        // List<Logro> logrosEnemigosDerrotados = DefinirLogrosEnemigosDerrotados();
        // logros.AddRange(logrosEnemigosDerrotados);

        List<Logro> logrosMonedasObtenidas = DefinirLogrosMonedasObtenidas();
        logros.AddRange(logrosMonedasObtenidas);

        return logros;
    }

    private static List<Logro> DefinirLogrosEnemigosDerrotados()
    {
        List<Logro> logros = [];

        logros.Add(new LogroContador(
            "enemigosDerrotados10",
            "Logro.enemigosDerrotados10.nombre",
            "Logro.enemigosDerrotados10.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            10
        ));

        logros.Add(new LogroContador(
            "enemigosDerrotados25",
            "Logro.enemigosDerrotados25.nombre",
            "Logro.enemigosDerrotados25.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            25
        ));

        logros.Add(new LogroContador(
            "enemigosDerrotados50",
            "Logro.enemigosDerrotados50.nombre",
            "Logro.enemigosDerrotados50.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            50
        ));

        logros.Add(new LogroContador(
            "enemigosDerrotados100",
            "Logro.enemigosDerrotados100.nombre",
            "Logro.enemigosDerrotados100.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            100
        ));

        logros.Add(new LogroContador(
            "enemigosDerrotados500",
            "Logro.enemigosDerrotados500.nombre",
            "Logro.enemigosDerrotados500.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            500
        ));

        logros.Add(new LogroContador(
            "enemigosDerrotados1000",
            "Logro.enemigosDerrotados1000.nombre",
            "Logro.enemigosDerrotados1000.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            1000
        ));

        return logros;
    }

    private static List<Logro> DefinirLogrosMonedasObtenidas()
    {
        List<Logro> logros = [];

        logros.Add(new LogroContador(
            "monedasObtenidas10",
            "Logro.monedasObtenidas10.nombre",
            "Logro.monedasObtenidas10.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            10
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas25",
            "Logro.monedasObtenidas25.nombre",
            "Logro.monedasObtenidas25.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            25
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas50",
            "Logro.monedasObtenidas50.nombre",
            "Logro.monedasObtenidas50.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            50
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas100",
            "Logro.monedasObtenidas100.nombre",
            "Logro.monedasObtenidas100.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            100
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas150",
            "Logro.monedasObtenidas150.nombre",
            "Logro.monedasObtenidas150.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            150
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas200",
            "Logro.monedasObtenidas200.nombre",
            "Logro.monedasObtenidas200.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            200
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas300",
            "Logro.monedasObtenidas300.nombre",
            "Logro.monedasObtenidas300.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            300
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas400",
            "Logro.monedasObtenidas400.nombre",
            "Logro.monedasObtenidas400.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            400
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas500",
            "Logro.monedasObtenidas500.nombre",
            "Logro.monedasObtenidas500.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            500
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas600",
            "Logro.monedasObtenidas600.nombre",
            "Logro.monedasObtenidas600.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            600
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas700",
            "Logro.monedasObtenidas700.nombre",
            "Logro.monedasObtenidas700.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            700
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas800",
            "Logro.monedasObtenidas800.nombre",
            "Logro.monedasObtenidas800.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            800
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas900",
            "Logro.monedasObtenidas900.nombre",
            "Logro.monedasObtenidas900.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            900
        ));

        logros.Add(new LogroContador(
            "monedasObtenidas1000",
            "Logro.monedasObtenidas1000.nombre",
            "Logro.monedasObtenidas1000.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            1000
        ));

        return logros;
    }
}
