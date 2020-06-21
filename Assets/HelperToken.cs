using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelperToken : MonoBehaviour, IDragHandler
{
    public Transform FreeTransform;
    public Image HelperTokenImage;

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(FreeTransform);
        gameObject.transform.transform.position = Input.mousePosition;
        GetComponent<RectTransform>().SetAsLastSibling();
    }

    public void SetSprite(Sprite sprite)
    {
        HelperTokenImage.sprite = sprite;
    }
}
