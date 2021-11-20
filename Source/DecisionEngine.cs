using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

public sealed class DecisionEngine
{
    private Globals globals;
    private BehaviorTree _behaviorTree;
    private string _decisionText = string.Empty;

    public DecisionEngine()
    {
        globals = Globals.Instance;

        _behaviorTree = new BehaviorTree();

        _behaviorTree.RootNode
            .AddChild(
                new BehaviorNode(
                    entranceCriteria:
                        new NodeTransitionCriteria(() => { return globals.Economy.Population == 1; }),
                    actions:
                        ImmutableList<Action>.Empty
                            .Add(
                                () => globals.PopupDialog(
                                        _decisionText: "Now that you live here, other people have noticed and would like to join you. Will you let other people live on your island?",
                                        _decisions: new List<string>() { "Yes", "No"})
                            )
                            .Add(
                                () =>
                                {
                                    var enableResidentialZoning = globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.ResidentialZoning);
                                    globals.Features = globals.Features.Remove(enableResidentialZoning);
                                    globals.Features = globals.Features.Add(enableResidentialZoning.WithValue(true));
                                }
                            )
                    )
                    .AddChild(
                        new BehaviorNode(
                            entranceCriteria:
                                new NodeTransitionCriteria(() => { return _decisionText == "Yes" && globals.Economy.Population == 3; }),
                            actions:
                                ImmutableList<Action>.Empty
                                    .Add(
                                        () => globals.PopupDialog(
                                                _decisionText: "More people have moved in, and they want the ability to buy things. Should we mint a currency for them?",
                                                _decisions: new List<string>() { "Yes", "No"})
                                            // FIXME - Continue the tree.
                                    )
                        )
                    )
                    .AddChild(
                        new BehaviorNode(
                            entranceCriteria:
                                new NodeTransitionCriteria(() =>
                                {
                                    return _decisionText == "No";
                                }),
                            actions:
                                ImmutableList<Action>.Empty
                                    .Add(
                                        () =>
                                        {
                                            var disablePopulationGrowth = globals.Features.First(_ => _.FloatFeature.Key == GameFeature.FeatureType.PopulationGrowth);
                                            globals.Features = globals.Features.Remove(disablePopulationGrowth);
                                            globals.Features = globals.Features.Add(disablePopulationGrowth.WithValue(0.0f));
                                        }
                                    )
                        )
                    )
            );
    }

    public void Update()
    {
        _behaviorTree.Update();
    }

    public void DecisionMade(string decisionText)
    {
        this._decisionText = decisionText;
    }
}