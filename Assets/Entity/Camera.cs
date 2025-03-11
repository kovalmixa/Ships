using System.Drawing;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    float cameraDistance;
    [SerializeField] float sensetivity;
    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            cameraDistance = Input.GetAxis("Mouse ScrollWheel") * sensetivity;
            if (componentBase is CinemachineFramingTransposer)
            {
                print(cameraDistance);
                (componentBase as CinemachineFramingTransposer).m_CameraDistance -= cameraDistance;
            }
        }
    }

    void Update()
    {
        Zoom();
    }
}
