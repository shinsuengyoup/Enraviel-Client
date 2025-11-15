using UnityEngine;
using Dataformat;

[CreateAssetMenu(fileName = "TileOption", menuName = "Scriptable Objects/TileOption")]
public class TileOption : ScriptableObject
{
    public TileType tileType;
    public bool isExplored;
    public int spawnId;
    public Color col;
        
}
