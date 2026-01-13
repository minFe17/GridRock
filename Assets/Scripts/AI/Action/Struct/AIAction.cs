using UnityEngine;

public readonly struct AIAction
{
    public readonly EAIActionType Type;      // 어떤 행동인가
    public readonly Vector2Int Target;       // 대상 좌표 (그리드 기준)
    public readonly int Priority;            // 선택 우선도

    public AIAction(EAIActionType type, Vector2Int target, int priority)
    {
        Type = type;
        Target = target;
        Priority = priority;
    }
}