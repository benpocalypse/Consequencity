using Godot;
using System;

public class Land : Spatial
{
    [Signal]
    public delegate void MySignal();

    private Vector2 Position;

    public override void _Ready()
    {
        
    }

    public void SetPosition(Vector2 _position)
    {
        Position = _position;
    }

    public void Selected()
    {
        ((Spatial)GetNode("Selected")).Visible = true;
        ((Spatial)GetNode("Unselected")).Visible = false;
    }

    public void Unselected()
    {
        ((Spatial)GetNode("Selected")).Visible = false;
        ((Spatial)GetNode("Unselected")).Visible = true;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
