using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera currentCamera;
    private CinemachinePositionComposer cinemachineRotationComposer;

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;
    private Vector2 _startingTrackedObjectOffset;

    private void Start()
    {
        if (currentCamera != null)
        {
            cinemachineRotationComposer = currentCamera.GetComponent<CinemachinePositionComposer>();
            _startingTrackedObjectOffset = cinemachineRotationComposer.TargetOffset;
        }
    }

    #region PanCamera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));  
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startPos = Vector2.zero;

        if (!panToStartingPos)
        {
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            startPos = _startingTrackedObjectOffset;
            endPos += startPos;
        }
        else
        {
            startPos = cinemachineRotationComposer.TargetOffset;
            endPos = _startingTrackedObjectOffset;
        }

        //Pan camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startPos, endPos, (elapsedTime / panTime));
            cinemachineRotationComposer.TargetOffset = panLerp;
            yield return null;
        }
    }
    #endregion

    #region SwapCamera
    public void SwapCameras(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //If current camera is the camera on the left, and the exit direction is towards the right then swap the camera
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true; //Enable the new camera

            cameraFromLeft.enabled = false; //Disable the old camera

            currentCamera = cameraFromRight; //Set current camera to new camera

            cinemachineRotationComposer = currentCamera.GetComponent<CinemachinePositionComposer>();
            _startingTrackedObjectOffset = cinemachineRotationComposer.TargetOffset;
        }

        //If current camera is the camera on the right, and the exit direction is towards the left then swap the camera
        if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true; //Enable the new camera

            cameraFromRight.enabled = false; //Disable the old camera

            currentCamera = cameraFromLeft; //Set current camera to new camera

            cinemachineRotationComposer = currentCamera.GetComponent<CinemachinePositionComposer>();
            _startingTrackedObjectOffset = cinemachineRotationComposer.TargetOffset;
        }
    }
    #endregion
}
