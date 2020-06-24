using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SwapRolePanel : MonoBehaviour
{
    public RoleSelectionManager RoleSelectionManager;
    
    public Toggle TroubleBrewingToggle;
    public Toggle BadMoonRisingToggle;
    public Toggle SectsAndVioletsToggle;
    public Toggle UnreleasedToggle;

    public GameObject SwapEntry;
    public GameObject SwapEntryDivider;
    public GameObject ScrollListContentPanel;

    List<SwapEntry> SwapEntries = new List<SwapEntry>();

    GrimoireToken TargetToken;

    UnityEvent OnSelectedListener;

    public void OpenSwapPanel(GrimoireToken token, UnityAction CloseEvent)
    {
        TargetToken = token;
        OnSelectedListener = new UnityEvent();
        OnSelectedListener.AddListener(CloseEvent);
        BuildRoleList();
    }

    void BuildRoleList()
    {
        ClearRoleList();

        bool showAll = TroubleBrewingToggle.isOn == false && BadMoonRisingToggle.isOn == false && SectsAndVioletsToggle.isOn == false && UnreleasedToggle.isOn == false;

        if (showAll || TroubleBrewingToggle.isOn)
        {
            AddRoles(RoleSelectionManager.TroubleBrewingData);
        }

        if (showAll || BadMoonRisingToggle.isOn)
        {
            AddRoles(RoleSelectionManager.BadMoonRisingData);
        }

        if (showAll || SectsAndVioletsToggle.isOn)
        {
            AddRoles(RoleSelectionManager.SectsAndVioletsData);
        }

        if (showAll || UnreleasedToggle.isOn)
        {
            AddRoles(RoleSelectionManager.UnreleasedData);
        }

        if (SwapEntries.Count == 0)
            return;

        SwapEntries = SwapEntries.OrderBy(roleEntry => roleEntry.RoleData.RoleType).ThenBy(roleEntry => roleEntry.RoleData.RoleName).ToList();

        AddDivider("Townsfolk", 0);

        int indexOfOutsider = SwapEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Outsider : false);
        if (indexOfOutsider >= 0)
            AddDivider("Outsider", indexOfOutsider);

        int indexOfMinion = SwapEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Minion : false);
        if (indexOfMinion >= 0)
            AddDivider("Minion", indexOfMinion);

        int indexOfDemon = SwapEntries.FindIndex(roleEntry => roleEntry.RoleData != null ? roleEntry.RoleData.RoleType == RoleType.Demon : false);
        if (indexOfDemon >= 0)
            AddDivider("Demon", indexOfDemon);

        foreach (var roleEntry in SwapEntries)
        {
            roleEntry.transform.SetParent(ScrollListContentPanel.transform);
            roleEntry.transform.localScale = Vector3.one;
        }
    }

    public void ClearRoleList()
    {
        foreach (var roleEntry in SwapEntries)
        {
            Object.Destroy(roleEntry.gameObject);
        }

        SwapEntries.Clear();
    }

    void AddRoles(RoleData[] roleData)
    {
        foreach (var role in roleData)
        {
            GameObject roleEntryObject = Instantiate(SwapEntry);
            roleEntryObject.name = "Entry:" + role.RoleName;

            TMP_Text roleText = roleEntryObject.GetComponentInChildren<TMP_Text>();
            roleText.text = role.RoleName;

            SwapEntry swapEntry = roleEntryObject.GetComponentInChildren<SwapEntry>();
            swapEntry.RoleData = role;

            roleText.color = RoleData.RoleColors[(int)role.RoleType];

            SwapEntries.Add(swapEntry);
        }
    }

    void AddDivider(string dividerName, int index)
    {
        GameObject divider = Instantiate(SwapEntryDivider);
        divider.name = dividerName + "Divider";
        divider.GetComponentInChildren<TMP_Text>().text = dividerName;
        SwapEntries.Insert(index, divider.GetComponentInChildren<SwapEntry>());
    }

    public void SelectRole(RoleData roleData)
    {
        TargetToken.ChangeRole(roleData);
        OnSelectedListener.Invoke();
    }

    public void OnCancel()
    {
        OnSelectedListener.Invoke();
    }

    public void UpdateEditionFilter()
    {
        BuildRoleList();
    }
}
