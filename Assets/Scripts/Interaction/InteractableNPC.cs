using UnityEngine;

public class InteractableNPC : MonoBehaviour, IInteractable // Implement IInteractable
{
    public DialogueManager dialogueManager; // Reference ไปยัง DialogueManager
    public Dialogue npcDialogue;           // ชุดบทสนทนาสำหรับ NPC นี้

    void Start()
    {
        // ตรวจสอบว่า DialogueManager ถูกกำหนดไว้
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>(); // พยายามหาใน Scene
        }
    }

    public void Interact() // เมธอด Interact จาก Interface IInteractable
    {
        Debug.Log("Interacting with NPC: " + gameObject.name);
        if (dialogueManager != null && npcDialogue != null)
        {
            dialogueManager.StartDialogue(npcDialogue); // เริ่มบทสนทนา
        }
        else
        {
            Debug.LogWarning("Dialogue Manager or NPC Dialogue not set for " + gameObject.name);
        }
    }
}