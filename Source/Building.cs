using Godot;
using System;

public partial class Building : Node3D
{
    public override void _Ready()
    {
        
    }

    public void SetType(Globals.LandSpaceType _type)
    {
        switch (_type)
        {
            case Globals.LandSpaceType.Residential:
                {
                    var material = new StandardMaterial3D();
                    material.AlbedoColor = new Color("6bc96c");
                    ((MeshInstance3D)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance3D)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Commercial:
                {
                    var material = new StandardMaterial3D();
                    material.AlbedoColor = new Color("aee2ff");
                    ((MeshInstance3D)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance3D)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Industrial:
                {
                    var material = new StandardMaterial3D();
                    material.AlbedoColor = new Color("ffb879");
                    ((MeshInstance3D)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance3D)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Agricultural:
                {
                    var material = new StandardMaterial3D();
                    material.AlbedoColor = new Color("f2ae99");
                    ((MeshInstance3D)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance3D)GetNode("Building2")).MaterialOverride = material;
                }
                break;
            
            default:
                {
                    var material = new StandardMaterial3D();
                    material.AlbedoColor = new Color(1,1,1,1);
                    ((MeshInstance3D)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance3D)GetNode("Building2")).MaterialOverride = material;
                }
                break;
        }
    }
}
