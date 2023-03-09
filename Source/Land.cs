using Godot;
using System;

public partial class Land : Node3D
{
    public Vector2 LandPosition;

    public override void _Ready()
    {

    }

    public void SetPosition(Vector2 position)
    {
        LandPosition = position;
    }

    public void SetLandType(Globals.LandSpaceType type)
    {
        switch (type)
        {
            case Globals.LandSpaceType.Residential:
                ((Node3D)GetNode("Residential")).Visible = true;
                ((Node3D)GetNode("Commercial")).Visible = false;
                ((Node3D)GetNode("Industrial")).Visible = false;
                ((Node3D)GetNode("Agricultural")).Visible = false;
                ((Node3D)GetNode("Transportation")).Visible = false;
                ((Node3D)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Commercial:
                ((Node3D)GetNode("Residential")).Visible = false;
                ((Node3D)GetNode("Commercial")).Visible = true;
                ((Node3D)GetNode("Industrial")).Visible = false;
                ((Node3D)GetNode("Agricultural")).Visible = false;
                ((Node3D)GetNode("Transportation")).Visible = false;
                ((Node3D)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Industrial:
                ((Node3D)GetNode("Residential")).Visible = false;
                ((Node3D)GetNode("Commercial")).Visible = false;
                ((Node3D)GetNode("Industrial")).Visible = true;
                ((Node3D)GetNode("Agricultural")).Visible = false;
                ((Node3D)GetNode("Transportation")).Visible = false;
                ((Node3D)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Agricultural:
                ((Node3D)GetNode("Residential")).Visible = false;
                ((Node3D)GetNode("Commercial")).Visible = false;
                ((Node3D)GetNode("Industrial")).Visible = false;
                ((Node3D)GetNode("Agricultural")).Visible = true;
                ((Node3D)GetNode("Transportation")).Visible = false;
                ((Node3D)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Transportation:
                ((Node3D)GetNode("Residential")).Visible = false;
                ((Node3D)GetNode("Commercial")).Visible = false;
                ((Node3D)GetNode("Industrial")).Visible = false;
                ((Node3D)GetNode("Agricultural")).Visible = false;
                ((Node3D)GetNode("Transportation")).Visible = true;
                ((Node3D)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.None:
                ((Node3D)GetNode("Residential")).Visible = false;
                ((Node3D)GetNode("Commercial")).Visible = false;
                ((Node3D)GetNode("Industrial")).Visible = false;
                ((Node3D)GetNode("Agricultural")).Visible = false;
                ((Node3D)GetNode("Transportation")).Visible = false;
                ((Node3D)GetNode("Land")).Visible = true;
                break;
        }
    }

    public bool IsSelected = false;
    public void Selected()
    {
        IsSelected = true;
        ((Node3D)GetNode("Selected")).Visible = true;
        ((Node3D)GetNode("Unselected")).Visible = false;
    }

    public void Unselected()
    {
        IsSelected = false;
        ((Node3D)GetNode("Selected")).Visible = false;
        ((Node3D)GetNode("Unselected")).Visible = true;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
