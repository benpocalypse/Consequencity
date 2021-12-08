using Godot;
using System;

public class MenuButton : Node2D
{
    public enum ButtonParentDirection
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
                _below?.ParentPressed(ButtonParentDirection.Above, pressed);
            }
            else
            {
                _right?.ParentPressed(ButtonParentDirection.Left, pressed);
                _left?.ParentPressed(ButtonParentDirection.Right, pressed);
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
            GetNode<Button>("Button").Pressed = false;
            _below?.ParentPressed(ButtonParentDirection.Above, parentPressed);
            _right?.ParentPressed(ButtonParentDirection.Left, parentPressed);
            _left?.ParentPressed(ButtonParentDirection.Right, parentPressed);
        }

        switch (parentDirection)
        {
            case ButtonParentDirection.Above:
                _below?.ParentPressed(ButtonParentDirection.Above, parentPressed);
                break;

            case ButtonParentDirection.Left:
                _right?.ParentPressed(ButtonParentDirection.Left, parentPressed);
                break;

            case ButtonParentDirection.Right:
                _left?.ParentPressed(ButtonParentDirection.Right, parentPressed);
                break;
        }
    }

    public override void _Ready()
    {
        GetNode<Button>("Button").Text = _unpressedText;
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
