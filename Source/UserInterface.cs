using Godot;
using System;

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

			var infoType = globals.Engine.SelectedLandList.Count > 0 ? globals.Engine.SelectedLandList[0].Type.ToString() : string.Empty;
			var infoPopulation = globals.Engine.SelectedLandList.Count > 0 ? globals.Engine.SelectedLandList[0].Population .ToString(): string.Empty;
			var infoValue = globals.Engine.SelectedLandList.Count > 0 ? globals.Engine.SelectedLandList[0].Value.ToString() : string.Empty;

			((RichTextLabel)GetNode("InfoPopup")).Text =
$@"Information:
     Type: {infoType}
     Population: {infoPopulation}
     Value: {infoValue}";

	 		((RichTextLabel)GetNode("InfoPopup")).Visible = infoType == string.Empty ? false : true;

			/*
			((RichTextLabel)GetNode("Demand")).BbcodeText = $"[right]Demand" + System.Environment.NewLine +
															$"{globals.Engine.Demand[Globals.LandSpaceType.Residential]} R" + System.Environment.NewLine +
															$"{globals.Engine.Demand[Globals.LandSpaceType.Commercial]} C" + System.Environment.NewLine +
															$"{globals.Engine.Demand[Globals.LandSpaceType.Industrial]} I" + System.Environment.NewLine +
															$"{globals.Engine.Demand[Globals.LandSpaceType.Agricultural]} A[/right]";
			*/

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
