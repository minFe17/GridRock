using UnityEngine;
using Utils;

/// <summary>
/// AI 시뮬레이션 서비스 (플레이어 행동 예측 + 보드 변화 계산)
/// </summary>
public class DefaultAISimulationService : IAISimulationService
{
    private AIContext _context;

    AISimulationState IAISimulationService.Simulate(in AIActionContext actionContext)
    {
        if (!TryGetContext(out _context))
            return default;

        // 플레이어의 행동 이후 위치 예측
        Vector2 predictedPosition = PredictCandidatePosition();

        bool[,] boardBefore = _context.Grid.Occupancy;
        bool[,] boardAfter = (bool[,])boardBefore.Clone();

        return BuildSimulationResult(predictedPosition, boardBefore, boardAfter, null);
    }

    AISimulationState IAISimulationService.SimulateCandidate(in AIActionContext actionContext, in IAIActionCandidate candidate)
    {
        if (!TryGetContext(out _context))
            return default;

        bool[,] boardBefore = _context.Grid.Occupancy;
        bool[,] boardAfter = BuildBoardAfterCandidate(boardBefore, _context.Player.GridPosition, candidate);

        Vector2 predictedPosition = PredictCandidatePosition();

        if (candidate.Action is BlockDropAction dropAction)
        {
            Vector2 dir = predictedPosition - (Vector2)_context.Player.GridPosition;
            predictedPosition += dir.normalized * 0.5f;
        }

        int occupiedBefore = CountOccupied(boardBefore);
        int occupiedAfter = CountOccupied(boardAfter);
        Debug.Log($"[SimulateCandidate] Before: {occupiedBefore}, After: {occupiedAfter}");

        return BuildSimulationResult(predictedPosition, boardBefore, boardAfter, candidate);
    }

    private int CountOccupied(bool[,] board)
    {
        int count = 0;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (board[x, y])
                    count++;
            }
        }
        return count;
    }

    private AISimulationState BuildSimulationResult(Vector2 predictedPosition, bool[,] boardBefore, bool[,] boardAfter, IAIActionCandidate candidate)
    {
        SpatialMetrics spatialBefore = SpatialAnalyzer.Analyze(boardBefore, predictedPosition);
        SpatialMetrics spatialAfter = SpatialAnalyzer.Analyze(boardAfter, predictedPosition);

        float futureTrapRisk = EstimateFutureRisk(predictedPosition, spatialAfter);

        PredictedWorldState predictedState = new PredictedWorldState(predictedPosition, spatialBefore, spatialAfter, futureTrapRisk);
        OutcomeEvaluation evaluation = OutcomeEvaluator.Evaluate(predictedState);
        BlockState blockState = BuildBlockState();

        return new AISimulationState(evaluation, new AIThreat(), _context.Player, blockState);
    }

    // 플레이어 위치 예측 (AI 목표: 방해 / 공격)
    private Vector2 PredictCandidatePosition()
    {
        Vector2 current = _context.Player.GridPosition;

        // 현재 주변 8방향 중 점유수 가장 높은 쪽으로 이동 (방해 목적)
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) };

        Vector2Int bestDirection = Vector2Int.zero;
        int maxOccupied = -1;

        foreach (Vector2Int direction in directions)
        {
            Vector2Int check = new Vector2Int((int)current.x + direction.x, (int)current.y + direction.y);
            if (_context.Grid.IsInBounds(check))
            {
                int occupied = CountAdjacentBlocks(check);
                if (occupied > maxOccupied)
                {
                    maxOccupied = occupied;
                    bestDirection = direction;
                }
            }
        }

        Vector2 predicted = current + (Vector2)bestDirection;

        // 그리드 범위 clamp
        predicted.x = Mathf.Clamp(predicted.x, 0, _context.Grid.Occupancy.GetLength(0) - 1);
        predicted.y = Mathf.Clamp(predicted.y, 0, _context.Grid.Occupancy.GetLength(1) - 1);

        return predicted;
    }

    private int CountAdjacentBlocks(Vector2Int pos)
    {
        int count = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) 
                    continue;
                Vector2 check = new Vector2(pos.x + dx, pos.y + dy);
                if (_context.Grid.IsOccupied(check))
                    count++;
            }
        }
        return count;
    }

    private float EstimateFutureRisk(Vector2 pos, SpatialMetrics spatial)
    {
        // 주변 블록 수 + DangerScore 기반 위험도 계산
        return spatial.AdjacentBlockCount * 0.5f + spatial.DangerScore;
    }

    private BlockState BuildBlockState()
    {
        if (!_context.ActiveBlock.HasValue)
            return new BlockState(EBlockType.Max, _context.Player.GridPosition, 0, true, 0f);

        BlockContext block = _context.ActiveBlock.Value;
        Vector2 pos = _context.Player.GridPosition;
        float pressure = 0f;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                if (_context.Grid.IsOccupied(pos + new Vector2(dx, dy)))
                    pressure += 0.5f;
            }
        }
        return new BlockState(block.BlockType, pos, block.Rotation, true, pressure);
    }

    private bool TryGetContext(out AIContext context)
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

        _context = context;
        return true;
    }

    #region BuildBoardAfterCandidate
    private bool[,] BuildBoardAfterCandidate(bool[,] boardBefore, Vector2 predictedPosition, IAIActionCandidate candidate)
    {
        if (candidate.Action is BlockDropAction dropAction)
            return BuildBoardAfterBlockPlacement(boardBefore, dropAction.BlockType, dropAction.Rotation, dropAction.DropCell);

        if (_context.ActiveBlock.HasValue)
        {
            BlockContext block = _context.ActiveBlock.Value;
            Vector2Int origin = new Vector2Int((int)predictedPosition.x, (int)predictedPosition.y);
            return BuildBoardAfterBlockPlacement(boardBefore, block.BlockType, block.Rotation, origin);
        }

        return (bool[,])boardBefore.Clone();
    }

    private bool[,] BuildBoardAfterBlockPlacement(bool[,] boardBefore, EBlockType blockType, int rotation, Vector2Int origin)
    {
        bool[,] boardAfter = (bool[,])boardBefore.Clone();
        Vector2Int[] cells = GetBlockCells(blockType, rotation);

        int width = boardAfter.GetLength(0);
        int height = boardAfter.GetLength(1);

        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int target = origin + cells[i];
            if (target.x < 0 || target.x >= width || target.y < 0 || target.y >= height)
                continue;

            boardAfter[target.x, target.y] = true;
        }

        return boardAfter;
    }

    private Vector2Int[] GetBlockCells(EBlockType blockType, int rotation)
    {
        Vector2Int[] baseCells;
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

        int normalizedRotation = ((rotation % 4) + 4) % 4;
        if (normalizedRotation == 0)
            return baseCells;

        Vector2Int[] rotated = new Vector2Int[baseCells.Length];
        for (int i = 0; i < baseCells.Length; i++)
            rotated[i] = Rotate(baseCells[i], normalizedRotation);

        return rotated;
    }

    private Vector2Int Rotate(Vector2Int p, int rotation)
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
    #endregion
}