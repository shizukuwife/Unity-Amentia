using UnityEngine;

// Implement IInteractable เพื่อให้ PlayerInteraction สามารถ Interact ได้
public class InteractableChurch : MonoBehaviour, IInteractable
{
    public CGDisplayManager cgDisplayManager; // Reference ไปยัง CGDisplayManager
    public CGSequence churchCGSequence;       // ลำดับ CG ที่จะแสดงเมื่อ Interact กับโบสถ์

    void Start()
    {
        // พยายามหา CGDisplayManager หากไม่ได้กำหนดใน Inspector
        if (cgDisplayManager == null)
        {
            cgDisplayManager = FindObjectOfType<CGDisplayManager>();
        }
        if (cgDisplayManager == null)
        {
            Debug.LogError("InteractableChurch: CGDisplayManager not found or assigned for InteractableChurch on " + gameObject.name + ". Please assign it in the Inspector."); // Error Debug
        }
        if (churchCGSequence == null || churchCGSequence.cgFrames == null || churchCGSequence.cgFrames.Length == 0)
        {
            Debug.LogWarning("InteractableChurch: No CG Sequence assigned or it's empty for InteractableChurch on " + gameObject.name + ". Please set it up in the Inspector."); // Warning Debug
        }
    }

    // เมธอด Interact จาก Interface IInteractable
    public void Interact()
    {
        Debug.Log("InteractableChurch: Interact method called on Church: " + gameObject.name); // Debug
        if (cgDisplayManager != null && churchCGSequence != null && churchCGSequence.cgFrames.Length > 0)
        {
            cgDisplayManager.StartCGSequence(churchCGSequence); // เริ่มแสดง CG Sequence
            Debug.Log("InteractableChurch: Started CG Sequence for Church."); // Debug
        }
        else
        {
            Debug.LogWarning("InteractableChurch: Cannot start CG Sequence for Church. Check CGDisplayManager and Church CG Sequence assignments in Inspector."); // Warning Debug
        }
    }
}