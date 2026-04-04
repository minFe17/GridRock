using UnityEngine;
using Utils;

/// <summary>
/// Scene에서 AIBrain.Update 루프를 돌리는 최소 러너
/// </summary>
public sealed class AIBrainRunner : MonoBehaviour
{
    [SerializeField] float _pressureBudgetMax = 8f;       // AI 행동 총 비용 제한
    [SerializeField] bool _runEveryFrame = true;          // 매 프레임 실행 여부
    [SerializeField] float _thinkInterval = 0.12f;        // AI 판단 주기
    [SerializeField] float _highStackRatio = 0.65f;       // 높은 스택 판단 기준
    [SerializeField] int _nearLineClearHoleThreshold = 1; // 라인 클리어 직전 기준
    [SerializeField] int _nearTetrisHoleThreshold = 2;    // 테트리스 가능 기준

    IAIBrain _brain;
    float _thinkTimer;
    static bool _manualWeightsApplied = false;

    void Awake()
    {
        DefaultAISimulationService simulationService = new DefaultAISimulationService();
        DefaultAIFairnessFilter fairnessFilter = new DefaultAIFairnessFilter(new AIPressureBudget(_pressureBudgetMax));

        _brain = new AIBrain(new DefaultGoalDecider(), new DefaultGoalTermination(), new DummyActionProvider(), new AIActionSelector(fairnessFilter, simulationService), new MockStrategyLearning(), simulationService);

        if (!_manualWeightsApplied)
        {
            _manualWeightsApplied = true;

            AIGoalWeightTable.Shared.Adjust(EAIGoalType.ApplyPressure, -0.2f);
            AIGoalWeightTable.Shared.Adjust(EAIGoalType.TrapPlayer, 0.1f);
            AIGoalWeightTable.Shared.Adjust(EAIGoalType.KillNow, 0.1f);
            AIGoalWeightTable.Shared.Adjust(EAIGoalType.ForceMistake, 0f);
        }
    }

    void Update()
    {
        if (!_runEveryFrame || _brain == null)
            return;

        AIContextBuilder builder = SimpleSingleton<AIContextBuilder>.Instance;

        if (!builder.TryBuild(out AIContext context))
        {
            TickIdle(Time.deltaTime);
            return;
        }

        if (!ShouldThink(Time.deltaTime))
            return;

        AIInterferenceTriggerState trigger = BuildTrigger(context);
        Tick(Time.deltaTime, trigger);
    }

    bool ShouldThink(float deltaTime)
    {
        _thinkTimer += deltaTime;
        if (_thinkTimer < _thinkInterval)
            return false;

        _thinkTimer = 0f;

        BoardManager board = BoardManager.Instance;
        if (board != null && board.IsBlockDropping)
            return false;

        return true;
    }

    AIInterferenceTriggerState BuildTrigger(in AIContext context)
    {
        bool[,] occupancy = context.Grid.Occupancy;
        int height = occupancy.GetLength(1);
        if (height == 0)
            return default;

        int topRow = FindTopFilledRow(occupancy);

        float stackRatio = (topRow < 0) ? 0f : (float)(topRow + 1) / height;
        bool isHighStack = stackRatio >= _highStackRatio;

        bool isNearLineClear = context.Grid.HoleCount <= _nearLineClearHoleThreshold;
        bool isNearTetris = context.Grid.HoleCount <= _nearTetrisHoleThreshold;

        BoardManager board = BoardManager.Instance;
        bool isCommitMoment = board == null || !board.IsBlockDropping;

        return new AIInterferenceTriggerState(isNearLineClear, isNearTetris, isHighStack, isCommitMoment);
    }

    static int FindTopFilledRow(bool[,] occupancy)
    {
        int width = occupancy.GetLength(0);
        int height = occupancy.GetLength(1);

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (occupancy[x, y])
                    return y;
            }
        }
        return -1;
    }

    void TickIdle(float deltaTime)
    {
    }

    public void Tick(float deltaTime, in AIInterferenceTriggerState trigger)
    {
        if (_brain == null)
            return;

        AIActionContext actionContext = new AIActionContext
        {
            DeltaTime = deltaTime,
            Brain = _brain as AIBrain
        };

        _brain.Update(deltaTime, trigger, actionContext);
    }
}