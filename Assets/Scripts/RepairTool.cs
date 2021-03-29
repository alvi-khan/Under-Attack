using UnityEngine;

public class RepairTool : MonoBehaviour
{
    [SerializeField] int healthIncrease = 50;
    [SerializeField] int cost = 20;
    public int HealthIncrease => healthIncrease;
    public int Cost => cost;
}
