using UnityEngine;
using Dataformat;
using JetBrains.Annotations;

[CreateAssetMenu(fileName = "TileOption", menuName = "Scriptable Objects/TileOption")]
public class TileOption : ScriptableObject
{
    public TileType tileType;
    public bool isExplored;
    public int spawnId;
    public Color col;


    public TileOption()
    {
        tileType = TileType.None;
        isExplored = false;
        spawnId = -1;
        col = Color.white;
    }

    public TileOption(TileType tileType, int spawnId = -1)
    {
        this.tileType = tileType;
        this.isExplored = false;
        this.spawnId = spawnId;
        col = Color.white;
    }

    public void SetTileOption(TileType tileType, int spawnId = -1)
    {
        this.tileType = tileType;
        this.isExplored = false;
        this.spawnId = spawnId;
        col = Color.clear;
    }
}
