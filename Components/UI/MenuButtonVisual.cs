using Godot;
using System;
using System.Collections.Immutable;

public class MenuButtonVisual : Node2D, IObserver, IObservable
{
    public bool SlidingIn = true;
    public bool SlidingOut = false;

    public override void _Ready()
    {
        var button = this.GetNode<Button>("Path2D/PathFollow2D/Button");
        button.Connect("pressed", this, nameof(_on_Button_pressed));
    }

    public void _on_Button_pressed()
    {
        Notify();
    }

    public override void _Process(float delta)
    {
        if (this.GetNode<Button>("Path2D/PathFollow2D/Button").Visible && SlidingIn)
        {
            var pathFollow2d = this.GetNode<PathFollow2D>("Path2D/PathFollow2D");
            pathFollow2d.Offset += 250 * delta;

            this.GetNode<Button>("Path2D/PathFollow2D/Button").Modulate = new Color(1,1,1,pathFollow2d.UnitOffset/1.0f);

            if (pathFollow2d.UnitOffset >= 1.0)
            {
                SlidingIn = false;
            }
        }

        if (SlidingOut == true)
        {
            var pathFollow2d = this.GetNode<PathFollow2D>("Path2D/PathFollow2D");
            pathFollow2d.Offset -= 250 * delta;

            this.GetNode<Button>("Path2D/PathFollow2D/Button").Modulate = new Color(1,1,1,pathFollow2d.UnitOffset/1.0f);

            if (pathFollow2d.UnitOffset <= 0)
            {
                this.GetNode<Button>("Path2D/PathFollow2D/Button").Visible = false;
                SlidingOut = false;
                SlidingIn = true;
            }
        }
    }

    // Observable
    private ImmutableList<IObserver> _observers = ImmutableList<IObserver>.Empty;
    public void AddObserver(IObserver observer)
    {
        _observers = _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers = _observers.Remove(observer);
    }

    public void Notify()
    {
        _observers.ForEach(obs => obs.PropertyChanged(null));
    }

    // Observer
    public void PropertyChanged(IObservable observable)
    {
        if (observable is MenuButton button)
        {
            var godotButton = this.GetNode<Button>("Path2D/PathFollow2D/Button");

            if (button.Visible == false)
            {
                SlidingIn = false;
                SlidingOut = true;
            }
            else
            {
                godotButton.Visible = button.Visible;
            }

            godotButton.Disabled = !button.IsEnabled;

            godotButton.Text = button.Pressed ?
                                button.PressedText :
                                button.UnpressedText;
        }
    }
}
