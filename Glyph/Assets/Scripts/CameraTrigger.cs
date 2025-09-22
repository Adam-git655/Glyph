using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraManager cameraManager;

    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    public CinemachineCamera cameraOnLeft;
    public CinemachineCamera cameraOnRight;

    public PanDirection panDirection;
    public float panDistance = 3f;
    public float panTime = 0.35f;

    private BoxCollider2D coll;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (panCameraOnContact)
            {
                cameraManager.PanCameraOnContact(panDistance, panTime, panDirection, false);
            }
        }
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

            if (panCameraOnContact)
            {
                cameraManager.PanCameraOnContact(panDistance, panTime, panDirection, true);
            }
        }
    }
}

public enum PanDirection
{
    Up,
    Down,
    Left, 
    Right
}

[CustomEditor(typeof(CameraTrigger))]
public class MyScriptEditor : Editor
{
    SerializedProperty cameraManagerProp;
    SerializedProperty swapCamerasProp;
    SerializedProperty panCameraOnContactProp;
    SerializedProperty cameraOnLeftProp;
    SerializedProperty cameraOnRightProp;
    SerializedProperty panDirectionProp;
    SerializedProperty panDistanceProp;
    SerializedProperty panTimeProp;

    private void OnEnable()
    {
        cameraManagerProp = serializedObject.FindProperty("cameraManager");
        swapCamerasProp = serializedObject.FindProperty("swapCameras");
        panCameraOnContactProp = serializedObject.FindProperty("panCameraOnContact");
        cameraOnLeftProp = serializedObject.FindProperty("cameraOnLeft");
        cameraOnRightProp = serializedObject.FindProperty("cameraOnRight");
        panDirectionProp = serializedObject.FindProperty("panDirection");
        panDistanceProp = serializedObject.FindProperty("panDistance");
        panTimeProp = serializedObject.FindProperty("panTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(cameraManagerProp);
        EditorGUILayout.PropertyField(swapCamerasProp);
        EditorGUILayout.PropertyField(panCameraOnContactProp);

        if (swapCamerasProp.boolValue)
        {
            EditorGUILayout.PropertyField(cameraOnLeftProp);
            EditorGUILayout.PropertyField(cameraOnRightProp);
        }
        if (panCameraOnContactProp.boolValue)
        {
            EditorGUILayout.PropertyField(panDirectionProp);
            EditorGUILayout.PropertyField(panDistanceProp);
            EditorGUILayout.PropertyField(panTimeProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
