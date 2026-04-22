using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_ButtonValue", menuName = "ButtonValues")]
public class ButtonValues : ScriptableObject 
{
    public enum ButtonType { Tap, Hold}
    public ButtonType type;

    public float baseRewardAmount;
    public float lifeTimeSeconds;

    [Header("Hold Settings (If applicable)")]
    public float requiredHoldTime = 1f;
}


