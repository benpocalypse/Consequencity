using Godot;
using System;

public class ThreeDTest : Spatial
{
	private Spatial camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = ((Spatial)this.GetNode("Cambase"));
	}

	public override void _Process(float delta)
	{
		//var camPosition = camera.GetCameraTransform().origin;
		
		var camPosition = camera.GlobalTransform.origin;
		
		if (Input.IsActionPressed("move_left"))
		{
			var newPosition = new Vector3(0, 0, -0.5f);
			camera.Translate(newPosition);
		}
		
		if (Input.IsActionPressed("move_right"))
		{
			var newPosition = new Vector3(0, 0, 0.5f);
			camera.Translate(newPosition);
		}
		
		if (Input.IsActionPressed("move_up"))
		{
			var newPosition = new Vector3(0.5f, 0, 0);
			camera.Translate(newPosition);
		}
		if (Input.IsActionPressed("move_down"))
		{
			var newPosition = new Vector3(-0.5f, 0, 0);
			camera.Translate(newPosition);
		}
	}
}
