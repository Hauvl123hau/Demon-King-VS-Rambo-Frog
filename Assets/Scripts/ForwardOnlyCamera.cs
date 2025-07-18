using UnityEngine;

public class ForwardOnlyCamera : MonoBehaviour
{
    [Header("Nhân vật cần theo dõi")]
    public Transform target;

    [Header("Độ cao cố định của camera (nếu không muốn theo dõi trục Y)")]
    public float fixedY = 0f;

    private float minCameraX;

    void Start()
    {
        minCameraX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 camPos = transform.position;

        if (target.position.x > camPos.x)
        {
            camPos.x = target.position.x;
            minCameraX = camPos.x;
        }
        else
        {
            camPos.x = minCameraX;
        }

        camPos.y = fixedY;

        camPos.z = transform.position.z;

        transform.position = camPos;
    }
}
