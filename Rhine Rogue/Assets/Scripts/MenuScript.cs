using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;


public class MenuScript : MonoBehaviour
{
    public GameObject mainPanel;
    public AudioMixer audioMixer;
    public UniversalRenderPipelineAsset urpAsset;
    public GameObject audioPanel;
    public Slider renderSlider;
    public TMP_InputField renderField;
    public Toggle vsyncToggle;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resDropdown;

    //TEST
    public MonsterDatabase database;

    public bool isPaused;
    private GameObject activePanel;

    Resolution[] resolutions;

    void Start()
    {
        //Retrieve game's current URP Asset
        urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        //Set the active panel to the audio section (settings menu opens up to audio first)
        activePanel = audioPanel;
        activePanel.SetActive(true);

        //Listen to whenever render resolution slider or field changes (when one changes, the other also corresponds)
        renderSlider.onValueChanged.AddListener(OnSliderChanged);
        renderField.onValueChanged.AddListener(OnFieldChanged);

        //Set vsyncToggle to current Vsync Setting; add listener to see when toggle is changed
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            vsyncToggle.onValueChanged.AddListener(SetVsync);
        }

        //Set fullscreenToggle to current Vsync Setting; add listener to see when toggle is changed
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetVsync);

        //Get all possible screen resolutions
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();
        
        //Add all resolutions to a readable list, which is added to dropdown; current resolution is selected for dropdown value
        List<string> options = new List<string>();
        int CurrentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add (resolutions[i].width + " x " + resolutions[i].height);
            
            if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
            {
                CurrentResIndex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = CurrentResIndex;
        resDropdown.RefreshShownValue();

    }
    
//SWITCH BETWEEN PANELS
    public void SetPanel(GameObject panel)
    {

        database.LoadRandomMonster();

        if (!activePanel)
        {
            activePanel = panel;
            activePanel.SetActive(true);
        }

        else
        {
            activePanel.SetActive(false);
            activePanel = panel;
            activePanel.SetActive(true);
        }

    }

//PAUSE AND RESUME
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        mainPanel.SetActive(true);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        mainPanel.SetActive(false);
    }

//AUDIO
    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("mastervol", vol);
    }

    public void SetMusicVolume(float vol)
    {
        audioMixer.SetFloat("musicvol", vol);
    }

    public void SetSFXVolume(float vol)
    {
        audioMixer.SetFloat("sfxvol", vol);
    }

//GRAPHICS
    private void OnSliderChanged(float num)
    {
        if (renderField.text != num.ToString())
        {
            renderField.text = num.ToString();
            urpAsset.renderScale = num;
        }
    }

    private void OnFieldChanged(string text)
    {
        if (renderSlider.value.ToString() != text)
        {
            if (float.TryParse(text, out float num))
            {
                renderSlider.value = num;
                urpAsset.renderScale = num;
            }
        }
    }

    public void SetGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetVsync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }

    public void SetFS(bool isFS)
    {
        Screen.fullScreen = isFS;
    }

    public void SetAntiAliasing(int index)
    {
        int[] options = {0, 2, 4, 8}; 
        urpAsset.msaaSampleCount = options[index];
    }

}
