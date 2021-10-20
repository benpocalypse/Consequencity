using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

public sealed class EconomicEngine
{
	public Dictionary<Vector2, LandSpace> Map = new Dictionary<Vector2, LandSpace>();
	public List<Agent> Agents = new List<Agent>();
	private Dictionary<Globals.LandSpaceType, int> _demand = new Dictionary<Globals.LandSpaceType, int>()
	{
		{ Globals.LandSpaceType.Residential, 0 },
		{ Globals.LandSpaceType.Commercial, 0 },
		{ Globals.LandSpaceType.Industrial, 0 },
		{ Globals.LandSpaceType.Agricultural, 0 }
	};
	public Dictionary<Globals.LandSpaceType, int> Demand
	{
		get => _demand;
	}

	private int _population = 0;
	public int Population 
	{
		get
		{
			_population = 0;
			foreach (var space in Map)
			{
				_population += space.Value.Population;
			}
			return _population;
		}
	}

	private const int _mapWidth = 30;
	private const int _mapHeight = 30;
	private float _timeCounter = 0.0f;
	private Random _random;

	public EconomicEngine()
	{
		_random = new Random();

		// FIXME - remove this code in the future and have proper map autogeneration.
		for (int i = 0; i < _mapWidth; i++)
		{
			for (int j = 0; j < _mapHeight; j++)
			{
				Map.Add(new Vector2(i,j), new LandSpace((Globals.LandSpaceType)_random.Next(0,5)));
			}
		}

		for (int i = 0; i < 100; i++)
		{
			Agents.Add(new Agent());
		}
	}

	public void Update(float timeStep)
	{
		_timeCounter += timeStep;

		if ( _timeCounter >= 1.0f )
		{
			UpdateAgents();
			UpdateDemand();
			UpdateMap();
		}
	}

	public void UpdateAgents()
	{
		var newHomeLocation = new Vector2(_random.Next(0, _mapWidth), _random.Next(0, _mapHeight));

		foreach (var agent in Agents)
		{
			// If our agent doesn't have a home, and should perform an action,
			// then let's try to find them a home.
			if (agent.HasHome == false && 
				agent.CanPerformAction() &&
				Map[newHomeLocation].Type == Globals.LandSpaceType.Residential &&
				Map[newHomeLocation].Population < Map[newHomeLocation].Density)
			{
				agent.Home = newHomeLocation;
				Map[newHomeLocation].Population += 1;
			}
		}
	}

	public void UpdateMap()
	{

	}

	public void UpdateDemand()
	{
		var agentsWithoutHomes = Agents.Where(agent => agent.HasHome == false).Count();

		// FIXME - what we actually want to do is count the occupied residential capacity, vs the total available capacity.
		var openResidentialCapacity = Map.Where(space => space.Value.Type == Globals.LandSpaceType.Residential).Count();

		// FIXME - The residential demand should factor in population, capaity, crime, value, etc.
		_demand[Globals.LandSpaceType.Residential] = (int) ((((float)agentsWithoutHomes) / ((float)Agents.Count)) * 100);
	}
}
