using UnityEngine;
using Unity.Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraControllTrigger : MonoBehaviour
{
    public CustomInspectorObjects _customInspectorObjects;
    private Collider2D _collider2D;

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
            Debug.LogWarning($"[{nameof(CameraControllTrigger)}] No Collider2D found on '{gameObject.name}'. Add a BoxCollider2D (Is Trigger = true).");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision) return;

        Vector2 exitDirection = (_collider2D != null) ? (collision.transform.position - _collider2D.bounds.center).normalized : Vector2.zero;

        if (collision.CompareTag("Player"))
        {
            if (_customInspectorObjects != null && _customInspectorObjects.panCameraOnContact)
            {
                CameraController._instance?.PanCameraOnContact(
                    _customInspectorObjects._panDistance,
                    _customInspectorObjects._panTime,
                    _customInspectorObjects._panDirection,
                    false
                );
            }

            if (_customInspectorObjects != null && _customInspectorObjects.swapCamera && _customInspectorObjects._cameraOnLeft != null && _customInspectorObjects._cameraOnRight != null)
            {
                CameraController._instance?.SwapCamera(_customInspectorObjects._cameraOnLeft, _customInspectorObjects._cameraOnRight, exitDirection);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision) return;

        if (collision.CompareTag("Player") && _customInspectorObjects != null && _customInspectorObjects.panCameraOnContact)
        {
            CameraController._instance?.PanCameraOnContact(
                _customInspectorObjects._panDistance,
                _customInspectorObjects._panTime,
                _customInspectorObjects._panDirection,
                true
            );
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCamera = false;
    public bool panCameraOnContact = false;
    [HideInInspector] public CinemachineCamera _cameraOnLeft;
    [HideInInspector] public CinemachineCamera _cameraOnRight;

    [HideInInspector] public PanDirection _panDirection;
    [HideInInspector] public float _panDistance = 3f;
    [HideInInspector] public float _panTime = 0.35f;
}

public enum PanDirection
{
    Left,
    Right,
    Up,
    Down
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraControllTrigger))]
public class MyScriptEditorTest : Editor
{
    CameraControllTrigger cameraControllTrigger;

    private void OnEnable()
    {
        cameraControllTrigger = (CameraControllTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControllTrigger._customInspectorObjects != null && cameraControllTrigger._customInspectorObjects.swapCamera)
        {
            cameraControllTrigger._customInspectorObjects._cameraOnLeft =
                EditorGUILayout.ObjectField("Camera On Left",
                    cameraControllTrigger._customInspectorObjects._cameraOnLeft,
                    typeof(CinemachineCamera), true) as CinemachineCamera;

            cameraControllTrigger._customInspectorObjects._cameraOnRight =
                EditorGUILayout.ObjectField("Camera On Right",
                    cameraControllTrigger._customInspectorObjects._cameraOnRight,
                    typeof(CinemachineCamera), true) as CinemachineCamera;
        }

        if (cameraControllTrigger._customInspectorObjects != null && cameraControllTrigger._customInspectorObjects.panCameraOnContact)
        {
            cameraControllTrigger._customInspectorObjects._panDirection =
                (PanDirection)EditorGUILayout.EnumPopup("Pan Direction", cameraControllTrigger._customInspectorObjects._panDirection);

            cameraControllTrigger._customInspectorObjects._panDistance =
                EditorGUILayout.FloatField("Pan Distance", cameraControllTrigger._customInspectorObjects._panDistance);

            cameraControllTrigger._customInspectorObjects._panTime =
                EditorGUILayout.FloatField("Pan Time", cameraControllTrigger._customInspectorObjects._panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControllTrigger);
        }
    }
}
#endif
