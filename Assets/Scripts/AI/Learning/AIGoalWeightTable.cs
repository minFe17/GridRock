using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI 목적 선택 가중치 테이블
/// 학습에 의해 값이 점진적으로 조정
/// </summary>
public class AIGoalWeightTable
{
    public static AIGoalWeightTable Shared { get; } = new AIGoalWeightTable();

    readonly Dictionary<EAIGoalType, float> _weights = new();

    public AIGoalWeightTable()
    {
        foreach (EAIGoalType goal in Enum.GetValues(typeof(EAIGoalType)))
        {
            if (goal == EAIGoalType.None || goal == EAIGoalType.Max)
                continue;

            _weights[goal] = 1f;
        }
    }

    public float GetWeights(EAIGoalType goal) => _weights[goal];

    public void Adjust(EAIGoalType goal, float delta)
    {
        _weights[goal] = Mathf.Clamp(_weights[goal] + delta, 0.2f, 3f);
    }

    public void Decay(float rate = 0.01f)
    {
        List<EAIGoalType> keys = new List<EAIGoalType>(_weights.Keys);

        foreach (EAIGoalType key in keys)
            _weights[key] = Mathf.Lerp(_weights[key], 1f, rate);
    }

    public void Normalize()
    {
        if (_weights.Count == 0)
            return;

        float sum = 0f;
        foreach (float value in _weights.Values)
            sum += value;

        float avg = sum / _weights.Count;
        if (avg <= 0f)
            return;

        List<EAIGoalType> keys = new List<EAIGoalType>(_weights.Keys);
        foreach (EAIGoalType key in keys)
            _weights[key] = Mathf.Clamp(_weights[key] / avg, 0.2f, 3f);
    }
}