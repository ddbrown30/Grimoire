 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetWindowResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
    }
}
