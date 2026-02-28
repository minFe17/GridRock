public readonly struct AIThreat
{
    public readonly int NearbyObstacles;
    public readonly float EscapeDistance;
    public readonly bool IsCornered;

    public AIThreat(int nearby, float escapeDist, bool cornered)
    {
        NearbyObstacles = nearby;
        EscapeDistance = escapeDist;
        IsCornered = cornered;
    }
}