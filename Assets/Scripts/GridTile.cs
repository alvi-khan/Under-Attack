using UnityEngine;

/// <summary>
/// Class to hold data about each grid tile.
/// </summary>
public class GridTile : MonoBehaviour
{
    public bool Occupied { get; set; }

    public Turret Turret { get; set; }  // the turret occupying this grid (if any)
}
