using System;
using System.Collections.Generic;
using System.Collections.Immutable;


public interface IObserver
{
    void PropertyChanged(IObservable observable);
}

public interface IObservable
{
    void Add(IObserver observer);
    void Remove(IObserver observer);
    void Notify();
}
