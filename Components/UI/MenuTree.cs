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
            RootButton.Direction = MenuButton.ButtonDirection.Below;
            RootButton.IsEnabled = true;
            RootButton.IsRootNode = true;
            RootButton.UnpressedText = unpressedText;
            RootButton.PressedText = pressedText;
            RootButton.RootParentId = 0;
            AddChild(RootButton);
        }
    }

    public MenuButton NewButton(MenuButton.ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId)
    {
        var newButtonScene = (PackedScene)ResourceLoader.Load("res://Components/UI/MenuButton.tscn");
        var newButton = (MenuButton)newButtonScene.Instance();
        newButton.Direction = direction;
        newButton.IsEnabled = true;
        newButton.Visible = false;
        newButton.UnpressedText = unpressedText;
        newButton.IsRootNode = isRootNode;
        newButton.RootParentId = rootParentId;
        newButton.PressedText = pressedText;

        return newButton;
    }

//  public override void _Process(float delta)
//  {
//
//  }
}
