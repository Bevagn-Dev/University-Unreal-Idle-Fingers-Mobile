using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonValues", menuName = "ButtonValues")]
public class ButtonValues : ScriptableObject 
{
    public enum ButtonType { Tap, MultiTap, Slider}
    public ButtonType type;

    [Space(10)]

    public float baseTapRewardAmount = 5f;
    public float lifeTimeSeconds = 2f;

    public Color color = Color.white;

}


