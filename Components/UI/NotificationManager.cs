using Godot;
using System;
using System.Collections.Immutable;
using System.Linq;

public sealed class NotificationManager : Node
{
    public enum NotificationType
    {
        Ephemeral = 0,
        Actionable,
        Sticky
    }

    public enum NotificationIconType
    {
        None = 0,
        Warning,
        Question
    }

    private bool _isReady = false;
    private float _secondTick = 0.0f;

    private ImmutableList<Notification> _notificationList = ImmutableList<Notification>.Empty;

    public override void _Ready()
    {
        _isReady = true;
    }

    public override void _Process(float delta)
    {
        _secondTick += delta;

        if (_secondTick >= 1.0f)
        {
            _secondTick = 0.0f;

            ImmutableList<Notification> notificationsToRemove = ImmutableList<Notification>.Empty;

            foreach (Notification notify in _notificationList)
            {
                if (notify.Type == NotificationType.Ephemeral)
                {
                    notify.EphemeralTime = notify.EphemeralTime.Subtract(new TimeSpan(0,0,1));

                    if (notify.EphemeralTime == new TimeSpan(0,0,0))
                    {
                        notificationsToRemove = notificationsToRemove.Add(notify);

                        New(
                            type: NotificationType.Ephemeral,
                            icon: NotificationManager.NotificationIconType.None,
                            text: "But now this one...");

                        New(
                            type: NotificationType.Ephemeral,
                            icon: NotificationManager.NotificationIconType.None,
                            text: "and this one will appear!");
                    }
                }
            }

            foreach (var notify in notificationsToRemove)
            {
                _notificationList = _notificationList.Remove(notify);
                notify.Dismiss();
            }
        }
    }

    public void New(NotificationType type, NotificationIconType icon, string text)
    {
        if (_isReady)
        {
            var notification = (PackedScene)ResourceLoader.Load("res://Components/UI/Notification.tscn");

            Notification newNotification = (Notification)notification.Instance();

            newNotification.Connect("Acknowledged", this, nameof(Notification_Acknowledged));
            newNotification.New(
                type: type,
                icon: icon,
                text: text
            );

            VBoxContainer vbox = GetNode<VBoxContainer>("UiLayer/VBoxContainer");
            vbox.AddChild(newNotification);
            _notificationList = _notificationList.Add(newNotification);
        }
    }

    public void Notification_Acknowledged(Notification acknowledgedNotification)
    {
        _notificationList = _notificationList.Remove(acknowledgedNotification);
        acknowledgedNotification.Dismiss();

        New(
            type: NotificationType.Ephemeral,
            icon: NotificationManager.NotificationIconType.None,
            text: "This notification will disappear in roughly 5 seconds.");
    }
}
