using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BlockController : MonoBehaviour
{
    private const int X_SIZE = 13;

    [SerializeField]
    private EBlockType _type;
    private BlockData _data;

    private bool _isDrop = false;
    private float _speed = 1f;
    private BlockBoard _board;

    public void MoveRight()
    {
        int xIndex = (int)((transform.localPosition.x - 0.76f) / 0.5f);
        foreach (CellIndex index in _data.index)
        {
            if (xIndex + index.x >= X_SIZE) return;
        }
        transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y);
    }
    public void MoveLeft()
    {
        int xIndex = (int)((transform.localPosition.x - 0.76f) / 0.5f);
        foreach (CellIndex index in _data.index)
        {
            if (xIndex + index.x <= 0) return;
        }
        transform.localPosition = new Vector3(transform.localPosition.x - 0.5f, transform.localPosition.y);
    }

    public void RotationRight()
    {

    }
    public void RotationLeft()
    {

    }
   

    public void StartDrop()
    {
        if (_type == EBlockType.Z)
            transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y);
        _isDrop = true;
        _board = transform.parent.parent.GetComponentInChildren<BlockBoard>();
    }
    public void StopDrop()
    {
        _isDrop = false;
    }
    private void Start()
    {
        _data = DataManager.Instance.FindBlock(_type);
        //데이터 찾을필요가있나?
    }
    private void Update()
    {
        if (!_isDrop) return;
        Drop();
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            MoveRight();
        else if(Keyboard.current.f4Key.wasPressedThisFrame)
            MoveLeft();
    }
    private void Drop()
    {
        transform.localPosition += Vector3.down * _speed * Time.deltaTime;
        _board.UpdateBoard(gameObject, _data);
    }


}
