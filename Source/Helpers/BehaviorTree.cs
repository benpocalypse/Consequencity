using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public sealed class BehaviorTree
{
    public BehaviorNode RootNode = new BehaviorNode(ImmutableList<Action>.Empty);

    public void Update()
    {
        ImmutableList<BehaviorNode> nodesToKeep = ImmutableList<BehaviorNode>.Empty;

        RootNode.Children.ForEach(_ => _.Update());
    }
}

public sealed class BehaviorNode
{
    public ImmutableList<Action> ActionList = ImmutableList<Action>.Empty;
    public ImmutableList<BehaviorNode> Children = ImmutableList<BehaviorNode>.Empty;
    public NodeTransitionCriteria EntranceCriteria = new NodeTransitionCriteria(null);

    private bool _actionsPerformed = false;
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

    public BehaviorNode(ImmutableList<Action> actions)
    {
        ActionList = actions;
    }

    public BehaviorNode(NodeTransitionCriteria entranceCriteria, ImmutableList<Action> actions)
    {
        EntranceCriteria = entranceCriteria;
        ActionList = actions;
    }

    public void Update()
    {
        // If we perform this branch, we've alread evaluated this node.
        if (_actionsPerformed == true)
        {
            var childCriteriaMet = false;

            foreach (var child in Children)
            {
                child.Update();

                // If one of our children has met it's entrance criteria, remove all the other children
                // so that they don't respond to input as well.
                if (child.EntranceCriteriaMet == true)
                {
                    childCriteriaMet = true;
                }
            }

            if (childCriteriaMet == true)
            {
                Children = Children.RemoveAll(_ => _.EntranceCriteriaMet == false);
            }
        }

        EntranceCriteriaMet = EntranceCriteria.Evaluate();

        if (EntranceCriteriaMet == true && _actionsPerformed == false)
        {
            _actionsPerformed = true;

            foreach (var nodeAction in ActionList)
            {
                nodeAction();
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
