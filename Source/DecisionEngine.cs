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
    private string _choiceText = string.Empty;
    public Stopwatch TransitionStopWatch = new Stopwatch();

    public DecisionEngine()
    {
        globals = Globals.Instance;

        _behaviorTree = new BehaviorTree();

        _behaviorTree.RootNode
            .AddChild(
                new BehaviorNode(
                    entranceCriteria: new NodeTransitionCriteria(() => true),
                    entranceActions: ImmutableList<Action>.Empty
                        .Add(
                                () => globals.Decisions.TransitionStopWatch.Start()
                            )
                )
                .AddChild(
                    new BehaviorNode(
                        entranceCriteria:
                            new NodeTransitionCriteria(() => globals.Decisions.TransitionStopWatch.ElapsedMilliseconds >= 1_000),
                        entranceActions:
                            ImmutableList<Action>.Empty
                                .Add(() =>
                                    {
                                        globals.Popup(
                                            _decisionText: "You did it. You made it to your own deserted island. Why don't you take some time to look around with the W, A, S, and D keys. You can also use the mouse wheel to zoom in and out.",
                                            _decisions: new List<string>() { "Ok"});
                                        globals.Decisions.TransitionStopWatch.Restart();
                                    }
                                )
                        )
                    .AddChild(
                        new BehaviorNode(
                            entranceCriteria:
                                new NodeTransitionCriteria(() =>
                                    globals.Features.GetGameFeatureValue(GameFeature.FeatureType.DialogAcknowledged) == true &&
                                    globals.Decisions.TransitionStopWatch.ElapsedMilliseconds >= 5_000),
                            entranceActions: ImmutableList<Action>.Empty
                                .Add(() =>
                                    {
                                        globals.Popup(
                                                _decisionText: "Now that you've had a look around the island, why not find a spot to build your house?",
                                                _decisions: new List<string>() { "Ok"});
                                        globals.Decisions.TransitionStopWatch.Stop();

                                        globals.Features = globals.Features.SetGameFeatureValue(GameFeature.FeatureType.PlayerCanPlaceSpecial, true);
                                        globals.Features = globals.Features.SetGameFeatureValue(GameFeature.FeatureType.PlayerHouseNotPlaced, true);
                                    }
                                )
                            )
                            .AddChild(
                                new BehaviorNode(
                                    entranceCriteria: new NodeTransitionCriteria(() => globals.Features.GetGameFeatureValue(GameFeature.FeatureType.PlayerHouseNotPlaced) == false),
                                    entranceActions: ImmutableList<Action>.Empty
                                        .Add(() =>  globals.Decisions.TransitionStopWatch.Restart())
                                )
                                .AddChild(
                                    new BehaviorNode(
                                        entranceCriteria:
                                            new NodeTransitionCriteria(() => globals.Decisions.TransitionStopWatch.ElapsedMilliseconds >= 5_000),
                                        entranceActions: ImmutableList<Action>.Empty
                                            .Add(() =>
                                                {
                                                    globals.Popup(
                                                        _decisionText: "You're probably getting tired of scavenging for food. Why not try building a few spaces for gardens?",
                                                        _decisions: new List<string>() { "Ok"});
                                                    globals.Features = globals.Features.SetGameFeatureValue(GameFeature.FeatureType.AgriculturalZoning, true);
                                                }
                                            )
                                    )
                                    .AddChild(
                                        new BehaviorNode(
                                            entranceCriteria:
                                                new NodeTransitionCriteria(() => globals.Economy.Map.Where(_ => _.Value.Type == Globals.LandSpaceType.Agricultural).Count() >= 3),
                                            entranceActions: ImmutableList<Action>.Empty
                                                .Add(() =>
                                                    {
                                                        globals.Popup(
                                                                _decisionText: "Nice work!",
                                                                _decisions: new List<string>() { "Ok"});
                                                            TransitionStopWatch.Stop();
                                                    }
                                                )
                                        )
                                    )
                                )
                            )
                    )
                )
            );
/*
            .AddChild(
                new BehaviorNode(
                    entranceCriteria:
                        new NodeTransitionCriteria(() =>
                            {
                                return globals.Economy.Map.Where(space => space.Value.Type == Globals.LandSpaceType.Agricultural).Count() >= 3;
                            }),
                    entranceActions:
                        ImmutableList<Action>.Empty
                            .Add(
                                () => globals.Popup(
                                        _decisionText: "More people have moved in, and they want the ability to buy things. Should we mint a currency for them?",
                                        _decisions: new List<string>() { "Yes", "No"})
                                    // FIXME - Continue the tree.
                            )
                )
            )*/
    }

    public void Update()
    {
        _behaviorTree.Update();
    }

    public void DecisionMadeEventHandler(string choiceText)
    {
        this._choiceText = choiceText;
    }
}