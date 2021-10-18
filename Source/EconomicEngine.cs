using System;
using System.Collections.Generic;
using Godot;

public sealed class EconomicEngine
{
	public Dictionary<Vector2, LandSpace> Map = new Dictionary<Vector2, LandSpace>();
	public List<Agent> Agents = new List<Agent>();
	private Dictionary<Globals.LandSpaceType, float> landDemand = new Dictionary<Globals.LandSpaceType, float>();

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
				Map.Add(new Vector2(i,j), new LandSpace((Globals.LandSpaceType)_random.Next(0,4)));
			}
		}

		for (int i = 0; i < 10; i++)
		{
			Agents.Add(new Agent());
		}
	}

	public void Update(float timeStep)
	{
		var newHomeLocation = new Vector2(_random.Next(0, _mapWidth), _random.Next(0, _mapHeight));

		_timeCounter += timeStep;

		foreach(var agent in Agents)
		{
			// If our agent doesn't have a home, and should perform an action,
			// then let's try to find them a home.
			if (agent.HasHome == false && agent.CanPerformAction() && Map[newHomeLocation].Type == Globals.LandSpaceType.Residential)
			{
				agent.Home = newHomeLocation;
				Map[newHomeLocation].Population += 1;
			}
		}
	}

	public void UpdateMap(float timeStep)
	{

	}

	public void UpdateDemand(float timeStep)
	{
		// Update demand values based on current map values.
	}
}
