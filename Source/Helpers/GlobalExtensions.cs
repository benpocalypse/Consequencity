using System;
using System.Collections.Immutable;
using System.Linq;

public static class Extensions
{
	public static ImmutableList<GameFeature> SetGameFeatureValue(this ImmutableList<GameFeature> features, GameFeature.FeatureType featureTypeToSet, bool boolValueToSet)
	{
		Globals globals = Globals.Instance;
        var globalFeature = features.First(_ => _.BooleanFeature.Key == featureTypeToSet);
        features = features.Remove(globalFeature);
        features = features.Add(globalFeature.WithValue(boolValueToSet));

        return features;
	}

    public static bool GetGameFeatureValue(this ImmutableList<GameFeature> features, GameFeature.FeatureType featureTypeToGet) =>
        features.First(_ => _.BooleanFeature.Key == featureTypeToGet).BooleanFeature.Value;
}