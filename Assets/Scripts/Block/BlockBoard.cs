using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockBoard : MonoBehaviour
{
    
    const int Y_SIZE = 18;
    const int X_SIZE = 13;

    private int[,] _board = new int[Y_SIZE, X_SIZE];

    private float _topY = 2.9f;
    private float _endY = -5.08f;
    private float _space = 0.5f;
    private int _centerIndexY = -1;
    private float _preY;

    public Dictionary<int, int> CheckBoard(BlockData data, Vector3 position)
    {
        Dictionary<int, int> blockTops = new Dictionary<int, int>();
        foreach (var index in data.index)
        {
            int xIndex = (int)((position.x ) / 0.5f) + index.x;
            int yIndex = 17;
            for (int y = 0; y <18; y++)
            {
                if (_board[y,xIndex]==1)
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
    public void UpdateBoard(GameObject obj, BlockData data)
    {
        Vector3 position = obj.transform.localPosition;
        if (position.y > 2.9f) return;
        if (Mathf.Abs(position.y - _preY) < _space) return;

        else if (position.y == 2.9f)
        {
            _preY = 2.9f;
            _centerIndexY = 0;
            return;
        }

        _preY = position.y;
        _centerIndexY++;
        

        //float y = position.y;


        Debug.Log(_centerIndexY);

        if (_centerIndexY == Y_SIZE - 1) //이거 그 센터 1짜리로 잡아놔서 그런가? 아근데 I는 아닌데그럼?
        {
            //obj.GetComponent<BlockController>().StopDrop();

            Vector2 pos = new Vector2(position.x, _endY);
            obj.transform.localPosition = pos;
            //값 보정해줘야됨

            AddIndex(data, pos);
        }
        else
        {
            int xIndex = (int)((position.x ) / 0.5f);
            bool isHit = false;
            //데이터 기반으로 체크해주면됨.
            foreach (CellIndex index in data.index)
            {

                if (_centerIndexY + index.y+1 > 17 || _centerIndexY + index.y<0) continue; //리턴해도되지않을까?
                else if (_board[_centerIndexY + index.y + 1, xIndex] != 1) continue;

                isHit = true;
            }

            if (!isHit) return;
            //obj.GetComponent<BlockController>().StopDrop();

            Vector2 pos = new Vector2(position.x, (_topY - _centerIndexY * _space));
            obj.transform.localPosition = pos;

            AddIndex(data, pos);
        }
        // 떨어지는거 이런식으로하지말고, x기준 아래에 y 있나없나 미리 체크하는 방식으로 변경해야될듯
    }//사용안함
    public void AddIndex(BlockData data, Vector3 position)
    {
        int xIndex = (int)((position.x ) / 0.5f);
        int yIndex = (int)(Mathf.Abs(position.y - 2.9f) / 0.5f);
        // +1안하면 보드 배열이 안맞고, 하면 위에 블록이 씹힌다. 왜????
        foreach (CellIndex index in data.index)
        {
            _board[index.y + yIndex, index.x + xIndex] = 1; //연산 꼬일수도있다
            Debug.Log(index.y + yIndex);
            Debug.Log(index.x + xIndex);
        }


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
        _space = length / (Y_SIZE-1);
        //이거 블록크기랑 다시 다 맞춰야될듯
    }

}
