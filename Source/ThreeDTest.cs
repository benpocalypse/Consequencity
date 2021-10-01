using Godot;
using System;

public class ThreeDTest : Spatial
{
	private Spatial cameraBase;
	private Camera camera;

	public override void _Ready()
	{
		cameraBase = ((Spatial)this.GetNode("Cambase"));
		camera = ((Camera)cameraBase.GetNode("Camera"));
		PopulateTestMap();
	}

	public override void _Process(float delta)
	{
		var camPosition = cameraBase.GlobalTransform.origin;
		
		if (Input.IsActionPressed("move_left"))
		{
			var newPosition = new Vector3(0, 0, -0.5f);
			cameraBase.Translate(newPosition);
		}
		
		if (Input.IsActionPressed("move_right"))
		{
			var newPosition = new Vector3(0, 0, 0.5f);
			cameraBase.Translate(newPosition);
		}
		
		if (Input.IsActionPressed("move_up"))
		{
			var newPosition = new Vector3(0.5f, 0, 0);
			cameraBase.Translate(newPosition);
		}
		if (Input.IsActionPressed("move_down"))
		{
			var newPosition = new Vector3(-0.5f, 0, 0);
			cameraBase.Translate(newPosition);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					GD.Print($"Left button was clicked at {mouseEvent.Position}");
					var fromPos = camera.ProjectRayOrigin(mouseEvent.Position);
					var toPos = fromPos + camera.ProjectRayNormal(mouseEvent.Position) * 1000;
					var space_state = GetWorld().DirectSpaceState;
					var selection = space_state.IntersectRay(fromPos, toPos);
					var test = ((StaticBody)selection["collider"]);

					var selectedLand = ((Land)test.GetParent());
					//selectedLand.Selected();

					var building = (PackedScene)ResourceLoader.Load("res://Scenes/Building.tscn");
					Spatial newBuilding = (Spatial)building.Instance();
					newBuilding.Translate(selectedLand.Translation);
					AddChild(newBuilding);

					break;

				case ButtonList.WheelUp:
					GD.Print("Wheel up");
					var newPosition = new Vector3(0, -0.5f, 0);
					cameraBase.Translate(newPosition);
					break;

				case ButtonList.WheelDown:
					newPosition = new Vector3(0, 0.5f, 0);
					cameraBase.Translate(newPosition);
					break;
			}
		}
	}

	public void PopulateTestMap()
	{
		for (int i = 0; i < 20; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				var land = (PackedScene)ResourceLoader.Load("res://Scenes/Land.tscn");
				Spatial newLand = (Spatial)land.Instance();
				var newPos = new Vector3(i * 2, 0, j * 2);
				newLand.Translate(newPos);
				AddChild(newLand);
			}
		}
	}
}