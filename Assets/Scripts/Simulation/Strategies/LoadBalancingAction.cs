public struct LoadBalancingAction
{
    public LoadBalancingActionType Type { get; set; }
    public int ProcessCount { get; set; }
    public Processor SourceProcessor { get; set; }
    public Processor TargetProcessor { get; set; }

    public LoadBalancingAction(LoadBalancingActionType type, int processCount, Processor sourceProcessor, Processor targetProcessor)
    {
        Type = type;
        ProcessCount = processCount;
        SourceProcessor = sourceProcessor;
        TargetProcessor = targetProcessor;
    }
}