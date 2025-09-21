using UnityEngine;

public class SwingingHammer : MonoBehaviour
{
    public float swingSpeed = 2f;
    public Transform leftTip;
    public Transform rightTip;

    private Quaternion leftRotation;
    private Quaternion rightRotation;
    private Vector3 pivot;

    private void Start()
    {
        pivot = transform.position;
        float leftAngle = Mathf.Atan2(leftTip.position.y - pivot.y, leftTip.position.x - pivot.x) * Mathf.Rad2Deg + 90f;
        float rightAngle = Mathf.Atan2(rightTip.position.y - pivot.y, rightTip.position.x - pivot.x) * Mathf.Rad2Deg + 90f;

        leftRotation = Quaternion.Euler(0f, 0f, leftAngle);
        rightRotation = Quaternion.Euler(0f, 0f, rightAngle);
    }

    private void Update()
    {
        float t = (Mathf.Sin(Time.time * swingSpeed) + 1f) / 2f;
        
        transform.rotation = Quaternion.Lerp(leftRotation, rightRotation, t);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("HammerHitPlayer");
        }
    }
}
