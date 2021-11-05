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
                    var material = new SpatialMaterial();
                    material.AlbedoColor = new Color("6bc96c");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Commercial:
                {
                    var material = new SpatialMaterial();
                    material.AlbedoColor = new Color("aee2ff");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Industrial:
                {
                    var material = new SpatialMaterial();
                    material.AlbedoColor = new Color("ffb879");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = material;
                }
                break;

            case Globals.LandSpaceType.Agricultural:
                {
                    var material = new SpatialMaterial();
                    material.AlbedoColor = new Color("f2ae99");
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = material;
                }
                break;
            
            default:
                {
                    var material = new SpatialMaterial();
                    material.AlbedoColor = new Color(1,1,1,1);
                    ((MeshInstance)GetNode("Building1")).MaterialOverride = material;
                    ((MeshInstance)GetNode("Building2")).MaterialOverride = material;
                }
                break;
        }
    }
}
