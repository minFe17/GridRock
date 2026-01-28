using System.Collections.Generic;

/// <summary>
/// AI의 상위 목적(EAIGoalType)에 따라
/// 실행이 허용되는 행동 유형(EAIActionTag)을 정의하는 정책 클래스
/// </summary>
public static class AIGoalActionPolicy
{
    /// 각 AI 목적별로 허용되는 행동 태그 집합
    /// Goal → ActionTag 매핑 테이블 역할
    static readonly Dictionary<EAIGoalType, HashSet<EAIActionTag>> allowedActions = new()
    {
        {
            // 즉시 사망 유도 목적일 때
            EAIGoalType.KillNow,
            new HashSet<EAIActionTag>
            {
                EAIActionTag.InstantKill // 즉각적인 사망 트리거만 허용
            }
        },
        {
            // 플레이어를 가두는 목적일 때
            EAIGoalType.TrapPlayer,
            new HashSet<EAIActionTag>
            {
                EAIActionTag.BlockEscape,   // 탈출 경로 차단
                EAIActionTag.CreateDanger   // 위험 지형 생성
            }
        },
        {
            // 실수를 유도하는 목적일 때
            EAIGoalType.ForceMistake,
            new HashSet<EAIActionTag>
            {
                EAIActionTag.CreateDanger,   // 판단을 흐리는 위험 생성
                EAIActionTag.ApplyPressure  // 선택을 강요하는 압박
            }
        },
        {
            // 지속적인 압박 유지 목적일 때
            EAIGoalType.ApplyPressure,
            new HashSet<EAIActionTag>
            {
                EAIActionTag.ApplyPressure  // 공간/시간 압박 행동만 허용
            }
        }
    };

    /// 현재 AI 목적에서 특정 행동 태그가 실행 가능한지 검사
    /// Action 선택 단계에서 필터링 용도로 사용
    public static bool IsAllowed(EAIGoalType goal, EAIActionTag tag)
    {
        return allowedActions.TryGetValue(goal, out var set) && set.Contains(tag);
    }
}