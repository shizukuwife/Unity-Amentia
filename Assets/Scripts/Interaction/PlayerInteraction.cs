using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRadius = 1f; // รัศมีการตรวจจับการ Interact
    public LayerMask interactableLayer; // Layer ที่กำหนดให้กับ Object ที่ Interact ได้

    void Update()
    {
        // ตรวจจับ Input การกดปุ่ม Interact (เช่น 'E' หรือ 'F')
        // หรือสามารถเปลี่ยนเป็น Input.GetMouseButtonDown(0) ถ้าต้องการคลิก
        if (Input.GetKeyDown(KeyCode.E)) // หรือ Input.GetMouseButtonDown(0)
        {
            // ใช้ Physics2D.OverlapCircle เพื่อหา Collider2D ในระยะรัศมี
            Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, interactionRadius, interactableLayer);

            if (hitCollider != null)
            {
                Debug.Log("PlayerInteraction: Found interactable: " + hitCollider.name); // Debug
                // พยายามดึง IInteractable component จาก object ที่ตรวจจับได้
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    Debug.Log("PlayerInteraction: Calling Interact() on " + hitCollider.name); // Debug
                    interactable.Interact(); // เรียกเมธอด Interact ของ object นั้น
                }
                else
                {
                    Debug.LogWarning("PlayerInteraction: Object " + hitCollider.name + " on Interactable layer does not have an IInteractable component."); // Warning Debug
                }
            }
            else
            {
                Debug.Log("PlayerInteraction: No interactable object found in range."); // Debug (อาจจะขึ้นบ่อยถ้าไม่มีอะไรให้ Interact)
            }
        }
    }

    // วาดรัศมีการตรวจจับใน Scene view เพื่อช่วยในการดีบั๊ก
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}