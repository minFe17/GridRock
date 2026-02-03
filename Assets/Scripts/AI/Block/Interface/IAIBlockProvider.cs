using System.Collections.Generic;

/// <summary>
/// AI에게 제공할 블록 후보 집합을 생성하는 인터페이스
/// </summary>
public interface IAIBlockProvider
{
    IReadOnlyList<IAIActionCandidate> GetCandidates();
}