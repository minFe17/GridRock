/// <summary>
/// AI 행동 실행 단계에서 사용되는 공통 컨텍스트 정보
/// 시간 흐름과 AI 내부 상태 접근을 제공
/// </summary>
public struct AIActionContext
{
    public float DeltaTime;   // 프레임 경과 시간
    public AIBrain Brain;     // 행동을 수행하는 AI 브레인 참조
}