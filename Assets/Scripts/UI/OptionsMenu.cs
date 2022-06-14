using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public MultiSelector loadBalancingStrategyInput;
    public TMP_InputField processorCountInput;
    public TMP_InputField minProcessSpawnIntervalInput, maxProcessSpawnIntervalInput;
    public TMP_InputField minProcessLoadInput, maxProcessLoadInput;
    public TMP_InputField loadThresholdInput;
    public TMP_InputField askAroundAttemptsInput;
    public TMP_InputField alleviateMinLoadInput;
    public TMP_InputField simulationSpeedInput;
    public TMP_InputField simulationSeedInput;

    public TogglableButtonText applyButton;

    public Color normalInputColor, invalidInputColor;
    public float colorChangeRate;

    private Dictionary<TMP_InputField, bool> inputValidity;

    private SimulationSettings currentSettings;

    public float minSimulationSpeed, maxSimulationSpeed;

    private bool valid;

    private void Awake()
    {
        inputValidity = new Dictionary<TMP_InputField, bool>
        {
            { processorCountInput, true },
            { minProcessSpawnIntervalInput, true },
            { maxProcessSpawnIntervalInput, true },
            { minProcessLoadInput, true },
            { maxProcessLoadInput, true },
            { loadThresholdInput, true },
            { askAroundAttemptsInput, true },
            { alleviateMinLoadInput, true },
            { simulationSpeedInput, true },
            { simulationSeedInput, true },
        };

        loadBalancingStrategyInput.OnChanged += OnLoadBalancingStrategyChanged;
        processorCountInput.onValueChanged.AddListener(s => OnProcessorCountUpdate(s));
        minProcessSpawnIntervalInput.onValueChanged.AddListener(s => OnMinProcessSpawnIntervalUpdate(s));
        maxProcessSpawnIntervalInput.onValueChanged.AddListener(s => OnMaxProcessSpawnIntervalUpdate(s));
        minProcessLoadInput.onValueChanged.AddListener(s => OnMinProcessLoadUpdate(s));
        maxProcessLoadInput.onValueChanged.AddListener(s => OnMaxProcessLoadUpdate(s));
        loadThresholdInput.onValueChanged.AddListener(s => OnLoadThresholdUpdate(s));
        askAroundAttemptsInput.onValueChanged.AddListener(s => OnAskAroundAttemptsUpdate(s));
        alleviateMinLoadInput.onValueChanged.AddListener(s => OnAlleviateMinLoadUpdate(s));
        simulationSpeedInput.onValueChanged.AddListener(s => OnSimulationSpeedUpdate(s));
        simulationSpeedInput.onValueChanged.AddListener(s => OnSimulationSeedUpdate(s));
    }

    private void OnLoadBalancingStrategyChanged()
    {
        currentSettings.loadBalancingStrategyType = (LoadBalancingStrategyType)loadBalancingStrategyInput.Selected;
    }

    private void Start()
    {
        UpdateInputs();
    }

    private void Update()
    {
        CheckValidity();
        UpdateColors();
    }

    private void CheckValidity()
    {
        valid = AreInputsValid();
    }

    private void UpdateInputs()
    {
        currentSettings = SimulationManager.Instance.simulationSettings;

        loadBalancingStrategyInput.Selected = (int)currentSettings.loadBalancingStrategyType;
        processorCountInput.text = currentSettings.processorCount.ToString();
        minProcessSpawnIntervalInput.text = currentSettings.minProcessSpawnInterval.ToString();
        maxProcessSpawnIntervalInput.text = currentSettings.maxProcessSpawnInterval.ToString();
        minProcessLoadInput.text = currentSettings.minProcessLoad.ToString();
        maxProcessLoadInput.text = currentSettings.maxProcessLoad.ToString();
        loadThresholdInput.text = currentSettings.processorLoadThreshold.ToString();
        askAroundAttemptsInput.text = currentSettings.askAroundAttempts.ToString();
        alleviateMinLoadInput.text = currentSettings.alleviateMinLoad.ToString();
        simulationSpeedInput.text = currentSettings.simulationSpeed.ToString();
        simulationSeedInput.text = currentSettings.simulationSeed.ToString();
    }

    private void ApplySettings()
    {
        SimulationManager.Instance.SetSimulationSettings(currentSettings);
        SimulationManager.Instance.SetupNewSimulation();
        SimulationManager.Instance.Rewind();
        SimulationManager.Instance.cameraMovement.ResetPosition();
        Hide();
    }

    public void Show()
    {
        UpdateInputs();
        UIManager.Instance.showOptionsMenu = true;
    }

    public void Hide()
    {
        UIManager.Instance.showOptionsMenu = false;
    }

    private void UpdateColors()
    {
        foreach (KeyValuePair<TMP_InputField, bool> pair in inputValidity)
        {
            pair.Key.textComponent.color = Color.Lerp(pair.Key.textComponent.color, pair.Value ? normalInputColor : invalidInputColor, colorChangeRate * Time.deltaTime);
        }

        applyButton.SetActive(valid);
    }

    private bool AreInputsValid()
    {
        bool valid = true;
        foreach (bool v in inputValidity.Values)
            valid &= v;
        return valid;
    }

    private void OnProcessorCountUpdate(string input)
    {
        if (int.TryParse(input, out int value))
        {
            if (value < 1)
                MarkInvalid(processorCountInput);
            else
            {
                MarkNormal(processorCountInput);
                currentSettings.processorCount = value;
            }

        }
        else
        {
            MarkInvalid(processorCountInput);
        }
    }

    private void OnMinProcessSpawnIntervalUpdate(string input, bool sub = false)
    {
        if (int.TryParse(input, out int minInterval))
        {
            if (minInterval < 0 || minInterval > currentSettings.maxProcessSpawnInterval)
                MarkInvalid(minProcessSpawnIntervalInput);
            else
            {
                MarkNormal(minProcessSpawnIntervalInput);
                currentSettings.minProcessSpawnInterval = minInterval;
            }
        }
        else
        {
            MarkInvalid(minProcessSpawnIntervalInput);
        }

        if (!sub)
            OnMaxProcessSpawnIntervalUpdate(maxProcessSpawnIntervalInput.text, true);
    }

    private void OnMaxProcessSpawnIntervalUpdate(string input, bool sub = false)
    {
        if (int.TryParse(input, out int maxInterval))
        {
            if (maxInterval < 0 || maxInterval < currentSettings.minProcessSpawnInterval)
                MarkInvalid(maxProcessSpawnIntervalInput);
            else
            {
                MarkNormal(maxProcessSpawnIntervalInput);
                currentSettings.maxProcessSpawnInterval = maxInterval;
            }
        }
        else
        {
            MarkInvalid(maxProcessSpawnIntervalInput);
        }

        if (!sub)
            OnMinProcessSpawnIntervalUpdate(minProcessSpawnIntervalInput.text, true);
    }

    private void OnMinProcessLoadUpdate(string input, bool sub = false)
    {
        if (int.TryParse(input, out int minLoad))
        {
            if (minLoad < 0 || minLoad > 100 || minLoad > currentSettings.maxProcessLoad)
                MarkInvalid(minProcessLoadInput);
            else
            {
                MarkNormal(minProcessLoadInput);
                currentSettings.minProcessLoad = minLoad;
            }
        }
        else
        {
            MarkInvalid(minProcessLoadInput);
        }

        if (!sub)
            OnMaxProcessLoadUpdate(maxProcessLoadInput.text, true);
    }

    private void OnMaxProcessLoadUpdate(string input, bool sub = false)
    {
        if (int.TryParse(input, out int maxLoad))
        {
            if (maxLoad < 0 || maxLoad > 100 || maxLoad < currentSettings.minProcessLoad)
                MarkInvalid(maxProcessLoadInput);
            else
            {
                MarkNormal(maxProcessLoadInput);
                currentSettings.maxProcessLoad = maxLoad;
            }
        }
        else
        {
            MarkInvalid(maxProcessLoadInput);
        }

        if (!sub)
            OnMinProcessLoadUpdate(minProcessLoadInput.text, true);
    }

    private void OnLoadThresholdUpdate(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value < 0 || value > 100)
                MarkInvalid(loadThresholdInput);
            else
            {
                MarkNormal(loadThresholdInput);
                currentSettings.processorLoadThreshold = value;
            }
        }
        else
        {
            MarkInvalid(loadThresholdInput);
        }
    }

    private void OnAskAroundAttemptsUpdate(string input)
    {
        if (int.TryParse(input, out int value))
        {
            if (value < 1)
                MarkInvalid(askAroundAttemptsInput);
            else
            {
                MarkNormal(askAroundAttemptsInput);
                currentSettings.askAroundAttempts = value;
            }

        }
        else
        {
            MarkInvalid(askAroundAttemptsInput);
        }
    }

    private void OnAlleviateMinLoadUpdate(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value < 0 || value > 100)
                MarkInvalid(alleviateMinLoadInput);
            else
            {
                MarkNormal(alleviateMinLoadInput);
                currentSettings.alleviateMinLoad = value;
            }
        }
        else
        {
            MarkInvalid(alleviateMinLoadInput);
        }
    }

    private void OnSimulationSpeedUpdate(string input)
    {
        if (float.TryParse(input, out float value))
        {
            if (value < minSimulationSpeed || value > maxSimulationSpeed)
                MarkInvalid(simulationSpeedInput);
            else
            {
                MarkNormal(simulationSpeedInput);
                currentSettings.simulationSpeed = value;
            }
        }
        else
        {
            MarkInvalid(simulationSpeedInput);
        }
    }

    private void OnSimulationSeedUpdate(string input)
    {
        if (int.TryParse(input, out int value))
        {
            MarkNormal(simulationSeedInput);
            currentSettings.simulationSeed = value;
        }
        else
        {
            MarkInvalid(simulationSeedInput);
        }
    }

    private void MarkInvalid(TMP_InputField input)
    {
        inputValidity[input] = false;
    }

    private void MarkNormal(TMP_InputField input)
    {
        inputValidity[input] = true;
    }

    public void OnApply()
    {
        ApplySettings();
    }

    public void OnReturn()
    {
        Hide();
    }
}
