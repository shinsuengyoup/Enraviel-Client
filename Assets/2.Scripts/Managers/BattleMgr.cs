using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using Dataformat;
using TMPro;

public class BattleMgr : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BattleMgr Instance { get; private set; }

    [SerializeField] private Grid grid;  // Tilemap이 속한 Grid
    private Tilemap tilemap;
    [SerializeField] private CreatureBase[] players;
    [SerializeField] private CreatureBase[] Monsters;

    private Dictionary<int, CreatureBase> characters;
    [SerializeField] RectTransform rtrfTileMenu;


    // 인게임 격자 무늬
    [SerializeField] private GridRenderer gridRenderer;
    private GameObject goGridRenderer;

    // 타일 정보 저장 (좌표 -> TileOption)
    private Dictionary<Vector3Int, TileOption> tileDataMap = new();

    private Vector3Int selectedCell;




    private int iSpawnIdx;
    private int iSpawnEnemyIdx;

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
        goGridRenderer = gridRenderer.gameObject;
        iSpawnIdx = 0;
        iSpawnEnemyIdx = 0;
        InitializeTiles();

        characters = new Dictionary<int, CreatureBase>();

        int idx = ConstData.CharNumberStart;
        foreach (var character in players)
        {
            characters.Add(idx, character);
            character.charNumber = idx;
            idx++;
            TurnMgr.Instance.SetCreature(character.charNumber);
        }

        idx = ConstData.MonNumberStart;
        foreach (var Monster in Monsters)
        {
            characters.Add(idx, Monster);
            Monster.charNumber = idx;
            idx++;
            TurnMgr.Instance.SetCreature(Monster.charNumber);
        }

        // 게임 시작 - 첫 번째 턴 시작
        TurnMgr.Instance.StartBattle();
    }

    void InitializeTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(cellPos);

            if (!tileDataMap.ContainsKey(cellPos))
                tileDataMap.Add(cellPos, new TileOption());

            if (tile != null)
            {
                // 타일 이름으로 타입 결정
                SetTileOption(tile.name, tileDataMap[cellPos], cellPos);
            }

        }

        // 타일맵 왼쪽 아래 시작점 좌표 계산
        Vector3Int bottomLeftCell = new(bounds.xMin, bounds.yMin, 0);
        Vector3 vec = grid.GetCellCenterWorld(bottomLeftCell) - tilemap.layoutGrid.cellSize * 0.5f + ConstData.gridMapOffset;
        gridRenderer.SetGridOption(vec, bounds.xMax - bounds.xMin);
    }

    #region Buttons
    public void OnBtnMove()
    {
        players[0].MoveToCell(selectedCell);
    }
    #endregion



    #region BattleInfo
    public CreatureBase GetCreature(int charnumber)
    {
        if (characters.ContainsKey(charnumber))
            return characters[charnumber];
        else
            return null;
    }
    #endregion

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

        // 저장된 데이터 조회
        if (tileDataMap.TryGetValue(cellPos, out var tileOption))
        {
            Debug.Log($"타일 타입: {tileOption.tileType}");
            Debug.Log($"이동가능: {tileOption.tileType != TileType.Wall}");

            // UI 메뉴 위치 지정
            PositionTileMenu(cellPos);
            selectedCell = cellPos;
        }
    }

    public void OnBtnActiveTileMap()
    {
        bool bActive = !goGridRenderer.activeSelf;
        goGridRenderer.SetActive(bActive);
    }

    /// <summary>
    /// 타일 메뉴 UI를 클릭한 셀 주변에 위치시킴
    /// </summary>
    private void PositionTileMenu(Vector3Int cellPos)
    {
        if (rtrfTileMenu == null)
        {
            Debug.LogWarning("rtrfTileMenu가 설정되지 않았습니다.");
            return;
        }

        // 타일의 월드 좌표 계산
        Vector3 tileWorldPos = tilemap.GetCellCenterWorld(cellPos);
        // Canvas를 찾기
        Canvas canvas = rtrfTileMenu.GetComponentInParent<Canvas>();
        // MainCamera를 사용하여 스크린 좌표로 변환 (게임 화면 기준)
        Vector2 screenPos = Camera.main.WorldToScreenPoint(tileWorldPos);
        // Canvas의 RenderMode에 따라 카메라 결정
        Camera uiCamera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        // 스크린 좌표를 UI 캔버스의 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rtrfTileMenu.parent as RectTransform,
            screenPos,
            uiCamera,
            out Vector2 localPoint
        );

        rtrfTileMenu.anchoredPosition = localPoint + (Vector2.up * 20);
    }

    #endregion TileEvent

    #region Tile Access
    /// <summary>
    /// 지정된 좌표의 타일 정보 조회
    /// </summary>
    public TileOption GetTileOption(Vector3Int cellPos)
    {
        if (tileDataMap.TryGetValue(cellPos, out var tileOption))
        {
            return tileOption;
        }
        return null;
    }

    /// <summary>
    /// 지정된 좌표에 타일이 존재하는지 확인
    /// </summary>
    public bool IsTileExists(Vector3Int cellPos)
    {
        return tilemap.GetTile(cellPos) != null;
    }

    /// <summary>
    /// 지정된 좌표의 타일 타입 조회
    /// </summary>
    public TileType GetTileType(Vector3Int cellPos)
    {
        if (tileDataMap.TryGetValue(cellPos, out var tileOption))
        {
            return tileOption.tileType;
        }
        return TileType.None;
    }

    /// <summary>
    /// Tilemap 참조 반환
    /// </summary>
    public Tilemap GetTilemap()
    {
        return tilemap;
    }
    #endregion Tile Access

    #region Tile calc
    public static void SCalcRangeTile(int range, Vector3Int curPos)
    {
        if (range <= 0)
            return;
        if (curPos == null)
            return;

        Instance.calcRangeTile(range, curPos);
    }

    private void calcRangeTile(int range, Vector3Int curPos)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase curTile = tilemap.GetTile(curPos);
        if (curTile == null)
        {
            Debug.LogError("현재 타일은 존재하지 않는 타일입니다.");
            return;
        }

        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            if (Vector3Int.Distance(curPos, cellPos) <= range)
            {
                tilemap.SetColor(cellPos, ConstData.CLR_RED_tile);
            }
            else
            {
                tilemap.SetColor(cellPos, Color.clear);
            }
        }
    }

    public static void SResetRangeTile()
    {
        BoundsInt bounds = Instance.tilemap.cellBounds;
        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            Instance.tilemap.SetColor(cellPos, Color.clear);
        }
    }

    public static Vector3 SConvertTilePos(Vector3Int cellpos)
    {
        return Instance.ConvertTilePos(cellpos);
    }
    private Vector3 ConvertTilePos(Vector3Int cellpos)
    {
        // 월드 포지션 계산
        Vector3 worldPosition = tilemap.CellToWorld(cellpos);

        return worldPosition;
    }

    #endregion


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
    public void SetTileOption(string tileName, TileOption tileOption, Vector3Int cellPos)
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
            case var t when t == FormatString.tileSpawn:
                tileOption.SetTileOption(TileType.Spawn, iSpawnIdx);
                if (players.Length > iSpawnIdx)
                {
                    players[iSpawnIdx].SetStartPos(cellPos, ConvertTilePos(cellPos));
                }
                iSpawnIdx++;
                break;
            case var t when t == FormatString.tileSpawnEnemy:
                tileOption.SetTileOption(TileType.SpawnEnemy, iSpawnEnemyIdx);
                iSpawnEnemyIdx++;
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
