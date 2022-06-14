using UnityEngine;

using Random = System.Random;

public class ProcessSpawner : MonoBehaviour
{
    private Random random;
    private int seed;

    private int interval;
    private float lastSpawnTime;

    public delegate void NewProcessHandler(Process process);
    public event NewProcessHandler OnNewProcess;

    private void Awake()
    {
        SimulationManager.Instance.Tick += OnSimulationTick;
        SimulationManager.Instance.Reset += OnSimulationReset;
    }

    private void OnDestroy()
    {
        SimulationManager.Instance.Tick -= OnSimulationTick;
        SimulationManager.Instance.Reset -= OnSimulationReset;
    }

    public void Initialize(int seed)
    {
        this.seed = seed;
        InitializeRandom();
    }

    private void InitializeRandom()
    {
        random = new Random(seed);

        lastSpawnTime = 0;
        GetNewInterval();
    }

    private void GetNewInterval()
    {
        interval = random.Next(SimulationManager.Instance.simulationSettings.minProcessSpawnInterval, SimulationManager.Instance.simulationSettings.maxProcessSpawnInterval);
    }

    private void OnSimulationReset(int time)
    {
        InitializeRandom();
    }

    private void OnSimulationTick(int time)
    {
        if (time >= lastSpawnTime + interval)
        {
            lastSpawnTime = time;
            GetNewInterval();

            Process process = new Process
            {
                cpuLoad = Mathf.Lerp(SimulationManager.Instance.simulationSettings.minProcessLoad / 100f, SimulationManager.Instance.simulationSettings.maxProcessLoad / 100f, (float)random.NextDouble())
            };

            OnNewProcess?.Invoke(process);
        }
    }
}
