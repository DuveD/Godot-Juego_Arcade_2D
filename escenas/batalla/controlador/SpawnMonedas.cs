using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo;
using Primerjuego2D.escenas.objetos.consumible;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class SpawnMonedas : Control
{
	// Grupo con las monedas creadas por este spawner.
	private const string GRUPO_MONEDAS_SPAWNEADAS = "monedas_spawneadas";

	// Intentos máximos para encontrar una posición libre; así nunca se bloquea el juego si no queda hueco.
	private const int MAX_INTENTOS_POSICION = 30;
	
	[Export]
	public int ProbabilidadSpawnMonedaEspecial = 30;

	public int MonedasRecogidas;

	// Señal "MonedaRecogida" para indicar que el jugador ha recogido una moneda.
	[Signal]
	public delegate void MonedaRecogidaEventHandler(Moneda moneda);

	[Export]
	public PackedScene MonedaPackedScene;

	[Export]
	public PackedScene MonedaEspecialPackedScene;

	[Export]
	public int DistanciaMinima = 200;

	private Jugador Jugador { get; set; }

	private PackedScene TextoFlotanteScene;

	public Node NodoContenedorMonedas => this.GetParent().GetParent();

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		this.TextoFlotanteScene = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<TextoFlotante>());

		this.MonedasRecogidas = 0;
	}

	public void IniciarSpawnLoop(Jugador jugador)
	{
		this.Jugador = jugador;
		Spawn();
	}

	private void Spawn()
	{
		// Moneda normal
		SpawnMoneda();

		bool existeMonedaEspecial = ObtenerMonedasSpawneadas().Any(m => m is MonedaEspecial);
		if (!existeMonedaEspecial)
		{
			// Moneda especial según probabilidad
			float prob = (float)ProbabilidadSpawnMonedaEspecial / 100;
			if (Randomizador.GetRandomFloat() <= prob)
			{
				CallDeferred(nameof(SpawnMoneda), true);
			}
		}
	}

	private Moneda SpawnMoneda(bool monedaEspecial = false)
	{
		if (monedaEspecial)
			LoggerJuego.Trace("Spawneamos una moneda especial.");
		else
			LoggerJuego.Trace("Spawneamos una moneda.");

		Moneda moneda;

		if (monedaEspecial)
		{
			moneda = MonedaEspecialPackedScene.Instantiate<MonedaEspecial>();
		}
		else
		{
			moneda = MonedaPackedScene.Instantiate<Moneda>();
		}

		moneda.TextoFlotanteScene = this.TextoFlotanteScene;
		moneda.Recogida += OnMonedaRecogida;
		moneda.AddToGroup(GRUPO_MONEDAS_SPAWNEADAS);

		moneda.Position = ObtenerPosicionAleatoriaSegura();
		NodoContenedorMonedas.AddChild(moneda);

		return moneda;
	}

	private Vector2 ObtenerPosicionAleatoriaSegura()
	{
		Vector2 centroJugador = Jugador.GlobalPosition;
		List<Moneda> monedasSpawneadas = ObtenerMonedasSpawneadas();
		Rect2 areaSpawn = new(GlobalPosition, Size);

		// Por si no encontramos ninguna posición libre: la más alejada del jugador de las probadas.
		Vector2 posicionMasAlejada = areaSpawn.GetCenter();
		float distanciaMasAlejada = -1;

		for (int intento = 0; intento < MAX_INTENTOS_POSICION; intento++)
		{
			Vector2 nuevaPos = Randomizador.ObtenerPosicionAleatoria(areaSpawn);

			bool cercaJugador = UtilidadesMatematicas.PuntosCerca(centroJugador, nuevaPos, this.DistanciaMinima);
			bool cercaOtraMoneda = monedasSpawneadas.Any(moneda => UtilidadesMatematicas.PuntosCerca(moneda.Position, nuevaPos, moneda.ObtenerRadioCollisionShape2D() * 2));

			if (!cercaJugador && !cercaOtraMoneda)
				return nuevaPos;

			float distanciaAlJugador = centroJugador.DistanceSquaredTo(nuevaPos);
			if (distanciaAlJugador > distanciaMasAlejada)
			{
				distanciaMasAlejada = distanciaAlJugador;
				posicionMasAlejada = nuevaPos;
			}
		}

		// Si no hay hueco, la moneda se coloca igualmente lo más lejos posible del jugador:
		// la siguiente moneda solo aparece al recoger esta, así que no colocarla cortaría la cadena.
		LoggerJuego.Warn("No hay posición libre para la moneda; se usa la más alejada del jugador.");

		return posicionMasAlejada;
	}

	private List<Moneda> ObtenerMonedasSpawneadas()
	{
		// QueueFree no saca el nodo del árbol hasta el final del frame, así que descartamos las monedas ya marcadas para borrar.
		return GetTree().GetNodesInGroup(GRUPO_MONEDAS_SPAWNEADAS)
			.OfType<Moneda>()
			.Where(moneda => !moneda.IsQueuedForDeletion())
			.ToList();
	}

	public void OnMonedaRecogida(Moneda moneda)
	{
		this.MonedasRecogidas += 1;

		// Emitimos la señal de que el jugador ha recogido una moneda.
		EmitSignal(SignalName.MonedaRecogida, moneda);

		bool logrosDesbloqueados = GestorLogros.EmitirEvento(Global.PerfilActivo, DefinicionLogros.EVENTO_LOGRO_MONEDA_OBTENIDA);
		if (logrosDesbloqueados)
			Global.GuardarPerfilActivo();

		bool esMonedaEspecial = moneda is MonedaEspecial;
		if (!esMonedaEspecial)
			CallDeferred(nameof(Spawn));
	}

	public void DestruirMonedas()
	{
		IEnumerable<Moneda> monedas = NodoContenedorMonedas.GetChildren().OfType<Moneda>();
		foreach (var moneda in monedas)
			moneda.QueueFree();

		if (monedas.Any())
			LoggerJuego.Trace($"Destruida(s) {monedas.Count()} moneda(s).");
	}
}