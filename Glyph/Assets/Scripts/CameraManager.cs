using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera currentCamera;

    public void SwapCameras(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //If current camera is the camera on the left, and the exit direction is towards the right then swap the camera
        if (currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true; //Enable the new camera

            cameraFromLeft.enabled = false; //Disable the old camera

            currentCamera = cameraFromRight; //Set current camera to new camera
        }

        //If current camera is the camera on the right, and the exit direction is towards the left then swap the camera
        if (currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true; //Enable the new camera

            cameraFromRight.enabled = false; //Disable the old camera

            currentCamera = cameraFromLeft; //Set current camera to new camera
        }
    }
}
