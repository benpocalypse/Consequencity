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
		PopulateEngineMap();
	}

	public static Vector3 PointOnCircle(float radius, float angleInDegrees, Vector3 origin, float factor)
	{
		// Convert from degrees to radians via multiplication by PI/180
		float x = (float)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.x;
		float y = (float)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.y;

		return new Vector3(90, y, factor *  x);
	}

	private float angleInDegrees = 0.0f;

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

		var sunLight = GetNode<Spatial>("SunLight");
		if (globals.GameRunning != Globals.GameRunningType.Paused)
		{
			angleInDegrees += delta * ((float)globals.Gamespeed) * 30;
			var pOc1 = PointOnCircle(90, angleInDegrees, new Vector3(30, 0, 90), 1);
			sunLight.Translation = pOc1;
			sunLight.LookAt(new Vector3(30,0,30), Vector3.Left);
		}

		//var pathToFollow = GetNode<PathFollow>("Path/PathFollow");
		//pathToFollow.Offset += 10 * delta;

		globals.Economy.Update(delta * ((float)globals.Gamespeed) * ((float)globals.GameRunning));
		globals.Decisions.Update();
		UpdateAgentMap();
	}

	private bool leftButtonClicked = false;
	private Vector3 initialClickPosition = new Vector3();
	private Vector3 previousClickPosition = new Vector3();
	private Vector3 currentClickPosition = new Vector3();
	private List<Highlight> highlightSelectionlist = new List<Highlight>();
	private List<Land> landSelectionList = new List<Land>();
	private Building playerHouse = null;

	// FIXME - Clean up this unweildy shitty monster of a function.
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed == true)
			{
				switch ((ButtonList)mouseEvent.ButtonIndex)
				{
					case ButtonList.Left:
						leftButtonClicked = true;

						var fromPos = camera.ProjectRayOrigin(mouseEvent.Position);
						var toPos = fromPos + camera.ProjectRayNormal(mouseEvent.Position) * 1000; // FIXME - what should this number actually be?
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
						globals.Economy.SelectedLandList.Clear();

						if (
							 (globals.InputMode == Globals.InputModeType.Select) ||
							 (globals.InputMode == Globals.InputModeType.Place && globals.PlacementMode == Globals.PlacementModeType.Zone)
						   )
						{
							var highlight = (PackedScene)ResourceLoader.Load("res://Components/Highlight.tscn");
							Highlight newHighlight = (Highlight)highlight.Instance();
							newHighlight.Translate(initialClickPosition);
							AddChild(newHighlight);

							if (!highlightSelectionlist.Contains(newHighlight))
							{
								highlightSelectionlist.Add(newHighlight);
							}
						}

						if (globals.InputMode == Globals.InputModeType.Place && globals.PlacementMode == Globals.PlacementModeType.Special)
						{
							if (globals.PlacementSpecial == Globals.PlacementSpecialType.PlayerHouse)
							{
								if (playerHouse == null)
								{
									var playerHouseScene = (PackedScene)ResourceLoader.Load("res://Components/Building.tscn");
									playerHouse = (Building)playerHouseScene.Instance();
									playerHouse.Translate(initialClickPosition);
									AddChild(playerHouse);

									globals.Features = globals.Features.SetGameFeatureValue(GameFeature.FeatureType.PlayerHouseNotPlaced, false);
								}
							}
						}

						// FIXME - We don't have "delete" implemented correctly.

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

						selectedLand.SetLandType(globals.PlacementModeTypeToLandSpaceType(globals.PlacementZone));
						globals.Economy.Map[selectedLand.Position].Type = globals.PlacementModeTypeToLandSpaceType(globals.PlacementZone);

						if (globals.PlacementZone == Globals.PlacementZoneType.Agricultural)
						{
							var gardenScene = (PackedScene)ResourceLoader.Load("res://Components/Garden.tscn");
							var garden = (Spatial)gardenScene.Instance();
							garden.Translate(initialClickPosition);
							AddChild(garden);
						}

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

						globals.Economy.SelectedLandList.Add(globals.Economy.Map[selectedLand.Position]);
						selectedLand.Selected();

						light.QueueFree();
					}

					highlightSelectionlist.Clear();
				}

				// FIXME - We don't have "delete" implemented correctly.
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

						if (globals.PlacementZone != Globals.PlacementZoneType.None)
						{
							// Clear the current list of highlights.
							foreach (Highlight light in highlightSelectionlist)
							{
								light.QueueFree();
							}

							highlightSelectionlist.Clear();

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
						}
						else if (globals.PlacementSpecial == Globals.PlacementSpecialType.PlayerHouse)
						{// TODO - What a hack this all is :/
							playerHouse.Translation = new Vector3(currentClickPosition.x,1.0f,currentClickPosition.z);
						}

						previousClickPosition = currentClickPosition;
					}
				}
				catch(Exception){}
			}

			// FIXME - We don't have "delete" implemented correctly.
		}
	}

	public void PopulateEngineMap()
	{
		foreach (var space in globals.Economy.Map)
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
		foreach (var agent in globals.Economy.Agents)
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
