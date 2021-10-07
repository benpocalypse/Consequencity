using System;
using System.Collections.Generic;
using Godot;

public sealed class EconomicEngine
{
	public Dictionary<Vector2, LandSpace> Map = new Dictionary<Vector2, LandSpace>();
	public List<Agent> Agents = new List<Agent>();

	private Dictionary<Globals.LandSpaceType, float> landDemand = new Dictionary<Globals.LandSpaceType, float>();

	private const int _mapWidth = 30;
	private const int _mapHeight = 30;
	private float _timeCounter = 0.0f;
	private Random _random;

	public EconomicEngine()
	{
		// FIXME - remove this code in the future and have proper map autogeneration.
		for (int i = 0; i < _mapWidth; i++)
		{
			for (int j = 0; j < _mapHeight; j++)
			{
				Map.Add(new Vector2(i,j), new LandSpace(Globals.LandSpaceType.None));
			}
		}

		for (int i = 0; i < 10; i++)
		{
			Agents.Add(new Agent());
		}

		_random = new Random();
	}

	public void Update(float timeStep)
	{
		_timeCounter += timeStep;

		foreach(var agent in Agents)
		{
			// If our agent doesn't have a home, and should perform an 
			if (agent.HasHome == false && agent.CanPerformAction())
			{
				agent.Home = new Vector2(_random.Next(0, _mapWidth), _random.Next(0, _mapHeight));
			}
		}
	}

	public void UpdateMap(float timeStep)
	{
		var g = Globals.Instance;
	}

	public void UpdateDemand(float timeStep)
	{
		// Update demand values based on current map values.
	}
}
