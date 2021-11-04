using Godot;
using System;

public class Building : Spatial
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
                    var test = new SpatialMaterial();
                    test.AlbedoColor = new Color("6bc96c");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = test;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = test;
                }
                break;

            case Globals.LandSpaceType.Commercial:
                {
                    var test = new SpatialMaterial();
                    test.AlbedoColor = new Color("aee2ff");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = test;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = test;
                }
                break;

            case Globals.LandSpaceType.Industrial:
                {
                    var test = new SpatialMaterial();
                    test.AlbedoColor = new Color("ffb879");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = test;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = test;
                }
                break;

            case Globals.LandSpaceType.Agricultural:
                {
                    var test = new SpatialMaterial();
                    test.AlbedoColor = new Color("f2ae99");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = test;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = test;
                }
                break;
            
            default:
                {
                    var test = new SpatialMaterial();
                    test.AlbedoColor = new Color(1,1,1,1);
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = test;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = test;
                }
                break;
        }
    }
}
