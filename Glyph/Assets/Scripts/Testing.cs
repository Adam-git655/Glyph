using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ScreenTransition.Instance.FadeIn();
        }
        
        if(Input.GetKeyDown(KeyCode.N)) ScreenTransition.Instance.FadeOut();
    }
}
