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
				// FIXME - Re-hook these up to the appropriate buttons from the new dynamic button system.
				/*
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
				*/
			}
		}
	}

	public override void _Ready()
	{
		globals = (Globals)GetNode("/root/ConsequencityGlobals");

		// FIXME - in the future only have this watch the features it cares about, not all the features.
		globals.Features.ForEach(feature => feature.AddObserver(this));

		var menuTree = GetNode<MenuTree>("MenuTree");
		menuTree.New(">", "^");

		menuTree
			.RootButton
				.AddChildButton(
					new MenuButton(
						direction: MenuButton.ButtonDirection.Below,
						unpressedText: "Zone",
						pressedText: "Zone",
						isRootNode: true,
						rootParentId: 2)
						.WithIsEnabled(false)
						.WithObserveGameFeature(globals.Features.First(feat => feat.BooleanFeature.Key == GameFeature.FeatureType.AgriculturalZoning))
						.AddChildButton(
							new MenuButton(
								direction: MenuButton.ButtonDirection.Right,
								unpressedText: "Residential",
								pressedText: "Residential",
								isRootNode: true,
								rootParentId: 4)
								.WithIsEnabled(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.ResidentialZoning).BooleanFeature.Value)
								.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.ResidentialZoning))
								.WithPressedAction(() =>
								{
									menuTree.RootButton.FindButtonByText("Commercial").ButtonUnpressed();
									menuTree.RootButton.FindButtonByText("Industrial").ButtonUnpressed();
									menuTree.RootButton.FindButtonByText("Agricultural").ButtonUnpressed();
									globals.PlacementMode = Globals.PlacementModeType.Zone;
									globals.PlacementZone = Globals.PlacementZoneType.Residential;
									globals.InputMode = Globals.InputModeType.Place;
								})
								.WithUnpressedAction(() =>
								{
									globals.PlacementMode = Globals.PlacementModeType.None;
									globals.PlacementZone = Globals.PlacementZoneType.None;
									globals.InputMode = Globals.InputModeType.None;
								})
								.AddChildButton(
									new MenuButton(
										direction: MenuButton.ButtonDirection.Right,
										unpressedText: "Agricultural",
										pressedText: "Agricultural",
										isRootNode: false,
										rootParentId: 3)
										.WithIsEnabled(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.AgriculturalZoning).BooleanFeature.Value)
										.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.AgriculturalZoning))
										.WithPressedAction(() =>
										{
											menuTree.RootButton.FindButtonByText("Commercial").ButtonUnpressed();
											menuTree.RootButton.FindButtonByText("Industrial").ButtonUnpressed();
											menuTree.RootButton.FindButtonByText("Residential").ButtonUnpressed();
											globals.PlacementMode = Globals.PlacementModeType.Zone;
											globals.PlacementZone = Globals.PlacementZoneType.Agricultural;
											globals.InputMode = Globals.InputModeType.Place;
										})
										.WithUnpressedAction(() =>
										{
											globals.PlacementMode = Globals.PlacementModeType.None;
											globals.PlacementZone = Globals.PlacementZoneType.None;
											globals.InputMode = Globals.InputModeType.None;
										})
										.AddChildButton(
											new MenuButton(
												direction: MenuButton.ButtonDirection.Right,
												unpressedText: "Commercial",
												pressedText: "Commercial",
												isRootNode: false,
												rootParentId: 3)
												.WithIsEnabled(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.CommercialZoning).BooleanFeature.Value)
												.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.CommercialZoning))
												.WithPressedAction(() =>
												{
													menuTree.RootButton.FindButtonByText("Residential").ButtonUnpressed();
													menuTree.RootButton.FindButtonByText("Industrial").ButtonUnpressed();
													menuTree.RootButton.FindButtonByText("Agricultural").ButtonUnpressed();
													globals.PlacementMode = Globals.PlacementModeType.Zone;
													globals.PlacementZone = Globals.PlacementZoneType.Commercial;
													globals.InputMode = Globals.InputModeType.Place;
												})
												.WithUnpressedAction(() =>
												{
													globals.PlacementMode = Globals.PlacementModeType.None;
													globals.PlacementZone = Globals.PlacementZoneType.None;
													globals.InputMode = Globals.InputModeType.None;
												})
												.AddChildButton(
												new MenuButton(
													direction: MenuButton.ButtonDirection.Right,
													unpressedText: "Industrial",
													pressedText: "Industrial",
													isRootNode: false,
													rootParentId: 3)
													.WithIsEnabled(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.IndustrialZoning).BooleanFeature.Value)
													.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.IndustrialZoning))
													.WithPressedAction(() =>
													{
														menuTree.RootButton.FindButtonByText("Commercial").ButtonUnpressed();
														menuTree.RootButton.FindButtonByText("Residential").ButtonUnpressed();
														menuTree.RootButton.FindButtonByText("Agricultural").ButtonUnpressed();
														globals.PlacementMode = Globals.PlacementModeType.Zone;
														globals.PlacementZone = Globals.PlacementZoneType.Industrial;
														globals.InputMode = Globals.InputModeType.Place;
													})
													.WithUnpressedAction(() =>
													{
														globals.PlacementMode = Globals.PlacementModeType.None;
														globals.PlacementZone = Globals.PlacementZoneType.None;
														globals.InputMode = Globals.InputModeType.None;
													})

												)
										)
								)
							)
						)
					.Below.AddChildButton(
						new MenuButton(
							direction: MenuButton.ButtonDirection.Below,
							unpressedText: "Place",
							pressedText: "Place",
							isRootNode: true,
							rootParentId: 1)
							.WithIsEnabled(false)
							.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.PlayerCanPlaceSpecial))
							.AddChildButton(
								new MenuButton(
									direction: MenuButton.ButtonDirection.Right,
									unpressedText: "Your House",
									pressedText: "Your House",
									isRootNode: false,
									rootParentId: 4
									)
									.WithIsEnabled(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.PlayerHouseNotPlaced).BooleanFeature.Value)
									.WithObserveGameFeature(globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.PlayerHouseNotPlaced))
									.WithPressedAction(() =>
									{
										menuTree.RootButton.FindButtonByText("Commercial").ButtonUnpressed();
										menuTree.RootButton.FindButtonByText("Residential").ButtonUnpressed();
										menuTree.RootButton.FindButtonByText("Industrial").ButtonUnpressed();
										menuTree.RootButton.FindButtonByText("Agricultural").ButtonUnpressed();
										globals.PlacementMode = Globals.PlacementModeType.Special;
										globals.PlacementSpecial = Globals.PlacementSpecialType.PlayerHouse;
										globals.PlacementZone = Globals.PlacementZoneType.None;
										globals.InputMode = Globals.InputModeType.Place;
									})
									.WithUnpressedAction(() =>
									{
										globals.PlacementMode = Globals.PlacementModeType.None;
										globals.PlacementSpecial = Globals.PlacementSpecialType.None;
										globals.PlacementZone = Globals.PlacementZoneType.None;
										globals.InputMode = Globals.InputModeType.None;
									})
								)
							);
							/*
					.Below.AddChildButton(
						new MenuButton(
							direction: MenuButton.ButtonDirection.Below,
							unpressedText: "Delete",
							pressedText: "Delete",
							isRootNode: false,
							rootParentId: 0)
							);
							*/

		menuTree.Visualize(menuTree.RootButton, 0, 0);
	}

	public override void _Process(float delta)
	{
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

	public void DisableAllButtons()
	{
		((Button)GetNode("PlayPauseButton")).Disabled = true;
		((Button)GetNode("SpeedupButton")).Disabled = true;
		((Button)GetNode("SlowdownButton")).Disabled = true;
	}

	public void EnableAllButtons()
	{
		((Button)GetNode("PlayPauseButton")).Disabled = false;
		((Button)GetNode("SpeedupButton")).Disabled = false;
		((Button)GetNode("SlowdownButton")).Disabled = false;
	}

	public void _on_PlayPauseButton_pressed()
	{
		if (globals.GameRunning == Globals.GameRunningType.Playing)
		{
			GD.Print("Play/Paused pressed while game is Playing.");
			((Button)GetNode("PlayPauseButton")).Text = ">";
			((Button)GetNode("SpeedupButton")).Disabled = true;
			((Button)GetNode("SlowdownButton")).Disabled = true;
			globals.GameRunning = Globals.GameRunningType.Paused;
		}
		else
		{
			GD.Print("Play/Paused pressed while game is Paused.");
			((Button)GetNode("PlayPauseButton")).Text = "||";

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
