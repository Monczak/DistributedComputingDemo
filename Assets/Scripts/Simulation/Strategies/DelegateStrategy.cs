using System.Collections.Generic;

public sealed class DelegateStrategy : LoadBalancingStrategy
{
    public override string StrategyName => "Delegate";

    public override LoadBalancingStrategyType StrategyType => LoadBalancingStrategyType.Delegate;

    public override IEnumerator<LoadBalancingAction> Balance(Processor source, List<Processor> otherProcessors)
    {
        LoadQueries = 0;
        if (source.Load > SimulationManager.Instance.simulationSettings.processorLoadThreshold / 100f)
        {
            List<Processor> processors = Shuffle(otherProcessors);
            foreach (Processor target in processors)
            {
                LoadQueries++;
                if (target.Load < SimulationManager.Instance.simulationSettings.processorLoadThreshold / 100f)
                {
                    yield return new LoadBalancingAction
                    {
                        Type = LoadBalancingActionType.SendProcess,
                        SourceProcessor = source,
                        TargetProcessor = target,
                        ProcessCount = 1,
                    };
                    yield break;
                }
            }
            yield return new LoadBalancingAction
            {
                Type = LoadBalancingActionType.DoNothing
            };
        }
        else
        {
            yield return new LoadBalancingAction
            {
                Type = LoadBalancingActionType.ExecuteLocally,
                SourceProcessor = source,
                TargetProcessor = source,
                ProcessCount = 1,
            };
        }
    }

    private List<T> Shuffle<T>(List<T> values)
    {
        List<T> result = new List<T>(values);
        int n = result.Count;

        while (n > 1)
        {
            n--;
            int index = SimulationManager.Instance.Random.Next(n + 1);
            (result[index], result[n]) = (result[n], result[index]);
        }

        return result;
    }
}
