using System.Collections.Generic;

public class AIActionSelector
{
    public IAIAction Select(IReadOnlyList<IAIAction> actions, EAIGoalType goal, in AISimulationState sim, in AIInterferenceTriggerState trigger)
    {
        bool allowInterfere = AIInterferencePolicy.CanInterfere(goal, trigger);

        foreach (IAIAction action in actions)
        {
            // 목적 기반 Action 허용 여부
            if (!AIGoalActionPolicy.IsAllowed(goal, action.ActionTag))
                continue;

            // 방해 Action인데 지금 타이밍이 아님
            if (!allowInterfere && action.ActionTag != EAIActionTagType.ApplyPressure)
                continue;

            if (action.CanExecute(sim))
                return action;
        }

        return null;
    }
}