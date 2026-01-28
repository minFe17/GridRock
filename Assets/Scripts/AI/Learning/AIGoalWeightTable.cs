using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 목적 선택 가중치 테이블
/// 학습에 의해 값이 점진적으로 조정
/// </summary>
public class AIGoalWeightTable
{
    readonly Dictionary<EAIGoalType, float> _weights = new();

    public AIGoalWeightTable()
    {
        foreach (EAIGoalType goal in EAIGoalType.GetValues(typeof(EAIGoalType)))
            _weights[goal] = 1.0f;
    }

    public float GetWeights(EAIGoalType goal) => _weights[goal];

    public void Adjust(EAIGoalType goal, float delta)
    {
        _weights[goal] = Mathf.Clamp(_weights[goal] + delta, 0.2f, 3.0f);
    }
}