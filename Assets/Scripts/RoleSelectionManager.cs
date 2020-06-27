using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

enum RoleSeletionState
{
    SelectingRoles,
    SelectingBluffs,
    SelectingTravelers
}
    

public class RoleSelectionManager : MonoBehaviour
{
    public Toggle TroubleBrewingToggle;
    public Toggle BadMoonRisingToggle;
    public Toggle SectsAndVioletsToggle;
    public Toggle UnreleasedToggle;
    public Toggle CustomScriptToggle;

    public Toggle TravelersToggle;

    public RoleData[] RoleDataList;
    public List<RoleData> TroubleBrewingData;
    public List<RoleData> BadMoonRisingData;
    public List<RoleData> SectsAndVioletsData;
    public List<RoleData> UnreleasedData;
    public List<RoleData> CustomScriptData;

    public RoleData[] TravelerDataList;
    public List<RoleData> TroubleBrewingTravelerData;
    public List<RoleData> BadMoonRisingTravelerData;
    public List<RoleData> SectsAndVioletsTravelerData;
    public List<RoleData> UnreleasedTravelerData;

    public GameObject RoleEntry;
    public GameObject RoleEntryDivider;
    public GameObject ScrollListContentPanel;

    List<RoleEntry> RoleEntries = new List<RoleEntry>();

    List<RoleData> InUseRoles = new List<RoleData>();
    List<RoleData> BluffRoles = new List<RoleData>();
    
    RoleSeletionState CurrentState = RoleSeletionState.SelectingRoles;

    void Start()
    {
        foreach (var roleData in RoleDataList)
        {
            switch (roleData.GameEdition)
            {
                case GameEdition.TroubleBrewing:
                    TroubleBrewingData.Add(roleData);
                    break;
                case GameEdition.BadMoonRising:
                    BadMoonRisingData.Add(roleData);
                    break;
                case GameEdition.SectsAndViolets:
                    SectsAndVioletsData.Add(roleData);
                    break;
                case GameEdition.Unreleased:
                    UnreleasedData.Add(roleData);
                    break;
            }
        }

        foreach (var roleData in TravelerDataList)
        {
            switch (roleData.GameEdition)
            {
                case GameEdition.TroubleBrewing:
                    TroubleBrewingTravelerData.Add(roleData);
                    break;
                case GameEdition.BadMoonRising:
                    BadMoonRisingTravelerData.Add(roleData);
                    break;
                case GameEdition.SectsAndViolets:
                    SectsAndVioletsTravelerData.Add(roleData);
                    break;
                case GameEdition.Unreleased:
                    UnreleasedTravelerData.Add(roleData);
                    break;
            }
        }

        BuildRoleList();
    }

    public void ResetManager()
    {
        ClearRoleList();
        InUseRoles.Clear();
        BluffRoles.Clear();
        CurrentState = RoleSeletionState.SelectingRoles;
    }

    public void BuildRoleList()
    {
        CurrentState = RoleSeletionState.SelectingRoles;

        foreach (var bluffRole in BluffRoles)
        {
            GrimoireManager.Instance.RemoveBluffToken(bluffRole);
        }
        BluffRoles.Clear();

        ClearRoleList();

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false && CustomScriptToggle.isOn == false;

        if (showAll || TroubleBrewingToggle.isOn)
        {
            AddRoles(TravelersToggle.isOn ? TroubleBrewingTravelerData : TroubleBrewingData);
        }

        if (showAll || BadMoonRisingToggle.isOn)
        {
            AddRoles(TravelersToggle.isOn ? BadMoonRisingTravelerData : BadMoonRisingData);
        }

        if (showAll || SectsAndVioletsToggle.isOn)
        {
            AddRoles(TravelersToggle.isOn ? SectsAndVioletsTravelerData : SectsAndVioletsData);
        }

        if (showAll || UnreleasedToggle.isOn)
        {
            AddRoles(TravelersToggle.isOn ? UnreleasedTravelerData : UnreleasedData);
        }

        if (TravelersToggle.isOn == false && (showAll || CustomScriptToggle.isOn))
        {
            AddRoles(CustomScriptData);
        }

        if (RoleEntries.Count == 0)
            return;

        RoleEntries = RoleEntries.OrderBy(roleEntry => roleEntry.RoleData.RoleType).ThenBy(roleEntry => roleEntry.RoleData.RoleName).ToList();

        AddDivider("Townsfolk", 0);

        int indexOfOutsider = RoleEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Outsider : false);
        if (indexOfOutsider >= 0)
            AddDivider("Outsider", indexOfOutsider);

        int indexOfMinion = RoleEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Minion : false);
        if (indexOfMinion >= 0)
            AddDivider("Minion", indexOfMinion);

        int indexOfDemon = RoleEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Demon : false);
        if (indexOfDemon >= 0)
            AddDivider("Demon", indexOfDemon);

        foreach (var roleEntry in RoleEntries)
        {
            roleEntry.transform.SetParent(ScrollListContentPanel.transform);
            roleEntry.transform.localScale = Vector3.one;
        }
    }

    void BuildTravelerList()
    {
        CurrentState = RoleSeletionState.SelectingTravelers;

        ClearRoleList();

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false && CustomScriptToggle.isOn == false;
        
        if (showAll || TroubleBrewingToggle.isOn)
        {
            AddRoles(TroubleBrewingTravelerData);
        }

        if (showAll || BadMoonRisingToggle.isOn)
        {
            AddRoles(BadMoonRisingTravelerData);
        }

        if (showAll || SectsAndVioletsToggle.isOn)
        {
            AddRoles(SectsAndVioletsTravelerData);
        }

        if (showAll || UnreleasedToggle.isOn)
        {
            AddRoles(UnreleasedTravelerData);
        }

        if (RoleEntries.Count == 0)
            return;

        RoleEntries = RoleEntries.OrderBy(roleEntry => roleEntry.RoleData.RoleType).ThenBy(roleEntry => roleEntry.RoleData.RoleName).ToList();

        foreach (var roleEntry in RoleEntries)
        {
            roleEntry.transform.SetParent(ScrollListContentPanel.transform);
            roleEntry.transform.localScale = Vector3.one;
        }
    }

    public void BuildBluffList()
    {
        CurrentState = RoleSeletionState.SelectingBluffs;

        ClearRoleList();

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false && CustomScriptToggle.isOn == false;

        if (showAll || TroubleBrewingToggle.isOn)
        {
            AddBluffRoles(TroubleBrewingData);
        }

        if (showAll || BadMoonRisingToggle.isOn)
        {
            AddBluffRoles(BadMoonRisingData);
        }

        if (showAll || SectsAndVioletsToggle.isOn)
        {
            AddBluffRoles(SectsAndVioletsData);
        }

        if (showAll || UnreleasedToggle.isOn)
        {
            AddBluffRoles(UnreleasedData);
        }

        if (showAll || CustomScriptToggle.isOn)
        {
            AddBluffRoles(CustomScriptData);
        }

        if (RoleEntries.Count == 0)
            return;
        
        RoleEntries = RoleEntries.OrderBy(roleEntry => roleEntry.RoleData.RoleType).ThenBy(roleEntry => roleEntry.RoleData.RoleName).ToList();

        int indexOfTownsfolk = RoleEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Townsfolk : false);
        if (indexOfTownsfolk >= 0)
            AddDivider("Townsfolk", indexOfTownsfolk);

        int indexOfOutsider = RoleEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Outsider : false);
        if (indexOfOutsider >= 0)
            AddDivider("Outsider", indexOfOutsider);

        foreach (var roleEntry in RoleEntries)
        {
            roleEntry.transform.SetParent(ScrollListContentPanel.transform);
            roleEntry.transform.localScale = Vector3.one;
        }
    }

    void AddRoles(List<RoleData> roleData)
    {
        foreach (var role in roleData)
        {
            if (RoleEntries.Exists(x => x.RoleData == role))
                continue;

            GameObject roleEntryObject = Instantiate(RoleEntry);
            roleEntryObject.name = "Entry:" + role.RoleName;

            TMP_Text roleText = roleEntryObject.GetComponentInChildren<TMP_Text>();
            roleText.text = role.RoleName;

            RoleEntry roleEntry = roleEntryObject.GetComponentInChildren<RoleEntry>();
            roleEntry.RoleData = role;

            roleText.color = RoleData.RoleColors[(int)role.RoleType];

            if (InUseRoles.Contains(role))
            {
                Toggle toggle = roleEntryObject.GetComponentInChildren<Toggle>();
                toggle.SetIsOnWithoutNotify(true);
            }

            RoleEntries.Add(roleEntry);
        }
    }

    void AddBluffRoles(List<RoleData> roleData)
    {
        foreach (var role in roleData)
        {
            if (InUseRoles.Contains(role))
                continue;

            if (RoleEntries.Exists(x => x.RoleData == role))
                continue;

            if (role.RoleType != RoleType.Townsfolk && role.RoleType != RoleType.Outsider)
                continue;

            GameObject roleEntryObject = Instantiate(RoleEntry);
            roleEntryObject.name = "Entry:" + role.RoleName;

            TMP_Text roleText = roleEntryObject.GetComponentInChildren<TMP_Text>();
            roleText.text = role.RoleName;

            RoleEntry roleEntry = roleEntryObject.GetComponentInChildren<RoleEntry>();
            roleEntry.RoleData = role;

            roleText.color = RoleData.RoleColors[(int)role.RoleType];

            if (BluffRoles.Contains(role))
            {
                Toggle toggle = roleEntryObject.GetComponentInChildren<Toggle>();
                toggle.SetIsOnWithoutNotify(true);
            }

            RoleEntries.Add(roleEntry);
        }
    }

    void AddDivider(string dividerName, int index)
    {
        GameObject divider = Instantiate(RoleEntryDivider);
        divider.name = dividerName + "Divider";
        divider.GetComponentInChildren<TMP_Text>().text = dividerName;
        RoleEntries.Insert(index, divider.GetComponentInChildren<RoleEntry>());
    }

    public void ClearRoleList()
    {
        foreach (var roleEntry in RoleEntries)
        {
            Object.Destroy(roleEntry.gameObject);
        }

        RoleEntries.Clear();
    }

    public void RoleEntryToggled(RoleData roleData, bool useRole)
    {
        if (useRole)
        {
            switch (CurrentState)
            {
                case RoleSeletionState.SelectingRoles:
                    {
                        InUseRoles.Add(roleData);
                        GrimoireManager.Instance.AddToken(roleData);
                    }
                    break;
                case RoleSeletionState.SelectingBluffs:
                    {
                        BluffRoles.Add(roleData);
                        GrimoireManager.Instance.AddBluffToken(roleData);
                    }
                    break;
                case RoleSeletionState.SelectingTravelers:
                    {
                        InUseRoles.Add(roleData);
                        GrimoireManager.Instance.AddToken(roleData);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case RoleSeletionState.SelectingRoles:
                    {
                        InUseRoles.Remove(roleData);
                        GrimoireManager.Instance.RemoveToken(roleData);
                    }
                    break;
                case RoleSeletionState.SelectingBluffs:
                    {
                        BluffRoles.Remove(roleData);
                        GrimoireManager.Instance.RemoveBluffToken(roleData);
                    }
                    break;
                case RoleSeletionState.SelectingTravelers:
                    {
                        InUseRoles.Remove(roleData);
                        GrimoireManager.Instance.RemoveToken(roleData);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    bool WasActiveBeforeTravelers;
    RoleSeletionState StateBeforeTravelers;
    public void ToggleTravelers(bool isOn)
    {
        if(isOn)
        {
            WasActiveBeforeTravelers = gameObject.activeSelf;
            gameObject.SetActive(true);
            StateBeforeTravelers = CurrentState;
            BuildTravelerList();
        }
        else
        {
            gameObject.SetActive(WasActiveBeforeTravelers);
            switch (StateBeforeTravelers)
            {
                case RoleSeletionState.SelectingRoles:
                    {
                        BuildRoleList();
                    }
                    break;
                case RoleSeletionState.SelectingBluffs:
                    {
                        BuildBluffList();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void OnCustomScriptLoaded()
    {
        TroubleBrewingToggle.SetIsOnWithoutNotify(false);
        BadMoonRisingToggle.SetIsOnWithoutNotify(false);
        SectsAndVioletsToggle.SetIsOnWithoutNotify(false);
        UnreleasedToggle.SetIsOnWithoutNotify(false);

        bool enableCustomScriptFilter = CustomScriptData.Count > 0;
        CustomScriptToggle.gameObject.SetActive(enableCustomScriptFilter);
        CustomScriptToggle.SetIsOnWithoutNotify(enableCustomScriptFilter);

        UpdateEditionFilter();
    }

    public void UpdateEditionFilter()
    {
        switch (CurrentState)
        {
            case RoleSeletionState.SelectingRoles:
                {
                    BuildRoleList();
                }
                break;
            case RoleSeletionState.SelectingBluffs:
                {
                    BuildBluffList();
                }
                break;
            case RoleSeletionState.SelectingTravelers:
                {
                    BuildTravelerList();
                }
                break;
            default:
                break;
        }
    }
}
