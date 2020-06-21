using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoleType
{
    Townsfolk,
    Outsider,
    Minion,
    Demon,
    Traveler
}

[CreateAssetMenu(fileName = "RoleData", menuName = "ScriptableObjects/RoleData", order = 1)]
public class RoleData : ScriptableObject
{
    public string RoleName;
    public RoleType RoleType;
    public int FirstNightOrder = -1;
    public int OtherNightOrder = -1;
    public bool IsNightOrderAfterDawn = false;

    public Sprite RoleTokenSprite;
    public Sprite[] HelperTokenSprites;
    
    public bool AddToGrimoire = true;

    public static Color[] RoleColors = { new Color(0, 0.6f, 1), new Color(0, 0.6f, 1), Color.red, Color.red, Color.magenta};
}
