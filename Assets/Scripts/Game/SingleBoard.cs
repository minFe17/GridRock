using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SingleBoard : MonoBehaviour
{
    private Dictionary<EBlockType, PoolingManager> _blocks = new Dictionary<EBlockType, PoolingManager>();

    private List<BlockData> _blockDatas;
    //이거 고민 좀 해봐야됨 블럭 전부 사라졌으면 다시 풀링에 집어넣는 방식으로갈지 등등
    private void Awake()
    {
        DataManager.Instance.LoadData();
        _blockDatas = DataManager.Instance.Data;
    }
    private void Start()
    {
        CreateBlocks();
    }

    private void Update()
    {
        if(Keyboard.current.f1Key.wasPressedThisFrame)
        {
            _blocks[EBlockType.I].Pop();
        }
        else if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            _blocks[EBlockType.J].Pop();
        }
        else if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            _blocks[EBlockType.L].Pop();
        }
    }

    private void CreateBlocks()
    {
        string path = "Prefabs/Block/";
        Transform child = transform.Find("Blocks");
        foreach (BlockData data in _blockDatas)
        {
            _blocks[data.type] = new PoolingManager("Prefabs/Block/" + data.name, child.gameObject);
        }
    }
}
