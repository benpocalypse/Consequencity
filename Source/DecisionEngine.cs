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
                            new NodeTransitionCriteria(() => globals.Decisions.TransitionStopWatch.ElapsedMilliseconds >= 5_000 ),
                        entranceActions:
                            ImmutableList<Action>.Empty
                                .Add(() =>
                                    {
                                        globals.PopupDialog(
                                            _decisionText: "You did it. You made it to your own deserted island. Why don't you take some time to look around with the W, A, S, and D keys. You can also use the mouse wheel to zoom in and out.",
                                            _decisions: new List<string>() { "Ok"});
                                        globals.Decisions.TransitionStopWatch.Restart();
                                    }
                                )
                        )
                    .AddChild(
                        new BehaviorNode(
                            entranceCriteria: new NodeTransitionCriteria(() => globals.Decisions.TransitionStopWatch.ElapsedMilliseconds >= 10_000 ),
                            entranceActions: ImmutableList<Action>.Empty
                                .Add(() =>
                                    {
                                        globals.PopupDialog(
                                                _decisionText: "Now that you've had a look around the island, why not find a spot to build your house?",
                                                _decisions: new List<string>() { "Ok"});
                                            globals.Decisions.TransitionStopWatch.Stop();
                                    }
                                ),
                            exitActions: ImmutableList<Action>.Empty
                                .Add(() =>
                                    {
                                        var enableSpecialPlacement = globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.PlayerCanPlaceSpecial);
                                        globals.Features = globals.Features.Remove(enableSpecialPlacement);
                                        globals.Features = globals.Features.Add(enableSpecialPlacement.WithValue(true));

                                        var enablePlayerHousePlacement = globals.Features.First(_ => _.BooleanFeature.Key == GameFeature.FeatureType.PlayerHouseNotPlaced);
                                        globals.Features = globals.Features.Remove(enablePlayerHousePlacement);
                                        globals.Features = globals.Features.Add(enablePlayerHousePlacement.WithValue(true));

                                    }
                                )
                        )
                        .AddChild(
                            new BehaviorNode(
                                entranceCriteria:
                                // FIXME - Need a way to detect when the player has placed their house.
                                    new NodeTransitionCriteria(() => true),
                                entranceActions: ImmutableList<Action>.Empty
                                    .Add(() =>
                                        {
                                            globals.PopupDialog(
                                                    _decisionText: "Why didn't this work?",
                                                    _decisions: new List<string>() { "Ok"});
                                                TransitionStopWatch.Stop();
                                        }
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
                                () => globals.PopupDialog(
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

    public void DecisionMade(string choiceText)
    {
        this._choiceText = choiceText;
    }
}