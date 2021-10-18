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

	private Globals globals;

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
	}

	public override void _Process(float delta)
	{
		((RichTextLabel)GetNode("Population")).Text = $"Population: {globals.Engine.Population}";

		if (Input.IsActionPressed("ui_cancel"))
		{
			GetTree().Quit();
		}		
	}

	public void _on_Residential_pressed()
	{
		if (globals.InputMode != Globals.InputModeType.Residential)
		{
			globals.InputMode = Globals.InputModeType.Residential;
		}
		else
		{
			globals.InputMode = Globals.InputModeType.None;
		}

		EmitSignal(nameof(On_ResidentialButton_pressed));
	}

	public void _on_Commercial_pressed()
	{
		if (globals.InputMode != Globals.InputModeType.Commercial)
		{
			globals.InputMode = Globals.InputModeType.Commercial;
		}
		else
		{
			globals.InputMode = Globals.InputModeType.None;
		}

		EmitSignal(nameof(On_CommercialButton_pressed));
	}

	public void _on_Industrial_pressed()
	{
		if (globals.InputMode != Globals.InputModeType.Industrial)
		{
			globals.InputMode = Globals.InputModeType.Industrial;
		}
		else
		{
			globals.InputMode = Globals.InputModeType.None;
		}

		EmitSignal(nameof(On_IndustrialButton_pressed));
	}

	public void _on_Agricultural_pressed()
	{
		if (globals.InputMode != Globals.InputModeType.Agricultural)
		{
			globals.InputMode = Globals.InputModeType.Agricultural;
		}
		else
		{
			globals.InputMode = Globals.InputModeType.None;
		}

		EmitSignal(nameof(On_AgriculturalButton_pressed));
	}

	public void _on_Transportation_pressed()
	{
		if (globals.InputMode != Globals.InputModeType.Transportation)
		{
			globals.InputMode = Globals.InputModeType.Transportation;
		}
		else
		{
			globals.InputMode = Globals.InputModeType.None;
		}

		EmitSignal(nameof(On_TransportationButton_pressed));
	}

	public void _on_Delete_pressed()
	{
		globals.InputMode = Globals.InputModeType.None;
		
		EmitSignal(nameof(On_DeleteButton_pressed));
	}
}
