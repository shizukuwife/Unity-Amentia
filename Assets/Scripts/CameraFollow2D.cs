using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target; // ตัวละครที่กล้องจะตาม
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10f); // กล้องอยู่ด้านหลัง

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
