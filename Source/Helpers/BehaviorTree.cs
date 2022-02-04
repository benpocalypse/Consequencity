using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public sealed class BehaviorTree
{
    public BehaviorNode RootNode = new BehaviorNode(ImmutableList<Action>.Empty);

    public void Update() => RootNode.Children.ForEach(_ => _.Update());
}

public sealed class BehaviorNode
{
    public ImmutableList<Action> EntranceActionList = ImmutableList<Action>.Empty;
    public ImmutableList<Action> ExitActionList = ImmutableList<Action>.Empty;
    public ImmutableList<BehaviorNode> Children = ImmutableList<BehaviorNode>.Empty;
    public NodeTransitionCriteria EntranceCriteria = new NodeTransitionCriteria(null);

    private bool _entranceActionsPerformed = false;
    private bool _exitActionsPerformed = false;
    public bool EntranceCriteriaMet = false;

    public BehaviorNode AddChild(BehaviorNode child)
    {
        Children = Children.Add(child);

        return this;
    }

    public void AddChildren(ImmutableList<BehaviorNode> children)
    {
        Children = Children.AddRange(children);
    }

    public BehaviorNode(NodeTransitionCriteria entranceCriteria)
    {
        EntranceCriteria = entranceCriteria;
    }

    public BehaviorNode(ImmutableList<Action> entranceActions)
    {
        EntranceActionList = entranceActions;
    }

    public BehaviorNode(NodeTransitionCriteria entranceCriteria, ImmutableList<Action> entranceActions)
    {
        EntranceCriteria = entranceCriteria;
        EntranceActionList = entranceActions;
    }

    public BehaviorNode(NodeTransitionCriteria entranceCriteria, ImmutableList<Action> entranceActions, ImmutableList<Action> exitActions)
    {
        EntranceCriteria = entranceCriteria;
        EntranceActionList = entranceActions;
        ExitActionList = exitActions;
    }

    public void Update()
    {
        // If we perform this branch, we've alread evaluated this node.
        if (_entranceActionsPerformed == true)
        {
            var areAnyChildEntranceCriteriaMet = false;

            foreach (var child in Children)
            {
                child.Update();

                // If one of our children has met it's entrance criteria, remove all the other children
                // so that they don't respond to input as well.
                if (child.EntranceCriteriaMet == true)
                {
                    areAnyChildEntranceCriteriaMet = true;
                }
            }

            if (areAnyChildEntranceCriteriaMet == true)
            {
                Children = Children.RemoveAll(_ => _.EntranceCriteriaMet == false);

                if (_exitActionsPerformed == false && ExitActionList.Count() > 0)
                {
                    ExitActionList.ForEach(exitAction => exitAction());
                    _exitActionsPerformed = true;
                }
            }
        }

        EntranceCriteriaMet = EntranceCriteria.Evaluate();

        // If we've met our entrance criteria, but haven't performed our actions, do so now.
        if (EntranceCriteriaMet == true && _entranceActionsPerformed == false)
        {
            _entranceActionsPerformed = true;

            EntranceActionList.ForEach(entranceAction => entranceAction());
        }
    }
}

public sealed class NodeTransitionCriteria
{
    private Func<bool> _condition = null;
    public NodeTransitionCriteria(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool Evaluate() => _condition != null ? _condition() : false;
}
