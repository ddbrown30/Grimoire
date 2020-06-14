using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameStep
{
    RoleSelection,
    BluffSelection,
    FirstNight,
    OtherNights
}

public class StepManager : MonoBehaviour
{
    public GameObject NextButton;
    public GameObject PrevButton;
    public TMP_Text StepText;

    public RoleSelectionManager RoleSelectionManager;
    public NightManager NightManager;

    GameStep CurrentStep = GameStep.RoleSelection;

    readonly string[] StepNames = { "Select Roles", "Select Bluffs", "First Night", "Other Nights" };

    public void NextStep()
    {
        GameStep oldStep = CurrentStep;
        ++CurrentStep;

        StepText.text = StepNames[(int)CurrentStep];

        PrevButton.SetActive(true);
        if (CurrentStep == GameStep.OtherNights)
        {
            NextButton.SetActive(false);
        }
        UpdateStep(oldStep);
    }

    public void PrevStep()
    {
        GameStep oldStep = CurrentStep;
        --CurrentStep;

        StepText.text = StepNames[(int)CurrentStep];

        NextButton.SetActive(true);
        if (CurrentStep == GameStep.RoleSelection)
        {
            PrevButton.SetActive(false);
        }

        UpdateStep(oldStep);
    }

    void UpdateStep(GameStep oldStep)
    {
        switch (CurrentStep)
        {
            case GameStep.RoleSelection:
                RoleSelectionManager.BuildRoleList();
                break;
            case GameStep.BluffSelection:
                RoleSelectionManager.gameObject.SetActive(true);
                NightManager.gameObject.SetActive(false);
                RoleSelectionManager.BuildBluffList();
                break;
            case GameStep.FirstNight:
                RoleSelectionManager.gameObject.SetActive(false);
                NightManager.gameObject.SetActive(true);
                NightManager.BuildFirstNightList();
                break;
            case GameStep.OtherNights:
                NightManager.BuildOtherNightsList();
                break;
            default:
                break;
        }
    }

    public void OnGrimoireReset()
    {
        CurrentStep = GameStep.RoleSelection;
        RoleSelectionManager.gameObject.SetActive(true);
        NightManager.gameObject.SetActive(false);
        RoleSelectionManager.BuildRoleList();
        NextButton.SetActive(true);
        PrevButton.SetActive(false);
        StepText.text = StepNames[(int)CurrentStep];
    }
    
    public void ToggleTravelers(bool isOn)
    {
        if (isOn)
        {
            PrevButton.SetActive(false);
            NextButton.SetActive(false);
            StepText.text = "Select Travelers";
        }
        else
        {
            StepText.text = StepNames[(int)CurrentStep];
            switch (CurrentStep)
            {
                case GameStep.RoleSelection:
                    PrevButton.SetActive(false);
                    NextButton.SetActive(true);
                    break;
                case GameStep.BluffSelection:
                    PrevButton.SetActive(true);
                    NextButton.SetActive(true);
                    break;
                case GameStep.FirstNight:
                    PrevButton.SetActive(true);
                    NextButton.SetActive(true);
                    break;
                case GameStep.OtherNights:
                    PrevButton.SetActive(true);
                    NextButton.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
