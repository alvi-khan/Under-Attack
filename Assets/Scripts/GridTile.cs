using UnityEngine;

public class GridTile : MonoBehaviour
{
    private bool _occupied;

    public bool Occupied
    {
        get => _occupied;
        set => _occupied = value;
    }
}
