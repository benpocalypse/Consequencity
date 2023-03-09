using Godot;
using System;
using System.Collections.Generic;

public partial class DecisionDialog : Popup
{
	[Signal]
	public delegate void DecisionMadeEventHandler(string _decisionText);

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
			//newButton.Connect("pressed", new Callable(this, nameof(_on_DecisionButton_pressed)) );
			newButton.Pressed += _on_DecisionButton_pressed;
			
			// , new Godot.Collections.Array { newButton.Text }
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

	public void _on_DecisionButton_pressed()//(string buttonText = "")
	{
		GD.Print("Decision Made button pressed!");
		EmitSignal(nameof(DecisionMadeEventHandler), "blaaah");//buttonText);
		this.QueueFree();
	}
}
