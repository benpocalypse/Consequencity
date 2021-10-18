using System;
using Godot;

public sealed class Agent
{
    public int Currency { get; } = 100;
    public bool HasHome { get; private set; } = false;
    public bool HasWork { get; private set; } = false;

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

    private Vector2 _work = new Vector2(0, 0);
    public Vector2 Work
    {
        get { return _work; }
        set
        {
            HasWork = true;
            _work = value;
        }
    }

    public bool HasBeenDrawn = false;
    private Random _random;
    private int _seed = 0;

    public Agent()
    {
        _random = new Random();
    }

    public bool CanPerformAction() => _seed == _random.Next(0, 1000);
}
