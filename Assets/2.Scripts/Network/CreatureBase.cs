using Dataformat;
using Unity.Netcode;
using UnityEngine;

public class CreatureBase : NetworkBehaviour
{
    public Vector3Int curCellPos;
    [SerializeField] Transform trfChar;
    private DefaultStat charStat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charStat = new DefaultStat();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveTile()
    {

    }

    public void SetStartPos(Vector3Int vectorint, Vector3 pos)
    {
        curCellPos = vectorint;
        trfChar.position = pos + ConstData.tilePosOffset;
    }
}
