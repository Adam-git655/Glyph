using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public void Break()
    {
        gameObject.SetActive(false); 
    }
}
