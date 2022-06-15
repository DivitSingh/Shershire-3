using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Heart, Crystal, Sherling, Trinket, Equipment, Skill
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item", order = 1)]
public class Item : ScriptableObject
{
    public GameObject Sprite;
    public Type Type;
    [TextArea(15, 20)]
    public string Description;
    public int Value;

    #region Pickup Variables
    public int Buff;
    #endregion

    #region Equipment Variables
    public int HP;
    public int Damage;
    public int Resistance;
    public int Strength;
    public int Recovery;
    public int Speed;
    #endregion

    #region Weapon Variables
    public Vector3 originChest;
    public Vector3 originWrist;
    #endregion
}