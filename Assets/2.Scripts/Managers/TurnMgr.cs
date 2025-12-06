using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMgr : MonoBehaviour
{
    public static TurnMgr Instance { get; private set; }
    private List<int> TurnOrder;
    private int currentTurnIndex = -1;

    public void Awake()
    {
        Instance = this;
        TurnOrder = new List<int>();
    }

    public void SetCreature(int num)
    {
        TurnOrder.Add(num);
    }

    public void SortTurnOrder()
    {
        // 아직 정렬 규칙없으니 SetCreature가 된 순서로 턴을 정함
    }

    /// <summary>
    /// 게임 시작 시 첫 번째 턴 시작
    /// </summary>
    public void StartBattle()
    {
        if (TurnOrder.Count == 0)
        {
            Debug.LogError("턴 순서가 설정되지 않았습니다.");
            return;
        }

        currentTurnIndex = 0;
        StartCurrentTurn();
    }

    /// <summary>
    /// 현재 턴의 캐릭터 턴 시작
    /// </summary>
    private void StartCurrentTurn()
    {
        int characterNumber = TurnOrder[currentTurnIndex];
        CreatureBase character = BattleMgr.Instance.GetCreature(characterNumber);

        if (character != null)
        {
            Debug.Log($"[턴 {currentTurnIndex + 1}] 캐릭터 #{characterNumber} 턴 시작");
            // 이동 범위 표시
            BattleMgr.SCalcRangeTile(3, character.curCellPos);
            // 턴 시작 콜백 호출 (AI 등에서 자동 행동)
            character.OnTurnStart();
        }
        else
        {
            Debug.LogError($"캐릭터 #{characterNumber}를 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 현재 턴 종료 및 다음 턴으로 진행
    /// </summary>
    public void EndTurn()
    {
        // 현재 턴의 캐릭터
        int currentCharNumber = TurnOrder[currentTurnIndex];
        Debug.Log($"캐릭터 #{currentCharNumber} 턴 종료");

        // 다음 턴으로 이동
        currentTurnIndex++;

        // 모든 캐릭터의 턴이 끝났으면 다시 처음부터
        if (currentTurnIndex >= TurnOrder.Count)
        {
            currentTurnIndex = 0;
            Debug.Log("=== 라운드 종료, 다시 시작 ===");
        }

        // 다음 캐릭터의 턴 시작
        StartCurrentTurn();
    }

    /// <summary>
    /// 현재 턴을 진행 중인 캐릭터 번호 반환
    /// </summary>
    public int GetCurrentCharacterNumber()
    {
        if (currentTurnIndex < 0 || currentTurnIndex >= TurnOrder.Count)
            return -1;
        return TurnOrder[currentTurnIndex];
    }

    /// <summary>
    /// 특정 캐릭터 번호가 현재 턴인지 확인
    /// </summary>
    public bool IsMyTurn(int charNumber)
    {
        return GetCurrentCharacterNumber() == charNumber;
    }


}
