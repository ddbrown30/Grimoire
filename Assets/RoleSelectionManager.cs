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

    public Toggle TravelersToggle;

    public RoleData[] TroubleBrewingData;
    public RoleData[] BadMoonRisingData;
    public RoleData[] SectsAndVioletsData;
    public RoleData[] UnreleasedData;

    public RoleData[] TroubleBrewingTravelerData;
    public RoleData[] BadMoonRisingTravelerData;
    public RoleData[] SectsAndVioletsTravelerData;
    public RoleData[] UnreleasedTravelerData;

    public GameObject RoleEntry;
    public GameObject RoleEntryDivider;
    public GameObject ScrollListContentPanel;

    List<RoleEntry> RoleEntries = new List<RoleEntry>();

    List<RoleData> InUseRoles = new List<RoleData>();
    List<RoleData> BluffRoles = new List<RoleData>();
    
    RoleSeletionState CurrentState = RoleSeletionState.SelectingRoles;

    void Start()
    {
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

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false;

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

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false;

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

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false;

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

        foreach (var roleEntry in RoleEntries)
        {
            roleEntry.transform.SetParent(ScrollListContentPanel.transform);
            roleEntry.transform.localScale = Vector3.one;
        }
    }

    void AddRoles(RoleData[] roleData)
    {
        foreach (var role in roleData)
        {
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

    void AddBluffRoles(RoleData[] roleData)
    {
        foreach (var role in roleData)
        {
            if (InUseRoles.Contains(role))
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
