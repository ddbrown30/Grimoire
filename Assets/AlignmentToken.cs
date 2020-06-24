using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlignmentToken : MonoBehaviour, IDragHandler
{
    public Sprite GoodSprite;
    public Sprite EvilSprite;

    Image ImageComponent;
    RectTransform RectTrans;

    bool IsGood = true;

    void Awake()
    {
        ImageComponent = GetComponentInChildren<Image>();
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        ImageComponent.alphaHitTestMinimumThreshold = 0.5f;
    }

    public void SetAlignment(bool isGood)
    {
        IsGood = isGood;
        ImageComponent.sprite = IsGood ? GoodSprite : EvilSprite;
    }

    public void ToggleAlignment()
    {
        SetAlignment(!IsGood);
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(GrimoireManager.Instance.transform);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);

        transform.SetParent(GetComponentInParent<Canvas>().transform);
        RectTrans.anchoredPosition = localPoint;
    }
}
