using UnityEditor.SearchService;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    const float SPACE_NUM = 0.45f;
    const int Y_LENGTH = 20;
    const int X_LENGTH = 15;
    
    private PoolingManager _poolingManager;

    private void OnEnable()
    {
        MakeBackGround();
    }
    private void OnDisable()
    {
        UnShowBackGround();
    }
    private void UnShowBackGround()
    {
        _poolingManager.ResetObjects();
    }
    private void MakeBackGround()
    {
        _poolingManager = new PoolingManager(Resources.Load<GameObject>("Prefabs/BackGroundBlock"), this.gameObject,500);

        for (int y = 0; y < Y_LENGTH; y++)
        {
            for (int x = 0; x < X_LENGTH; x++)
            {
                GameObject obj = _poolingManager.Pop();
                obj.transform.localPosition = new Vector3(x*SPACE_NUM,y*SPACE_NUM,0);
            }
            
        }
    }
}
