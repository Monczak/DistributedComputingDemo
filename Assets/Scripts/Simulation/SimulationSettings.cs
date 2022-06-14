public struct SimulationSettings
{
    public LoadBalancingStrategyType loadBalancingStrategyType;
    public int processorCount;
    public int minProcessSpawnInterval, maxProcessSpawnInterval;
    public float minProcessLoad, maxProcessLoad;
    public float processorLoadThreshold;

    // Strategy 1: Ask Around
    public int askAroundAttempts;

    // Strategy 3: Alleviate
    public float alleviateMinLoad;
    public float alleviateProcessPercentage;

    public float simulationSpeed;
    public int simulationSeed;
}
