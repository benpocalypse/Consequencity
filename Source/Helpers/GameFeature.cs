using System.Collections.Generic;
using System.Collections.Immutable;

public sealed class GameFeature : IObservable
{
    public enum FeatureType
    {
        ResidentialZoning = 0,
        CommercialZoning,
        IndustrialZoning,
        AgriculturalZoning,
        TransportationZoning,
        DeleteZoning
    }

    private KeyValuePair<FeatureType, bool> _booleanFeature = new KeyValuePair<FeatureType, bool>();
    public KeyValuePair<FeatureType, bool> BooleanFeature
    {
        get => _booleanFeature;
        set
        {
                if (_booleanFeature.Value != value.Value)
                {
                    _booleanFeature = value;
                    Notify();
                }
        }
    }

    private KeyValuePair<FeatureType, float> _floatFeature = new KeyValuePair<FeatureType, float>();
    public KeyValuePair<FeatureType, float> FloatFeature
    {
        get => _floatFeature;
        set
        {
                if (_floatFeature.Value != value.Value)
                {
                    _floatFeature = value;
                    Notify();
                }
        }
    }

    private ImmutableList<IObserver> _observers = ImmutableList<IObserver>.Empty;

    public GameFeature(FeatureType feature, bool enabled)
    {
        BooleanFeature = new KeyValuePair<FeatureType, bool>(feature, enabled);
    }

    public GameFeature WithValue(bool value)
    {
        this._booleanFeature = new KeyValuePair<FeatureType, bool>(_booleanFeature.Key, value);

        return this;
    }

    public GameFeature WithValue(float value)
    {
        this._floatFeature = new KeyValuePair<FeatureType, float>(_floatFeature, value);

        return this;
    }

    public void Add(IObserver observer)
    {
        _observers = _observers.Add(observer);
    }

    public void Remove(IObserver observer)
    {
        _observers = _observers.Remove(observer);
    }

    public void Notify()
    {
        _observers.ForEach(observer => observer.PropertyChanged(this));
    }
}
