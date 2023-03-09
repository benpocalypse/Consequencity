using Godot;
using System;

public partial class MenuTree : Node2D
{
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
        RootButton.Visible = true;
    }

    public void Visualize(MenuButton node, int xOffset, int yOffset)
    {
        var resouceStringName = string.Empty;

        switch (node.Direction)
        {
            case MenuButton.ButtonDirection.Below:
                resouceStringName = "res://Components/UI/MenuButtonVisualBelow.tscn";
                break;

            case MenuButton.ButtonDirection.Right:
                resouceStringName = "res://Components/UI/MenuButtonVisualRight.tscn";
                break;

            case MenuButton.ButtonDirection.Left:
                resouceStringName = "res://Components/UI/MenuButtonVisualBelow.tscn";
                break;
        }

        PackedScene visualButtonScene = (PackedScene)ResourceLoader.Load(resouceStringName);
        var newButton = (Node)visualButtonScene.Instantiate();

        // FIXME - move this concern into the MenuButtonVisual node
        var button = newButton.GetNode<Button>("Path2D/PathFollow2D/Button");
        button.Text = node.UnpressedText;

        if (xOffset == 0 && yOffset == 0)
        {
            button.Visible = true;
        }
        else
        {
            button.Visible = false;
        }

        ((MenuButtonVisual)newButton).AddObserver(node);
        ((MenuButton)node).AddObserver((MenuButtonVisual)newButton);

        ((MenuButtonVisual)newButton).Translate(new Vector2(xOffset, yOffset));
        this.AddChild(newButton);

        if (node.Left != null)
        {
            Visualize(node.Left, xOffset - 100, yOffset + 0);
        }

        if (node.Right != null)
        {
            Visualize(node.Right, xOffset + 100, yOffset + 0);
        }

        if (node.Below != null)
        {
            Visualize(node.Below, xOffset + 0, yOffset + 50);
        }
    }

//  public override void _Process(float delta)
//  {
//
//  }
}
