using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AlignmentToken : MonoBehaviour, IDragHandler, IDropHandler
{
    public Sprite GoodSprite;
    public Sprite EvilSprite;

    Image ImageComponent;
    RectTransform RectTrans;

    bool IsGood = true;

    void Awake()
    {
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        ImageComponent = GetComponentInChildren<Image>();
    }

    public void ToggleAlignment()
    {
        IsGood = !IsGood;
        ImageComponent.sprite = IsGood ? GoodSprite : EvilSprite;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);

        transform.SetParent(GetComponentInParent<Canvas>().transform);
        RectTrans.anchoredPosition = localPoint;
    }

    public void OnDrop(PointerEventData eventData)
    {
    }
}
