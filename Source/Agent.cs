using System;
using Godot;

public sealed class Agent
{
    public int Currency { get; } = 100;
    public bool HasHome { get; private set; } = false;
    public bool HasJob { get; private set; } = false;

    private Vector2 _home = new Vector2(0, 0);
    public Vector2 Home 
    {
        get { return _home; }
        set
        {
            HasHome = true;
            _home = value;
        }
    }

    private Vector2 _jobLocation = new Vector2(0, 0);
    public Vector2 JobLocation
    {
        get { return _jobLocation; }
        set
        {
            HasJob = true;
            _jobLocation = value;
        }
    }

    private Globals.JobType _jobType = Globals.JobType.None;
    public Globals.JobType JobType
    {
        get { return _jobType; }
        set
        {
            _jobType = value;
        }
    }

    public bool HomeHasBeenDrawn = false;
    public bool JobHasBeenDrawn = false;
    private Random _random;
    private int _seed = 0;

    public Agent()
    {
        _random = new Random();
        _seed = _random.Next(0, 1000);
    }

    public bool CanPerformAction() => _seed == _random.Next(0, 1000);
}
