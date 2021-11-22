using Godot;
using System;
using System.Collections.Generic;

public class DecisionDialog : PopupDialog
{
    [Signal]
    public delegate void DecisionMade(string _decisionText);

    private string _decisionText = string.Empty;
    private List<string> _decisionButtonText = new List<string>();

    public override void _Ready()
    {
        var decisionText = GetNode<Label>("VBoxContainer/DecisionText");
        decisionText.Text = _decisionText;
        var blerpSize = decisionText.RectSize;

        foreach (var decision in _decisionButtonText)
        {
            var newButton = new Button();
            newButton.Text = decision;
            newButton.Connect("pressed", this, nameof(_on_DecisionButton_pressed), new Godot.Collections.Array { newButton.Text });
            newButton.RectMinSize = new Vector2(72, 40);
            GetNode<HBoxContainer>("VBoxContainer/HBoxContainer").AddChild(newButton);
        }
    }

    public void New(string _decisionText, List<string> _decisionButtonText)
    {
        this._decisionText = _decisionText;
        this._decisionButtonText = _decisionButtonText;
        this.Popup_();
    }

    public void _on_DecisionButton_pressed(string buttonText)
    {
        EmitSignal(nameof(DecisionMade), buttonText);
        this.QueueFree();
    }
}
