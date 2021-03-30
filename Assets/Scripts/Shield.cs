using UnityEngine;

/// <summary>
/// Holds data about the shield.
/// </summary>
public class Shield : MonoBehaviour
{
    [SerializeField] int shieldAmount = 50;
    [SerializeField] int cost = 50;
    public int ShieldAmount => shieldAmount;
    public int Cost => cost;
}
