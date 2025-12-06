using UnityEngine;
using System.Collections.Generic;
using Dataformat;

public class Monster : CreatureBase
{
    private const int PLAYER_CHAR_NUMBER = 1;  // 플레이어 캐릭터 번호

    public override void OnTurnStart()
    {
        // 자신의 턴이 아니면 무시
        if (!TurnMgr.Instance.IsMyTurn(charNumber))
            return;

        // AI 동작 실행
        ExecuteAIAction();
    }

    /// <summary>
    /// 3가지 AI 동작 중 하나를 랜덤하게 실행
    /// </summary>
    private void ExecuteAIAction()
    {
        int action = Random.Range(0, 3);

        switch (action)
        {
            case 0:
                Debug.Log($"[AI #{charNumber}] 동작 1: 주변 3칸 랜덤 이동");
                MoveRandomAround();
                break;
            case 1:
                Debug.Log($"[AI #{charNumber}] 동작 2: 플레이어에게 접근");
                MoveTowardPlayer();
                break;
            case 2:
                Debug.Log($"[AI #{charNumber}] 동작 3: 플레이어에게서 도망");
                MoveAwayFromPlayer();
                break;
        }
    }

    /// <summary>
    /// 동작 1: 주변 3칸 범위 내에서 랜덤하게 이동
    /// </summary>
    private void MoveRandomAround()
    {
        List<Vector3Int> validMoves = new();

        // 현재 위치에서 3칸 범위의 모든 타일 확인
        for (int x = curCellPos.x - 3; x <= curCellPos.x + 3; x++)
        {
            for (int y = curCellPos.y - 3; y <= curCellPos.y + 3; y++)
            {
                Vector3Int targetPos = new(x, y, 0);

                // 현재 위치는 제외
                if (targetPos == curCellPos)
                    continue;

                // 이동 가능한 타일인지 확인 (벽, 존재하지 않는 타일 제외)
                if (BattleMgr.Instance.IsTileExists(targetPos) &&
                    BattleMgr.Instance.GetTileType(targetPos) != TileType.Wall)
                {
                    validMoves.Add(targetPos);
                }
            }
        }

        if (validMoves.Count > 0)
        {
            Vector3Int randomTarget = validMoves[Random.Range(0, validMoves.Count)];
            MoveToCell(randomTarget);
        }
        else
        {
            Debug.LogWarning($"[AI #{charNumber}] 이동할 수 있는 타일이 없습니다.");
            TurnMgr.Instance.EndTurn();
        }
    }

    /// <summary>
    /// 동작 2: 플레이어에게 가장 가까운 경로로 3칸 이동
    /// </summary>
    private void MoveTowardPlayer()
    {
        CreatureBase player = BattleMgr.Instance.GetCreature(PLAYER_CHAR_NUMBER);
        if (player == null)
        {
            Debug.LogWarning($"[AI #{charNumber}] 플레이어를 찾을 수 없습니다.");
            TurnMgr.Instance.EndTurn();
            return;
        }

        Vector3Int playerPos = player.curCellPos;
        Vector3Int targetPos = FindNearestMoveTowardTarget(playerPos);

        if (targetPos != curCellPos)
        {
            MoveToCell(targetPos);
        }
        else
        {
            Debug.LogWarning($"[AI #{charNumber}] 플레이어로 이동할 수 없습니다.");
            TurnMgr.Instance.EndTurn();
        }
    }

    /// <summary>
    /// 동작 3: 플레이어에게서 멀어지는 경로로 3칸 이동
    /// </summary>
    private void MoveAwayFromPlayer()
    {
        CreatureBase player = BattleMgr.Instance.GetCreature(PLAYER_CHAR_NUMBER);
        if (player == null)
        {
            Debug.LogWarning($"[AI #{charNumber}] 플레이어를 찾을 수 없습니다.");
            TurnMgr.Instance.EndTurn();
            return;
        }

        Vector3Int playerPos = player.curCellPos;
        Vector3Int targetPos = FindFarthestMoveAwayFromTarget(playerPos);

        if (targetPos != curCellPos)
        {
            MoveToCell(targetPos);
        }
        else
        {
            Debug.LogWarning($"[AI #{charNumber}] 도망칠 수 없습니다.");
            TurnMgr.Instance.EndTurn();
        }
    }

    /// <summary>
    /// 3칸 범위 내에서 목표에 가장 가까운 위치 찾기
    /// </summary>
    private Vector3Int FindNearestMoveTowardTarget(Vector3Int targetPos)
    {
        Vector3Int bestMove = curCellPos;
        float nearestDistance = Vector3Int.Distance(curCellPos, targetPos);

        // 현재 위치에서 3칸 범위의 모든 타일 확인
        for (int x = curCellPos.x - 3; x <= curCellPos.x + 3; x++)
        {
            for (int y = curCellPos.y - 3; y <= curCellPos.y + 3; y++)
            {
                Vector3Int checkPos = new(x, y, 0);

                // 현재 위치는 제외
                if (checkPos == curCellPos)
                    continue;

                // 이동 가능한 타일인지 확인
                if (!BattleMgr.Instance.IsTileExists(checkPos) ||
                    BattleMgr.Instance.GetTileType(checkPos) == TileType.Wall)
                    continue;

                // 목표까지의 거리 계산
                float distance = Vector3Int.Distance(checkPos, targetPos);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    bestMove = checkPos;
                }
            }
        }

        return bestMove;
    }

    /// <summary>
    /// 3칸 범위 내에서 목표에서 가장 먼 위치 찾기
    /// </summary>
    private Vector3Int FindFarthestMoveAwayFromTarget(Vector3Int targetPos)
    {
        Vector3Int bestMove = curCellPos;
        float farthestDistance = Vector3Int.Distance(curCellPos, targetPos);

        // 현재 위치에서 3칸 범위의 모든 타일 확인
        for (int x = curCellPos.x - 3; x <= curCellPos.x + 3; x++)
        {
            for (int y = curCellPos.y - 3; y <= curCellPos.y + 3; y++)
            {
                Vector3Int checkPos = new(x, y, 0);

                // 현재 위치는 제외
                if (checkPos == curCellPos)
                    continue;

                // 이동 가능한 타일인지 확인
                if (!BattleMgr.Instance.IsTileExists(checkPos) ||
                    BattleMgr.Instance.GetTileType(checkPos) == TileType.Wall)
                    continue;

                // 목표까지의 거리 계산
                float distance = Vector3Int.Distance(checkPos, targetPos);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    bestMove = checkPos;
                }
            }
        }

        return bestMove;
    }
}
