using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrimoireToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsAlive = true;

    public Image DeadMarker;
    public InputField NameText;
    public Sprite TokenSprite;

    public bool AddToGrimoire = true;

    bool UseTargetPos = true;
    Vector2 TargetPos = new Vector2();
    public void SetTargetPos(Vector2 targetPos) { TargetPos = targetPos; }
    public void SetUseTargetPos(bool use) { UseTargetPos = use; }

    public RoleData RoleData;

    public HelperToken[] HelperTokens;

    RectTransform RectTrans;

    bool IsBluffToken;

    void Awake()
    {
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        DeadMarker.enabled = false;
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

    public void SetIsBluffToken()
    {
        IsBluffToken = true;
        GetComponentInChildren<Button>().enabled = false;
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
        foreach (var token in HelperTokens)
        {
            token.transform.localScale = Vector3.one * scale;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsBluffToken)
            eventData.pointerDrag = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        int index = GrimoireManager.Instance.GetIndexFromPosition(localPoint);
        GrimoireManager.Instance.MoveTownsfolkToIndex(index, this);

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
        GrimoireManager.Instance.SetHoverTarget(TokenSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GrimoireManager.Instance.SetHoverTarget(null);
    }
}
