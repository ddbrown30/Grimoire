using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


enum NightState
{
    FirstNight,
    OtherNight
}

public class NightManager : MonoBehaviour
{
    public GameObject NightEntry;
    public GameObject ScrollListContentPanel;

    public int MinionInfoOrder;
    public int DemonInfoOrder;

    List<GameObject> NightEntries = new List<GameObject>();

    NightState NightState = NightState.FirstNight;

    public static NightManager Instance;

    struct EntryInfo
    {
        public EntryInfo(string text, Color color, int nightOrder, RoleData roleData)
        {
            Text = text;
            Color = color;
            NightOrder = nightOrder;
            RoleData = roleData;
        }
        public string Text;
        public Color Color;
        public int NightOrder;
        public RoleData RoleData;
    }

    public NightManager()
    {
        Instance = this;
    }

    public void BuildFirstNightList()
    {
        NightState = NightState.FirstNight;

        ClearList();

        List<EntryInfo> entryInfos = new List<EntryInfo>();
        List<EntryInfo> afterDawnEntryInfos = new List<EntryInfo>();

        if (GrimoireManager.Instance.TownSize >= 7)
        {
            entryInfos.Add(new EntryInfo("Minion Info", Color.white, MinionInfoOrder, null));
            entryInfos.Add(new EntryInfo("Demon Info", Color.white, DemonInfoOrder, null));
        }

        List<RoleData> roles = GrimoireManager.Instance.GetLivingPlayerRoles();
        foreach (var role in roles)
        {
            if (role.FirstNightOrder < 0)
                continue;

            if (role.IsNightOrderAfterDawn)
            {
                afterDawnEntryInfos.Add(new EntryInfo(role.RoleName, RoleData.RoleColors[(int)role.RoleType], role.FirstNightOrder, role));
            }
            else
            {
                entryInfos.Add(new EntryInfo(role.RoleName, RoleData.RoleColors[(int)role.RoleType], role.FirstNightOrder, role));
            }
        }

        entryInfos = entryInfos.OrderBy(x => x.NightOrder).ToList();
        foreach (var entryInfo in entryInfos)
        {
            AddEntry(entryInfo.Text, entryInfo.Color, entryInfo.RoleData);
        }

        AddEntry("Dawn", Color.white, null);

        afterDawnEntryInfos = afterDawnEntryInfos.OrderBy(x => x.NightOrder).ToList();
        foreach (var afterDawnEntryInfo in afterDawnEntryInfos)
        {
            AddEntry(afterDawnEntryInfo.Text, afterDawnEntryInfo.Color, afterDawnEntryInfo.RoleData);
        }
    }

    public void BuildOtherNightsList()
    {
        NightState = NightState.OtherNight;

        ClearList();

        AddEntry("Dusk", Color.white, null);

        List<RoleData> roles = GrimoireManager.Instance.GetLivingPlayerRoles();
        roles = roles.OrderBy(x => x.OtherNightOrder).ToList();
        foreach (var role in roles)
        {
            if (role.OtherNightOrder < 0)
                continue;

            AddEntry(role.RoleName, RoleData.RoleColors[(int)role.RoleType], role);
        }

        AddEntry("Dawn", Color.white, null);
    }

    void AddEntry(string text, Color color, RoleData roleData)
    {
        GameObject nightEntryObject = Instantiate(NightEntry);
        nightEntryObject.name = "Entry:" + text;

        TMP_Text roleText = nightEntryObject.GetComponentInChildren<TMP_Text>();
        roleText.text = text;
        roleText.color = color;

        nightEntryObject.transform.SetParent(ScrollListContentPanel.transform);
        nightEntryObject.transform.localScale = Vector3.one;

        NightEntry nightEntry = nightEntryObject.GetComponentInChildren<NightEntry>();
        nightEntry.SetRoleData(roleData);

        NightEntries.Add(nightEntryObject);
    }

    public void RefreshList()
    {
        if(NightState == NightState.FirstNight)
        {
            BuildFirstNightList();
        }
        else
        {
            BuildOtherNightsList();
        }
    }

    public void ClearList()
    {
        foreach (var nightEntry in NightEntries)
        {
            Object.Destroy(nightEntry);
        }

        NightEntries.Clear();
    }

    bool WasActiveBeforeTravelers;
    public void ToggleTravelers(bool isOn)
    {
        if (isOn)
        {
            WasActiveBeforeTravelers = gameObject.activeSelf;
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(WasActiveBeforeTravelers);
            RefreshList();
        }
    }
}
