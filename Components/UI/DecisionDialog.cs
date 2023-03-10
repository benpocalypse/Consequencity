using Godot;
using System;
using System.Collections.Generic;

public partial class DecisionDialog : Popup
{
	[Signal]
	public delegate void DecisionMadeEventHandler(string decisionText);
	
	[Signal]
	public delegate void TestEventHandler();

	private string _decisionText = string.Empty;
	private List<string> _decisionButtonText = new List<string>();

	public override void _Ready()
	{
		var decisionText = GetNode<Label>("DecisionText");
		decisionText.Text = _decisionText;

		foreach (var decision in _decisionButtonText)
		{
			var newButton = new Button();
			newButton.Text = decision;
			newButton.Pressed += () => _on_DecisionButton_pressed(_decisionText);
			newButton.CustomMinimumSize = new Vector2(50, 30);
			newButton.Size = new Vector2(50, 30);

			GetNode<HBoxContainer>("HBoxContainer").AddChild(newButton);
		}
	}

	public void New(string decisionText, List<string> decisionButtonText)
	{
		this._decisionText = decisionText;
		this._decisionButtonText = decisionButtonText;
		this.Popup();
	}

	public void _on_DecisionButton_pressed(string buttonText = "")
	{
		//EmitSignal(SignalName.DecisionMade, buttonText);
		EmitSignal(SignalName.Test);
		this.QueueFree();
	}
}
