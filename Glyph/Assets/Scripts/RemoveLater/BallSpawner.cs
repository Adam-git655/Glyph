using Unity.Mathematics;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    
    private void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(ballPrefab, mousePosition, quaternion.identity);
        }
    }
}
