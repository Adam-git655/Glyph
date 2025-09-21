using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController _instance;

    [SerializeField] private CinemachineCamera[] _cinemachineCameras;

    [Header("Control for lerping the Y Damping during player jump/ fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool _isLerpingYDamping { get; private set; }
    public bool _lerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _positionTransposer;

    private float _normYPanAmount;
    private Vector3 _startingTrackedObjectOffset;

    [Header("Horizontal look-ahead")]
    [Tooltip("How much extra offset to add in front of the player when facing that direction.")]
    [SerializeField] private float _horizontalExtraOffset = 2f;
    [Tooltip("Time to lerp the TargetOffset.x to the new value.")]
    [SerializeField] private float _offsetLerpTime = 0.12f;
    private Coroutine _offsetLerpCoroutine;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if (_instance != this) _instance = this;

        RefreshCurrentCamera();
    }

    public void RefreshCurrentCamera()
    {
        if (_cinemachineCameras != null && _cinemachineCameras.Length > 0)
        {
            for (int i = 0; i < _cinemachineCameras.Length; i++)
            {
                if (_cinemachineCameras[i] != null && _cinemachineCameras[i].enabled)
                {
                    _currentCamera = _cinemachineCameras[i];
                    break;
                }
            }
        }

        if (_currentCamera == null)
            _currentCamera = GetComponentInChildren<CinemachineCamera>();

        if (_currentCamera == null)
        {
            Debug.LogError("[CameraController] No CinemachineCamera found. Disabling CameraController.");
            enabled = false;
            return;
        }

        var compBase = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (compBase == null)
        {
            Debug.LogError("[CameraController] GetCinemachineComponent returned null.");
            return;
        }

        _positionTransposer = compBase as CinemachinePositionComposer;
        if (_positionTransposer == null)
        {
            Debug.LogError("[CameraController] Body is not a PositionComposer.");
            return;
        }

        _normYPanAmount = _positionTransposer.Damping.y;
        _startingTrackedObjectOffset = _positionTransposer.TargetOffset;
    }

    #region Lerp the Y Damping
    public void LerpYDamping(bool isPlayerFalling)
    {
        if (_lerpYPanCoroutine != null) StopCoroutine(_lerpYPanCoroutine);
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        _isLerpingYDamping = true;

        float startDampAmount = _positionTransposer.Damping.y;
        float endDampAmount = isPlayerFalling ? _fallPanAmount : _normYPanAmount;

        _lerpedFromPlayerFalling = isPlayerFalling;

        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _fallYPanTime);
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, t);

            Vector3 damping = _positionTransposer.Damping;
            damping.y = lerpedPanAmount;
            _positionTransposer.Damping = damping;

            yield return null;
        }

        Vector3 finalDamping = _positionTransposer.Damping;
        finalDamping.y = endDampAmount;
        _positionTransposer.Damping = finalDamping;

        _isLerpingYDamping = false;
        _lerpYPanCoroutine = null;
    }
    #endregion

    public void SetFacingDirection(bool isFacingRight)
    {
        if (_positionTransposer == null) return;

        float baseX = _startingTrackedObjectOffset.x;
        float extra = Mathf.Abs(_horizontalExtraOffset);
        float targetX = baseX + (isFacingRight ? extra : -extra);

        if (_offsetLerpCoroutine != null) StopCoroutine(_offsetLerpCoroutine);
        _offsetLerpCoroutine = StartCoroutine(LerpTargetOffsetX(targetX, _offsetLerpTime));
    }

    public void SetFacingDirectionImmediate(bool isFacingRight)
    {
        if (_positionTransposer == null) return;

        float baseX = _startingTrackedObjectOffset.x;
        float extra = Mathf.Abs(_horizontalExtraOffset);
        float targetX = baseX + (isFacingRight ? extra : -extra);

        Vector3 t = _positionTransposer.TargetOffset;
        t.x = targetX;
        _positionTransposer.TargetOffset = t;
    }

    private IEnumerator LerpTargetOffsetX(float targetX, float duration)
    {
        float startX = _positionTransposer.TargetOffset.x;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curX = Mathf.Lerp(startX, targetX, t);
            Vector3 newOffset = _positionTransposer.TargetOffset;
            newOffset.x = curX;
            _positionTransposer.TargetOffset = newOffset;
            yield return null;
        }

        Vector3 final = _positionTransposer.TargetOffset;
        final.x = targetX;
        _positionTransposer.TargetOffset = final;

        _offsetLerpCoroutine = null;
    }

    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        if (_panCameraCoroutine != null) StopCoroutine(_panCameraCoroutine);
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPosition));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        Vector3 endPosition;
        Vector3 startPosition = _positionTransposer.TargetOffset;

        if (!panToStartingPosition)
        {
            Vector3 dir = Vector3.zero;
            switch (panDirection)
            {
                case PanDirection.Left: dir = Vector3.left; break;
                case PanDirection.Right: dir = Vector3.right; break;
                case PanDirection.Up: dir = Vector3.up; break;
                case PanDirection.Down: dir = Vector3.down; break;
            }
            endPosition = startPosition + dir * panDistance;
        }
        else
        {
            endPosition = _startingTrackedObjectOffset;
        }

        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            _positionTransposer.TargetOffset = Vector3.Lerp(startPosition, endPosition, elapsedTime / panTime);
            yield return null;
        }

        _positionTransposer.TargetOffset = endPosition;
    }
    #endregion

    #region Swap Camera
    public void SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 playerExitDirection)
    {
        if (cameraFromLeft == null || cameraFromRight == null) return;
        if (_currentCamera == cameraFromLeft && playerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;
            cameraFromLeft.enabled = false;
            _currentCamera = cameraFromRight;
            RefreshCurrentCamera(); 
        }
        else if (_currentCamera == cameraFromRight && playerExitDirection.x < 0f)
        {
            cameraFromRight.enabled = false;
            cameraFromLeft.enabled = true;
            _currentCamera = cameraFromLeft;
            RefreshCurrentCamera();
        }
    }
    #endregion
}
