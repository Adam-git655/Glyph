using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField] private float minThrowForce;
    [SerializeField] private float maxThrowForce;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    public void Init(Vector2 direction , float throwForce)
    {
        if(_rb == null) print("rb is Missing!");
        
        float tForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        _rb.linearVelocity = direction * tForce;
    }
}
