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
	public delegate void On_DeleteButton_pressed();

	private Globals globals;

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
	}

	public override void _Process(float delta)
	{
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

	public void _on_Delete_pressed()
	{
		globals.InputMode = Globals.InputModeType.None;
		
		EmitSignal(nameof(On_DeleteButton_pressed));
	}
}
