using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockBoard : MonoBehaviour
{

    const int Y_SIZE = 18;
    const int X_SIZE = 13;

    private int[,] _board = new int[Y_SIZE, X_SIZE];
    private int[] _xIndexCount = new int[Y_SIZE];

    private float _topY = 2.9f;
    private float _endY = -5.08f;
    private float _space = 0.5f;
    private int _maxTopIndex = Y_SIZE; // 낮을수록 높은 배열 인덱스 입니다. 0->최상위배열인덱스
    private float _preY;


    public int MaxTopIndex
    {
        //가장 높은 블록 Y인덱스 값입니다. 0이 제일 높고, 17이 제일 낮습니다.
        //18일경우 블록이 아직 없는 것 입니다.
        get { return _maxTopIndex; }
    }
    public int GetEmptyBlockNum
    {
        get
        {
            int empty = 0;
            foreach (int block in _board)
            {
                if (block == 0) empty++;
            }
            return empty;
        }
    }
    public Dictionary<int, int> CheckBoard(BlockData data, Vector3 position)
    {
        Dictionary<int, int> blockTops = new Dictionary<int, int>();
        foreach (var index in data.index)
        {
            int xIndex = (int)((position.x) / 0.5f) + index.x; //위치 인덱스 보정
            int yIndex = Y_SIZE-1;
            for (int y = 0; y < Y_SIZE; y++)
            {
                if (_board[y, xIndex] == 1)
                {
                    yIndex = y;
                    break;
                }
            }
            blockTops[xIndex] = yIndex;
            Debug.Log(yIndex);
        }
        return blockTops;
    }
    public bool[,] BuildOccupancyMap()
    {
        bool[,] occupancy = new bool[X_SIZE, Y_SIZE];

        for (int y = 0; y < Y_SIZE; y++)
        {
            for (int x = 0; x < X_SIZE; x++)
                occupancy[x, y] = _board[y, x] == 1;
        }

        return occupancy;
    }
    public void AddIndex(BlockData data, Vector3 position)
    {
        int xIndex = (int)((position.x) / 0.5f);
        int yIndex = (int)(Mathf.Abs(position.y - 2.9f) / 0.5f) + 1;
        // +1안하면 보드 배열이 안맞고, 하면 위에 블록이 씹힌다. 왜????
        foreach (CellIndex index in data.index)
        {  
            _board[index.y + yIndex, index.x + xIndex] = 1; //연산 꼬일수도있다
            _xIndexCount[index.y+yIndex]++;
            Debug.Log(index.y + yIndex);
            Debug.Log(index.x + xIndex);

            if (index.y + yIndex < _maxTopIndex)
                _maxTopIndex = index.y + yIndex;
        }

        int clearYIndex = CheckIndexes();

        if (clearYIndex != -1)
            ClearIndexX(clearYIndex);

        string arr = "";
        for (int i = 0; i < Y_SIZE; i++)
        {
            for (int j = 0; j < X_SIZE; j++)
            {
                arr += _board[i, j].ToString() + " ";
            }
            arr += "\n";
        }
        Debug.Log(arr);
    }

    private void Awake()
    {
        float length = Mathf.Abs(_endY - _topY);
        _space = length / (Y_SIZE - 1);
        //이거 블록크기랑 다시 다 맞춰야될듯
    }

    private int CheckIndexes()
    {
        for(int y=0;y< Y_SIZE;y++)
        {
            if (_xIndexCount[y] == X_SIZE)
                return y;
        }
        return -1;
    }
    private void ClearIndexX(int yIndex)
    {
        //라인에있는 블럭 확인해서 지우고 위에층 내리기 작업해야됨
    }

}
