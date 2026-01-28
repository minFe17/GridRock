using System.Collections.Generic;

/// <summary>
/// 현재 시뮬레이션 상태를 기준으로 실행 가능한 AI 행동을 하나 선택하여 수행하는 실행자
/// </summary>
public class AIActionRunner
{
    // 행동 목록을 순회하며 실행 조건을 만족하는 첫 행동을 실행
    public void Run(IReadOnlyList<IAIAction> actions, in AISimulationState sim, in AIActionContext context)
    {
        foreach (IAIAction action in actions)
        {
            // 현재 상태에서 실행 가능한 행동인지 검사
            if (action.CanExecute(sim))
            {
                // 조건을 만족한 첫 행동만 실행
                action.Execute(context);
                break;
            }
        }
    }
}