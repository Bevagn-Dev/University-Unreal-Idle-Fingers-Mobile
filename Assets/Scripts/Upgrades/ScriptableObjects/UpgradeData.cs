using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeCategory { Hold, Press, Auto }
public enum CurrencyType { Hold, Tap }

[CreateAssetMenu(fileName = "UpgradeValues", menuName = "UpgradeValues")]
public class UpgradeData : ScriptableObject
{
public string upgradeID;
    public string displayName;
    public UpgradeCategory category;
    public CurrencyType currencyType;

    public float baseCost;
    public float costMultiplier = 1.15f;
    public int maxLevel = 50;

    public float baseValue;
    public float valueIncrement;
}

