using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public DialogueManager dialogueManager; // Reference ไปยัง DialogueManager
    public Dialogue initialDialogue;       // บทสนทนาเริ่มต้นของเกม

    void Start()
    {
        Debug.Log("GameStartManager: Start called."); // Debug
        if (dialogueManager != null && initialDialogue != null)
        {
            // เรียก StartDialogue โดยไม่ระบุ controlledByExternal (ซึ่งจะใช้ค่า default เป็น false)
            // ทำให้ DialogueManager รู้ว่าต้องจัดการ Input ด้วยตัวเอง
            dialogueManager.StartDialogue(initialDialogue);
            Debug.Log("GameStartManager: Started initial dialogue."); // Debug
        }
        else
        {
            Debug.LogError("GameStartManager: Dialogue Manager or Initial Dialogue not set in GameStartManager! Please assign them in the Inspector."); // Error Debug
        }
    }
}