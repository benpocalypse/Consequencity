using System;
using System.Collections.Generic;
using System.Collections.Immutable;


public interface IObserver
{
    void PropertyChanged(IObservable observable);
}

public interface IObservable
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void Notify();
}
