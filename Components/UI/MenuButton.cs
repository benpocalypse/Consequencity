using Godot;
using System;

public class MenuButton : Node2D
{

    public enum ButtonParentDirection
    {
        left,
        right,
        above,
        below
    };

    private string _text = string.Empty;
    public string ButtonText
    {
        get => _text;
        set
        {
            _text = value;
            GetNode<Button>("Button").Text = _text;
        }
    }

    private bool _isEnabled = false;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    private MenuButton _left = null;
    public MenuButton Left
    {
        get => _left;
        set => _left = value;
    }

    private MenuButton _right = null;
    public MenuButton Right
    {
        get => _right;
        set => _right = value;
    }

    private MenuButton _below = null;
    public MenuButton Below
    {
        get => _below;
        set => _below = value;
    }

    public void ButtonPressed()
    {
        if ( _isEnabled && Visible )
        {
            var pressed = GetNode<Button>("Button").Pressed;

            if (_right == null && _left == null )
            {
                _below?.ParentPressed(ButtonParentDirection.above, pressed);
            }
            else
            {
                _right?.ParentPressed(ButtonParentDirection.left, pressed);
                _left?.ParentPressed(ButtonParentDirection.right, pressed);
            }
        }
    }

    public void ParentPressed(ButtonParentDirection parentDirection, bool parentPressed)
    {
        // FIXME - move into position?
        if (_isEnabled)
        {
            Visible = !Visible;
        }

        if (parentPressed == false)
        {
            Visible = false;
        }

        var pressed = GetNode<Button>("Button").Pressed;

        switch (parentDirection)
        {
            case ButtonParentDirection.above:
                _below?.ParentPressed(ButtonParentDirection.above, parentPressed);
                break;

            case ButtonParentDirection.below:
                // FIXME - I don't think this can happen.
                break;

            case ButtonParentDirection.left:
                _right?.ParentPressed(ButtonParentDirection.left, parentPressed);
                break;

            case ButtonParentDirection.right:
                _left?.ParentPressed(ButtonParentDirection.right, parentPressed);
                break;
        }
    }

    public override void _Ready()
    {

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }

    public void _on_Button_pressed()
    {
        this.ButtonPressed();
    }
}
