using System.Collections.Generic;
using UnityEngine;

public class ProcessorManager : MonoBehaviour
{
    public GameObject processorPrefab;

    public int processorCount;
    public List<ProcessorController> processorControllers;
    public List<Processor> processors;

    public float radius;
    public float margin;

    public LineManager lineManager;
    public ProcessBlockManager processBlockManager;

    public float MeanLoad { get; private set; }
    public float LoadDeviation { get; private set; }
    public int LoadQueries { get; private set; }
    public int Migrations { get; private set; }

    private void Awake()
    {
        SimulationManager.Instance.LateLateTick += CalculateStats;
        SimulationManager.Instance.Reset += ResetStats;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnProcessors()
    {
        foreach (ProcessorController controller in processorControllers)
            Destroy(controller.gameObject);
        processorControllers = new List<ProcessorController>();
        processors = new List<Processor>();

        for (int i = 0; i < processorCount; i++)
        {
            ProcessorController controller = Instantiate(processorPrefab, transform).GetComponent<ProcessorController>();
            processorControllers.Add(controller);
            processors.Add(controller.GetComponent<Processor>());

            controller.Setup(i, (float)i / processorCount);
        }
        LayoutProcessors();

        foreach (Processor processor in processors)
        {
            LoadBalancingStrategy strategy = SimulationManager.Instance.simulationSettings.loadBalancingStrategyType switch
            {
                LoadBalancingStrategyType.AskAround => new AskAroundStrategy(),
                LoadBalancingStrategyType.Delegate => new DelegateStrategy(),
                LoadBalancingStrategyType.Alleviate => new AlleviateStrategy(),
                _ => throw new System.NotImplementedException(),
            };
            processor.Setup(this, processors, strategy);
        }
    }

    private void LayoutProcessors()
    {
        radius = processorCount + margin;
        for (int i = 0; i < processorCount; i++)
        {
            float angle = Mathf.PI * 2 * ((float)i / processorCount) + Mathf.PI / 2;
            processorControllers[i].transform.position = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    public void SendProcessBlocks(ProcessorController sourceProcessor, ProcessorController targetProcessor, int count)
    {
        lineManager.SpawnLine(sourceProcessor, targetProcessor, processBlockManager.blockMoveTime * 2 + count * processBlockManager.blockSendInterval);
        processBlockManager.SendProcessBlocks(sourceProcessor, targetProcessor, count);
    }

    private void CalculateStats(int _)
    {
        MeanLoad = 0;
        foreach (Processor processor in processors)
            MeanLoad += processor.Load;
        MeanLoad /= processors.Count;

        float devSum = 0;
        foreach (Processor processor in processors)
            devSum += (processor.Load - MeanLoad) * (processor.Load - MeanLoad);
        LoadDeviation = Mathf.Sqrt(devSum / processorCount);

        LoadQueries = 0;
        Migrations = 0;
        foreach (Processor processor in processors)
        {
            LoadQueries += processor.LoadQueries;
            Migrations += processor.Migrations;
        }
    }

    private void ResetStats(int _)
    {
        MeanLoad = 0;
        LoadDeviation = 0;
        LoadQueries = 0;
        Migrations = 0;
    }
}
