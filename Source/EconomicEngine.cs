using System;
using System.Collections.Generic;
using Godot;

public sealed class EconomicEngine
{
	public Dictionary<Vector2, LandSpace> Map = new Dictionary<Vector2, LandSpace>();

	private Dictionary<Globals.LandSpaceType, float> landDemand = new Dictionary<Globals.LandSpaceType, float>();

	public EconomicEngine()
	{
		// FIXME - remove this code in the future and have proper map autogeneration.
		for (int i = 0; i < 30; i++)
		{
			for (int j = 0; j < 30; j++)
			{
				Map.Add(new Vector2(i,j), new LandSpace(Globals.LandSpaceType.None));
			}
		}
	}

	public void Update(float timeStep)
	{

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
