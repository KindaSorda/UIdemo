using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Status Effect", order = 1)]
public class SO_StatusEffect : ScriptableObject
{
    public string effectName;
    public Sprite thumbnailSprite;
    public bool isBuff;
    public int duration;
    public int stacks;
    [TextArea(0,20)]public string description;

    [Header("Status Variables")]
    public float percentSpeedEffect;
    public float percentAttackEffect;
    public float percentDefenseEffect;
}
