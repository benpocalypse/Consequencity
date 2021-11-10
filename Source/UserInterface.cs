using Godot;
using System;
using System.Linq;

public class UserInterface : Control
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

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
	}

	public override void _Process(float delta)
	{
		_timeCounter += delta;

		if ( _timeCounter >= 1.0f )
		{
			((RichTextLabel)GetNode("Date")).Text = $"Date: {globals.Engine.Date.ToString("MMMM dd, yyyy")}";
			((RichTextLabel)GetNode("Population")).Text = $"Population: {globals.Engine.Population}";

			// FIXME - Have this account for multi-select.
			var infoType = globals.Engine.SelectedLandList.Count == 1 ?
				globals.Engine.SelectedLandList[0].Type.ToString() :
				globals.Engine.SelectedLandList.Count == 0 ?
					string.Empty :
					globals.Engine.SelectedLandList.Select(_ => _.Type).Distinct().Count() == 1 ?
						globals.Engine.SelectedLandList[0].Type.ToString() :
						"Multiple Types Selected";

			var infoDensity = globals.Engine.SelectedLandList.Sum(_ => _.Density).ToString();
			var infoPopulation = globals.Engine.SelectedLandList.Sum(_ => _.Population).ToString();
			var infoValue = globals.Engine.SelectedLandList.Sum(_ => _.Value).ToString();
			var infoCrime = globals.Engine.SelectedLandList.Sum(_ => _.Crime).ToString();
			var infoPollution = globals.Engine.SelectedLandList.Sum(_ => _.Pollution).ToString();

			((RichTextLabel)GetNode("InfoPopup")).Text =
$@"Information:
   Type: {infoType}
   Density: {infoDensity}
   Population: {infoPopulation}
   Value: {infoValue}
   Crime: {infoCrime}
   Pollution: {infoPollution}";

	 		((RichTextLabel)GetNode("InfoPopup")).Visible = infoType == string.Empty ? false : true;

			((ProgressBar)GetNode("ResidentialProgress")).Value = globals.Engine.Demand[Globals.LandSpaceType.Residential];
			((ProgressBar)GetNode("CommercialProgress")).Value = globals.Engine.Demand[Globals.LandSpaceType.Commercial];
			((ProgressBar)GetNode("IndustrialProgress")).Value = globals.Engine.Demand[Globals.LandSpaceType.Industrial];
			((ProgressBar)GetNode("AgriculturalProgress")).Value = globals.Engine.Demand[Globals.LandSpaceType.Agricultural];
		}

		if (Input.IsActionPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}

	public void _on_Residential_pressed()
	{
		if (globals.PlacementMode != Globals.PlacementModeType.Residential)
		{
			globals.PlacementMode = Globals.PlacementModeType.Residential;
		}

		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_ResidentialButton_pressed));
	}

	public void _on_Commercial_pressed()
	{
		if (globals.PlacementMode != Globals.PlacementModeType.Commercial)
		{
			globals.PlacementMode = Globals.PlacementModeType.Commercial;
		}

		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_CommercialButton_pressed));
	}

	public void _on_Industrial_pressed()
	{
		if (globals.PlacementMode != Globals.PlacementModeType.Industrial)
		{
			globals.PlacementMode = Globals.PlacementModeType.Industrial;
		}

		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_IndustrialButton_pressed));
	}

	public void _on_Agricultural_pressed()
	{
		if (globals.PlacementMode != Globals.PlacementModeType.Agricultural)
		{
			globals.PlacementMode = Globals.PlacementModeType.Agricultural;
		}

		globals.InputMode = Globals.InputModeType.Place;

		EmitSignal(nameof(On_AgriculturalButton_pressed));
	}

	public void _on_Transportation_pressed()
	{
		if (globals.PlacementMode != Globals.PlacementModeType.Transportation)
		{
			globals.PlacementMode = Globals.PlacementModeType.Transportation;
		}

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
		EmitSignal(nameof(On_SelectButton_pressed));
	}
}
