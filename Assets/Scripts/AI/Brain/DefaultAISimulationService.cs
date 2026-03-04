using UnityEngine;
using Utils;

public class DefaultAISimulationService : IAISimulationService
{
    AIContext _context;
    AISimulationState IAISimulationService.Simulate()
    {
        _context = SimpleSingleton<AIContextBuilder>.Instance.Build();

        Vector2 predictedPosition = PredictCandidatePosition();
        SpatialMetrics spatial = AnalyzePlayerSpace(predictedPosition);
        float futureTrapRisk = EstimateFutureRisk(predictedPosition, spatial);

        PredictedWorldState predictedState = new PredictedWorldState(predictedPosition, spatial, futureTrapRisk);

        OutcomeEvaluation evaluation = OutcomeEvaluator.Evaluate(predictedState);

        BlockState blockState = BuildBlockState();

        return new AISimulationState(evaluation, new AIThreat(), _context.Player, blockState);
    }

    Vector2 PredictCandidatePosition()
    {
        // 임시: 현재 플레이어 위치 + 오른쪽으로 1타일 이동
        return new Vector2(0, 0); // 실제 위치로 교체 필요
    }

    SpatialMetrics AnalyzePlayerSpace(Vector2 pos)
    {
        bool[,] board = _context.Grid.Occupancy;

        return SpatialAnalyzer.Analyze(board, pos);
    }

    float EstimateFutureRisk(Vector2 pos, SpatialMetrics spatial)
    {
        // 임시: 안전한 상태 가정
        return 0f;
    }

    BlockState BuildBlockState()
    {
        BlockContext? blockContext = _context.ActiveBlock;
        GridContext grid = _context.Grid;
        PlayerContext player = _context.Player;

        if (!blockContext.HasValue)
            return new BlockState(EBlockType.Max, Vector2.zero, 0, true, 0);

        float pressure = 0f;
        Vector2 pos = player.GridPosition; // 실제 위치 계산 필요
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (grid.IsOccupied(pos + new Vector2(x, y)))
                    pressure += 0.5f;
            }
        }
        BlockContext block = blockContext.Value;
        return new BlockState(block.BlockType, pos, block.Rotation, true, pressure);
    }
}