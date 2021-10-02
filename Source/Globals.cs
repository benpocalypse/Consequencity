using Godot;
using System;
using System.Collections.Generic;

public class Globals : Node
{
	public enum Scenes
	{
		Unknown,
		Titlescreen,
		//CharacterSelectScreen,
		//CreditsScreen,
		Gameplay,
		ThreeDTest,
		//Level2,
		//Level3,
		Gameover
	};
	
	public enum LandSpaceType
	{
		Residential = 0,
		Commercial = 1,
		Industrial = 2,
		Agricultural = 3,
		None = 4
		// Water???
	}

	public enum InputModeType
	{
		None,
		Residential,
		Commercial,
		Industrial,
		Delete
	}

	public LandSpaceType InputModeTypeToLandSpaceType(InputModeType _type)
	{
		LandSpaceType result = LandSpaceType.None;
		switch (_type)
		{
			case InputModeType.None:
				result = LandSpaceType.None;
				break;

			case InputModeType.Residential:
				result = LandSpaceType.Residential;
				break;

			case InputModeType.Commercial:
				result = LandSpaceType.Commercial;
				break;

			case InputModeType.Industrial:
				result = LandSpaceType.Industrial;
				break;
		}

		return result;
	}

	public const int MaximumLandValue = 100;
	public InputModeType InputMode = InputModeType.None;


	// TODO - Maybe this isn't needed? Perhaps just the amount that each property type affects it's next nearest
	//        neighbor is good enough? We'll see I guess.
	private Dictionary<LandSpaceType, int> ValueAffectAgacency = new Dictionary<Globals.LandSpaceType, int>()
	{
		{LandSpaceType.Residential, 2},
		{LandSpaceType.Commercial, 2},
		{LandSpaceType.Industrial, 6},
		{LandSpaceType.Agricultural, 4 }
	};

	private Dictionary<LandSpaceType, float> ValueAffect = new Dictionary<Globals.LandSpaceType, float>()
	{
		{LandSpaceType.Residential, 0.5f},
		{LandSpaceType.Commercial, 0.8f},
		{LandSpaceType.Industrial, 1.0f},
		{LandSpaceType.Agricultural, 0.2f}
	};

	static Globals()
	{
	}

	private Globals()
	{
	}

	private static readonly Globals instance = new Globals();
	public static Globals Instance
	{
		get
		{
			return instance;
		}
	}

	public const int ScreenWidth = 1280;
	public const int ScreenHeight = 720;
/*
	public readonly Vector2 MoneyBagLocation = new Vector2(1200, 56);

	public int PlayerScore = 0;
	public int PlayerMaxHealth = 3;
	public int PlayerHealth = 3;
	public int LeftArmDamage = 1;
	public int RightArmDamage = 1;
*/

	// Data to persist
	private const string saveFile = "user://saveFile.save";
	public int HighestScore = 0;
	public bool FirstTimePlaying = true;

	public Node CurrentSceneFile { get; set; }

	public override void _Ready()
	{
		//RestorePersistedData();

		Viewport root = GetTree().Root;
		CurrentSceneFile = root.GetChild(root.GetChildCount() - 1);
	}

	public override void _Notification(int notification)
	{
		// Save on quit. Note that you can call `DataManager.Save()` whenever you want
		if (notification == MainLoop.NotificationWmQuitRequest)
		{
			StorePersistedData();
			GetTree().Quit();
		}
	}

/*
	public override void _Process(float delta)
	{

	}
*/

	public void StorePersistedData()
	{
		var file = new File();

		var fileExists = file.FileExists(saveFile);

		if (!fileExists)
		{
			file.Open(saveFile, File.ModeFlags.Write);
		}
		else
		{
			file.Open(saveFile, File.ModeFlags.ReadWrite);
		}

		file.StoreVar(FirstTimePlaying);
		file.StoreVar(HighestScore);

		file.Close();
	}

	public void RestorePersistedData()
	{
		var file = new File();

		var ifExists = file.FileExists(saveFile);

		if (file.FileExists(saveFile))
		{
			file.Open(saveFile, File.ModeFlags.Read);

			FirstTimePlaying = (bool)file.GetVar();
			HighestScore = (int)file.GetVar();

			file.Close();
		}
	}

	public void GotoScene(Scenes nextScene)
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:
		CallDeferred(nameof(DeferredGotoScene), "res://Scenes/" + nextScene.ToString() + ".tscn");
	}

	public void DeferredGotoScene(string path)
	{
		// It is now safe to remove the current scene
		CurrentSceneFile.Free();

		// Load a new scene.
		var nextScene = (PackedScene)GD.Load(path);

		// Instance the new scene.
		CurrentSceneFile = nextScene.Instance();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(CurrentSceneFile);

		// Optionally, to make it compatible with the SceneTree.change_scene() API.
		GetTree().CurrentScene = CurrentSceneFile;
	}
}
