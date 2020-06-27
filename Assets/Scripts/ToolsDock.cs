using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsDock : MonoBehaviour
{
    public GameObject ToolsParent;
    public GameObject OpenButton;
    public GameObject CloseButton;

    public void OpenDock()
    {
        ToolsParent.SetActive(true);
        OpenButton.SetActive(false);
        CloseButton.SetActive(true);
    }

    public void CloseDock()
    {
        ToolsParent.SetActive(false);
        OpenButton.SetActive(true);
        CloseButton.SetActive(false);
    }
}
