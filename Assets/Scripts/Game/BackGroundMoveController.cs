using UnityEngine;

public class BackGroundMoveController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private Vector3 _frontPosition;

    [SerializeField]
    private float _endPosition;

    private bool _isMove = true;

    public bool IsMove
    {
        set {  _isMove = value; }
    }
    private void Update()
    {
        if (!_isMove) return;
        Move();
    }
    private void Move()
    {
        if ((transform.localPosition.x <= _endPosition))
        {
            transform.localPosition = _frontPosition;
        }
        transform.localPosition += Vector3.left * _moveSpeed * Time.deltaTime;
    }

}
