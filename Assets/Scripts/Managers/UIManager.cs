using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Activity Markers")]
    public ActivityMarker playMarker;

    [Header("Options Menu")]
    public OptionsMenu optionsMenu;
    public bool showOptionsMenu;

    [Header("HUD")]
    public ProcessorManager processorManager;
    public TMP_Text meanLoadText, loadDeviationText, loadQueriesText, migrationsText;

    private Controls controls;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(gameObject);

        controls = new Controls();
        controls.Enable();
    }

    private void Update()
    {
        UpdateActivityMarkers();
        UpdateHUD();

        SimulationManager.Instance.cameraMovement.panDown = showOptionsMenu;
        SimulationManager.Instance.cameraMovement.fixInPlace = showOptionsMenu;
    }

    private void UpdateActivityMarkers()
    {
        playMarker.active = SimulationManager.Instance.IsPlaying() && SimulationManager.Instance.GetPlayingSpeed() > 0;
    }

    private void UpdateHUD()
    {
        meanLoadText.text = $"Mean Load: {processorManager.MeanLoad * 100f:0.00}%";
        loadDeviationText.text = $"Load Deviation: {processorManager.LoadDeviation * 100f:0.00}";
        loadQueriesText.text = $"Load Queries: {processorManager.LoadQueries}";
        migrationsText.text = $"Migrations: {processorManager.Migrations}";
    }

    public void OnMenuButtonPressed()
    {
        showOptionsMenu = true;
        optionsMenu.Show();
    }

    public void OnPlayButtonPressed()
    {
        if (SimulationManager.Instance.IsPlaying())
            SimulationManager.Instance.Pause();
        else
            SimulationManager.Instance.PlayForward();

    }

    public void OnRewindButtonPressed()
    {
        SimulationManager.Instance.Rewind();
    }

    public void OnForwardButtonPressed()
    {
        SimulationManager.Instance.Forward();
    }
}
