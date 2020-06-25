﻿using UnityEngine;
using UnityEngine.Events;

public class ModalManager : MonoBehaviour
{

    public GameObject ModalPanelObject;       //Reference to the Panel Object
    public DialogPanel ModalDialogPanel;       //Reference to the DialogPanel
    public SwapRolePanel ModalSwapRolePanel;
    private static ModalManager MainModalManager; //Reference to the Modal Panel, to make sure it's been included

    public static ModalManager Instance()
    {
        if (!MainModalManager)
        {
            MainModalManager = FindObjectOfType(typeof(ModalManager)) as ModalManager;
            if (!MainModalManager)
            {
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }
        }
        return MainModalManager;
    }

    public void MessageBox(string Question, UnityAction YesEvent, UnityAction NoEvent, UnityAction CancelEvent, UnityAction OkEvent, string MessageType)
    {
        ModalPanelObject.SetActive(true);  //Activate the Panel; its default is "off" in the Inspector
        ModalDialogPanel.gameObject.SetActive(true);  //Activate the dialog; its default is "off" in the Inspector
        ModalDialogPanel.MessageBox(Question, YesEvent, NoEvent, CancelEvent, OkEvent, MessageType, ClosePanel);
    }

    public void OpenSwapPanel(GrimoireToken token)
    {
        ModalPanelObject.SetActive(true);  //Activate the Panel; its default is "off" in the Inspector
        ModalSwapRolePanel.gameObject.SetActive(true);  //Activate the dialog; its default is "off" in the Inspector
        ModalSwapRolePanel.OpenSwapPanel(token, ClosePanel);
    }

    void ClosePanel()
    {
        ModalPanelObject.SetActive(false);
        ModalDialogPanel.gameObject.SetActive(false);
        ModalSwapRolePanel.gameObject.SetActive(false);
    }
}
