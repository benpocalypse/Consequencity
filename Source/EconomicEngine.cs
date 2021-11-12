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

	private int _funds = 0;
	public int Funds
	{
		get => _funds;
		set => _funds = value;
	}

	private DateTime _date = new DateTime();
	public DateTime Date
	{
		get { return _date; }
		private set { _date = value; }
	}

	private List<LandSpace> _selectedLandList = new List<LandSpace>();
	public List<LandSpace> SelectedLandList
	{
		get => _selectedLandList;
		set => _selectedLandList =  value;
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

			_date += new TimeSpan(1, 0, 0, 0);

			_timeCounter = 0.0f;
		}
	}

	public void UpdateAgents()
	{
		var newHomeLocation = new Vector2(_random.Next(0, _mapWidth), _random.Next(0, _mapHeight));
		var newJobLocation = new Vector2(_random.Next(0, _mapWidth), _random.Next(0, _mapHeight));

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

			if (agent.HasHome &&
				agent.CanPerformAction() &&
				(
					Map[newJobLocation].Type == Globals.LandSpaceType.Commercial ||
					Map[newJobLocation].Type == Globals.LandSpaceType.Industrial ||
					Map[newJobLocation].Type == Globals.LandSpaceType.Agricultural
				) &&
				Map[newJobLocation].Population < Map[newJobLocation].Density)
			{
				agent.JobLocation = newJobLocation;
				agent.JobType = ((Globals.JobType)Map[newJobLocation].Type);
			}
		}
	}

	public void UpdateMap()
	{

	}

	public void UpdateDemand()
	{
		var agentsWithoutHomes = float.Parse(Agents.Where(agent => agent.HasHome == false).Count().ToString());
		var agentsWithoutJobs = float.Parse(Agents.Where(agent => agent.HasJob == false).Count().ToString());

		var agentsWithCommercialJobs = float.Parse(Agents.Where(agent => agent.HasJob == false && agent.JobType == Globals.JobType.Commercial).Count().ToString());

		var totalAgents = float.Parse(Agents.Count.ToString());

		var openResidentialCapacity = float.Parse(
				Map.Where(space => space.Value.Type == Globals.LandSpaceType.Residential).Count().ToString()
			);
		var openCommercialCapacity = float.Parse(
				Map.Where(space => space.Value.Type == Globals.LandSpaceType.Commercial).Count().ToString()
			);
		var openIndustrialCapacity = float.Parse(
				Map.Where(space => space.Value.Type == Globals.LandSpaceType.Industrial).Count().ToString()
			);
		var openAgriculturalCapacity = float.Parse(
				Map.Where(space => space.Value.Type == Globals.LandSpaceType.Agricultural).Count().ToString()
			);

		// FIXME - The residential demand should factor in population, capaity, crime, value, etc.
		// FIXME - Decouple the CIA demands so that they reflect who wants what type of job, how many jobs are available, etc.
		_demand[Globals.LandSpaceType.Residential] = (int) ((agentsWithoutHomes / totalAgents) * 100);
		_demand[Globals.LandSpaceType.Commercial] = (int) ((agentsWithoutJobs / totalAgents) * 100);
		_demand[Globals.LandSpaceType.Industrial] = (int) ((agentsWithoutJobs / totalAgents) * 100);
		_demand[Globals.LandSpaceType.Agricultural] = (int) ((agentsWithoutJobs / totalAgents) * 100);
	}
}
