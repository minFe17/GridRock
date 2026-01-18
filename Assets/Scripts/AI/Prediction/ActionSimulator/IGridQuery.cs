using UnityEngine;

/// <summary>
/// AI가 그리드(맵) 상태를 조회하기 위한 인터페이스
/// 실제 그리드 구현과 분리하여 예측·시뮬레이션에서 사용
/// </summary>
public interface IGridQuery
{
    bool IsBlocked(Vector2Int pos);   // 해당 위치가 막혀 있는지 (벽, 블록 등)
    bool HasGround(Vector2Int pos);   // 해당 위치 아래에 바닥이 있는지
    bool IsInsideGrid(Vector2Int pos); // 그리드 범위 안에 있는 좌표인지
}