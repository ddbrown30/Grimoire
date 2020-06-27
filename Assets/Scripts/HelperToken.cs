using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelperToken : MonoBehaviour, IDragHandler
{
    public Image HelperTokenImage;

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(GrimoireManager.Instance.transform);
        gameObject.transform.transform.position = Input.mousePosition;
        GetComponent<RectTransform>().SetAsLastSibling();

        HelperTokenImage.alphaHitTestMinimumThreshold = 0.5f;
    }

    public void SetSprite(Sprite sprite)
    {
        HelperTokenImage.sprite = sprite;
    }
}
