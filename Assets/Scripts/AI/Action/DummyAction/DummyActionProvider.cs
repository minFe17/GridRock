using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// AI가 수행 가능한 행동 후보(ActionCandidate)를 생성하는 Provider
/// </summary>
public class DummyActionProvider : IAIActionProvider
{
    readonly AIPrediction _prediction = new AIPrediction();

    const int PredictLookAheadCells = 2; // 플레이어 이동 예측 거리
    const int DraftPoolSize = 3;         // AI가 고려할 블록 DraftPool 최대 개수

    // 목표(Goal)에 맞는 행동 후보 목록 생성
    public IReadOnlyList<IAIActionCandidate> GetCandidates(EAIGoalType goal)
    {
        AIContext context = SimpleSingleton<AIContextBuilder>.Instance.Build();

        List<IAIActionCandidate> candidates = new List<IAIActionCandidate>();

        // 현재 사용할 수 있는 블록 DraftPool 구성
        IReadOnlyList<BlockOptionContext> draftPool = BuildDraftPool(context);

        // 각 블록에 대해 배치 가능한 후보 생성
        for (int blockSlot = 0; blockSlot < draftPool.Count; blockSlot++)
            BuildCandidatesForBlock(goal, context, draftPool[blockSlot].BlockType, blockSlot, candidates);

        return candidates;
    }

    // AI가 사용할 블록 후보 목록 생성
    static IReadOnlyList<BlockOptionContext> BuildDraftPool(in AIContext context)
    {
        List<BlockOptionContext> draftPool = new List<BlockOptionContext>(DraftPoolSize);

        if (context.AvailableBlocks != null)
        {
            int max = Mathf.Min(DraftPoolSize, context.AvailableBlocks.Count);
            for (int i = 0; i < max; i++)
                draftPool.Add(context.AvailableBlocks[i]);
        }

        // Draft가 없을 경우 현재 ActiveBlock 사용
        if (draftPool.Count == 0 && context.ActiveBlock.HasValue)
            draftPool.Add(new BlockOptionContext(context.ActiveBlock.Value.BlockType));

        return draftPool;
    }

    // 특정 블록 타입에 대해 가능한 모든 배치 후보 생성
    void BuildCandidatesForBlock(EAIGoalType goal, in AIContext context, EBlockType blockType, int blockSlot, List<IAIActionCandidate> output)
    {
        bool[,] board = context.Grid.Occupancy;
        if (board == null)
            return;

        int width = board.GetLength(0);
        int height = board.GetLength(1);

        // 플레이어 이동 예측 위치
        List<int> predictedXs = _prediction.PredictFutureXs(context.Player, width);

        int rotationCount = GetRotationCount(blockType);

        // Goal → ActionTag 매핑
        EAIActionTagType actionTag = MapTag(goal);

        // 모든 회전 상태 탐색
        for (int rotation = 0; rotation < rotationCount; rotation++)
        {
            Vector2Int[] shape = GetRotatedShape(blockType, rotation);

            // 모든 X 위치 탐색
            for (int x = 0; x < width; x++)
            {
                if (!CanPlaceAtX(shape, x, width))
                    continue;

                // 실제 낙하 가능한 Y 위치 탐색
                if (!TryFindDropY(board, shape, x, height, out int dropY))
                    continue;

                Vector2Int targetCell = new Vector2Int(x, dropY);

                // 플레이어 예측 위치 기준 압박 비용 계산
                float pressureCost = CalculatePressureCostMulti(goal, predictedXs, targetCell.x);

                output.Add(new BlockDropActionCandidate(actionTag, pressureCost, new BlockDropAction(actionTag, blockType, rotation, targetCell, blockSlot, predictedXs)));
            }
        }
    }

    // Goal 타입을 ActionTag로 변환
    static EAIActionTagType MapTag(EAIGoalType goal)
    {
        switch (goal)
        {
            case EAIGoalType.KillNow:
                return EAIActionTagType.InstantKill;
            case EAIGoalType.TrapPlayer:
                return EAIActionTagType.BlockEscape;
            case EAIGoalType.ForceMistake:
                return EAIActionTagType.CreateDanger;
            default:
                return EAIActionTagType.ApplyPressure;
        }
    }

    // 플레이어 예측 위치와 블록 위치 간 거리 기반 압박 비용 계산
    static float CalculatePressureCost(EAIGoalType goal, int predictedX, int placementX)
    {
        float distance = Mathf.Abs(predictedX - placementX);
        float baseCost;
        switch (goal)
        {
            case EAIGoalType.KillNow:
                baseCost = 0.25f;
                break;
            case EAIGoalType.TrapPlayer:
                baseCost = 0.5f;
                break;
            case EAIGoalType.ForceMistake:
                baseCost = 0.7f;
                break;
            default:
                baseCost = 0.9f;
                break;
        }

        return baseCost + (distance * 0.05f);
    }

    // 블록이 보드 X 범위를 벗어나는지 검사
    static bool CanPlaceAtX(Vector2Int[] shape, int originX, int width)
    {
        for (int i = 0; i < shape.Length; i++)
        {
            int x = originX + shape[i].x;
            if (x < 0 || x >= width)
                return false;
        }

        return true;
    }

    // 블록이 낙하 가능한 Y 위치 탐색
    static bool TryFindDropY(bool[,] board, Vector2Int[] shape, int originX, int height, out int dropY)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            if (!CanOccupy(board, shape, originX, y))
                continue;

            if (y == 0 || !CanOccupy(board, shape, originX, y - 1))
            {
                dropY = y;
                return true;
            }
        }

        dropY = -1;
        return false;
    }

    // 해당 위치에 블록을 배치할 수 있는지 검사
    static bool CanOccupy(bool[,] board, Vector2Int[] shape, int originX, int originY)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        for (int i = 0; i < shape.Length; i++)
        {
            int x = originX + shape[i].x;
            int y = originY + shape[i].y;

            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            if (board[x, y])
                return false;
        }

        return true;
    }

    // 블록 타입에 따른 회전 가능 횟수 반환
    static int GetRotationCount(EBlockType blockType)
    {
        switch (blockType)
        {
            case EBlockType.O:
                return 1;
            case EBlockType.I:
            case EBlockType.S:
            case EBlockType.Z:
                return 2;
            default:
                return 4;
        }
    }

    // 블록 회전 적용
    static Vector2Int[] GetRotatedShape(EBlockType blockType, int rotation)
    {
        Vector2Int[] baseShape = GetBaseShape(blockType);

        int normalized = ((rotation % 4) + 4) % 4;
        if (normalized == 0)
            return baseShape;

        Vector2Int[] rotated = new Vector2Int[baseShape.Length];
        for (int i = 0; i < baseShape.Length; i++)
            rotated[i] = RotateClockwise(baseShape[i], normalized);

        return rotated;
    }

    // 블록 기본 형태 정의
    static Vector2Int[] GetBaseShape(EBlockType blockType)
    {
        switch (blockType)
        {
            case EBlockType.I:
                return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
            case EBlockType.O:
                return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
            case EBlockType.T:
                return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
            case EBlockType.S:
                return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1) };
            case EBlockType.Z:
                return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
            case EBlockType.J:
                return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1) };
            case EBlockType.L:
                return new[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
            default:
                return new[] { Vector2Int.zero };
        }
    }

    // 좌표 시계 방향 회전
    static Vector2Int RotateClockwise(Vector2Int cell, int rotation)
    {
        switch (rotation)
        {
            case 1:
                return new Vector2Int(cell.y, -cell.x);
            case 2:
                return new Vector2Int(-cell.x, -cell.y);
            case 3:
                return new Vector2Int(-cell.y, cell.x);
            default:
                return cell;
        }
    }

    static float CalculatePressureCostMulti(EAIGoalType goal, List<int> predictedXs, int placementX)
    {
        float bestScore = float.MaxValue;

        for (int i = 0; i < predictedXs.Count; i++)
        {
            float dist = Mathf.Abs(predictedXs[i] - placementX);
            float weight = (i == 0) ? 0.5f : 1f;
            float score = dist * weight;

            if (score < bestScore)
                bestScore = score;
        }

        float baseCost;

        switch (goal)
        {
            case EAIGoalType.KillNow:
                baseCost = 0.25f;
                break;
            case EAIGoalType.TrapPlayer:
                baseCost = 0.5f;
                break;
            case EAIGoalType.ForceMistake:
                baseCost = 0.7f;
                break;
            default:
                baseCost = 0.9f;
                break;
        }

        return baseCost + (bestScore * 0.05f);
    }
}    