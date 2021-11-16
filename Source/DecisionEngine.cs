using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

public sealed class DecisionEngine
{
    private Globals globals;
    private BehaviorTree _behaviorTree;
    private string _previousDecision = string.Empty;

    public DecisionEngine()
    {
        globals = Globals.Instance;

        Func<bool> isPopulationOne = () => { return globals.Economy.Population == 1; };
        Func<bool> isPopulationThree = () => { return _previousDecision == "Yes" && globals.Economy.Population == 3; };

        _behaviorTree = new BehaviorTree();

        _behaviorTree.RootNode
            .AddChild(
                new BehaviorNode(
                    _actions: ImmutableList<Action>.Empty
                        .Add(
                            () => globals.PopupDialog(
                                _decisionText: "Now that you live here, other people have noticed and would like to join you. Will you let other people live on the island?",
                                _decisions: new List<string>() { "Yes", "No"})
                        ),
                    _entranceCriteria: new NodeTransitionCriteria(isPopulationOne)
                    )
                    .AddChild(
                        new BehaviorNode(
                            _actions: ImmutableList<Action>.Empty
                                .Add(
                                    () => globals.PopupDialog(
                                        _decisionText: "More people have moved in, and they want the ability to buy things. Should we mint a currency for them?",
                                        _decisions: new List<string>() { "Yes", "No"})
                                ),
                            _entranceCriteria: new NodeTransitionCriteria(isPopulationThree)
                        )
                    )
            );
    }
    public void Update()
    {
        _behaviorTree.Update();
    }

    public void DecisionMade(string _decisionText)
    {
        _previousDecision = _decisionText;
    }
}