using Unity.Cinemachine;
using UnityEngine;
/// <summary>
/// Controls the camera distance by reading the mouse scroll wheel.
/// Attach this component to the same object that holds the CinemachineOrbitalFollow.
/// </summary>
/// 
[RequireComponent(typeof(CinemachineOrbitalFollow))]
public class CameraZoomController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader input;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomStep = 0.6f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float zoomSmoothing = 8f;

    private CinemachineOrbitalFollow _orbital;

    private float _targetDistance;

    private void Awake()
    {
        _orbital = GetComponent<CinemachineOrbitalFollow>();
        // Start from whatever radius was configured in the Inspector

        _targetDistance = _orbital.RadialAxis.Value;
    }
    private void Update()
    {
        if (input == null)
        {
            return;
        }

        float scroll = input.ZoomInput;

        // The wheel reports about 120 units per notch, so only the sign is used
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _targetDistance -= Mathf.Sign(scroll) * zoomStep;
            _targetDistance = Mathf.Clamp(_targetDistance, minDistance, maxDistance);
        }

        // Ease towards the target distance so the zoom never jumps in a single frame
        _orbital.RadialAxis.Value = Mathf.Lerp(
        _orbital.RadialAxis.Value, _targetDistance, Time.deltaTime * zoomSmoothing);
    }
}