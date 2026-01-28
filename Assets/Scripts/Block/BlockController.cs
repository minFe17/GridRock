using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private EBlockType _type;

    private BlockData _data;

    private void Start()
    {
        _data = DataManager.Instance.FindBlock(_type);
    }

}
