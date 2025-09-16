using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Smoothing")]
    [Tooltip("Time used by SmoothDamp (lower = snappier).")]
    [SerializeField] private float _smoothTime = 0.05f;

    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        if (_playerTransform == null)
            Debug.LogWarning("CameraFollow: playerTransform not assigned!");
    }

    private void LateUpdate()
    {
        if (_playerTransform == null) return;
        Vector3 target = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, _smoothTime);
    }
}
