using System.Collections.Generic;
public struct BlockData
{
    public string name;
    public EBlockType type;
    public List<CellIndex> index;
}
public enum EBlockType
{
    I,
    J,
    L,
    O,
    S,
    T,
    Z,
    Max,
}
public struct CellIndex
{
    public int y;
    public int x;
}