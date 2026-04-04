using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goal + ActionTag 단위 가중치 테이블
/// </summary>
public class AIActionWeightTable
{
    public static AIActionWeightTable Shared { get; } = new AIActionWeightTable();

    private Dictionary<(EAIGoalType, EAIActionTagType), float> _weights;

    public AIActionWeightTable()
    {
        _weights = new Dictionary<(EAIGoalType, EAIActionTagType), float>();

        foreach (EAIGoalType goal in Enum.GetValues(typeof(EAIGoalType)))
        {
            if (goal == EAIGoalType.None || goal == EAIGoalType.Max)
                continue;

            foreach (EAIActionTagType tag in Enum.GetValues(typeof(EAIActionTagType)))
            {
                _weights[(goal, tag)] = 1f;
            }
        }
    }

    public float GetWeight(EAIGoalType goal, EAIActionTagType tag)
    {
        return _weights.TryGetValue((goal, tag), out float w) ? w : 1f;
    }

    public void Adjust(EAIGoalType goal, EAIActionTagType tag, float delta)
    {
        (EAIGoalType goal, EAIActionTagType tag) key = (goal, tag);

        if (!_weights.ContainsKey(key))
            return;

        _weights[key] = Mathf.Clamp(_weights[key] + delta, 0.2f, 3f);
    }

    public void Decay(float rate = 0.01f)
    {
        List<(EAIGoalType, EAIActionTagType)> keys = new List<(EAIGoalType, EAIActionTagType)>(_weights.Keys);

        foreach ((EAIGoalType, EAIActionTagType) key in keys)
            _weights[key] = Mathf.Lerp(_weights[key], 1f, rate);
    }
}