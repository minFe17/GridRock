using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 행동 패턴 우선순위 테이블
/// 성공률 기반으로 가중치가 변함
/// </summary>
public class AIActionPriorityTable
{
    private readonly Dictionary<EAIActionTag, float> _priorities = new();

    public AIActionPriorityTable(IEnumerable<EAIActionTag> tags)
    {
        foreach (var tag in tags)
            _priorities[tag] = 1.0f;
    }

    public float GetPriorities(EAIActionTag tag) => _priorities[tag];

    public void Adjust(EAIActionTag tag, float delta)
    {
        _priorities[tag] = Mathf.Clamp(_priorities[tag] + delta, 0.1f, 5.0f);
    }
}