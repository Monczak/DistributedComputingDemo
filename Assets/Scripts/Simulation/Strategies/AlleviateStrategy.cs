using System.Collections.Generic;

public sealed class AlleviateStrategy : LoadBalancingStrategy
{
    public override string StrategyName => "Alleviate";

    public override LoadBalancingStrategyType StrategyType => LoadBalancingStrategyType.Alleviate;

    public override IEnumerator<LoadBalancingAction> Balance(Processor source, List<Processor> otherProcessors)
    {
        if (source.Load < SimulationManager.Instance.simulationSettings.processorLoadThreshold / 100f)
        {
            yield return new LoadBalancingAction
            {
                Type = LoadBalancingActionType.ExecuteLocally,
                SourceProcessor = source,
                TargetProcessor = source,
                ProcessCount = 1
            };
        }

        if (source.Load < SimulationManager.Instance.simulationSettings.alleviateMinLoad / 100f)
        {
            Processor target = otherProcessors[SimulationManager.Instance.Random.Next(otherProcessors.Count)];

            LoadQueries++;
            if (target.Load > SimulationManager.Instance.simulationSettings.processorLoadThreshold / 100f)
            {
                int count = (int)(target.ProcessCount * SimulationManager.Instance.simulationSettings.alleviateProcessPercentage / 100f);
                Migrations += count;
                yield return new LoadBalancingAction
                {
                    Type = LoadBalancingActionType.ReceiveProcess,
                    SourceProcessor = source,
                    TargetProcessor = target,
                    ProcessCount = count
                };
                yield break;
            }
        }
    }
}
