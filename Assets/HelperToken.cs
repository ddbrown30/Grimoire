using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelperToken : MonoBehaviour, IDragHandler
{
    public Transform FreeTransform;

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.SetParent(FreeTransform);
        gameObject.transform.transform.position = Input.mousePosition;
        GetComponent<RectTransform>().SetAsLastSibling();
    }
}
