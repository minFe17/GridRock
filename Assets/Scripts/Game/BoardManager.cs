using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance => Utils.MonoSingleton<BoardManager>.Instance;

    private Dictionary<EBlockType, PoolingManager> _blocks = new Dictionary<EBlockType, PoolingManager>();

    private List<BlockData> _blockDatas;
    private List<BlockData> _randomBlocks = new List<BlockData>();

    private BlockController _dropBlock;
    private bool _isDrop = false;


    private Vector2 _spawnPosition = new Vector2(0, 3.9f);
    //이거 고민 좀 해봐야됨 블럭 전부 사라졌으면 다시 풀링에 집어넣는 방식으로갈지 등등

    public List<BlockData> RandomBlocks
    {
        get { return _randomBlocks; }
    }
    public BlockController DropBlock
    {
        get { return _dropBlock; }
    }

    public bool IsBlockDropping => _isDrop;

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
        if (_randomBlocks.Count == 0)
            GetRandomBlocks();
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            SelectBlock(1);
        }
        else if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            SelectBlock(2);
        }
        else if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            SelectBlock(3);
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

    private void GetRandomBlocks()
    {
        for (int i = 0; i < 3; i++)
        {
            _randomBlocks.Add(_blockDatas[Random.Range(0, _blockDatas.Count)]);
            Debug.Log(_randomBlocks[i].name);
        }

        UpdateBlockContext();
    }

    private void UpdateBlockContext()
    {
        List<BlockOptionContext> blocks = new List<BlockOptionContext>();

        for (int i = 0; i < _randomBlocks.Count; i++)
            blocks.Add(new BlockOptionContext(_randomBlocks[i].type));

        SimpleSingleton<AIContextBuilder>.Instance.AvailableBlocks = blocks;
    }

    public void SelectBlock(int num)
    {
        TrySelectBlockSlot(num);
    }

    public bool TrySelectBlockSlot(int slotNumber)
    {
        if (slotNumber < 1)
            return false;

        if (_isDrop)
            return false;

        int index = slotNumber - 1;
        if (index < 0 || index >= _randomBlocks.Count)
            return false;

        BlockData selectedData = _randomBlocks[index];
        GameObject block = _blocks[selectedData.type].Pop();
        block.transform.localPosition = _spawnPosition;

        _dropBlock = block.GetComponent<BlockController>();
        _dropBlock.StartDrop();

        _isDrop = true;

        _randomBlocks.RemoveAt(index);

        BlockContext activeBlock = new BlockContext(selectedData.type, 0);
        SimpleSingleton<AIContextBuilder>.Instance.ActiveBlock = activeBlock;
        UpdateBlockContext();
        return true;
    }

    public void NotifyDropFinished(BlockController block)
    {
        if (_dropBlock == block)
            _isDrop = false;
    }
}