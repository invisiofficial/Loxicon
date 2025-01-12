using System;

using UnityEngine;

using TMPro;

public abstract class PresetFactory<T> : MonoBehaviour
{
    #region Events

    public event Action<Preset<T>> OnPresetChosen;

    #endregion

    [Header("Presets settings")]
    [SerializeField] protected TMP_InputField presetNameInputField;
    [SerializeField] protected GameObject presetButtonPrefab;
    [SerializeField] protected GameObject createButtonPrefab;
    [SerializeField] protected string storageKey;

    protected UnityPresetManager<T> _presetManager;
    protected GameObject _createButtonObject;
    protected ColorFlicker _previousFlicker;

    protected virtual void Start()
    {
        // Creating preset manager
        _presetManager = new UnityPresetManager<T>(storageKey);

        // Loading existing presets
        _presetManager.Load();

        // Updating fields
        UpdateFields();

        // Updating presets
        UpdatePresets();
    }

    protected void CreatePresetButton(int index, string name)
    {
        // Creating new preset button
        GameObject gameObject = Instantiate(presetButtonPrefab, this.transform);
        PresetFactoryWorker presetFactoryWorker = gameObject.AddComponent<PresetFactoryWorker>();
        ColorFlicker colorFlicker = gameObject.GetComponentInChildren<ColorFlicker>();
        presetFactoryWorker.OnLeftClick += () => OnPresetButtonLeftClick(index, colorFlicker);
        presetFactoryWorker.OnRightClick += () => OnPresetButtonRightClick(index);
        gameObject.SetActive(true);
        gameObject.GetComponentInChildren<TMP_Text>().text = name;

        // Moving create button to bottom
        _createButtonObject.transform.SetAsLastSibling();

        void OnPresetButtonLeftClick(int index, ColorFlicker currentFlicker)
        {
            // Flicking the button
            currentFlicker.Flick();
            if (_previousFlicker != null) _previousFlicker.Flick();
            _previousFlicker = currentFlicker;

            // Getting preset
            Preset<T> preset = _presetManager.GetPresets()[index];

            // Updating fields based on preset
            SetPreset(preset);

            // Invoking event
            OnPresetChosen?.Invoke(preset);
        }

        void OnPresetButtonRightClick(int index)
        {
            // Removing from preset list
            _presetManager.RemovePreset(index);
            _presetManager.Save();

            // Updating presets
            UpdatePresets();

            // Invoking event
            OnPresetChosen?.Invoke(null);
        }
    }

    protected void CreateCreateButton()
    {
        _createButtonObject = Instantiate(createButtonPrefab, this.transform);
        PresetFactoryWorker presetFactoryWorker = _createButtonObject.AddComponent<PresetFactoryWorker>();
        presetFactoryWorker.OnLeftClick += OnCreateButtonClick;
        _createButtonObject.SetActive(true);

        void OnCreateButtonClick()
        {
            // Adding to preset list
            Preset<T> newPreset = GetPreset();
            _presetManager.AddPreset(newPreset);
            _presetManager.Save();

            // Updating presets
            UpdatePresets();
        }
    }
    
    protected abstract void UpdateFields();
    
    protected abstract void UpdatePresets();

    protected abstract Preset<T> GetPreset();

    protected abstract void SetPreset(Preset<T> preset);
}
