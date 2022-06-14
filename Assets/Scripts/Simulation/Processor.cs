using System.Collections.Generic;
using UnityEngine;

public class Processor : MonoBehaviour
{
    private ProcessorController controller;
    private ProcessSpawner spawner;
    private ProcessorManager manager;

    private float load;
    private List<Process> processes;
    private List<Processor> otherProcessors;

    public float Load => load;
    public int ProcessCount => processes.Count;

    public int LoadQueries => loadBalancingStrategy.LoadQueries;
    public int Migrations => loadBalancingStrategy.Migrations;

    private Queue<Process> waitingProcesses;

    private LoadBalancingStrategy loadBalancingStrategy;

    private void Awake()
    {
        controller = GetComponent<ProcessorController>();
        spawner = GetComponent<ProcessSpawner>();
        processes = new List<Process>();
        waitingProcesses = new Queue<Process>();

        spawner.OnNewProcess += OnNewProcess;
        SimulationManager.Instance.LateTick += OnTick;
        SimulationManager.Instance.Reset += ResetProcessor;

        UpdateLoad();
    }

    private void OnDestroy()
    {
        spawner.OnNewProcess -= OnNewProcess;
        SimulationManager.Instance.LateTick -= OnTick;
        SimulationManager.Instance.Reset -= ResetProcessor;
    }

    private void ResetProcessor(int _ = 0)
    {
        load = 0;
        processes = new List<Process>();
        waitingProcesses = new Queue<Process>();
        loadBalancingStrategy.Initialize();

        UpdateLoad();
    }

    public void Setup(ProcessorManager manager, IEnumerable<Processor> processors, LoadBalancingStrategy strategy)
    {
        this.manager = manager;

        otherProcessors = new List<Processor>(processors);
        otherProcessors.Remove(this);

        loadBalancingStrategy = strategy;
        loadBalancingStrategy.Initialize();
    }

    private void OnTick(int time)
    {
        Log("Tick");

        IEnumerator<LoadBalancingAction> actionGenerator = loadBalancingStrategy?.Balance(this, otherProcessors);
        if (actionGenerator != null)
        {
            while (actionGenerator.MoveNext())
            {
                LoadBalancingAction action = actionGenerator.Current;
                switch (action.Type)
                {
                    case LoadBalancingActionType.ExecuteLocally:
                        if (waitingProcesses.Count > 0)
                        {
                            Log("Execute Locally");
                            ExecuteProcess(waitingProcesses.Dequeue());
                        }
                        break;
                    case LoadBalancingActionType.SendProcess:
                        if (waitingProcesses.Count > 0)
                        {
                            Log("Send Process");
                            action.TargetProcessor.ExecuteProcess(waitingProcesses.Dequeue());
                            manager.SendProcessBlocks(controller, action.TargetProcessor.controller, 1);
                        }
                        break;
                    case LoadBalancingActionType.ReceiveProcess:
                        Log("Receive Process");
                        manager.SendProcessBlocks(action.TargetProcessor.controller, controller, action.ProcessCount);
                        for (int i = 0; i < action.ProcessCount; i++)
                        {
                            ExecuteProcess(action.TargetProcessor.PullRandomProcess());
                        }
                        break;
                    case LoadBalancingActionType.DoNothing:
                        Log("Do Nothing");
                        break;

                    default:
                        break;
                }
            }
        }

    }

    private void OnNewProcess(Process process)
    {
        Log($"New process with load {process.cpuLoad}");
        waitingProcesses.Enqueue(process);

        UpdateLoad();
    }

    private void UpdateLoad()
    {
        load = 0;
        foreach (Process process in processes)
            load += process.cpuLoad;

        controller.load = load;
    }

    private void Log(string message)
    {
        Debug.Log($"[{gameObject.name}] {message}");
    }

    public void ExecuteProcess(Process process)
    {
        processes.Add(process);
        UpdateLoad();
    }

    public Process PullRandomProcess()
    {
        Process process = processes[SimulationManager.Instance.Random.Next(processes.Count)];
        processes.Remove(process);
        UpdateLoad();
        return process;
    }
}
