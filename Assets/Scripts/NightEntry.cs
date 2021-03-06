﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NightEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RoleData RoleData;
    public GameObject TakeScreenshotButton;

    public void SetRoleData(RoleData roleData)
    {
        RoleData = roleData;
        if (RoleData && (RoleData.RoleName == "Spy" || RoleData.RoleName == "Widow"))
            TakeScreenshotButton.SetActive(true);
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
