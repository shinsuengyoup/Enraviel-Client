using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Dataformat;

public class BattleMgr : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Grid grid;  // Tilemap이 속한 Grid

    // 타일 정보 저장 (좌표 -> TileOption)
    private Dictionary<Vector3Int, TileOption> tileDataMap = new();

    void Start()
    {
        InitializeTiles();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }


    void InitializeTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPos))
            {
                var tileOption = new TileOption
                {
                    tileType = TileType.None,
                    isExplored = false,
                    spawnId = -1,
                    col = Color.white
                };
                tileDataMap[cellPos] = tileOption;
            }
        }
    }

    void HandleTileClick()
    {
        // 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일 좌표로 변환
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        // 타일 정보 확인
        TileBase tileBase = tilemap.GetTile(cellPos);
        if (tileBase == null) return;

        Debug.Log(tileDataMap.TryGetValue(cellPos, out var tileOption2));
        // 저장된 데이터 조회
        if (tileDataMap.TryGetValue(cellPos, out var tileOption))
        {
            Debug.Log($"타일 타입: {tileOption.tileType}");
            Debug.Log($"이동가능: {tileOption.tileType != TileType.Wall}");
        }
    }

    // 타일 속성 변경
    public void ChangeTileType(Vector3Int cellPos, TileType newType)
    {
        if (tileDataMap.TryGetValue(cellPos, out var tileOption))
        {
            tileOption.tileType = newType;

            // 시각적 변경 (색상 등)
            tilemap.SetColor(cellPos, tileOption.col);
        }
    }
}
