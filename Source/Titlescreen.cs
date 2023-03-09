using Godot;
using System;

public partial class Titlescreen : Node
{
	private Globals globals;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");
	}

	private void _on_Start_Button_pressed()
	{
		globals.GotoScene(Globals.Scenes.ThreeDTest);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
