using System.Drawing;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    float cameraDistance;
    [SerializeField] float sensitivity;
    public float minZoom;
    public float maxZoom;
    public float smoothSpeed = 5f;
    float targetZoom;
    float currentZoom;
    private void Start()
    {
        currentZoom = virtualCamera.m_Lens.OrthographicSize;
        targetZoom = currentZoom;
    }
    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            targetZoom -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;  // Изменяем целевой зум
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);  // Ограничиваем целевой зум
           
        }
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, smoothSpeed * Time.deltaTime);
        virtualCamera.m_Lens.OrthographicSize = currentZoom;
    }

    void Update()
    {
        Zoom();
    }
}
