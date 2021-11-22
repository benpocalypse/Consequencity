using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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
		Transportation = 4,
		None = 5
	}

	public enum JobType
	{
		None = 0,
		Commercial = 1,
		Industrial = 2,
		Agricultural = 3
	}

	public enum PlacementModeType
	{
		None,
		Residential,
		Commercial,
		Industrial,
		Agricultural,
		Transportation,
	}

	public enum InputModeType
	{
		None,
		Select,
		Place
	}

	public enum GameRunningType
	{
		Paused = 0,
		Playing = 1
	}

	public enum GametimeType
	{
		Normal = 1,
		Fast = 2,
		Faster = 3,
		Fasteset = 4
	}

	public EconomicEngine Economy;
	public DecisionEngine Decisions;
	public PlacementModeType PlacementMode = PlacementModeType.None;
	public InputModeType InputMode = InputModeType.None;

	public ImmutableList<GameFeature> Features = ImmutableList<GameFeature>.Empty
			.Add(new GameFeature(GameFeature.FeatureType.ResidentialZoning, false))
			.Add(new GameFeature(GameFeature.FeatureType.CommercialZoning, false))
			.Add(new GameFeature(GameFeature.FeatureType.IndustrialZoning, false))
			.Add(new GameFeature(GameFeature.FeatureType.AgriculturalZoning, false))
			.Add(new GameFeature(GameFeature.FeatureType.DeleteZoning, false))
			.Add(new GameFeature(GameFeature.FeatureType.PopulationGrowthRate, 1.0f));


	private GameRunningType _gameRunning = GameRunningType.Playing;
	public GameRunningType GameRunning
	{
		get => _gameRunning;
		set => _gameRunning = value;
	}

	private GametimeType _gamespeed = GametimeType.Normal;
	public GametimeType Gamespeed
	{
		get => _gamespeed;
		set => _gamespeed = value;
	}

	static Globals()
	{
	}

	private Globals()
	{
	}

	private static Globals instance;
	public static Globals Instance
	{
		get
		{
			return instance;
		}
	}

	public void PopupDialog(string _decisionText, List<string> _decisions)
	{
		var decisionDialogScene = (PackedScene)ResourceLoader.Load("res://Components/UI/DecisionDialog.tscn");
		var decisionDialog = (DecisionDialog)decisionDialogScene.Instance();
		decisionDialog.New(
			_decisionText: _decisionText,
			_decisionButtonText: _decisions);

		decisionDialog.Connect("DecisionMade", this, "_on_DecisionMade");

		_gameRunning = GameRunningType.Paused;
		this.AddChild(decisionDialog);
		decisionDialog.PopupCentered();
	}

	public void _on_DecisionMade(string decisionText)
	{
		GD.Print($"Decision: {decisionText}");
		Decisions.DecisionMade(decisionText);
		_gameRunning = GameRunningType.Playing;
	}

	public LandSpaceType PlacementModeTypeToLandSpaceType(PlacementModeType _type)
	{
		LandSpaceType result = LandSpaceType.None;
		switch (_type)
		{
			case PlacementModeType.None:
				result = LandSpaceType.None;
				break;

			case PlacementModeType.Residential:
				result = LandSpaceType.Residential;
				break;

			case PlacementModeType.Commercial:
				result = LandSpaceType.Commercial;
				break;

			case PlacementModeType.Industrial:
				result = LandSpaceType.Industrial;
				break;

			case PlacementModeType.Agricultural:
				result = LandSpaceType.Agricultural;
				break;

			case PlacementModeType.Transportation:
				result = LandSpaceType.Transportation;
				break;
		}

		return result;
	}

/*
	public readonly Vector2 MoneyBagLocation = new Vector2(1200, 56);

	public int PlayerScore = 0;
	public int PlayerMaxHealth = 3;
	public int PlayerHealth = 3;
	public int LeftArmDamage = 1;
	public int RightArmDamage = 1;
*/

	// Save file data to persist
	private const string saveFile = "user://saveFile.save";
	public int HighestScore = 0;
	public bool FirstTimePlaying = true;

	public Node CurrentSceneFile { get; set; }

	public override void _Ready()
	{
		//RestorePersistedData();

		Viewport root = GetTree().Root;
		CurrentSceneFile = root.GetChild(root.GetChildCount() - 1);

		instance = GetNode<Globals>("/root/ConsequencityGlobals");
		Decisions =  new DecisionEngine();
		Economy = new EconomicEngine(this.Features);
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
