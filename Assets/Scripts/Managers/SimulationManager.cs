using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance { get; private set; }

    [Header("Simulation Data")]
    public int simulationSeed;

    [Header("Simulation Elements")]
    public VisualizationManager visualizationManager;

    [Header("Simulation Timing")]
    public float simulationDuration;
    public float currentTime;
    [HideInInspector] public float previousTime;
    public float speed;

    [Header("Comfort Features")]
    public CameraMovement cameraMovement;

    public SimulationSettings simulationSettings;

    private bool playing = false;
    private float playingSpeed;

    public System.Random Random { get; private set; }

    public delegate void TickHandler(int time);
    public event TickHandler Tick, LateTick, LateLateTick;
    public event TickHandler Reset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        previousTime = currentTime;

        cameraMovement = Camera.main.GetComponent<CameraMovement>();

        Reset += ResetSimulation;

        LoadDefaultSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupNewSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        TickEveryUnit();

        previousTime = currentTime;
    }

    public void SetupNewSimulation()
    {
        Random = new System.Random(simulationSeed);
        simulationDuration = 60 * simulationSettings.simulationSpeed;
        visualizationManager.Setup();
    }

    public void ResetSimulation(int _)
    {
        Random = new System.Random(simulationSeed);
    }

    private void LoadDefaultSettings()
    {
        simulationSettings = new SimulationSettings
        {
            loadBalancingStrategyType = LoadBalancingStrategyType.AskAround,
            simulationSpeed = 5,
            processorCount = 20,
            processorLoadThreshold = 60f,
            minProcessLoad = 1f,
            maxProcessLoad = 30f,
            minProcessSpawnInterval = 1,
            maxProcessSpawnInterval = 10,
            askAroundAttempts = 10,
            alleviateMinLoad = 50f,
            alleviateProcessPercentage = 50f,
            simulationSeed = 1337
        };
        simulationSeed = simulationSettings.simulationSeed;
    }

    public void SetSimulationSettings(SimulationSettings settings)
    {
        simulationSettings = settings;
        simulationSeed = settings.simulationSeed;
    }

    private void TickEveryUnit()
    {
        int lower = Mathf.FloorToInt(previousTime);
        if (Mathf.Approximately(previousTime, 0) && (IsPlaying() || currentTime == simulationDuration))
            lower--;

        for (int i = lower; i < (int)currentTime; i++)
        {
            Tick?.Invoke(i + 1);
            LateTick?.Invoke(i + 1);
            LateLateTick?.Invoke(i + 1);
        }

    }

    public bool IsPlaying()
    {
        return playing;
    }

    public float GetPlayingSpeed()
    {
        return playingSpeed;
    }

    public void PlayForward()
    {
        if (!playing || playingSpeed < 0)
        {
            playing = true;
            playingSpeed = speed;
        }
        else
        {
            Pause();
        }
    }

    public void PlayBackward()
    {
        if (!playing || playingSpeed > 0)
        {
            playing = true;
            playingSpeed = -speed;
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        playing = false;
        playingSpeed = 0;
    }

    public void Rewind()
    {
        playingSpeed = 0;
        currentTime = 0;
        playing = false;

        Reset?.Invoke(0);
    }

    public void Forward()
    {
        playingSpeed = 0;
        currentTime = simulationDuration;
        playing = false;
    }

    private void UpdateTime()
    {
        currentTime += playingSpeed * simulationSettings.simulationSpeed * Time.deltaTime;
        ClampTime();
    }

    private void ClampTime()
    {
        if (currentTime < 0)
        {
            Pause();
            currentTime = 0;
        }
        if (currentTime > simulationDuration)
        {
            Pause();
            currentTime = simulationDuration;
        }
    }
}
