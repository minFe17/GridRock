using UnityEngine;

/// <summary>
/// Scene에서 AIBrain.Update 루프를 돌리는 최소 러너
/// </summary>
public sealed class AIBrainRunner : MonoBehaviour
{
    [SerializeField] float _pressureBudgetMax = 8f;
    [SerializeField] bool _runEveryFrame = true;

    IAIBrain _brain;

    void Awake()
    {
        DefaultAISimulationService simulationService = new DefaultAISimulationService();
        DefaultAIFairnessFilter fairnessFilter = new DefaultAIFairnessFilter(new AIPressureBudget(_pressureBudgetMax));

        _brain = new AIBrain(new DefaultGoalDecider(), new DefaultGoalTermination(), new DummyActionProvider(), new AIActionSelector(fairnessFilter, simulationService), new MockStrategyLearning(), simulationService);
    }

    void Update()
    {
        if (!_runEveryFrame || _brain == null)
            return;

        Tick(Time.deltaTime);
    }

    public void Tick(float deltaTime)
    {
        if (_brain == null)
            return;

        AIInterferenceTriggerState trigger = new AIInterferenceTriggerState(false, false, false, true);

        AIActionContext actionContext = new AIActionContext
        {
            DeltaTime = deltaTime,
            Brain = _brain as AIBrain
        };

        _brain.Update(deltaTime, trigger, actionContext);
    }
}