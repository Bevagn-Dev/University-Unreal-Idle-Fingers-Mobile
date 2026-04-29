using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeValues", menuName = "UpgradeValues")]
public class UpgradeData : ScriptableObject
{
    public enum CurrencyType { HoldCurrency, TapCurrency }
    public CurrencyType costCurrency;
    
    public float baseCost = 10f;
    public float costGrowth = 1.2f;
    
    public enum UpgradeTarget {  }
    public UpgradeTarget targetStat;
    public float valueIncreasePerLevel = 0.5f;
}
