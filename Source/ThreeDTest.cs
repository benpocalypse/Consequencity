using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ThreeDTest : Node3D
{
	private Node3D cameraBase;
	private Camera3D camera;
	private Globals globals;

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
		cameraBase = ((Node3D)this.GetNode("Cambase"));
		camera = ((Camera3D)cameraBase.GetNode("Camera3D"));
		PopulateEngineMap();
	}

	public static Vector3 PointOnCircle(float radius, float angleInDegrees, Vector3 origin, float factor)
	{
		// Convert from degrees to radians via multiplication by PI/180
		float x = (float)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.X;
		float y = (float)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.Y;

		return new Vector3(90, y, factor *  x);
	}

	private double _angleInDegrees = 0.0f;

	public override void _Process(double delta)
	{
		var camPosition = cameraBase.GlobalTransform.Origin;

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

		var sunLight = GetNode<Node3D>("SunLight");
		if (globals.GameRunning != Globals.GameRunningType.Paused)
		{
			_angleInDegrees += delta * ((double)globals.Gamespeed) * 30;
			var pOc1 = PointOnCircle(90, ((float)_angleInDegrees), new Vector3(30, 0, 90), 1);
			sunLight.Position = pOc1;
			sunLight.LookAt(new Vector3(30,0,30), Vector3.Left);
		}

		//var pathToFollow = GetNode<PathFollow3D>("Path3D/PathFollow3D");
		//pathToFollow.Offset += 10 * delta;

		globals.Economy.Update(((float)delta) * ((float)globals.Gamespeed) * ((float)globals.GameRunning));
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
				switch (mouseEvent.ButtonIndex)
				{
					case MouseButton.Left:
						leftButtonClicked = true;

						var fromPos = camera.ProjectRayOrigin(mouseEvent.Position);
						var toPos = fromPos + camera.ProjectRayNormal(mouseEvent.Position) * 1000; // FIXME - what should this number actually be?
						var space_state = GetWorld3D().DirectSpaceState;
						var selection = space_state.IntersectRay(PhysicsRayQueryParameters3D.Create(fromPos, toPos));

						try
						{
							var selectedCollider = ((StaticBody3D)selection["collider"]);
							var selectedLand = ((Land)selectedCollider.GetParent());

							initialClickPosition = new Vector3(selectedLand.LandPosition.X, selectedLand.LandPosition.Y, 0);
							previousClickPosition = new Vector3(selectedLand.LandPosition.X, selectedLand.LandPosition.Y, 0);
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
							Highlight newHighlight = (Highlight)highlight.Instantiate();
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
									playerHouse = (Building)playerHouseScene.Instantiate();
									playerHouse.Translate(initialClickPosition);
									AddChild(playerHouse);

									globals.Features = globals.Features.SetGameFeatureValue(GameFeature.FeatureType.PlayerHouseNotPlaced, false);
								}
							}
						}

						// FIXME - We don't have "delete" implemented correctly.

						break;

					case MouseButton.WheelUp :
						var newPosition = new Vector3(0, -0.5f, 0);
						cameraBase.Translate(newPosition);
						break;

					case MouseButton.WheelDown:
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
						var spaceState = GetWorld3D().DirectSpaceState;
						var result = spaceState.IntersectRay(
							PhysicsRayQueryParameters3D.Create(
							from: new Vector3(light.Position.X + 0.5f, 1.0f, light.Position.Z + 0.5f),
							to: new Vector3(light.Position.X + 0.5f, 0.0f, light.Position.Z + 0.5f),
							exclude: null,
							collisionMask: 2147483647)
						);

						var selectedCollider = ((StaticBody3D)result["collider"]);
						var selectedLand = ((Land)selectedCollider.GetParent());

						selectedLand.SetLandType(globals.PlacementModeTypeToLandSpaceType(globals.PlacementZone));
						globals.Economy.Map[selectedLand.LandPosition].Type = globals.PlacementModeTypeToLandSpaceType(globals.PlacementZone);

						if (globals.PlacementZone == Globals.PlacementZoneType.Agricultural)
						{
							var gardenScene = (PackedScene)ResourceLoader.Load("res://Components/Garden.tscn");
							var garden = (Node3D)gardenScene.Instantiate();
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
						var spaceState = GetWorld3D().DirectSpaceState;
						var result = spaceState.IntersectRay(
							PhysicsRayQueryParameters3D.Create(
							from: new Vector3(light.Position.X + 0.5f, 1.0f, light.Position.Z + 0.5f),
							to: new Vector3(light.Position.X + 0.5f, 0.0f, light.Position.Z + 0.5f),
							exclude: null,
							collisionMask: 2147483647)
						);

						var selectedCollider = ((StaticBody3D)result["collider"]);
						var selectedLand = ((Land)selectedCollider.GetParent());

						landSelectionList.Add(selectedLand);

						globals.Economy.SelectedLandList.Add(globals.Economy.Map[selectedLand.LandPosition]);
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
				var spaceState = GetWorld3D().DirectSpaceState;
				var selection = spaceState.IntersectRay(PhysicsRayQueryParameters3D.Create(fromPos, toPos));

				try
				{
					var selectedCollider = ((StaticBody3D)selection["collider"]);
					var selectedLand = ((Land)selectedCollider.GetParent());
					// FIXME - might need to make selectedLand.Position into a Vector3
					currentClickPosition = new Vector3(selectedLand.LandPosition.X, selectedLand.LandPosition.Y, 0);

					if (currentClickPosition != previousClickPosition)
					{
						var xIncrement = 2.0f;
						var zIncrement = 2.0f;

						if (initialClickPosition.X > currentClickPosition.X)
						{
							xIncrement = -2.0f;
						}

						if (initialClickPosition.Z > currentClickPosition.Z)
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
							for (var x = initialClickPosition.X; x != currentClickPosition.X + xIncrement; x += xIncrement)
							{
								for (var z = initialClickPosition.X; z != currentClickPosition.Z + zIncrement; z += zIncrement)
								{
									var highlight = (PackedScene)ResourceLoader.Load("res://Components/Highlight.tscn");
									Highlight newHighlight = (Highlight)highlight.Instantiate();
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
							playerHouse.Position = new Vector3(currentClickPosition.X, 1.0f, currentClickPosition.Z);
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
			Land newLand = (Land)land.Instantiate();
			var newPos = new Vector3(space.Key.X * 2, 0, space.Key.Y * 2);
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
				Building newBuilding = (Building)building.Instantiate();
				var housePosition = new Vector3(agent.Home.X * 2, 0, agent.Home.Y * 2);
				newBuilding.Translate(housePosition);
				newBuilding.SetType(Globals.LandSpaceType.Residential);
				AddChild(newBuilding);
			}

			if (agent.HasJob != false && agent.JobHasBeenDrawn == false)
			{
				agent.JobHasBeenDrawn = true;
				var building = (PackedScene)ResourceLoader.Load("res://Components/Building.tscn");
				Building newBuilding = (Building)building.Instantiate();
				newBuilding.SetType((Globals.LandSpaceType)agent.JobType);
				var jobPosition = new Vector3(agent.JobLocation.X* 2, 0, agent.JobLocation.Y * 2);
				newBuilding.Translate(jobPosition);
				AddChild(newBuilding);
			}
		}
	}
}
