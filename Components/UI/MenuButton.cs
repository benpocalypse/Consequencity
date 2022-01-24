using System;
using System.Diagnostics;

public class MenuButton
{
    public enum ButtonDirection
    {
        Left,
        Right,
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

    // FIXME - Godot overrides.
    public bool Visible = true;
    public bool Pressed = false;
    public string Text = string.Empty;

    private bool _isRootNode = false;
    public bool IsRootNode
    {
        get => _isRootNode;
        set => _isRootNode = value;
    }

    private int _rootParentId = 0;
    public int RootParentId
    {
        get => _rootParentId;
        set => _rootParentId = value;
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

    public delegate void ButtonCreated();
    public MenuButton(ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId)
    {
        _direction = direction;
        _unpressedText = unpressedText;
        _pressedText = pressedText;
        _isRootNode = isRootNode;
        _rootParentId = rootParentId;

        this?.ButtonCreated();
    }

    public MenuButton AddChildButton(MenuButton child)
    {
        switch (child.Direction)
        {
            case ButtonDirection.Left:
                _left = child;
                //_left.Translate(new Vector2(-100, 0));
                //AddChild(Left);
                break;

            case ButtonDirection.Right:
                _right = child;
                //_right.Translate(new Vector2(100, 0));
                //AddChild(Right);
                break;

            case ButtonDirection.Below:
                _below = child;
                //_below.Translate(new Vector2(0, 50));
                //AddChild(Below);
                break;
        }

        //AddChild(child);

        return this;
    }

    public void ButtonPressed()
    {
        if ( _isEnabled && Visible )
        {
            Text = !Pressed ? _unpressedText :
                _pressedText == string.Empty ?
                    _unpressedText :
                    _pressedText;

            _left?.ParentPressed(ButtonDirection.Left, IsRootNode, RootParentId, Pressed);
            _right?.ParentPressed(ButtonDirection.Right, IsRootNode, RootParentId, Pressed);
            _below?.ParentPressed(ButtonDirection.Below, IsRootNode, RootParentId, Pressed);
        }
    }

    public void ParentPressed(ButtonDirection parentDirection, bool isRooteNode, int rootPressedId, bool parentPressed)
    {
        // FIXME - move into position?
        if (_isEnabled && (rootPressedId <= RootParentId || rootPressedId == 0) && isRooteNode == true)
        {
            switch (parentDirection)
            {
                case ButtonDirection.Below:
                    _below?.ParentPressed(ButtonDirection.Below, isRooteNode, rootPressedId, parentPressed);
                    Visible = !Visible;
                    break;

                case ButtonDirection.Left:
                    _left?.ParentPressed(ButtonDirection.Left, isRooteNode, rootPressedId, parentPressed);
                    Visible = !Visible;
                    break;

                case ButtonDirection.Right:
                    _right?.ParentPressed(ButtonDirection.Right, isRooteNode, rootPressedId, parentPressed);
                    Visible = !Visible;
                    break;
            }
        }
    }

    public void _on_Button_pressed()
    {
        this.ButtonPressed();
    }
}
