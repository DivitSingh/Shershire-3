using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy System/Minion", order = 1)]
public class Enemy : ScriptableObject
{
    #region Stats
    public string Name;

    public int HP;
    public int Damage;
    public int Strength;
    public int Resistance;
    public int Recovery;
    public int Speed;
    #endregion

    public List<GameObject> itemDrops;

    #region Audio    
    public AudioClip Block;
    public AudioClip Hurt;
    public AudioClip Death;
    #endregion
}
