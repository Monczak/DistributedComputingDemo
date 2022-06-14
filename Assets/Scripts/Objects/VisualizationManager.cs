using UnityEngine;

public class VisualizationManager : MonoBehaviour
{
    public ProcessorManager processorManager;
    public LineManager lineManager;
    public ProcessBlockManager processBlockManager;
    public CameraMovement cameraMovement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {
        processorManager.processorCount = SimulationManager.Instance.simulationSettings.processorCount;
        processorManager.SpawnProcessors();

        foreach (ProcessorController processor in processorManager.processorControllers)
        {
            ProcessSpawner spawner = processor.GetComponent<ProcessSpawner>();
            spawner.Initialize(SimulationManager.Instance.Random.Next());
        }

        cameraMovement.SetZoomOutRadius(GetCameraRadiusToFit());
    }

    private float GetCameraRadiusToFit() => processorManager.radius * 7.34f + 17.5f;    // Magic
}
