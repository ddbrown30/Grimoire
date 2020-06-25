using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrimoireToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool IsAlive = true;

    public GameObject HelperTokenPrefab;
    public GameObject AlignmentTokenPrefab;
    public ContextMenu ContextMenuPrefab;

    public Image DeadMarker;
    public InputField NameText;

    public Image RoleTokenImage;

    public float MinTokenScale = 0.5f;
    public float MaxTokenScale = 2f;

    bool UseTargetPos = true;
    Vector2 TargetPos = new Vector2();
    public void SetTargetPos(Vector2 targetPos) { TargetPos = targetPos; }
    public void SetUseTargetPos(bool use) { UseTargetPos = use; }

    public RoleData RoleData { private set; get; }
    public RoleData SwappedRoleData { private set; get; }

    List<HelperToken> HelperTokens = new List<HelperToken>();
    AlignmentToken AlignmentToken;

    RectTransform RectTrans;
    Vector2 DragOffset;

    bool IsBluffToken;

    ContextMenu TokenContextMenu;

    void Awake()
    {
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        RoleTokenImage.alphaHitTestMinimumThreshold = 0.5f;
        DeadMarker.enabled = false;

        if(IsBluffToken == false)
        {
            CreateHelperTokens();
            SetRoleTokenScale(GrimoireManager.Instance.RoleScaleSlider.value);
            SetHelperTokenScale(GrimoireManager.Instance.HelperScaleSlider.value);
        }

        if (RoleData.RoleType == RoleType.Traveler)
        {
            CreateAlignmentToken();
        }
    }

    void Update()
    {
        Vector2 pos2d =  RectTrans.anchoredPosition;
        if (UseTargetPos && pos2d != TargetPos)
        {
            const float moveRate = 8f;
            pos2d = Vector2.Lerp(pos2d, TargetPos, moveRate * Time.deltaTime);

            if ((pos2d - TargetPos).sqrMagnitude < 1f)
            {
                pos2d = TargetPos;
            }

            RectTrans.anchoredPosition = pos2d;
        }
    }

    public void ToggleAlive()
    {
        if (IsBluffToken)
            return;

        if (IsAlive)
        {
            IsAlive = false;
            DeadMarker.enabled = true;
            NightManager.Instance.RefreshList();
        }
        else
        {
            IsAlive = true;
            DeadMarker.enabled = false;
            NightManager.Instance.RefreshList();
        }
    }

    public void SetPlayerName(string name)
    {
        NameText.text = name;
    }

    public void SetRoleData(RoleData roleData)
    {
        RoleData = roleData;
        RoleTokenImage.sprite = RoleData.RoleTokenSprite;
    }

    void ChangeRole()
    {
        ModalManager.Instance().OpenSwapPanel(this);
    }

    void CreateAlignmentToken()
    {
        GameObject tokenObj = Instantiate(AlignmentTokenPrefab);
        tokenObj.transform.SetParent(transform);
        tokenObj.transform.localScale = Vector3.one;
        tokenObj.transform.localPosition = Vector3.zero;

        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        RectTransform tokenRect = tokenObj.GetComponent<RectTransform>();
        tokenRect.anchoredPosition = new Vector2(parentRect.rect.xMin + (tokenRect.rect.width / 2f), parentRect.rect.yMin + (tokenRect.rect.height / 2f));

        AlignmentToken = tokenObj.GetComponent<AlignmentToken>();
    }

    void SwapAlignment()
    {
        if(AlignmentToken)
        {
            AlignmentToken.ToggleAlignment();
        }
        else
        {
            CreateAlignmentToken();
            if (RoleData.RoleType == RoleType.Demon || RoleData.RoleType == RoleType.Minion)
            {
                AlignmentToken.SetAlignment(true); //Demons and minions start as evil, so if we've just swapped, they must now be good
            }
            else
            {
                AlignmentToken.SetAlignment(false); //Townsfolk and outsiders start as good, so if we've just swapped, they must now be evil
            }
        }
    }

    public void ChangeRole(RoleData roleData)
    {
        SwappedRoleData = roleData;
        RoleTokenImage.sprite = SwappedRoleData.RoleTokenSprite;
        DestroyHelperTokens();
        CreateHelperTokens();

        SetHelperTokenScale(GrimoireManager.Instance.HelperScaleSlider.value);
    }

    void CreateHelperTokens()
    {
        RoleData dataToUse = SwappedRoleData ? SwappedRoleData : RoleData;
        foreach (var helperTokenSprite in dataToUse.HelperTokenSprites)
        {
            GameObject helperTokenObj = Instantiate(HelperTokenPrefab);
            helperTokenObj.transform.SetParent(GrimoireManager.Instance.HelperTokenAttach.transform);
            helperTokenObj.transform.localScale = Vector3.one;

            HelperToken helperToken = helperTokenObj.GetComponentInChildren<HelperToken>();
            helperToken.SetSprite(helperTokenSprite);

            HelperTokens.Add(helperToken);
        }
    }

    void DestroyHelperTokens()
    {
        foreach (var helperToken in HelperTokens)
        {
            Object.Destroy(helperToken.gameObject);
        }

        HelperTokens.Clear();
    }

    void DestroyAlignmentToken()
    {
        if(AlignmentToken)
        {
            Object.Destroy(AlignmentToken.gameObject);
            AlignmentToken = null;
        }
    }

    public void DestroyOwnedTokens()
    {
        DestroyHelperTokens();
        DestroyAlignmentToken();
    }

    public void ResetHelperTokens(Transform attachTransform)
    {
        foreach (var helperToken in HelperTokens)
        {
            helperToken.transform.SetParent(attachTransform);
        }
    }

    public void SetIsBluffToken()
    {
        IsBluffToken = true;
        GetComponentInChildren<Button>().enabled = false;
    }

    public void SetRoleTokenScale(float scaleNormalized)
    {
        transform.localScale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, scaleNormalized);
    }

    public void SetHelperTokenScale(float scaleNormalized)
    {
        Vector3 newScale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, scaleNormalized);

        foreach (var token in HelperTokens)
        {
            token.transform.localScale = newScale;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsBluffToken)
            eventData.pointerDrag = null;

        eventData.eligibleForClick = false; //Disable clicks so that we still receive OnDrop

        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTrans, Input.mousePosition, null, out DragOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        int index = GrimoireManager.Instance.GetIndexFromPosition(localPoint);
        GrimoireManager.Instance.MoveTownsfolkToIndex(index, this);

        transform.SetAsLastSibling();

        localPoint -= DragOffset;
        RectTrans.anchoredPosition = localPoint;
        TargetPos = localPoint;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.parent.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        int index = GrimoireManager.Instance.GetIndexFromPosition(localPoint);
        GrimoireManager.Instance.UpdateGrimoire();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GrimoireManager.Instance.SetHoverTarget(SwappedRoleData ? SwappedRoleData.RoleTokenSprite : RoleData.RoleTokenSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GrimoireManager.Instance.SetHoverTarget(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsBluffToken)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ToggleAlive();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            TokenContextMenu = Instantiate<GameObject>(ContextMenuPrefab.gameObject, GetComponentInParent<Canvas>().transform).GetComponent<ContextMenu>();
            TokenContextMenu.transform.localScale = Vector3.one;
            TokenContextMenu.transform.localPosition = Vector3.zero;

            TokenContextMenu.AddMenuItem("Change role", ChangeRole);
            TokenContextMenu.AddMenuItem("Swap alignment", SwapAlignment);
            TokenContextMenu.FinaliseMenu();

            ContextMenu.HideAllMenus();//hide other menus
            TokenContextMenu.ShowAtMousePosition();//show the menu
        }
    }
}
