using Godot;
using System;

public class MenuButton : Node2D
{
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

    // FIXME - abandon using non-godot stuff to make all this work. Just glue on to godot where I can.
    private bool _isVisible = false;
    public bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }


    private bool _isPressed = false;
    public bool IsPressed
    {
        get => _isPressed;
        set
        {
            _isPressed = value;
            GetNode<ButtonText>("Button").Pressed = _isPressed;
        }
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

    public void Pressed()
    {
        if ( _isEnabled && _isVisible )
        {
            _isPressed = !_isPressed;
        }

        _right?.ParentPressed();
        _left?.ParentPressed();
        _below?.ParentPressed();
    }

    public void ParentPressed()
    {
        // FIXME - move into position?
        if (_isEnabled)
        {
            _isVisible = !_isVisible;
        }

        _right?.ParentPressed();
        _left?.ParentPressed();
        _below?.ParentPressed();
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
        this.Pressed();
    }
}
