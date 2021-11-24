using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class UserInterface : Control, IObserver
{
	[Signal]
	public delegate void On_ResidentialButton_pressed();

	[Signal]
	public delegate void On_CommercialButton_pressed();

	[Signal]
	public delegate void On_IndustrialButton_pressed();

	[Signal]
	public delegate void On_AgriculturalButton_pressed();

	[Signal]
	public delegate void On_TransportationButton_pressed();

	[Signal]
	public delegate void On_DeleteButton_pressed();

	[Signal]
	public delegate void On_SelectButton_pressed();

	private Globals globals;

	private float _timeCounter = 0.0f;
	private bool _menuFadingIn = false;
	private bool _menuFadingOut = false;

	public void PropertyChanged(IObservable observable)
	{
		if (observable is GameFeature feature)
		{
			switch (feature.BooleanFeature.Key)
			{
				case GameFeature.FeatureType.ResidentialZoning:
					GetNode<Button>("Path2D/PathFollow2D/Residential").Disabled = !feature.BooleanFeature.Value;
					break;

				case GameFeature.FeatureType.CommercialZoning:
					GetNode<Button>("Path2D/PathFollow2D/Path2D/PathFollow2D/Commercial").Disabled = !feature.BooleanFeature.Value;
					break;

				case GameFeature.FeatureType.IndustrialZoning:
					GetNode<Button>("Path2D/PathFollow2D/Path2D/PathFollow2D/Industrial").Disabled = !feature.BooleanFeature.Value;
					break;

				case GameFeature.FeatureType.AgriculturalZoning:
					GetNode<Button>("Path2D/PathFollow2D/Path2D/PathFollow2D/Agricultural").Disabled = !feature.BooleanFeature.Value;
					break;

				case GameFeature.FeatureType.TransportationZoning:
					GetNode<Button>("Path2D/PathFollow2D/Path2D/PathFollow2D/Transporation").Disabled = !feature.BooleanFeature.Value;
					break;

				case GameFeature.FeatureType.DeleteZoning:
					GetNode<Button>("Path2D/PathFollow2D/Delete").Disabled = !feature.BooleanFeature.Value;
					break;
			}
		}
	}

    public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");

		var notification = (PackedScene)ResourceLoader.Load("res://Components/UI/Notification.tscn");
		Notification newNotification = (Notification)notification.Instance();
		((Control)newNotification).SetPosition(new Vector2((1920-470)/2, 75));
		AddChild(newNotification);

		// FIXME - in the future only have this watch the features it cares about, not all the features.
		globals.Features.ForEach(_ => _.Add(this));
	}

	public override void _Process(float delta)
	{
		if (_menuFadingIn == true)
		{
			var rootPathFollow2d = ((PathFollow2D)GetNode("Path2D/PathFollow2D"));
			rootPathFollow2d.Offset += 150 * delta;

			var rootPathChildren = rootPathFollow2d.GetChildren();

			foreach (var rootChild in rootPathChildren)
			{
				if (rootChild.GetType().ToString() == "Godot.Button")
				{
					((Button)rootChild).Modulate = new Color(1,1,1,rootPathFollow2d.Offset/48);
				}

				if (rootPathFollow2d.Offset >= 47)
				{
					if (rootChild.GetType().ToString() == "Godot.Button")
					{
						//((BaseButton)rootChild).Disabled = false;
					}

					var childPathFollow2d = ((PathFollow2D)GetNode("Path2D/PathFollow2D/Path2D/PathFollow2D"));
					childPathFollow2d.Offset += 150 * delta;

					var subPathChildren = childPathFollow2d.GetChildren();

					foreach (var subChild in subPathChildren)
					{
						((Button)subChild).Modulate = new Color(1,1,1,childPathFollow2d.Offset/136);
					}
				}
			}
		}

		if (_menuFadingOut == true)
		{
			var childPathFollow2d = ((PathFollow2D)GetNode("Path2D/PathFollow2D/Path2D/PathFollow2D"));
			childPathFollow2d.Offset -= 400 * delta;

			var subPathChildren = childPathFollow2d.GetChildren();

			foreach (var subChild in subPathChildren)
			{
				((Button)subChild).Modulate = new Color(1,1,1,childPathFollow2d.Offset/136);
			}

			if (childPathFollow2d.Offset <= 0)
			{
				var rootPathFollow2d = ((PathFollow2D)GetNode("Path2D/PathFollow2D"));
				rootPathFollow2d.Offset -= 150 * delta;

				var rootPathChildren = rootPathFollow2d.GetChildren();

				foreach (var rootChild in rootPathChildren)
				{
					if (rootChild.GetType().ToString() == "Godot.Button")
					{
						((Button)rootChild).Modulate = new Color(1,1,1,rootPathFollow2d.Offset/48);
					}

					if (rootPathFollow2d.Offset <= 0)
					{
						if (rootChild.GetType().ToString() == "Godot.Button")
							{
								//((BaseButton)rootChild).Disabled = true;
							}
					}
				}
			}
		}

		_timeCounter += delta;


		// Update our Info/Popup displays
		if ( _timeCounter >= 1.0f )
		{
			var speed = globals.GameRunning == Globals.GameRunningType.Playing ?
				globals.Gamespeed.ToString() :
				"Paused";
			GetNode<Label>("StatPanel/VBoxContainer/Gamespeed").Text = $"Speed: {speed}";
			GetNode<Label>("StatPanel/VBoxContainer/Date").Text = $"Date: {globals.Economy.Date.ToString("MMMM dd, yyyy")}";
			GetNode<Label>("StatPanel/VBoxContainer/Population").Text = $"Population: {globals.Economy.Population}";
			GetNode<Label>("StatPanel/VBoxContainer/Funds").Text = $"Funds: {globals.Economy.Funds}";

			// FIXME - Have this account for multi-select.
			var infoType = globals.Economy.SelectedLandList.Count == 1 ?
				globals.Economy.SelectedLandList[0].Type.ToString() :
				globals.Economy.SelectedLandList.Count == 0 ?
					string.Empty :
					globals.Economy.SelectedLandList.Select(_ => _.Type).Distinct().Count() == 1 ?
						globals.Economy.SelectedLandList[0].Type.ToString() :
						"Multiple Types Selected";

			var infoDensity = globals.Economy.SelectedLandList.Sum(_ => _.Density).ToString();
			var infoPopulation = globals.Economy.SelectedLandList.Sum(_ => _.Population).ToString();
			var infoValue = globals.Economy.SelectedLandList.Sum(_ => _.Value).ToString();
			var infoCrime = globals.Economy.SelectedLandList.Sum(_ => _.Crime).ToString();
			var infoPollution = globals.Economy.SelectedLandList.Sum(_ => _.Pollution).ToString();

			((RichTextLabel)GetNode("InfoPanel/InfoPopup")).Text =
$@"Information:
   Type: {infoType}
   Density: {infoDensity}
   Population: {infoPopulation}
   Value: {infoValue}
   Crime: {infoCrime}
   Pollution: {infoPollution}";

	 		((PanelContainer)GetNode("InfoPanel")).Visible =
				((RichTextLabel)GetNode("InfoPanel/InfoPopup")).Visible =
					infoType == string.Empty ? false : true;

			((ProgressBar)GetNode("ResidentialProgress")).Value = globals.Economy.Demand[Globals.LandSpaceType.Residential];
			((ProgressBar)GetNode("CommercialProgress")).Value = globals.Economy.Demand[Globals.LandSpaceType.Commercial];
			((ProgressBar)GetNode("IndustrialProgress")).Value = globals.Economy.Demand[Globals.LandSpaceType.Industrial];
			((ProgressBar)GetNode("AgriculturalProgress")).Value = globals.Economy.Demand[Globals.LandSpaceType.Agricultural];
		}

		if (Input.IsActionPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}

	public void _on_Residential_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.Residential;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_ResidentialButton_pressed));
	}

	public void _on_Commercial_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.Commercial;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_CommercialButton_pressed));
	}

	public void _on_Industrial_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.Industrial;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_IndustrialButton_pressed));
	}

	public void _on_Agricultural_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.Agricultural;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_AgriculturalButton_pressed));
	}

	public void _on_Transportation_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.Transportation;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_TransportationButton_pressed));
	}

	// FIXME - Revisit this. May need another input mode or something like that.
	public void _on_Delete_pressed()
	{
		globals.PlacementMode = Globals.PlacementModeType.None;
		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_DeleteButton_pressed));
	}

	public void _on_Select_pressed()
	{
		globals.InputMode = Globals.InputModeType.Select;
		globals.PlacementMode = Globals.PlacementModeType.None;

		EmitSignal(nameof(On_SelectButton_pressed));
	}

	public void _on_MenuButton_pressed()
	{
		if (_menuFadingIn == false)
		{
			((Button)GetNode("MenuButton")).Text = "^";
			_menuFadingIn = true;
			_menuFadingOut = false;
			globals.InputMode = Globals.InputModeType.None;
			globals.PlacementMode = Globals.PlacementModeType.None;
		}
		else
		{
			((Button)GetNode("MenuButton")).Text = ">";
			_menuFadingIn = false;
			_menuFadingOut = true;
			globals.InputMode = Globals.InputModeType.None;
			globals.PlacementMode = Globals.PlacementModeType.None;
		}
	}

	public void _on_PlayPauseButton_pressed()
	{
		if (globals.GameRunning == Globals.GameRunningType.Playing)
		{
			((Button)GetNode("PlayPauseButton")).Text = "||";
			((Button)GetNode("SpeedupButton")).Disabled = true;
			((Button)GetNode("SlowdownButton")).Disabled = true;
			globals.GameRunning = Globals.GameRunningType.Paused;
		}
		else
		{
			((Button)GetNode("PlayPauseButton")).Text = ">";

			if (globals.Gamespeed != Globals.GametimeType.Fasteset)
			{
				((Button)GetNode("SpeedupButton")).Disabled = false;
			}

			if (globals.Gamespeed != Globals.GametimeType.Normal)
			{
				((Button)GetNode("SlowdownButton")).Disabled = false;
			}

			globals.GameRunning = Globals.GameRunningType.Playing;
		}
	}

	public void _on_SpeedupButton_pressed()
	{
		if (globals.Gamespeed < Globals.GametimeType.Fasteset)
		{
			globals.Gamespeed++;
			((Button)GetNode("SpeedupButton")).Disabled = false;
			((Button)GetNode("SlowdownButton")).Disabled = false;
		}
		else
		{
			((Button)GetNode("SpeedupButton")).Disabled = true;
		}
	}

	public void _on_SlowdownButton_pressed()
	{
		if (globals.Gamespeed > Globals.GametimeType.Normal)
		{
			globals.Gamespeed--;
			((Button)GetNode("SlowdownButton")).Disabled = false;
			((Button)GetNode("SpeedupButton")).Disabled = false;
		}
		else
		{
			((Button)GetNode("SlowdownButton")).Disabled = true;
		}
	}
}
