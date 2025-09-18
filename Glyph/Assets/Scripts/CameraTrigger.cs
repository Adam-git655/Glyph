using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraManager cameraManager;

    public bool swapCameras = false;
    public CinemachineCamera cameraOnLeft;
    public CinemachineCamera cameraOnRight;

    private BoxCollider2D coll;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;
            if (swapCameras && cameraOnLeft != null && cameraOnRight != null)
            {
                cameraManager.SwapCameras(cameraOnLeft, cameraOnRight, exitDirection);
            }
        }
    }
}

[CustomEditor(typeof(CameraTrigger))]
public class MyScriptEditor : Editor
{
    SerializedProperty cameraManagerProp;
    SerializedProperty swapCamerasProp;
    SerializedProperty cameraOnLeftProp;
    SerializedProperty cameraOnRightProp;

    private void OnEnable()
    {
        cameraManagerProp = serializedObject.FindProperty("cameraManager");
        swapCamerasProp = serializedObject.FindProperty("swapCameras");
        cameraOnLeftProp = serializedObject.FindProperty("cameraOnLeft");
        cameraOnRightProp = serializedObject.FindProperty("cameraOnRight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(cameraManagerProp);
        EditorGUILayout.PropertyField(swapCamerasProp);

        if (swapCamerasProp.boolValue)
        {
            EditorGUILayout.PropertyField(cameraOnLeftProp);
            EditorGUILayout.PropertyField(cameraOnRightProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
