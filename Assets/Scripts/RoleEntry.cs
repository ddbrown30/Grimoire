using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoleEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RoleData RoleData;
    public Toggle UseRoleToggle;

    RoleSelectionManager RoleSelectionManager;

    public void Start()
    {
        RoleSelectionManager = GetComponentInParent<RoleSelectionManager>();
    }

    public void ToggleUseRole(bool useRole)
    {
        if(RoleSelectionManager == null)
            RoleSelectionManager = GetComponentInParent<RoleSelectionManager>();

        RoleSelectionManager.RoleEntryToggled(RoleData, useRole);
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
