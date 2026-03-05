using NUnit.Framework;
using UnityEngine;

public class SpatialAnalyzerTests
{
    [Test]
    public void OpenLane_ShouldHaveWideHorizontalEscape()
    {
        bool[,] board = new bool[5, 5];
        Vector2 pos = new Vector2(2, 2);

        SpatialMetrics spatial = SpatialAnalyzer.Analyze(board, pos);

        Assert.AreEqual(5, spatial.ReachableTileCount);
        Assert.AreEqual(2, spatial.EscapeRouteCount);
        Assert.IsTrue(spatial.HasEscapeRoute);
        Assert.IsFalse(spatial.IsCornered);
    }

    [Test]
    public void Surrounded_ShouldOnlyReachSelf()
    {
        bool[,] board = new bool[5, 5];

        board[1, 1] = true; board[1, 2] = true; board[1, 3] = true;
        board[2, 1] = true; board[2, 3] = true;
        board[3, 1] = true; board[3, 2] = true; board[3, 3] = true;

        Vector2 pos = new Vector2(2, 2);

        SpatialMetrics spatial = SpatialAnalyzer.Analyze(board, pos);

        Assert.AreEqual(1, spatial.ReachableTileCount);
        Assert.AreEqual(0, spatial.EscapeRouteCount);
        Assert.IsFalse(spatial.HasEscapeRoute);
        Assert.IsTrue(spatial.IsCornered);
        Assert.Greater(spatial.DangerScore, 7f);
    }

    [Test]
    public void BlockedLeftLane_ShouldHaveSingleEscapeRoute()
    {
        bool[,] board = new bool[6, 4];
        board[1, 1] = true;

        Vector2 pos = new Vector2(2, 1);
        SpatialMetrics spatial = SpatialAnalyzer.Analyze(board, pos);

        Assert.AreEqual(1, spatial.EscapeRouteCount);
    }
}