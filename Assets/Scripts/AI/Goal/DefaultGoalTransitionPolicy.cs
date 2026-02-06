using System.Collections.Generic;

/// <summary>
/// Goal 간 전이 가능 여부를 정의하는 기본 전이 정책 구현체
/// </summary>
public class DefaultGoalTransitionPolicy : IAIGoalTransitionPolicy
{
    private static readonly Dictionary<EAIGoalType, HashSet<EAIGoalType>> transitions = new()
    {
        {
            EAIGoalType.ApplyPressure, new HashSet<EAIGoalType> { EAIGoalType.ForceMistake, EAIGoalType.TrapPlayer }
        },
        {
            EAIGoalType.ForceMistake, new HashSet<EAIGoalType> { EAIGoalType.KillNow }        
        },
        {
            EAIGoalType.TrapPlayer, new HashSet<EAIGoalType> { EAIGoalType.KillNow }
        },
        {
            EAIGoalType.KillNow, new HashSet<EAIGoalType>()
        }
    };

    bool IAIGoalTransitionPolicy.CanTransition(EAIGoalType from, EAIGoalType to)
    {
        return transitions.TryGetValue(from, out HashSet<EAIGoalType> hashSet) && hashSet.Contains(to);
    }
}
