using Godot;
using System;

public class MenuTree : Node2D
{
    private int _numVertical = 1;
    private int _numHorizontal = 1;

    private bool _isReady = false;
    public MenuButton RootButton;

    public override void _Ready()
    {
        _isReady = true;
    }

    public void New(string unpressedText, string pressedText)
    {
        RootButton = new MenuButton(
            direction: MenuButton.ButtonDirection.Below,
            unpressedText: unpressedText,
            pressedText: pressedText,
            isRootNode: true,
            rootParentId: 0
        );

        /*
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

            var curve = RootButton.GetNode<Path2D>("Path2D").Curve;
            curve.ClearPoints();

            AddChild(RootButton);
        }
        */
    }

/*
    public MenuButton NewButton(MenuButton.ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId)
    {
        var newButtonScene = (PackedScene)ResourceLoader.Load("res://Components/UI/MenuButton.tscn");
        var newButton = (MenuButton)newButtonScene.Instance();
        GD.Print($"newButton.Id = {newButton.NativeInstance}");
        newButton.Direction = direction;
        newButton.IsEnabled = true;
        newButton.Visible = false;
        newButton.UnpressedText = unpressedText;
        newButton.IsRootNode = isRootNode;
        newButton.RootParentId = rootParentId;
        newButton.PressedText = pressedText;

        var curve = newButton.GetNode<Path2D>("Path2D").Curve;
        curve.ClearPoints();
        curve.AddPoint(new Vector2(0,0));

        switch (direction)
        {
            case MenuButton.ButtonDirection.Left:
                curve.AddPoint(new Vector2(-100,0));
                newButton.Translate(new Vector2(-100, 50 * _numVertical));
                break;

            case MenuButton.ButtonDirection.Right:
                curve.AddPoint(new Vector2(100, 0));
                newButton.Translate(new Vector2(100 * _numHorizontal, 50 * _numVertical));
                _numHorizontal += 1;
                break;

            case MenuButton.ButtonDirection.Below:
                curve.AddPoint(new Vector2(0, 50));
                newButton.Translate(new Vector2(0, 50 * _numVertical));
                _numVertical += 1;
                break;
        }

        AddChild(newButton);

        return newButton;
    }
*/

//  public override void _Process(float delta)
//  {
//
//  }
}
