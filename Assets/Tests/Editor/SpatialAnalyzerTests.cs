using NUnit.Framework;
using UnityEngine;

public class SpatialAnalyzerTests
{
    [Test]
    public void OpenArea_ShouldHaveLargeEscape()
    {
        bool[,] board = new bool[5, 5];
        Vector2 pos = new Vector2(2, 2);

        SpatialMetrics spatial = SpatialAnalyzer.Analyze(board, pos);

        Assert.AreEqual(25, spatial.EscapePathLength);
    }

    [Test]
    public void Surrounded_ShouldHaveZeroEscape()
    {
        bool[,] board = new bool[5, 5];

        board[1, 1] = true; board[1, 2] = true; board[1, 3] = true;
        board[2, 1] = true; board[2, 3] = true;
        board[3, 1] = true; board[3, 2] = true; board[3, 3] = true;

        Vector2 pos = new Vector2(2, 2);

        SpatialMetrics spatial = SpatialAnalyzer.Analyze(board, pos);

        Assert.AreEqual(1, spatial.EscapePathLength);
    }
}