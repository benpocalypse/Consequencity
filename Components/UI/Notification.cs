using Godot;
using System;
using static NotificationManager;

public class Notification : Control
{
    [Signal]
    public delegate void Acknowledged();
    private bool _isReady = false;
    private NotificationType _type = NotificationType.Ephemeral;
    public NotificationType Type
    {
        get => _type;
    }
    private NotificationIconType _icon = NotificationIconType.None;
    private string _notificationText = string.Empty;
    private TimeSpan _ephemeralTime = new TimeSpan();
    public TimeSpan EphemeralTime
    {
        get => _ephemeralTime;
        set => _ephemeralTime = value;
    }

    public override void _Ready()
    {
        _isReady = true;

        if (_notificationText != string.Empty)
        {
             var labelText = GetNode<Label>("UiLayer/PanelContainer/HBoxContainer/NotificationText");
            labelText.Text = _notificationText;

            var button = GetNode<Button>("UiLayer/PanelContainer/HBoxContainer/Button");
            switch (_type)
            {
                case NotificationType.Actionable:
                    button.Connect("pressed", this, nameof(_on_Button_pressed));
                    button.Visible = true;
                    break;

                case NotificationType.Ephemeral:
                    _ephemeralTime = new TimeSpan(0,0,5);
                    button.Visible = false;
                    break;

                case NotificationType.Sticky:
                    button.Visible = false;
                    break;
            }
        }
    }


    public void _on_Button_pressed()
    {
        if (_type == NotificationType.Actionable)
        {
            EmitSignal(nameof(Acknowledged), this);
        }
    }

    public void New(NotificationType type, NotificationIconType icon, string text)
    {
        if (_isReady)
        {
            var labelText = GetNode<Label>("UiLayer/PanelContainer/HBoxContainer/NotificationText");
            labelText.Text = text;

            var button = GetNode<Button>("UiLayer/PanelContainer/HBoxContainer/Button");
            switch (type)
            {
                case NotificationType.Actionable:
                button.Connect("pressed", this, nameof(_on_Button_pressed));
                    button.Visible = true;
                    break;

                case NotificationType.Ephemeral:
                    _ephemeralTime = new TimeSpan(0,0,5);
                    button.Visible = false;
                    break;

                case NotificationType.Sticky:
                    button.Visible = false;
                    break;
            }
        }
        else
        {
            _notificationText = text;
            _type = type;
            _icon = icon;
        }
    }
}
