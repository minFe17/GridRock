using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private GameObject _centerBlock;

    public void RotationRight()
    {
        transform.Rotate(0f, 0f, 90f);
    }
    public void RotationLeft() 
    {
        transform.Rotate(0f, 0f, -90f);
    }
    
}
