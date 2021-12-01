using System;

public sealed class MenuItem
{
    private bool _enabled = false;
    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
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
        if (_enabled && !_isPressed && _right != null)
        {
            _right.ParentPressed();
        }

        _isPressed = !_isPressed;
    }

    public void ParentPressed()
    {
        // FIXME - move into position?
        _isVisible = !_isVisible;
    }
}
