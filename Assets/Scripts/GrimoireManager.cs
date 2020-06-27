using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class GrimoireManager : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnResetGrimoire;

    public static GrimoireManager Instance;
    List<GrimoireToken> RoleTokens = new List<GrimoireToken>();
    List<GrimoireToken> HiddenTokens = new List<GrimoireToken>();
    List<GrimoireToken> BluffTokens = new List<GrimoireToken>();

    public GameObject RoleTokenPrefab;
    public ContextMenu ContextMenuPrefab;

    public GameObject RoleTokenAttach;
    public GameObject HelperTokenAttach;
    public GameObject BluffTokenAttach;
    public GameObject AlignmentTokenAttach;

    public Image HoverImage;

    private ModalManager ModalManager;

    public Slider RoleScaleSlider;
    public Slider HelperScaleSlider;

    public Image BackgroundImage;
    public TMP_Dropdown BackgroundSelectDropdown;

    private float TownRadius;
    public float MinTokenRadius = 300f;
    public float MaxTokenRadius = 500f;
    public Slider RadiusSlider;

    public TMP_Text RoleCountText;

    bool PlayerNamesVisible = true;

    public int TownSize { get { return RoleTokens.Count; } }

    ContextMenu GrimoireContextMenu;


    public GrimoireManager()
    {
        Instance = this;
    }

    void Awake()
    {
        ModalManager = ModalManager.Instance();
    }

    void Start()
    {
        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, RadiusSlider.value);
    }

    public GrimoireToken AddToken(RoleData roleData)
    {
        GameObject tokenObj = Instantiate(RoleTokenPrefab);
        tokenObj.transform.SetParent(RoleTokenAttach.transform);
        tokenObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        GrimoireToken grimoireToken = tokenObj.GetComponentInChildren<GrimoireToken>();
        grimoireToken.name = "RoleToken:" + roleData.RoleName;
        grimoireToken.SetRoleData(roleData);

        if (roleData.AddToGrimoire)
        {
            RoleTokens.Add(grimoireToken);
        }
        else
        {
            grimoireToken.gameObject.SetActive(false);
            HiddenTokens.Add(grimoireToken);
        }

        UpdateGrimoire();
        UpdateRoleCounts();

        return grimoireToken;
    }

    public void RemoveToken(RoleData roleData)
    {
        int tokenIndex = RoleTokens.FindIndex(x => x.RoleData == roleData);
        if (tokenIndex < 0)
        {
            tokenIndex = HiddenTokens.FindIndex(x => x.RoleData == roleData);
            if (tokenIndex < 0)
                return;

            HiddenTokens[tokenIndex].DestroyOwnedTokens();

            Object.Destroy(HiddenTokens[tokenIndex].gameObject);
            HiddenTokens.RemoveAt(tokenIndex);
            return;
        }

        RoleTokens[tokenIndex].DestroyOwnedTokens();

        Object.Destroy(RoleTokens[tokenIndex].gameObject);
        RoleTokens.RemoveAt(tokenIndex);
        UpdateGrimoire();
        UpdateRoleCounts();
    }

    public void AddBluffToken(RoleData roleData)
    {
        GameObject tokenObj = Instantiate(RoleTokenPrefab);
        tokenObj.transform.SetParent(BluffTokenAttach.transform);
        tokenObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        GrimoireToken grimoireToken = tokenObj.GetComponentInChildren<GrimoireToken>();
        grimoireToken.name = "BluffToken:" + roleData.RoleName;
        grimoireToken.SetUseTargetPos(false);
        grimoireToken.SetRoleData(roleData);
        grimoireToken.SetTargetPos(grimoireToken.transform.position);
        grimoireToken.transform.localScale = Vector3.one * 0.75f;
        grimoireToken.SetIsBluffToken();
        BluffTokens.Add(grimoireToken);
    }

    public void RemoveBluffToken(RoleData roleData)
    {
        int tokenIndex = BluffTokens.FindIndex(x => x.RoleData == roleData);
        if (tokenIndex < 0)
            return;

        Object.Destroy(BluffTokens[tokenIndex].gameObject);
        BluffTokens.RemoveAt(tokenIndex);
    }

    public void RandomizeGrimoire()
    {
        RoleTokens = RoleTokens.OrderBy(x => Random.value).ToList();
        UpdateGrimoire();
    }

    public void ResetGrimoire()
    {
        foreach (var token in RoleTokens)
        {
            token.DestroyOwnedTokens();
            Object.Destroy(token.gameObject);
        }

        foreach (var token in HiddenTokens)
        {
            token.DestroyOwnedTokens();
            Object.Destroy(token.gameObject);
        }

        foreach (var token in BluffTokens)
        {
            Object.Destroy(token.gameObject);
        }

        RoleTokens.Clear();
        HiddenTokens.Clear();
        BluffTokens.Clear();
        UpdateGrimoire();
        UpdateRoleCounts();
        OnResetGrimoire.Invoke();
    }


    public Vector2 RotateVector(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public void UpdateGrimoire()
    {
        Vector2 pos2d = RoleTokenAttach.GetComponent<RectTransform>().anchoredPosition;
        int townSize = RoleTokens.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;
        for (int i = 0; i < townSize; ++i)
        {
            Vector2 dir = RotateVector(Vector2.up, -angleDivision * i);
            GrimoireToken token = RoleTokens[i].GetComponentInChildren<GrimoireToken>();
            token.SetTargetPos(pos2d + (dir * TownRadius));
        }

        SetPlayerNamesVisible(PlayerNamesVisible);
    }

    public void UpdateRoleCounts()
    {
        int townsfolk = 0;
        int outsiders = 0;
        int minions = 0;
        int demons = 0;

        foreach (var role in RoleTokens)
        {
            switch (role.RoleData.RoleType)
            {
                case RoleType.Townsfolk:
                    ++townsfolk;
                    break;
                case RoleType.Outsider:
                    ++outsiders;
                    break;
                case RoleType.Minion:
                    ++minions;
                    break;
                case RoleType.Demon:
                    ++demons;
                    break;
                default:
                    break;
            }
        }

        RoleCountText.text = string.Format("<color=#009BFF>T: {0}</color> | <color=#009BFF>O: {1}</color> | <color=\"red\">M: {2}</color> | <color=\"red\">D: {3}</color>", townsfolk, outsiders, minions, demons);
    }

    public int GetIndexFromPosition(Vector2 position)
    {
        int townSize = RoleTokens.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;

        Vector2 pos2d = gameObject.GetComponent<RectTransform>().anchoredPosition;
        Vector2 dir = (position - pos2d).normalized;


        float upAngle = Mathf.Atan2(Vector2.up.y, Vector2.up.x);
        float posAngle = Mathf.Atan2(dir.y, dir.x);
        float angle = Mathf.DeltaAngle(posAngle * Mathf.Rad2Deg, upAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        if(angle < 0)
            angle += Mathf.PI * 2;
        return (int)(angle / angleDivision);
    }

    public void MoveTownsfolkToIndex(int index, GrimoireToken token)
    {
        int currentIndex = RoleTokens.IndexOf(token);
        if (currentIndex == index)
        {
            UpdateGrimoire();
            return;
        }

        RoleTokens.Remove(token);
        if (index < RoleTokens.Count)
        {
            RoleTokens.Insert(index, token);
        }
        else
        {
            RoleTokens.Add(token);
        }

        UpdateGrimoire();
    }

    public List<RoleData> GetLivingPlayerRoles()
    {
        List <RoleData> retVal = new List<RoleData>();
        foreach (var token in RoleTokens)
        {
            if(token.IsAlive)
                retVal.Add(token.SwappedRoleData ? token.SwappedRoleData : token.RoleData);
        }

        return retVal;
    }

    public void OnClickResetGrimoire()
    {
        ModalManager.MessageBox("Reset the Grimoire?", ResetGrimoire, null, null, null, "YesNo");
    }

    public void OnClickResetHelperTokens()
    {
        ModalManager.MessageBox("Reset the position of all helper tokens?", ResetHelperTokens, null, null, null, "YesNo");
    }

    public void OnClickRandomize()
    {
        ModalManager.MessageBox("Randomize player positions?", RandomizeGrimoire, null, null, null, "YesNo");
    }

    public void RoleScaleSliderChanged(float value)
    {
        foreach (var token in RoleTokens)
        {
            token.SetRoleTokenScale(RoleScaleSlider.value);
        }
    }

    public void HelperScaleSliderChanged(float value)
    {
        foreach (var token in RoleTokens)
        {
            token.SetHelperTokenScale(HelperScaleSlider.value);
        }
    }

    public void RadiusSliderChanged(float value)
    {
        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, value);
        UpdateGrimoire();
    }

    public void SetHoverTarget(Sprite sprite)
    {
        bool show = sprite != null;
        HoverImage.gameObject.SetActive(show);
        HoverImage.sprite = sprite;
    }

    public void SetPlayerNamesVisible(bool visible)
    {
        PlayerNamesVisible = visible;
        foreach (var token in RoleTokens)
        {
            token.NameText.gameObject.SetActive(visible);
        }
    }

    public void ResetHelperTokens()
    {
        foreach (var token in RoleTokens)
        {
            token.ResetHelperTokens(HelperTokenAttach.transform);
        }

        foreach (var token in HiddenTokens)
        {
            token.ResetHelperTokens(HelperTokenAttach.transform);
        }
    }

    public void SetBackgroundImage(Sprite backgroundImage)
    {
        //Sprite backgroundImage = BackgroundSelectDropdown.options[index].image;
        if(backgroundImage != null)
        {
            BackgroundImage.gameObject.SetActive(true);
            BackgroundImage.sprite = backgroundImage;
        }
        else
        {
            BackgroundImage.gameObject.SetActive(false);
            BackgroundImage.sprite = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GrimoireContextMenu = Instantiate<GameObject>(ContextMenuPrefab.gameObject, GetComponentInParent<Canvas>().transform).GetComponent<ContextMenu>();
            GrimoireContextMenu.transform.localScale = Vector3.one;
            GrimoireContextMenu.transform.localPosition = Vector3.zero;

            GrimoireContextMenu.AddMenuItem("Reset Grimoire", OnClickResetGrimoire);
            GrimoireContextMenu.AddMenuItem("Reset helper tokens", OnClickResetHelperTokens);
            GrimoireContextMenu.AddMenuItem("Randomize player positions", OnClickRandomize);
            GrimoireContextMenu.FinaliseMenu();

            ContextMenu.HideAllMenus();//hide other menus
            GrimoireContextMenu.ShowAtMousePosition();//show the menu
        }
    }
}
