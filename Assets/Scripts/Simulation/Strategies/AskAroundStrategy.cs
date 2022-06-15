using System.Collections.Generic;

public sealed class AskAroundStrategy : LoadBalancingStrategy
{
    public override string StrategyName => "Ask Around";

    public override LoadBalancingStrategyType StrategyType => LoadBalancingStrategyType.AskAround;

    public override IEnumerator<LoadBalancingAction> Balance(Processor source, List<Processor> otherProcessors)
    {
        LoadQueries = 0;
        for (int i = 0; i < SimulationManager.Instance.simulationSettings.askAroundAttempts; i++)
        {
            Processor target = otherProcessors[SimulationManager.Instance.Random.Next(otherProcessors.Count)];

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
        if (source.Load < SimulationManager.Instance.simulationSettings.processorLoadThreshold / 100f)
        {
            yield return new LoadBalancingAction
            {
                Type = LoadBalancingActionType.ExecuteLocally,
                SourceProcessor = source,
                TargetProcessor = source,
                ProcessCount = 1,
            };
        }
        else
        {
            yield return new LoadBalancingAction
            {
                Type = LoadBalancingActionType.DoNothing
            };
        }
    }
}
