using System.Collections.Generic;
using System.Collections.Immutable;

public sealed class GameFeature : IObservable
{
    public enum FeatureType
    {
        ResidentialZoning = 0,
        CommercialZoning = 1,
        IndustrialZoning = 2,
        AgriculturalZoning = 3,
        TransportationZoning = 4,
        DeleteZoning = 5,
        PopulationGrowthRate = 6,
        PlayerHousePlaced = 7
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

    public GameFeature(FeatureType feature, bool value)
    {
        _booleanFeature = new KeyValuePair<FeatureType, bool>(feature, value);
    }

    public GameFeature(FeatureType feature, float value)
    {
        _floatFeature = new KeyValuePair<FeatureType, float>(feature, value);
    }

    public GameFeature WithValue(bool value)
    {
        BooleanFeature = new KeyValuePair<FeatureType, bool>(_booleanFeature.Key, value);

        return this;
    }

    public GameFeature WithValue(float value)
    {
        FloatFeature = new KeyValuePair<FeatureType, float>(_floatFeature.Key, value);

        return this;
    }

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
        _observers.ForEach(observer => observer.PropertyChanged(this));
    }
}
