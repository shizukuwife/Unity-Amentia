using System; // สำหรับ Action (Event)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // สำหรับ TextMeshPro
using UnityEngine.UI; // สำหรับ UI Image, GameObject

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText; // ข้อความบทสนทนา
    public TextMeshProUGUI characterNameText; // ชื่อตัวละคร
    public Image characterSpriteImage; // รูปตัวละคร
    public GameObject dialoguePanel; // Panel หลักของ UI บทสนทนาทั้งหมด

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f; // ความเร็วในการพิมพ์ข้อความ

    // คิวสำหรับเก็บประโยค ชื่อ และสไปรท์ของตัวละคร
    // ทำให้เป็น public เพื่อให้ CGDisplayManager สามารถตรวจสอบ sentences.Count ได้
    public Queue<string> sentences;
    private Queue<string> characterNames;
    private Queue<Sprite> characterSprites;

    // Event ที่จะถูกเรียกเมื่อ Dialogue จบลง
    // static Event สามารถถูกสมัครรับได้จากที่ไหนก็ได้
    public static event Action OnDialogueEnded;

    // ตัวแปรสถานะเพื่อบอกว่า DialogueManager กำลังถูกควบคุมโดย Script ภายนอกหรือไม่
    private bool isControlledByExternal = false;

    void Awake()
    {
        sentences = new Queue<string>();
        characterNames = new Queue<string>();
        characterSprites = new Queue<Sprite>();

        if (dialoguePanel != null) // ป้องกัน NullReferenceException
        {
            dialoguePanel.SetActive(false); // ซ่อน Panel บทสนทนาเมื่อเริ่มต้น
            Debug.Log("DialogueManager: Awake - Dialogue Panel set to inactive initially."); // Debug
        }
        else
        {
            Debug.LogError("DialogueManager: Awake - dialoguePanel is not assigned! Please assign it in the Inspector."); // Error Debug
        }
    }

    // เมธอดสำหรับเริ่มบทสนทนา
    // controlledByExternal = true หมายความว่า CGDisplayManager จะควบคุมการกด
    public void StartDialogue(Dialogue dialogue, bool controlledByExternal = false)
    {
        Debug.Log($"DialogueManager: StartDialogue called. Dialogue panel active: {(dialoguePanel != null ? dialoguePanel.activeSelf.ToString() : "N/A")}, Controlled by external: {controlledByExternal}"); // Debug

        this.isControlledByExternal = controlledByExternal; // ตั้งค่าสถานะการควบคุม

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true); // เปิด Panel บทสนทนา
            Debug.Log("DialogueManager: Dialogue Panel set to active."); // Debug
        }
        else
        {
            Debug.LogError("DialogueManager: StartDialogue - dialoguePanel is null, cannot activate! Please assign it."); // Error Debug
            return; // หยุดการทำงานถ้า Panel เป็น null
        }

        if (dialogue == null)
        {
            Debug.LogError("DialogueManager: StartDialogue - Input Dialogue is null! Cannot start dialogue."); // Error Debug
            EndDialogue(); // จบ Dialogue เลยถ้าข้อมูลเป็น null
            return;
        }

        sentences.Clear(); // ล้างคิวเก่า
        characterNames.Clear();
        characterSprites.Clear();
        Debug.Log("DialogueManager: Queues cleared."); // Debug

        if (dialogue.dialogueLines == null)
        {
            Debug.LogError("DialogueManager: StartDialogue - dialogueLines array is null within the Dialogue object!"); // Error Debug
            EndDialogue();
            return;
        }

        if (dialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("DialogueManager: StartDialogue - Dialogue has no lines. Ending dialogue immediately."); // Warning Debug
            EndDialogue();
            return;
        }

        // เพิ่มทุกบรรทัดบทสนทนาลงในคิว
        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            sentences.Enqueue(line.sentence);
            characterNames.Enqueue(line.characterName);
            characterSprites.Enqueue(line.characterSprite);
        }
        Debug.Log($"DialogueManager: {sentences.Count} sentences enqueued."); // Debug

        DisplayNextSentence(); // แสดงประโยคแรกทันที
    }

    // เมธอดสำหรับแสดงประโยคถัดไป
    public void DisplayNextSentence()
    {
        Debug.Log($"DialogueManager: DisplayNextSentence called. Sentences remaining: {sentences.Count}"); // Debug

        if (sentences.Count == 0)
        {
            Debug.Log("DialogueManager: No more sentences in queue. Ending dialogue."); // Debug
            EndDialogue(); // ถ้าไม่มีประโยคแล้ว ก็จบบทสนทนา
            return;
        }

        string currentSentence = sentences.Dequeue(); // ดึงประโยคถัดไป
        string currentName = characterNames.Dequeue();
        Sprite currentSprite = characterSprites.Dequeue();

        // ตรวจสอบและตั้งค่า UI Elements
        if (characterNameText != null) characterNameText.text = currentName;
        else Debug.LogError("DialogueManager: characterNameText is not assigned! Please assign it."); // Error Debug

        if (characterSpriteImage != null) characterSpriteImage.sprite = currentSprite;
        else Debug.LogError("DialogueManager: characterSpriteImage is not assigned! Please assign it."); // Error Debug

        Debug.Log($"DialogueManager: Displaying sentence '{currentSentence}' from character '{currentName}'."); // Debug

        StopAllCoroutines(); // หยุด Coroutine การพิมพ์ข้อความเก่า
        StartCoroutine(TypeSentence(currentSentence)); // เริ่มพิมพ์ข้อความใหม่
    }

    // Coroutine สำหรับการพิมพ์ข้อความทีละตัวอักษร
    IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText == null)
        {
            Debug.LogError("DialogueManager: dialogueText is not assigned, cannot type sentence! Please assign it."); // Error Debug
            yield break; // หยุด Coroutine ทันที
        }

        dialogueText.text = ""; // เคลียร์ข้อความเก่า
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter; // เพิ่มตัวอักษรทีละตัว
            yield return new WaitForSeconds(typingSpeed); // รอตามความเร็วที่กำหนด
        }
        Debug.Log("DialogueManager: Finished typing sentence."); // Debug
    }

    // เมธอดสำหรับจบบทสนทนา
    void EndDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // ซ่อน Panel บทสนทนา
            Debug.Log("DialogueManager: End of dialogue. Panel set to inactive."); // Debug
        }
        else
        {
            Debug.LogError("DialogueManager: EndDialogue - dialoguePanel is null, cannot deactivate!"); // Error Debug
        }
        OnDialogueEnded?.Invoke(); // เรียก Event แจ้งว่า Dialogue จบแล้ว
        this.isControlledByExternal = false; // รีเซ็ตสถานะการควบคุม
        Debug.Log("DialogueManager: OnDialogueEnded event invoked. isControlledByExternal reset to false."); // Debug
    }

    // Update จะถูกเรียกทุกเฟรม
    void Update()
    {
        if (dialoguePanel == null) return; // ป้องกัน NullReference ถ้า Panel หายไป

        // ตรวจจับการคลิกเมาส์ซ้ายเพื่อเลื่อน Dialogue
        // จะรับ Input เองก็ต่อเมื่อ Dialogue Panel เปิดอยู่ AND ไม่ได้ถูกควบคุมจากภายนอก
        if (dialoguePanel.activeSelf && !isControlledByExternal && Input.GetMouseButtonDown(0))
        {
            Debug.Log("DialogueManager: Update - Mouse click detected (not controlled externally). Displaying next sentence."); // Debug
            DisplayNextSentence();
        }
    }
}