using System;

public sealed class MenuItem
{
    private bool _isEnabled = false;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

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
        set => _isPressed = value;
    }

    private MenuItem _left = null;
    public MenuItem Left
    {
        get => _left;
        set => _left = value;
    }

    private MenuItem _right = null;
    public MenuItem Right
    {
        get => _right;
        set => _right = value;
    }

    private MenuItem _below = null;
    public MenuItem Below
    {
        get => _below;
        set => _below = value;
    }

    // FIXME - maybe remove this?
    public MenuItem(MenuItem below)
    {
        _below = below;
    }

    public MenuItem(MenuItem left = null, MenuItem right = null, MenuItem below = null)
    {
        _left = left;
        _right = right;
        _below = below;
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
}