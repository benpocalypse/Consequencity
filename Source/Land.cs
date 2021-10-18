using Godot;
using System;

public class Land : Spatial
{
    public Vector2 Position;

    public override void _Ready()
    {
        
    }

    public void SetPosition(Vector2 _position)
    {
        Position = _position;
    }

    public void SetLandType(Globals.LandSpaceType _type)
    {
        switch (_type)
        {
            case Globals.LandSpaceType.Residential:
                ((Spatial)GetNode("Residential")).Visible = true;
                ((Spatial)GetNode("Commercial")).Visible = false;
                ((Spatial)GetNode("Industrial")).Visible = false;
                ((Spatial)GetNode("Agricultural")).Visible = false;
                ((Spatial)GetNode("Transportation")).Visible = false;
                ((Spatial)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Commercial:
                ((Spatial)GetNode("Residential")).Visible = false;
                ((Spatial)GetNode("Commercial")).Visible = true;
                ((Spatial)GetNode("Industrial")).Visible = false;
                ((Spatial)GetNode("Agricultural")).Visible = false;
                ((Spatial)GetNode("Transportation")).Visible = false;
                ((Spatial)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Industrial:
                ((Spatial)GetNode("Residential")).Visible = false;
                ((Spatial)GetNode("Commercial")).Visible = false;
                ((Spatial)GetNode("Industrial")).Visible = true;
                ((Spatial)GetNode("Agricultural")).Visible = false;
                ((Spatial)GetNode("Transportation")).Visible = false;
                ((Spatial)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Agricultural:
                ((Spatial)GetNode("Residential")).Visible = false;
                ((Spatial)GetNode("Commercial")).Visible = false;
                ((Spatial)GetNode("Industrial")).Visible = false;
                ((Spatial)GetNode("Agricultural")).Visible = true;
                ((Spatial)GetNode("Transportation")).Visible = false;
                ((Spatial)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.Transportation:
                ((Spatial)GetNode("Residential")).Visible = false;
                ((Spatial)GetNode("Commercial")).Visible = false;
                ((Spatial)GetNode("Industrial")).Visible = false;
                ((Spatial)GetNode("Agricultural")).Visible = false;
                ((Spatial)GetNode("Transportation")).Visible = true;
                ((Spatial)GetNode("Land")).Visible = false;
                break;

            case Globals.LandSpaceType.None:
                ((Spatial)GetNode("Residential")).Visible = false;
                ((Spatial)GetNode("Commercial")).Visible = false;
                ((Spatial)GetNode("Industrial")).Visible = false;
                ((Spatial)GetNode("Agricultural")).Visible = false;
                ((Spatial)GetNode("Transportation")).Visible = false;
                ((Spatial)GetNode("Land")).Visible = true;
                break;
        }
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
