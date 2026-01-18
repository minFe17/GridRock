using UnityEngine;

/// <summary>
/// 현재 게임 월드 상태를 수집하여 AI가 판단에 사용할 AIContext를 생성하는 역할
/// </summary>
public class AIContextBuilder : MonoBehaviour
{
    // 싱글턴

    public AIContext BuildContext()
    {
        // PlayerContext, GridContext, BlockContext, AIStateContext를 여기서 수집해서 반환
        return default;
    }
}