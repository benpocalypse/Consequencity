using System;
using System.Collections.Immutable;

public sealed class BehaviorTree
{
    public BehaviorNode RootNode = new BehaviorNode(ImmutableList<Action>.Empty);

    public void Update()
    {
        foreach (var child in RootNode.Children)
        {
            child.Update();
        }
    }
}

public sealed class BehaviorNode
{
    public ImmutableList<Action> ActionList = ImmutableList<Action>.Empty;
    public ImmutableList<BehaviorNode> Children = ImmutableList<BehaviorNode>.Empty;
    public NodeTransitionCriteria EntranceCriteria = new NodeTransitionCriteria(null);

    private bool _actionsPerformed = false;
    public bool ShouldEvaluate = true;

    public BehaviorNode AddChild(BehaviorNode _child)
    {
        Children = Children.Add(_child);

        return this;
    }

    public void AddChildren(ImmutableList<BehaviorNode> _children)
    {
        Children = Children.AddRange(_children);
    }

    public BehaviorNode(NodeTransitionCriteria _entranceCriteria)
    {
        EntranceCriteria = _entranceCriteria;
    }

    public BehaviorNode(ImmutableList<Action> _actions)
    {
        ActionList = _actions;
    }

    public BehaviorNode(ImmutableList<Action> _actions, NodeTransitionCriteria _entranceCriteria)
    {
        ActionList = _actions;
        EntranceCriteria = _entranceCriteria;
    }

    public void Update()
    {
        if (ShouldEvaluate == true)
        {
            if (EntranceCriteria.Evaluate() == true && _actionsPerformed == false)
            {
                _actionsPerformed = true;

                foreach (var nodeAction in ActionList)
                {
                    nodeAction();
                }
            }

            foreach (var child in Children)
            {
                child.Update();
            }
        }
    }
}

public sealed class NodeTransitionCriteria
{
    private Func<bool> _condition;
    public NodeTransitionCriteria(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool Evaluate() => _condition != null ? _condition() : false;
}
