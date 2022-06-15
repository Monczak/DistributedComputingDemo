using System.Collections.Generic;

public abstract class LoadBalancingStrategy
{
    public abstract string StrategyName { get; }
    public abstract LoadBalancingStrategyType StrategyType { get; }

    public int LoadQueries { get; protected set; }

    public void Initialize()
    {
        LoadQueries = 0;
    }

    public abstract IEnumerator<LoadBalancingAction> Balance(Processor source, List<Processor> otherProcessors);
}
