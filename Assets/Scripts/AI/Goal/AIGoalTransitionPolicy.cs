using System.Collections.Generic;

/// <summary>
/// AI 목적 간 전환 가능 여부를 정의하는 정책 클래스
/// </summary>
public static class AIGoalTransitionPolicy
{
    // 각 목적에서 전환 가능한 다음 목적 목록
    private static readonly Dictionary<EAIGoalType, HashSet<EAIGoalType>> transitions = new()
    {
        {
            EAIGoalType.ApplyPressure,
            new HashSet<EAIGoalType>
            {
                EAIGoalType.ForceMistake,
                EAIGoalType.TrapPlayer
            }
        },
        {
            EAIGoalType.ForceMistake,
            new HashSet<EAIGoalType>
            {
                EAIGoalType.KillNow
            }
        },
        {
            EAIGoalType.TrapPlayer,
            new HashSet<EAIGoalType>
            {
                EAIGoalType.KillNow
            }
        },
        {
            EAIGoalType.KillNow,
            new HashSet<EAIGoalType>()
        }
    };

    public static bool CanTransition(EAIGoalType from, EAIGoalType to)
    {
        return transitions.TryGetValue(from, out var set) && set.Contains(to);
    }
}