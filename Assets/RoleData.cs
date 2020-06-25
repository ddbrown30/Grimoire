using System;
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

public enum GameEdition
{
    None = 0,
    TroubleBrewing = 1,
    BadMoonRising = 2,
    SectsAndViolets = 4,
    Unreleased = 8
}

[CreateAssetMenu(fileName = "RoleData", menuName = "ScriptableObjects/RoleData", order = 1)]
public class RoleData : ScriptableObject
{
    public string RoleName;
    public string ScriptToolId;
    public RoleType RoleType;
    public GameEdition GameEdition;
    public int FirstNightOrder = -1;
    public int OtherNightOrder = -1;
    public bool IsNightOrderAfterDawn = false;

    public Sprite RoleTokenSprite;
    public Sprite[] HelperTokenSprites;
    
    public bool AddToGrimoire = true;

    public static Color[] RoleColors = { new Color(0, 0.6f, 1), new Color(0, 0.6f, 1), Color.red, Color.red, Color.magenta};
}
