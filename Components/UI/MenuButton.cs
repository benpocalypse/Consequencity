using Godot;
using System;

public class MenuButton : Node2D
{
    public enum ButtonDirection
    {
        Left,
        Right,
        Above,
        Below
    };

    private string _unpressedText = string.Empty;
    public string UnpressedText
    {
        get => _unpressedText;
        set
        {
            _unpressedText = value;
        }
    }

    private string _pressedText = string.Empty;
    public string PressedText{
        get => _pressedText;
        set
        {
            _pressedText = value;
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

    private ButtonDirection _direction = ButtonDirection.Below;
    public ButtonDirection Direction
    {
        get => _direction;
        set =>  _direction = value;
    }

    public void AddChild(MenuButton child)
    {
        switch (child.Direction)
        {
            case ButtonDirection.Left:
                _left = child;
                AddChild(Left);
                break;

            case ButtonDirection.Right:
                _right = child;
                AddChild(Right);
                break;

            case ButtonDirection.Below:
                _below = child;
                AddChild(Below);
                break;

            case ButtonDirection.Above:
                break;
        }
    }

    public void ButtonPressed()
    {
        if ( _isEnabled && Visible )
        {
            var pressed = GetNode<Button>("Button").Pressed;

            GetNode<Button>("Button").Text = !pressed ? _unpressedText :
                _pressedText == string.Empty ?
                    _unpressedText :
                    _pressedText;

            if (_right == null && _left == null )
            {
                _below?.ParentPressed(ButtonDirection.Above, pressed);
            }
            else
            {
                _right?.ParentPressed(ButtonDirection.Left, pressed);
                _left?.ParentPressed(ButtonDirection.Right, pressed);
            }
        }
    }

    public void ParentPressed(ButtonDirection parentDirection, bool parentPressed)
    {
        // FIXME - move into position?
        if (_isEnabled)
        {
            Visible = !Visible;
        }

        if (parentPressed == false)
        {
            Visible = false;
            GetNode<Button>("Button").Pressed = false;
            _below?.ParentPressed(ButtonDirection.Above, parentPressed);
            _right?.ParentPressed(ButtonDirection.Left, parentPressed);
            _left?.ParentPressed(ButtonDirection.Right, parentPressed);
        }

        switch (parentDirection)
        {
            case ButtonDirection.Above:
                _below?.ParentPressed(ButtonDirection.Above, parentPressed);
                break;

            case ButtonDirection.Left:
                _right?.ParentPressed(ButtonDirection.Left, parentPressed);
                break;

            case ButtonDirection.Right:
                _left?.ParentPressed(ButtonDirection.Right, parentPressed);
                break;
        }
    }

    public override void _Ready()
    {
        GetNode<Button>("Button").Text = _unpressedText;
    }

    public void _on_Button_pressed()
    {
        this.ButtonPressed();
    }
}
