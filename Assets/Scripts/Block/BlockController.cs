using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BlockController : MonoBehaviour
{
    private const int X_SIZE = 13;

    [SerializeField]
    private EBlockType _type;
    private BlockData _data;

    private bool _isDrop = false;
    private float _speed = 1f;
    private BlockBoard _board;
    private Dictionary<int,int> _blockTops = new Dictionary<int, int> ();
    private float _preY = 0;
    private float _space = 0.5f;
    

    public void MoveRight()
    {
        int xIndex = (int)((transform.localPosition.x - 0.76f) / 0.5f);
        foreach (CellIndex index in _data.index)
        {
            if (xIndex + index.x >= X_SIZE) return;
        }
        transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y);

        _blockTops = _board.CheckBoard(_data, transform.localPosition);
    }
    public void MoveLeft()
    {
        int xIndex = (int)((transform.localPosition.x - 0.76f) / 0.5f);
        foreach (CellIndex index in _data.index)
        {
            if (xIndex + index.x <= 0) return;
        }
        transform.localPosition = new Vector3(transform.localPosition.x - 0.5f, transform.localPosition.y);

        _blockTops = _board.CheckBoard(_data, transform.localPosition);
    }

    public void RotationRight()
    {
        //밖으로 튀어나오는거 예외처리해줘야됨
        transform.Rotate(0f, 0f, -90f);

        for (int i = 0; i < _data.index.Count; i++)
        {
            CellIndex index = _data.index[i];

            int temp = index.x;
            index.x = -1*index.y;
            index.y = temp;

            _data.index[i] = index;
        }

        _blockTops = _board.CheckBoard(_data, transform.localPosition);
    }
    public void RotationLeft()
    {
        //밖으로 튀어나오는거 예외처리해줘야됨
        transform.Rotate(0f, 0f, 90f);
        for (int i = 0; i < _data.index.Count; i++)
        {
            CellIndex index = _data.index[i];

            int temp = index.x;
            index.x = index.y;
            index.y = -1 * temp;

            _data.index[i] = index;
        }
        _blockTops = _board.CheckBoard(_data, transform.localPosition);
    }
   

    public void StartDrop()
    {
        if (_type == EBlockType.Z)
            transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y);
        _isDrop = true;
        if(_board == null)
            _board = transform.parent.parent.GetComponentInChildren<BlockBoard>();
        if(_data.index == null)
            _data = DataManager.Instance.FindBlock(_type);
        _blockTops = _board.CheckBoard(_data, transform.localPosition);
    }
    public void StopDrop()
    {
        _isDrop = false;
    }
    private void Update()
    {
        if (!_isDrop) return;
        Drop();
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            MoveRight();
        else if(Keyboard.current.f4Key.wasPressedThisFrame)
            MoveLeft();

        if (Keyboard.current.f7Key.wasPressedThisFrame)
            RotationRight();
        else if (Keyboard.current.f6Key.wasPressedThisFrame)
            RotationLeft();
    }
    private void Drop()
    {
        transform.localPosition += Vector3.down * _speed * Time.deltaTime;
        if (transform.localPosition.y > 2.9) return;
        if (Mathf.Abs(transform.localPosition.y - _preY) < _space) return;
        else if (transform.localPosition.y == 2.9f)
        {
            _preY = 2.9f;
            return;
        }

        _preY = transform.localPosition.y;

        //_board.UpdateBoard(gameObject, _data);

        foreach (var index in _data.index)
        {
            int xIndex = (int)((transform.localPosition.x - 0.76f) / 0.5f) + index.x;
            int yIndex = (int)(Mathf.Abs(_preY-2.9f)/0.5f)+index.y + 1; //블럭 보정값
            //Debug.Log(yIndex);
            if (_blockTops[xIndex] == yIndex)
            {
                StopDrop();
                _board.AddIndex(_data, transform.localPosition);
                return;
            }
        }
    }



}
