using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BoardManager : MonoBehaviour
{
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
    public void SelectBlock(int num)
    {
        //1,2,3 숫자 선택해서 함수 실행하시면 알아서 스폰하게 뺐습니다.
        //1,2,3 존재시 1 빼시면 2,3 -> 1,2 순서로 바뀌게 구현했습니다~!
        if (_randomBlocks.Count < num) return;
        //블럭 떨어지는 중이면 실행안되게 막기

        int index = num - 1;
        GameObject block = _blocks[_randomBlocks[index].type].Pop();
        block.transform.localPosition = _spawnPosition;
        _dropBlock = block.GetComponent<BlockController>();
        _dropBlock.StartDrop();
        //드롭되는거 코루틴으로 빼야될듯 캐릭터 컨트롤하는거랑 같이 써야되니까.

        _randomBlocks.RemoveAt(index);
    }
    
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
        for(int i=0;i<3;i++)
        {
            _randomBlocks.Add(_blockDatas[Random.Range(0, _blockDatas.Count)]);
            Debug.Log(_randomBlocks[i].name);
        }

    }
}
