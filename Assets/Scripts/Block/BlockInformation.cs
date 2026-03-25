using UnityEngine;

public class BlockInformation : MonoBehaviour
{
    private int _yIndex;
    private int _xIndex;

    public int YIndex
    {
        get { return _yIndex; }
        set { _yIndex = value; }
    }
    public int XIndex
    {
        get { return _xIndex; }
        set { _xIndex = value; }
    }
}
