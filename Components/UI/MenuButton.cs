using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

public class MenuButton : IObservable, IObserver
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
    public bool Visible = false;
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

    private bool _isEnabled = true;
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

    private Action _actionToPeform = null;
    public Action ActionToPerform
    {
        get => _actionToPeform;
        set => _actionToPeform = value;
    }

    public MenuButton(ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId)
    {
        _direction = direction;
        _unpressedText = unpressedText;
        _pressedText = pressedText;
        _isRootNode = isRootNode;
        _rootParentId = rootParentId;
    }

    public MenuButton(ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId, Action newAction)
    {
        _direction = direction;
        _unpressedText = unpressedText;
        _pressedText = pressedText;
        _isRootNode = isRootNode;
        _rootParentId = rootParentId;
        _actionToPeform = newAction;
    }

    public MenuButton WithAction(Action newAction)
    {
        this._actionToPeform = newAction;
        return this;
    }

    public MenuButton WithIsEnabled(bool enabled)
    {
        this._isEnabled = enabled;
        return this;
    }

    public MenuButton AddChildButton(MenuButton child)
    {
        switch (child.Direction)
        {
            case ButtonDirection.Left:
                _left = child;
                break;

            case ButtonDirection.Right:
                _right = child;
                break;

            case ButtonDirection.Below:
                _below = child;
                break;
        }

        return this;
    }

    public void ButtonPressed()
    {
        //if ( _isEnabled && Visible )
        if (Visible)
        {
            Pressed = !Pressed;
            Text = !Pressed ? _unpressedText :
                _pressedText == string.Empty ?
                    _unpressedText :
                    _pressedText;

            _left?.ParentPressed(ButtonDirection.Left, IsRootNode, RootParentId);
            _right?.ParentPressed(ButtonDirection.Right, IsRootNode, RootParentId);
            _below?.ParentPressed(ButtonDirection.Below, IsRootNode, RootParentId);

            Notify();
        }
    }

    public void ParentPressed(ButtonDirection parentDirection, bool isRooteNode, int rootPressedId)
    {
        //if (_isEnabled && (rootPressedId <= RootParentId || rootPressedId == 0) && isRooteNode == true)
        if ((rootPressedId <= RootParentId || rootPressedId == 0) && isRooteNode == true)
        {
            switch (parentDirection)
            {
                case ButtonDirection.Below:
                    _below?.ParentPressed(ButtonDirection.Below, isRooteNode, rootPressedId);
                    Visible = !Visible;
                    Notify();
                    break;

                case ButtonDirection.Left:
                    _left?.ParentPressed(ButtonDirection.Left, isRooteNode, rootPressedId);
                    Visible = !Visible;
                    Notify();
                    break;

                case ButtonDirection.Right:
                    _right?.ParentPressed(ButtonDirection.Right, isRooteNode, rootPressedId);
                    Visible = !Visible;
                    Notify();
                    break;
            }
        }
    }

    // Observable
    private ImmutableList<IObserver> _observers = ImmutableList<IObserver>.Empty;
    public void AddObserver(IObserver observer)
    {
        _observers = _observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _observers = _observers.Remove(observer);
    }

    public void Notify()
    {
        _observers.ForEach(obs => obs.PropertyChanged(this));
    }

    // Observer
    public void PropertyChanged(IObservable observable)
    {
        // FIXME - Buttons will also need to watch for when GameFeatures change to enable/disable themselves.
        if (observable is GameFeature feature)
        {

        }
        else
        {
            ButtonPressed();
        }
    }
}
