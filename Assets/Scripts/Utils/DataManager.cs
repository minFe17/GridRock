using UnityEngine;
using Utils;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;
public class DataManager : SimpleSingleton<DataManager>
{
    private List<BlockData> _data;
    private Dictionary<EBlockType, BlockData> _dictionaryData = new Dictionary<EBlockType, BlockData>();

    public List<BlockData> Data
    {
        get { return _data; } 
    }
    public BlockData FindBlock(EBlockType type)
    { return _dictionaryData[type]; }

    public void LoadData()
    {
        LoadBlockData();
    }
    private void LoadBlockData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/BlockData");
        _data = JsonConvert.DeserializeObject<List<BlockData>>(jsonFile.text);

        foreach (BlockData data in _data)
        {
            _dictionaryData[data.type] = data;
        }
    }
}
