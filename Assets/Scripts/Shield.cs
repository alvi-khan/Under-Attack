using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] int shieldAmount = 50;
    [SerializeField] int cost = 50;
    public int ShieldAmount => shieldAmount;
    public int Cost => cost;
}
