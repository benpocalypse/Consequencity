using Godot;
using System;

public class MenuTree : Node2D
{
    private bool _isReady = false;
    public MenuButton RootButton;

    public override void _Ready()
    {
        _isReady = true;
    }

    public void New(string unpressedText, string pressedText)
    {
        if (_isReady)
        {
            var rootButtonScene = (PackedScene)ResourceLoader.Load("res://Components/UI/MenuButton.tscn");
            RootButton = (MenuButton)rootButtonScene.Instance();
            //rootButton.Translate(new Vector2(100, 100));
            RootButton.Direction = MenuButton.ButtonDirection.Below;
            RootButton.IsEnabled = true;
            RootButton.UnpressedText = unpressedText;
            RootButton.PressedText = pressedText;
            AddChild(RootButton);
        }
    }

    public MenuButton NewButton(MenuButton.ButtonDirection direction, string unpressedText, string pressedText)
    {
        var newButtonScene = (PackedScene)ResourceLoader.Load("res://Components/UI/MenuButton.tscn");
        var newButton = (MenuButton)newButtonScene.Instance();
        newButton.Direction = direction;
        newButton.IsEnabled = true;
        newButton.Visible = false;
        newButton.UnpressedText = unpressedText;
        newButton.PressedText = pressedText;

        return newButton;
    }

    public MenuTree WithChildButton(MenuButton.ButtonDirection direction, string unpressedText, string pressedText)
    {
        var newButtonScene = (PackedScene)ResourceLoader.Load("res://Components/UI/MenuButton.tscn");
        var newButton = (MenuButton)newButtonScene.Instance();
        newButton.Direction = direction;
        newButton.IsEnabled = true;
        newButton.UnpressedText = unpressedText;
        newButton.PressedText = pressedText;
        RootButton.AddChild(newButton);

        return this;
    }

//  public override void _Process(float delta)
//  {
//
//  }
}
