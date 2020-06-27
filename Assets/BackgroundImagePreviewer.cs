using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImagePreviewer : MonoBehaviour
{
    public void PreviewImage(Image dropdownImage)
    {
        GrimoireManager.Instance.SetBackgroundImage(dropdownImage.sprite);
    }
}
