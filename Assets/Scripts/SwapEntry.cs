using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RoleData RoleData;

    SwapRolePanel SwapRolePanel;

    public void Start()
    {
        SwapRolePanel = GetComponentInParent<SwapRolePanel>();
    }

    public void OnClicked()
    {
        if(SwapRolePanel == null)
            SwapRolePanel = GetComponentInParent<SwapRolePanel>();

        SwapRolePanel.SelectRole(RoleData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(RoleData)
            GrimoireManager.Instance.SetHoverTarget(RoleData.RoleTokenSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GrimoireManager.Instance.SetHoverTarget(null);
    }
}
