using UnityEngine;
using System.Collections.Generic;

public class BlockBoard : MonoBehaviour
{
    const int Y_SIZE = 20;
    const int X_SIZE = 15;

    private int[,] _board = new int[Y_SIZE, X_SIZE];

    private void Update()
    {
         //블럭 내려오면 데이터값 받아서 위치에 1로 표시해주기
         //이전 블록위치랑 현재 바뀐블록위치 넣어둘 리스트 필요.
         
    }
}
