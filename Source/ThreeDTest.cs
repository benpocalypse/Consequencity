using Godot;
using System;

public class ThreeDTest : Spatial
{
	private Spatial cameraBase;
	private Camera camera;
	private Globals globals;

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
		cameraBase = ((Spatial)this.GetNode("Cambase"));
		camera = ((Camera)cameraBase.GetNode("Camera"));
		GetNode("UiLayer").GetNode("UserInterface").Connect("On_ResidentialButton_pressed", this, nameof(On_ResidentialButton_pressed));
		PopulateEngineMap();
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

		globals.Engine.Update(delta);
		UpdateAgentMap();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch ((ButtonList)mouseEvent.ButtonIndex)
			{
				case ButtonList.Left:
					var fromPos = camera.ProjectRayOrigin(mouseEvent.Position);
					var toPos = fromPos + camera.ProjectRayNormal(mouseEvent.Position) * 1000;
					var space_state = GetWorld().DirectSpaceState;
					var selection = space_state.IntersectRay(fromPos, toPos);
					
					try
					{
						var selectedCollider = ((StaticBody)selection["collider"]);
						var selectedLand = ((Land)selectedCollider.GetParent());
						selectedLand.SetLandType(globals.InputModeTypeToLandSpaceType(globals.InputMode));
						globals.Engine.Map[selectedLand.Position].Type = globals.InputModeTypeToLandSpaceType(globals.InputMode);
						
						selectedLand.Selected();
					}
					catch(Exception ex)
					{
						GD.Print($"Exception occurred when selecting a land: {ex}");
					}

					break;

				case ButtonList.WheelUp:
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

	private void On_ResidentialButton_pressed()
	{
		GD.Print("Residential Button pressed!");
		GD.Print($"InputMode = {globals.InputMode}");
	}

	public void PopulateEngineMap()
	{
		foreach (var space in globals.Engine.Map)
		{
			var land = (PackedScene)ResourceLoader.Load("res://Components/Land.tscn");
			Land newLand = (Land)land.Instance();
			var newPos = new Vector3(space.Key.x * 2, 0, space.Key.y * 2);
			newLand.SetLandType(space.Value.Type);
			newLand.SetPosition(space.Key);
			newLand.Translate(newPos);
			AddChild(newLand);
		}
	}

	// FIXME - all the update logic should be in our Economic Engine, not here.
	public void UpdateAgentMap()
	{
		foreach (var agent in globals.Engine.Agents)
		{
			if (agent.HasHome != false && agent.HomeHasBeenDrawn == false)
			{
				agent.HomeHasBeenDrawn = true;
				var building = (PackedScene)ResourceLoader.Load("res://Components/Building.tscn");
				Building newBuilding = (Building)building.Instance();
				var housePosition = new Vector3(agent.Home.x * 2, 0, agent.Home.y * 2);
				newBuilding.Translate(housePosition);
				newBuilding.SetType(Globals.LandSpaceType.Residential);
				AddChild(newBuilding);	
			}

			if (agent.HasJob != false && agent.JobHasBeenDrawn == false)
			{
				agent.JobHasBeenDrawn = true;
				var building = (PackedScene)ResourceLoader.Load("res://Components/Building.tscn");
				Building newBuilding = (Building)building.Instance();
				newBuilding.SetType((Globals.LandSpaceType)agent.JobType);
				var jobPosition = new Vector3(agent.JobLocation.x * 2, 0, agent.JobLocation.y * 2);
				newBuilding.Translate(jobPosition);
				AddChild(newBuilding);	
			}
		}
	}
}
