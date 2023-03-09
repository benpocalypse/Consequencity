using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

public static class MenuButtonExtensions
{
    public static MenuButton FindButtonByText(this MenuButton button, string buttonText)
    {
        MenuButton result = null;

        if (button != null && button.UnpressedText == buttonText)
        {
            result =  button;
        }
        else
        {
            if (button.Left != null)
            {
                result = button?.Left.FindButtonByText(button?.Left, buttonText);
            }

            if (result == null)
            {
                if (button.Right != null)
                {
                    result = button?.Right.FindButtonByText(button?.Right, buttonText);
                }
            }

            if (result == null)
            {
                if (button.Below != null)
                {
                    result = button?.Below.FindButtonByText(button?.Below, buttonText);
                }
            }
        }

        return result;
    }
}

public partial class MenuButton : IObservable, IObserver
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

    private bool _hasChildren = false;
    public bool HasChildren
    {
        get => _hasChildren;
        set => _hasChildren = value;
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

    private Action _actionWhenPressed = null;
    private Action _actionWhenUnpressed = null;

    public MenuButton(ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId)
    {
        _direction = direction;
        _unpressedText = unpressedText;
        _pressedText = pressedText;
        _hasChildren = isRootNode;
        _rootParentId = rootParentId;
    }

    public MenuButton(ButtonDirection direction, string unpressedText, string pressedText, bool isRootNode, int rootParentId, Action newAction)
    {
        _direction = direction;
        _unpressedText = unpressedText;
        _pressedText = pressedText;
        _hasChildren = isRootNode;
        _rootParentId = rootParentId;
        _actionWhenPressed = newAction;
    }

    public MenuButton WithPressedAction(Action newAction)
    {
        this._actionWhenPressed = newAction;
        return this;
    }

    public MenuButton WithUnpressedAction(Action newAction)
    {
        this._actionWhenUnpressed = newAction;
        return this;
    }

    public MenuButton WithIsEnabled(bool enabled)
    {
        this._isEnabled = enabled;
        return this;
    }

    public MenuButton WithObserveGameFeature(GameFeature feature)
    {
        feature.AddObserver(this);
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
        if (Visible)
        {
            Pressed = !Pressed;
            Text = !Pressed ? _unpressedText :
                _pressedText == string.Empty ?
                    _unpressedText :
                    _pressedText;

            if (Pressed && _actionWhenPressed != null)
            {
                _actionWhenPressed();
            }

            if (!Pressed && _actionWhenUnpressed != null)
            {
                _actionWhenUnpressed();
            }

            _left?.ParentPressed(ButtonDirection.Left, Pressed, HasChildren, RootParentId);
            _right?.ParentPressed(ButtonDirection.Right, Pressed, HasChildren, RootParentId);
            _below?.ParentPressed(ButtonDirection.Below, Pressed, HasChildren, RootParentId);

            Notify();
        }
    }

    public void ButtonUnpressed()
    {
        if (Visible)
        {
            Pressed = false;

            Text = !Pressed ? _unpressedText :
                _pressedText == string.Empty ?
                    _unpressedText :
                    _pressedText;

            if (_actionWhenUnpressed != null)
            {
                _actionWhenUnpressed();
            }

            Notify();
        }
    }

    public void ParentPressed(ButtonDirection parentDirection, bool rootPressed, bool hasChildren, int rootPressedId)
    {
        if ((rootPressedId <= RootParentId || rootPressedId == 0) && hasChildren == true)
        {
            if (rootPressed == false)
            {
                _below?.ParentPressed(ButtonDirection.Below, rootPressed, hasChildren, rootPressedId);
                _left?.ParentPressed(ButtonDirection.Left, rootPressed, hasChildren, rootPressedId);
                _right?.ParentPressed(ButtonDirection.Right, rootPressed, hasChildren, rootPressedId);

                ButtonUnpressed();
                Visible = false;
                Notify();
            }
            else
            {
                switch (parentDirection)
                {
                    case ButtonDirection.Below:
                        _below?.ParentPressed(ButtonDirection.Below, rootPressed, hasChildren, rootPressedId);
                        Visible = !Visible;
                        Notify();
                        break;

                    case ButtonDirection.Left:
                        _left?.ParentPressed(ButtonDirection.Left, rootPressed, hasChildren, rootPressedId);
                        Visible = !Visible;
                        Notify();
                        break;

                    case ButtonDirection.Right:
                        _right?.ParentPressed(ButtonDirection.Right, rootPressed, hasChildren, rootPressedId);
                        Visible = !Visible;
                        Notify();
                        break;
                }
            }
        }
    }

    public MenuButton FindButtonByText(MenuButton button, string buttonText)
    {
        MenuButton result = null;

        if (button != null && button.UnpressedText == buttonText)
        {
            result =  button;
        }
        else
        {
            if (button.Left != null)
            {
                result = button?.Left.FindButtonByText(Left, buttonText);
            }

            if (result == null)
            {
                if (button.Right != null)
                {
                    result = button?.Right.FindButtonByText(Right, buttonText);
                }
            }

            if (result == null)
            {
                if (button.Below != null)
                {
                    result = button?.Below.FindButtonByText(Below, buttonText);
                }
            }
        }

        return result;
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
        // TODO - This assumes only the Enabled property will be updated here.
        if (observable is GameFeature feature)
        {
            IsEnabled = feature.BooleanFeature.Value;
            Notify();
        }
        else
        {
            ButtonPressed();
        }
    }
}
