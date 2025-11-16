using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Dataformat;

public class BattleMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BattleMgr Instance { get; private set; }

    [SerializeField] private Grid grid;  // Tilemap이 속한 Grid
    private Tilemap tilemap;
    [SerializeField] private GridRenderer gridRenderer;

    // 타일 정보 저장 (좌표 -> TileOption)
    private Dictionary<Vector3Int, TileOption> tileDataMap = new();

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("BattleMgr 싱글톤이 이미 존재합니다. 중복 인스턴스를 제거합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        tilemap = grid.transform.Find("Tiles").GetComponent<Tilemap>();
        InitializeTiles();
    }

    void InitializeTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        tilemap.color = Color.clear;


        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(cellPos);


            if (!tileDataMap.ContainsKey(cellPos))
                tileDataMap.Add(cellPos, new TileOption());

            if (tile != null)
            {
                // 타일 이름으로 타입 결정
                SetTileOption(tile.name, tileDataMap[cellPos]);
            }

        }

        // 타일맵 왼쪽 아래 시작점 좌표 계산
        Vector3Int bottomLeftCell = new(bounds.xMin, bounds.yMin, 0);
        Vector3 vec = grid.GetCellCenterWorld(bottomLeftCell) - tilemap.layoutGrid.cellSize * 0.5f + ConstData.gridMapOffset;
        gridRenderer.SetGridOption(vec, bounds.xMax - bounds.xMin);
    }







    #region TileEvent
    public void HandleTileClick()
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
    #endregion TileEvent

    #region TileOption
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

    /// <summary>
    /// 타일 이름에 따라 TileOption 설정
    /// </summary>
    public void SetTileOption(string tileName, TileOption tileOption)
    {
        switch (tileName)
        {
            case var t when t == FormatString.tileWall:  // "block_tile"
                tileOption.SetTileOption(TileType.Wall);
                break;
            case var t when t == FormatString.tileSlow:  // "Slow"
                tileOption.SetTileOption(TileType.Slow);
                break;
            case var t when t == FormatString.tileTick:  // "Tick"
                tileOption.SetTileOption(TileType.Tick);
                break;
            default:
                tileOption.SetTileOption(TileType.None);
                break;
        }
    }
    #endregion TileOption










    #region Editor
    // 에디터에서 그리드 라인 표시
    private void OnDrawGizmos()
    {
        // if (tilemap == null) return;

        // BoundsInt bounds = tilemap.cellBounds;
        // Vector3 cellSize = tilemap.layoutGrid.cellSize;

        // Gizmos.color = Color.green;

        // // 수직선 그리기 (x축)
        // for (int x = bounds.xMin + 1; x <= bounds.xMax + 1; x++)
        // {
        //     Vector3 start = grid.GetCellCenterWorld();
        //     Vector3 end = grid.GetCellCenterWorld(new Vector3Int(x, bounds.yMax, 0));
        //     start -= cellSize * 0.5f + ConstData.gridMapOffset;
        //     end -= cellSize * 0.5f + ConstData.gridMapOffset;
        //     Gizmos.DrawLine(start, end);
        // }

        // // 수평선 그리기 (y축)
        // for (int y = bounds.yMin + 1; y <= bounds.yMax; y++)
        // {
        //     Vector3 start = grid.GetCellCenterWorld(new Vector3Int(bounds.xMin + 1, y, 0));
        //     Vector3 end = grid.GetCellCenterWorld(new Vector3Int(bounds.xMax + 1, y, 0));
        //     start -= cellSize * 0.5f + ConstData.gridMapOffset;
        //     end -= cellSize * 0.5f + ConstData.gridMapOffset;
        //     Gizmos.DrawLine(start, end);
        // }
    }
    #endregion Editor
}
