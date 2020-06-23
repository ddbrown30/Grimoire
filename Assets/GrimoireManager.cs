using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using TMPro;

public class GrimoireManager : MonoBehaviour
{
    public UnityEvent OnResetGrimoire;

    public static GrimoireManager Instance;
    List<GrimoireToken> RoleTokens = new List<GrimoireToken>();
    List<GrimoireToken> HiddenTokens = new List<GrimoireToken>();
    List<GrimoireToken> BluffTokens = new List<GrimoireToken>();
    List<GameObject> AlignmentTokens = new List<GameObject>();

    public GameObject RoleTokenPrefab;
    public GameObject HelperTokenPrefab;
    public GameObject AlignmentTokenPrefab;

    public GameObject BluffTokenAttach;
    public GameObject HelperTokenAttach;
    public GameObject AlignmentTokenAttach;

    public Image HoverImage;

    private ModalPanel ModalPanel;

    public float MinTokenScale = 0.5f;
    public float MaxTokenScale = 2f;
    public Slider RoleScaleSlider;
    public Slider HelperScaleSlider;

    private float TownRadius;
    public float MinTokenRadius = 300f;
    public float MaxTokenRadius = 500f;
    public Slider RadiusSlider;

    public TMP_Text RoleCountText;

    public Toggle ShowPlayerNamesToggle;

    public int TownSize { get { return RoleTokens.Count; } }


    public GrimoireManager()
    {
        Instance = this;
    }

    void Awake()
    {
        ModalPanel = ModalPanel.Instance();
    }

    void Start()
    {
        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, RadiusSlider.value);
    }

    public void AddToken(RoleData roleData)
    {
        GameObject tokenObj = Instantiate(RoleTokenPrefab);
        tokenObj.transform.SetParent(transform);
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
        
        foreach (var helperTokenSprite in roleData.HelperTokenSprites)
        {
            GameObject helperTokenObj = Instantiate(HelperTokenPrefab);
            helperTokenObj.transform.SetParent(HelperTokenAttach.transform);
            helperTokenObj.transform.localScale = Vector3.one;

            HelperToken helperToken = helperTokenObj.GetComponentInChildren<HelperToken>();
            helperToken.FreeTransform = transform;
            helperToken.SetSprite(helperTokenSprite);

            grimoireToken.AddHelperToken(helperToken);
        }

        grimoireToken.SetRoleTokenScale(Mathf.Lerp(MinTokenScale, MaxTokenScale, RoleScaleSlider.value));
        grimoireToken.SetHelperTokenScale(Mathf.Lerp(MinTokenScale, MaxTokenScale, HelperScaleSlider.value));

        UpdateGrimoire();
        UpdateRoleCounts();
    }

    public void RemoveToken(RoleData roleData)
    {
        int tokenIndex = RoleTokens.FindIndex(x => x.RoleData == roleData);
        if (tokenIndex < 0)
        {
            tokenIndex = HiddenTokens.FindIndex(x => x.RoleData == roleData);
            if (tokenIndex < 0)
                return;

            HiddenTokens[tokenIndex].DestroyHelperTokens();

            Object.Destroy(HiddenTokens[tokenIndex].gameObject);
            HiddenTokens.RemoveAt(tokenIndex);
            return;
        }

        RoleTokens[tokenIndex].DestroyHelperTokens();

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
        grimoireToken.NameText.gameObject.SetActive(false);
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
            token.DestroyHelperTokens();
            Object.Destroy(token.gameObject);
        }

        foreach (var token in HiddenTokens)
        {
            token.DestroyHelperTokens();
            Object.Destroy(token.gameObject);
        }

        foreach (var token in BluffTokens)
        {
            Object.Destroy(token.gameObject);
        }

        foreach (var token in AlignmentTokens)
        {
            Object.Destroy(token);
        }        

        RoleTokens.Clear();
        HiddenTokens.Clear();
        BluffTokens.Clear();
        AlignmentTokens.Clear();
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
        Vector2 pos2d = gameObject.GetComponent<RectTransform>().anchoredPosition;
        int townSize = RoleTokens.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;
        for (int i = 0; i < townSize; ++i)
        {
            Vector2 dir = RotateVector(Vector2.up, -angleDivision * i);
            GrimoireToken token = RoleTokens[i].GetComponentInChildren<GrimoireToken>();
            token.SetTargetPos(pos2d + (dir * TownRadius));
        }

        SetPlayerNamesVisible(ShowPlayerNamesToggle.isOn);
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

        RoleCountText.text = string.Format("T: {0} | O: {1} | M: {2} | D: {3}", townsfolk, outsiders, minions, demons);
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
                retVal.Add(token.RoleData);
        }

        return retVal;
    }

    public void OnClickReset()
    {
        ModalPanel.MessageBox("Reset the Town Square?", ResetGrimoire, null, null, null, "YesNo");
    }

    public void OnClickRandomize()
    {
        ModalPanel.MessageBox("Randomize player positions?", RandomizeGrimoire, null, null, null, "YesNo");
    }

    public void RoleScaleSliderChanged(float value)
    {
        Vector2 scale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, value);

        foreach (var token in RoleTokens)
        {
            token.SetRoleTokenScale(Mathf.Lerp(MinTokenScale, MaxTokenScale, RoleScaleSlider.value));
        }
    }

    public void HelperScaleSliderChanged(float value)
    {
        Vector2 scale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, value);

        foreach (var token in RoleTokens)
        {
            token.SetHelperTokenScale(Mathf.Lerp(MinTokenScale, MaxTokenScale, HelperScaleSlider.value));
        }

        foreach (var token in AlignmentTokens)
        {
            token.gameObject.transform.localScale = scale;
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
        foreach (var token in RoleTokens)
        {
            InputField inputField = token.GetComponentInChildren<InputField>(true);
            inputField.gameObject.SetActive(visible);
        }
    }

    public void AddAlignmentToken()
    {
        GameObject tokenObj = Instantiate(AlignmentTokenPrefab);
        tokenObj.GetComponent<RectTransform>().anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
        tokenObj.transform.SetParent(AlignmentTokenAttach.transform);
        tokenObj.transform.localScale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, HelperScaleSlider.value);
        tokenObj.transform.localPosition = Vector3.zero;
        AlignmentTokens.Add(tokenObj);
    }

    public void RemoveAlignmentToken()
    {
        if (AlignmentTokens.Count == 0)
            return;

        Object.Destroy(AlignmentTokens[0]);
        AlignmentTokens.RemoveAt(0);
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
}
