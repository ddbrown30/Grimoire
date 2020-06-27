using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool IsOpen;
    bool IsMouseOver;

    public GameObject PanelParent;

    public Slider RoleScaleSlider;
    public Slider HelperScaleSlider;
    public Slider RadiusSlider;
    public BackgroundImageSelector BackgroundSelector;
    public Toggle PlayerNamesToggle;

    float DefaultRoleScale = 0.5f;
    float DefaultHelperScale = 0.5f;
    float DefaultRadiusScale = 0.6f;
    int DefaultBackgroundIndex = 0;
    bool DefaultPlayerNames = true;

    void Awake()
    {
        
        RoleScaleSlider.value = PlayerPrefs.GetFloat("RoleScale", DefaultRoleScale);
        HelperScaleSlider.value = PlayerPrefs.GetFloat("HelperScale", DefaultHelperScale);
        RadiusSlider.value = PlayerPrefs.GetFloat("Radius", DefaultRadiusScale);
        BackgroundSelector.SetImageIndex(PlayerPrefs.GetInt("BackgroundIndex", DefaultBackgroundIndex));
        PlayerNamesToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("PlayerNames", Convert.ToInt32(DefaultPlayerNames)));
    }

    void ResetToDefaults()
    {
        RoleScaleSlider.value = DefaultRoleScale;
        HelperScaleSlider.value = DefaultHelperScale;
        RadiusSlider.value = DefaultRadiusScale;
        BackgroundSelector.SetImageIndex(DefaultBackgroundIndex);
        PlayerNamesToggle.isOn = DefaultPlayerNames;

        PlayerPrefs.SetFloat("RoleScale", RoleScaleSlider.value);
        PlayerPrefs.SetFloat("HelperScale", HelperScaleSlider.value);
        PlayerPrefs.SetFloat("Radius", RadiusSlider.value);
        PlayerPrefs.SetInt("BackgroundIndex", BackgroundSelector.CurrentIndex);
        PlayerPrefs.SetInt("PlayerNames", Convert.ToInt32(PlayerNamesToggle.isOn));
    }

    public void OpenOptionsPanel()
    {
        IsOpen = true;
        PanelParent.SetActive(true);
    }

    void CloseOptionsPanel()
    {
        IsOpen = false;
        PanelParent.SetActive(false);

        //Save our current values when we close the menu
        PlayerPrefs.SetFloat("RoleScale", RoleScaleSlider.value);
        PlayerPrefs.SetFloat("HelperScale", HelperScaleSlider.value);
        PlayerPrefs.SetFloat("Radius", RadiusSlider.value);
        PlayerPrefs.SetInt("BackgroundIndex", BackgroundSelector.CurrentIndex);
        PlayerPrefs.SetInt("PlayerNames", Convert.ToInt32(PlayerNamesToggle.isOn));
    }

    public void OnClickResetOptions()
    {
        ModalManager.Instance().MessageBox("Reset all options to their default values?", ResetToDefaults, null, null, null, "YesNo");
    }

    void Update()
    {
        if(IsOpen && IsMouseOver == false)
        {
            if (Input.GetMouseButtonDown(0))
                CloseOptionsPanel();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
    }
}
