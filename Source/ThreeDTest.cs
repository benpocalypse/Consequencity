using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

	private bool leftButtonClicked = false;
	private Vector3 initialClickPosition = new Vector3();
	private Vector3 previousClickPosition = new Vector3();
	private Vector3 currentClickPosition = new Vector3();
	private List<Highlight> highlightSelectionlist = new List<Highlight>();
	private List<Land> landSelectionList = new List<Land>();
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{
				switch ((ButtonList)mouseEvent.ButtonIndex)
				{
					case ButtonList.Left:
						leftButtonClicked = true;

						var fromPos = camera.ProjectRayOrigin(mouseEvent.Position);
						var toPos = fromPos + camera.ProjectRayNormal(mouseEvent.Position) * 1000; // FIXME - what should this actually be?
						var space_state = GetWorld().DirectSpaceState;
						var selection = space_state.IntersectRay(fromPos, toPos);

						try
						{
							var selectedCollider = ((StaticBody)selection["collider"]);
							var selectedLand = ((Land)selectedCollider.GetParent());

							initialClickPosition = selectedLand.Translation;
							previousClickPosition = selectedLand.Translation;
						}
						catch(Exception){}

						landSelectionList.ForEach(_ => _.Unselected());
						landSelectionList.Clear();

						if (globals.InputMode == Globals.InputModeType.Select)
						{
							globals.Engine.SelectedLandList.Clear();
							var highlight = (PackedScene)ResourceLoader.Load("res://Components/Highlight.tscn");
							Highlight newHighlight = (Highlight)highlight.Instance();
							newHighlight.Translate(initialClickPosition);
							AddChild(newHighlight);

							if (!highlightSelectionlist.Contains(newHighlight))
							{
								highlightSelectionlist.Add(newHighlight);
							}
						}

						if (globals.InputMode == Globals.InputModeType.Place && globals.PlacementMode != Globals.PlacementModeType.None)
						{
							globals.Engine.SelectedLandList.Clear();
							var highlight = (PackedScene)ResourceLoader.Load("res://Components/Highlight.tscn");
							Highlight newHighlight = (Highlight)highlight.Instance();
							newHighlight.Translate(initialClickPosition);
							AddChild(newHighlight);

							if (!highlightSelectionlist.Contains(newHighlight))
							{
								highlightSelectionlist.Add(newHighlight);
							}
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
			else
			{
				leftButtonClicked = false;

				if (globals.InputMode == Globals.InputModeType.Place)
				{
					foreach (Highlight light in highlightSelectionlist)
					{
						var spaceState = GetWorld().DirectSpaceState;
						var result = spaceState.IntersectRay(
							from: new Vector3(light.Translation.x + 0.5f, 1.0f, light.Translation.z + 0.5f),
							to: new Vector3(light.Translation.x + 0.5f, 0.0f, light.Translation.z + 0.5f),
							exclude: null,
							collisionMask: 2147483647,
							collideWithBodies: true,
							collideWithAreas: true);

						var selectedCollider = ((StaticBody)result["collider"]);
						var selectedLand = ((Land)selectedCollider.GetParent());

						selectedLand.SetLandType(globals.PlacementModeTypeToLandSpaceType(globals.PlacementMode));
						globals.Engine.Map[selectedLand.Position].Type = globals.PlacementModeTypeToLandSpaceType(globals.PlacementMode);

						light.QueueFree();
					}

					highlightSelectionlist.Clear();
				}

				if (globals.InputMode == Globals.InputModeType.Select)
				{
					foreach (Highlight light in highlightSelectionlist)
					{
						var spaceState = GetWorld().DirectSpaceState;
						var result = spaceState.IntersectRay(
							from: new Vector3(light.Translation.x + 0.5f, 1.0f, light.Translation.z + 0.5f),
							to: new Vector3(light.Translation.x + 0.5f, 0.0f, light.Translation.z + 0.5f),
							exclude: null,
							collisionMask: 2147483647,
							collideWithBodies: true,
							collideWithAreas: true);

						var selectedCollider = ((StaticBody)result["collider"]);
						var selectedLand = ((Land)selectedCollider.GetParent());

						landSelectionList.Add(selectedLand);

						globals.Engine.SelectedLandList.Add(globals.Engine.Map[selectedLand.Position]);
						selectedLand.Selected();

						light.QueueFree();
					}

					highlightSelectionlist.Clear();
				}
			}
		}

		if (leftButtonClicked && inputEvent is InputEventMouseMotion motionEvent)
		{

			if (globals.InputMode != Globals.InputModeType.None)
			{
				var fromPos = camera.ProjectRayOrigin(motionEvent.Position);
				var toPos = fromPos + camera.ProjectRayNormal(motionEvent.Position) * 1000; // FIXME - what should this actually be?
				var space_state = GetWorld().DirectSpaceState;
				var selection = space_state.IntersectRay(fromPos, toPos);

				try
				{
					var selectedCollider = ((StaticBody)selection["collider"]);
					var selectedLand = ((Land)selectedCollider.GetParent());
					currentClickPosition = selectedLand.Translation;

					if (currentClickPosition != previousClickPosition)
					{
						// Clear the current list of highlights.
						foreach (Highlight light in highlightSelectionlist)
						{
							light.QueueFree();
						}

						highlightSelectionlist.Clear();

						var xIncrement = 2.0f;
						var zIncrement = 2.0f;

						if (initialClickPosition.x > currentClickPosition.x)
						{
							xIncrement = -2.0f;
						}

						if (initialClickPosition.z > currentClickPosition.z)
						{
							zIncrement = -2.0f;
						}

						// Draw a rectangle of highlights from the initial click position to the current click position.
						for (var x = initialClickPosition.x; x != currentClickPosition.x + xIncrement; x += xIncrement)
						{
							for (var z = initialClickPosition.z; z != currentClickPosition.z + zIncrement; z += zIncrement)
							{
								var highlight = (PackedScene)ResourceLoader.Load("res://Components/Highlight.tscn");
								Highlight newHighlight = (Highlight)highlight.Instance();
								newHighlight.Translate(new Vector3(x, 0, z));
								AddChild(newHighlight);

								if (!highlightSelectionlist.Contains(newHighlight))
								{
									highlightSelectionlist.Add(newHighlight);
								}
							}
						}

						previousClickPosition = currentClickPosition;
					}
				}
				catch(Exception){}
			}
		}
	}

	private void On_ResidentialButton_pressed()
	{
		GD.Print("Residential Button pressed!");
		GD.Print($"PlacementMode = {globals.PlacementMode}");
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
