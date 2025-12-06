using Dataformat;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class CreatureBase : NetworkBehaviour
{
    public Vector3Int curCellPos;
    [SerializeField] Transform trfChar;
    private DefaultStat charStat;
    public int charNumber;

    // 이동 관련 변수
    private readonly Queue<Vector3Int> movementQueue = new();
    private bool isMoving = false;
    [SerializeField] private float moveSpeed = 5f;  // 셀 이동 시간 (초)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        charStat = new DefaultStat();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && movementQueue.Count > 0)
        {
            MoveTile();
        }
    }

    /// <summary>
    /// 목표 셀로 이동 (최단 경로 자동 계산)
    /// </summary>
    public void MoveToCell(Vector3Int targetCellPos)
    {
        // 이미 이동 중이면 무시
        if (isMoving)
            return;

        // 목표 셀이 현재 위치와 같으면 무시
        if (targetCellPos == curCellPos)
            return;

        // BFS로 최단 경로 계산
        List<Vector3Int> path = FindShortestPath(curCellPos, targetCellPos);
        BattleMgr.SResetRangeTile();

        if (path != null && path.Count > 0)
        {
            // 경로를 큐에 추가
            foreach (Vector3Int cell in path)
            {
                movementQueue.Enqueue(cell);
            }
            isMoving = true;
        }
    }

    /// <summary>
    /// BFS를 이용한 최단 경로 계산
    /// 규칙: 한 칸씩 이동, 대각선 불가, 벽 피하기
    /// </summary>
    private List<Vector3Int> FindShortestPath(Vector3Int startPos, Vector3Int targetPos)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> parentMap = new();
        HashSet<Vector3Int> visited = new();

        queue.Enqueue(startPos);
        visited.Add(startPos);
        parentMap[startPos] = startPos;

        // 인접한 4방향 (상, 하, 좌, 우)
        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up,      // (0, 1, 0)
            Vector3Int.down,    // (0, -1, 0)
            Vector3Int.left,    // (-1, 0, 0)
            Vector3Int.right    // (1, 0, 0)
        };

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            // 목표에 도달했으면 경로 재구성
            if (current == targetPos)
            {
                return ReconstructPath(parentMap, startPos, targetPos);
            }

            // 인접한 셀 탐색
            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = current + direction;

                // 이미 방문했으면 건너뛰기
                if (visited.Contains(neighbor))
                    continue;

                // 벽이거나 존재하지 않는 타일이면 건너뛰기
                if (!IsWalkableTile(neighbor))
                    continue;

                visited.Add(neighbor);
                parentMap[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }

        // 경로를 찾지 못함
        Debug.LogWarning($"목표 셀({targetPos})로 가는 경로를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// BFS 경로를 역추적하여 경로 리스트 생성
    /// </summary>
    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> parentMap, Vector3Int startPos, Vector3Int targetPos)
    {
        List<Vector3Int> path = new();
        Vector3Int current = targetPos;

        while (current != startPos)
        {
            path.Add(current);
            current = parentMap[current];
        }

        path.Reverse();  // 시작점에서 목표점으로 가는 순서로 변경
        return path;
    }

    /// <summary>
    /// 타일이 이동 가능한지 확인
    /// </summary>
    private bool IsWalkableTile(Vector3Int cellPos)
    {
        // 타일이 존재하지 않으면 이동 불가
        if (!BattleMgr.Instance.IsTileExists(cellPos))
            return false;

        // 타일 타입이 Wall이면 이동 불가
        TileType tileType = BattleMgr.Instance.GetTileType(cellPos);
        if (tileType == TileType.Wall)
            return false;

        return true;
    }

    /// <summary>
    /// 한 칸씩 이동 처리
    /// </summary>
    private void MoveTile()
    {
        if (movementQueue.Count == 0)
        {
            isMoving = false;
            return;
        }

        Vector3Int nextCell = movementQueue.Peek();
        Vector3 targetWorldPos = BattleMgr.SConvertTilePos(nextCell) + ConstData.tilePosOffset;
        Vector3 currentWorldPos = trfChar.position;

        // 이미 목표에 도달했다면 다음 셀로
        if (Vector3.Distance(currentWorldPos, targetWorldPos) < 0.01f)
        {
            curCellPos = nextCell;
            trfChar.position = targetWorldPos;
            movementQueue.Dequeue();

            // 다음 셀이 있으면 계속, 없으면 이동 완료
            if (movementQueue.Count == 0)
            {
                isMoving = false;
                OnMovementComplete();
            }
        }
        else
        {
            // 목표 위치로 부드럽게 이동
            trfChar.position = Vector3.Lerp(currentWorldPos, targetWorldPos, Time.deltaTime * moveSpeed);
        }
    }

    /// <summary>
    /// 이동 완료 콜백
    /// </summary>
    private void OnMovementComplete()
    {
        Debug.Log($"캐릭터 #{charNumber}가 {curCellPos}에 도착했습니다.");

        // 현재 턴의 캐릭터인지 확인
        if (TurnMgr.Instance.GetCurrentCharacterNumber() == charNumber)
        {
            // 턴 종료 및 다음 턴 진행
            TurnMgr.Instance.EndTurn();
        }
    }

    /// <summary>
    /// 턴 시작 시 호출되는 메서드 (자식 클래스에서 오버라이드 가능)
    /// </summary>
    public virtual void OnTurnStart()
    {
        // 기본 구현은 없음 (자식 클래스에서 필요시 오버라이드)
    }

    public void SetStartPos(Vector3Int vectorint, Vector3 pos)
    {
        curCellPos = vectorint;
        trfChar.position = pos + ConstData.tilePosOffset;
        BattleMgr.SCalcRangeTile(3, curCellPos);
    }
}
