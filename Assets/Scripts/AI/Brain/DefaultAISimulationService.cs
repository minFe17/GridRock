using UnityEngine;
using Utils;

public class DefaultAISimulationService : IAISimulationService
{
    AIContext _context;

    AISimulationState IAISimulationService.Simulate(in AIActionContext actionContext)
    {
        if (!TryGetContext(out _context))
            return default;

        // 플레이어의 행동 이후 위치 예측
        Vector2 predictedPosition = PredictCandidatePosition();

        bool[,] boardBefore = _context.Grid.Occupancy;
        bool[,] boardAfter = BuildBoardAfterBlockPlacement(boardBefore, predictedPosition);
        return BuildSimulationResult(predictedPosition, boardBefore, boardAfter);
    }

    AISimulationState IAISimulationService.SimulateCandidate(in AIActionContext actionContext, in IAIActionCandidate candidate)
    {
        if (!TryGetContext(out _context))
            return default;

        Vector2 predictedPosition = PredictCandidatePosition();

        bool[,] boardBefore = _context.Grid.Occupancy;
        bool[,] boardAfter = BuildBoardAfterCandidate(boardBefore, predictedPosition, candidate);

        return BuildSimulationResult(predictedPosition, boardBefore, boardAfter);
    }

    AISimulationState BuildSimulationResult(Vector2 predictedPosition, bool[,] boardBefore, bool[,] boardAfter)
    {
        SpatialMetrics spatialBefore = SpatialAnalyzer.Analyze(boardBefore, predictedPosition);
        SpatialMetrics spatialAfter = SpatialAnalyzer.Analyze(boardAfter, predictedPosition);

        float futureTrapRisk = EstimateFutureRisk(predictedPosition, spatialAfter);

        PredictedWorldState predictedState = new PredictedWorldState(predictedPosition, spatialBefore, spatialAfter, futureTrapRisk);

        OutcomeEvaluation evaluation = OutcomeEvaluator.Evaluate(predictedState);
        BlockState blockState = BuildBlockState();

        return new AISimulationState(evaluation, new AIThreat(), _context.Player, blockState);
    }

    // 행동 이후 플레이어 위치 예측
    Vector2 PredictCandidatePosition()
    {
        // 임시
        return new Vector2(_context.Player.GridPosition.x, _context.Player.GridPosition.y);
    }

    bool[,] BuildBoardAfterCandidate(bool[,] boardBefore, Vector2 predictedPosition, IAIActionCandidate candidate)
    {
        if (candidate.Action is BlockDropAction dropAction)
            return BuildBoardAfterBlockPlacement(boardBefore, dropAction.BlockType, dropAction.Rotation, dropAction.DropCell);

        return BuildBoardAfterBlockPlacement(boardBefore, predictedPosition);
    }

    // 블록을 특정 위치에 배치한 이후의 보드 상태 생성
    bool[,] BuildBoardAfterBlockPlacement(bool[,] boardBefore, Vector2 predictedPosition)
    {
        // 현재 활성 블록이 없다면 변경 없이 반환
        if (!_context.ActiveBlock.HasValue)
            return (bool[,])boardBefore.Clone();

        BlockContext block = _context.ActiveBlock.Value;

        // 블록 기준 위치
        Vector2Int origin = new Vector2Int((int)predictedPosition.x, (int)predictedPosition.y);

        return BuildBoardAfterBlockPlacement(boardBefore, block.BlockType, block.Rotation, origin);
    }

    bool[,] BuildBoardAfterBlockPlacement(bool[,] boardBefore, EBlockType blockType, int rotation, Vector2Int origin)
    {
        bool[,] boardAfter = (bool[,])boardBefore.Clone();
        Vector2Int[] cells = GetBlockCells(blockType, rotation);

        int width = boardAfter.GetLength(0);
        int height = boardAfter.GetLength(1);

        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int target = origin + cells[i];

            // 보드 범위 벗어나면 무시
            if (target.x < 0 || target.x >= width || target.y < 0 || target.y >= height)
                continue;

            // 해당 위치를 점유 상태로 변경
            boardAfter[target.x, target.y] = true;
        }

        return boardAfter;
    }

    // 블록 타입에 따른 기본 셀 구조 정의
    Vector2Int[] GetBlockCells(EBlockType blockType, int rotation)
    {
        Vector2Int[] baseCells;

        // 각 블록 타입의 기본 상대 좌표 정의
        switch (blockType)
        {
            case EBlockType.I:
                baseCells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
                break;
            case EBlockType.O:
                baseCells = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
                break;
            case EBlockType.T:
                baseCells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                break;
            case EBlockType.S:
                baseCells = new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) };
                break;
            case EBlockType.Z:
                baseCells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
                break;
            case EBlockType.J:
                baseCells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1) };
                break;
            case EBlockType.L:
                baseCells = new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
                break;
            default:
                baseCells = new[] { Vector2Int.zero };
                break;
        }

        // 회전 정규화 (음수 회전 대응)
        int normalizedRotation = ((rotation % 4) + 4) % 4;
        if (normalizedRotation == 0)
            return baseCells;

        Vector2Int[] rotated = new Vector2Int[baseCells.Length];

        // 90도 단위 회전 적용
        for (int i = 0; i < baseCells.Length; i++)
            rotated[i] = Rotate(baseCells[i], normalizedRotation);

        return rotated;
    }

    // 좌표를 90도 단위로 회전
    Vector2Int Rotate(Vector2Int p, int rotation)
    {
        switch (rotation)
        {
            case 1:
                return new Vector2Int(p.y, -p.x);
            case 2:
                return new Vector2Int(-p.x, -p.y);
            case 3:
                return new Vector2Int(-p.y, p.x);
            default:
                return p;
        }
    }

    // 미래 봉쇄 위험도 계산
    float EstimateFutureRisk(Vector2 pos, SpatialMetrics spatial)
    {
        // 임시: 안전한 상태 가정
        return 0f;
    }

    // 현재 블록 상태 및 주변 압박도 계산
    BlockState BuildBlockState()
    {
        BlockContext? blockContext = _context.ActiveBlock;
        GridContext grid = _context.Grid;
        PlayerContext player = _context.Player;

        // 블록이 없는 경우 기본 상태 반환
        if (!blockContext.HasValue)
            return new BlockState(EBlockType.Max, Vector2.zero, 0, true, 0);

        float pressure = 0f;
        Vector2 pos = player.GridPosition;

        // 플레이어 주변 8방향 점유 여부 기반 압박 계산
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                // 주변 셀이 점유되어 있다면 압박 증가
                if (grid.IsOccupied(pos + new Vector2(x, y)))
                    pressure += 0.5f;
            }
        }
        BlockContext block = blockContext.Value;
        return new BlockState(block.BlockType, pos, block.Rotation, true, pressure);
    }

    bool TryGetContext(out AIContext context)
    {
        AIContextBuilder builder = SimpleSingleton<AIContextBuilder>.Instance;

        if (!builder.TryBuild(out context))
        {
#if UNITY_EDITOR
            throw new System.Exception("AIContext Build Failed");
#else
            Debug.LogWarning("AIContext not ready");
            return false;
#endif
        }

        if (context.Grid.Occupancy == null)
            return false;

        return true;
    }
}