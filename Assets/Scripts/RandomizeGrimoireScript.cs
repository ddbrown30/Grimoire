using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeGrimoireScript : MonoBehaviour
{
    public Toggle ConfirmToggle;
    public void RandomizeGrimoire()
    {
        if (ConfirmToggle.isOn)
        {
            ConfirmToggle.isOn = false;
            GrimoireManager.Instance.RandomizeGrimoire();
        }
    }
}
