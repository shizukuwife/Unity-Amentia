using System; // สำหรับ Action (Event)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // สำหรับ Image
using TMPro; // สำหรับ TextMeshPro (ถ้าใช้ใน DialogueManager)

public class CGDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cgPanel; // GameObject ที่เป็น Panel หรือ Image หลักสำหรับ CG
    public Image cgImage;      // Image component ที่จะใช้แสดง Sprite ของ CG

    public DialogueManager dialogueManager; // Reference ไปยัง DialogueManager

    private Queue<CGFrame> cgFramesQueue; // คิวสำหรับเก็บ CGFrame
    private bool isCgActive = false; // สถานะว่า CG Sequence กำลังแสดงอยู่หรือไม่
    private bool isWaitingForDialogueEnd = false; // สถานะว่ากำลังรอ Dialogue จบในเฟรมปัจจุบัน

    void Awake()
    {
        cgFramesQueue = new Queue<CGFrame>();

        if (cgPanel != null)
        {
            cgPanel.SetActive(false); // ซ่อน CG Panel ไว้ตอนเริ่มต้น
            Debug.Log("CGDisplayManager: Awake - CG Panel set to inactive initially."); // Debug
        }
        else
        {
            Debug.LogError("CGDisplayManager: Awake - cgPanel is not assigned! Please assign it in the Inspector."); // Error Debug
        }

        if (cgImage == null) Debug.LogError("CGDisplayManager: Awake - cgImage is not assigned! Please assign it in the Inspector."); // Error Debug

        // พยายามหา DialogueManager หากไม่ได้กำหนดใน Inspector
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            if (dialogueManager == null) Debug.LogError("CGDisplayManager: Awake - DialogueManager not found or assigned! Please assign it."); // Error Debug
            else Debug.Log("CGDisplayManager: Awake - DialogueManager found via FindObjectOfType."); // Debug
        }
        else
        {
            Debug.Log("CGDisplayManager: Awake - DialogueManager assigned in Inspector."); // Debug
        }
    }

    void OnEnable()
    {
        // สมัครรับ Event เมื่อ Dialogue จบ (แก้ Error CS0070)
        DialogueManager.OnDialogueEnded += HandleDialogueEnded;
        Debug.Log("CGDisplayManager: OnEnable - Subscribed to DialogueManager.OnDialogueEnded."); // Debug
    }

    void OnDisable()
    {
        // ยกเลิกการสมัครรับ Event เมื่อ GameObject ถูกปิดใช้งานหรือถูกทำลาย (แก้ Error CS0070)
        DialogueManager.OnDialogueEnded -= HandleDialogueEnded;
        Debug.Log("CGDisplayManager: OnDisable - Unsubscribed from DialogueManager.OnDialogueEnded."); // Debug
    }

    // เมธอดนี้จะถูกเรียกโดย DialogueManager เมื่อ Dialogue จบ
    private void HandleDialogueEnded()
    {
        isWaitingForDialogueEnd = false; // ตั้งค่าสถานะเป็น false
        Debug.Log("CGDisplayManager: HandleDialogueEnded - Dialogue has ended. isWaitingForDialogueEnd set to false."); // Debug
        // เราจะไม่เรียก DisplayNextCGFrame ที่นี่ทันที แต่จะรอ Input ใน Update()
    }

    // เมธอดสำหรับเริ่มต้น Sequence ของ CG และ Dialogue
    public void StartCGSequence(CGSequence sequence)
    {
        Debug.Log($"CGDisplayManager: StartCGSequence called. isCgActive: {isCgActive}"); // Debug
        if (isCgActive) return; // ป้องกันการเรียกซ้อน

        if (sequence == null || sequence.cgFrames == null || sequence.cgFrames.Length == 0)
        {
            Debug.LogWarning("CGDisplayManager: Attempted to start an empty CG sequence. Ending sequence."); // Warning Debug
            EndCGSequence();
            return;
        }

        isCgActive = true;
        if (cgPanel != null)
        {
            cgPanel.SetActive(true); // แสดง CG Panel
            Debug.Log("CGDisplayManager: CG Panel set to active."); // Debug
        }
        else
        {
            Debug.LogError("CGDisplayManager: cgPanel is null, cannot activate! Please assign it."); // Error Debug
            return;
        }

        cgFramesQueue.Clear(); // ล้างคิวเก่า
        foreach (CGFrame frame in sequence.cgFrames)
        {
            cgFramesQueue.Enqueue(frame);
        }
        Debug.Log($"CGDisplayManager: CG frames enqueued. Total frames: {cgFramesQueue.Count}"); // Debug

        DisplayNextCGFrame(); // เริ่มแสดงเฟรมแรก
    }

    // แสดง CG Frame ถัดไป หรือเลื่อน Dialogue ในเฟรมปัจจุบัน
    public void DisplayNextCGFrame()
    {
        Debug.Log($"CGDisplayManager: DisplayNextCGFrame called. isWaitingForDialogueEnd: {isWaitingForDialogueEnd}, CG frames left: {cgFramesQueue.Count}"); // Debug

        if (isWaitingForDialogueEnd)
        {
            Debug.Log("CGDisplayManager: Still waiting for dialogue to end. Skipping frame advance."); // Debug
            return; // ป้องกันการกดไปเฟรมถัดไปขณะ Dialogue กำลังทำงาน
        }

        // ตรวจสอบว่า Dialogue ของเฟรมปัจจุบันยังแสดงอยู่หรือไม่ (และยังมีประโยคที่เหลือ)
        if (dialogueManager != null && dialogueManager.dialoguePanel != null && dialogueManager.dialoguePanel.activeSelf && dialogueManager.sentences.Count > 0)
        {
            Debug.Log($"CGDisplayManager: Dialogue in current frame still active. Sentences left: {dialogueManager.sentences.Count}. Advancing sentence."); // Debug
            dialogueManager.DisplayNextSentence();
            return; // ยังคงอยู่ใน Dialogue ของเฟรมปัจจุบัน
        }
        else if (dialogueManager == null)
        {
            Debug.LogError("CGDisplayManager: dialogueManager is null when trying to check dialogue status!"); // Error Debug
        }
        else if (dialogueManager.dialoguePanel == null)
        {
            Debug.LogError("CGDisplayManager: dialogueManager.dialoguePanel is null when trying to check dialogue status!"); // Error Debug
        }
        else if (!dialogueManager.dialoguePanel.activeSelf)
        {
            Debug.Log("CGDisplayManager: Dialogue Panel is not active, likely no dialogue or it already ended."); // Debug
        }
        else if (dialogueManager.sentences.Count == 0)
        {
            Debug.Log("CGDisplayManager: Dialogue has no more sentences (already ended or empty)."); // Debug
        }

        // ถ้ามาถึงตรงนี้ หมายความว่า Dialogue ของเฟรมปัจจุบันจบแล้ว หรือไม่มี Dialogue เลยในเฟรมนั้น
        if (cgFramesQueue.Count == 0)
        {
            Debug.Log("CGDisplayManager: No more CG frames in queue. Ending sequence."); // Debug
            EndCGSequence();
            return;
        }

        CGFrame currentFrame = cgFramesQueue.Dequeue(); // ดึง CG Frame ถัดไป
        Debug.Log($"CGDisplayManager: Dequeued new CG Frame. CG Sprite: {(currentFrame.cgSprite != null ? currentFrame.cgSprite.name : "None")}"); // Debug

        // --- แสดง CG Sprite ---
        if (cgImage != null && currentFrame.cgSprite != null)
        {
            cgImage.sprite = currentFrame.cgSprite;
            cgImage.gameObject.SetActive(true); // ตรวจสอบให้แน่ใจว่า Image component ถูกเปิดใช้งาน
            Debug.Log($"CGDisplayManager: CG Image set to {currentFrame.cgSprite.name} and activated."); // Debug
        }
        else
        {
            Debug.LogWarning("CGDisplayManager: CG Sprite missing for current frame or cgImage is null. Showing blank image."); // Warning Debug
            if (cgImage != null)
            {
                cgImage.sprite = null; // ตั้งเป็น null เพื่อให้ว่างเปล่า
                cgImage.gameObject.SetActive(true);
            }
        }

        // --- แสดง Dialogue สำหรับ CG Frame นี้ (ถ้ามี) ---
        if (dialogueManager != null && currentFrame.dialogue != null && currentFrame.dialogue.dialogueLines.Length > 0)
        {
            Debug.Log($"CGDisplayManager: Starting dialogue for current CG Frame. Dialogue lines count: {currentFrame.dialogue.dialogueLines.Length}"); // Debug
            dialogueManager.StartDialogue(currentFrame.dialogue, true); // บอก DialogueManager ว่า CGDisplayManager จะเป็นคนควบคุม Input
            isWaitingForDialogueEnd = true; // ตั้งค่าสถานะว่ากำลังรอ Dialogue จบ
            Debug.Log("CGDisplayManager: isWaitingForDialogueEnd set to true."); // Debug
        }
        else
        {
            Debug.LogWarning("CGDisplayManager: No dialogue object or dialogue lines for current CG Frame."); // Warning Debug
            if (dialogueManager != null && dialogueManager.dialoguePanel != null)
            {
                dialogueManager.dialoguePanel.SetActive(false); // หากไม่มี Dialogue ในเฟรมนี้ ให้ซ่อน Dialogue Panel
                Debug.Log("CGDisplayManager: Dialogue Panel hidden as no dialogue for this frame."); // Debug
            }
            isWaitingForDialogueEnd = false; // ถ้าไม่มี Dialogue ก็ไม่ต้องรอ
            Debug.Log("CGDisplayManager: isWaitingForDialogueEnd set to false (no dialogue)."); // Debug
        }
    }

    void EndCGSequence()
    {
        Debug.Log("CGDisplayManager: EndCGSequence called."); // Debug
        isCgActive = false;
        if (cgPanel != null) cgPanel.SetActive(false);
        if (cgImage != null) cgImage.sprite = null; // เคลียร์ Sprite
        if (dialogueManager != null && dialogueManager.dialoguePanel != null)
        {
            dialogueManager.dialoguePanel.SetActive(false); // ซ่อน Dialogue Panel ด้วย
        }
        isWaitingForDialogueEnd = false; // รีเซ็ตสถานะ

        Debug.Log("CGDisplayManager: CG Sequence Ended. UI hidden."); // Debug
        // เพิ่ม Logic หลัง CG จบ (เช่น กลับสู่การควบคุม Player, โหลด Scene ใหม่)
    }

    // Update จะถูกเรียกทุกเฟรม
    void Update()
    {
        // รับ Input เมื่อ CG Sequence กำลังทำงานและผู้เล่นคลิกเมาส์ซ้าย
        if (isCgActive && Input.GetMouseButtonDown(0))
        {
            Debug.Log("CGDisplayManager: Update - Mouse click detected (CG active)."); // Debug

            // ถ้า Dialogue ของเฟรมปัจจุบันกำลังแสดงอยู่ และยังไม่จบประโยคสุดท้าย
            if (dialogueManager != null && dialogueManager.dialoguePanel != null && dialogueManager.dialoguePanel.activeSelf && dialogueManager.sentences.Count > 0)
            {
                Debug.Log("CGDisplayManager: Update - Dialogue is active, advancing sentence."); // Debug
                dialogueManager.DisplayNextSentence(); // สั่งให้ DialogueManager แสดงประโยคถัดไป
            }
            else // ถ้า Dialogue จบแล้ว หรือไม่มี Dialogue ให้เลื่อนไป CG Frame ถัดไป
            {
                Debug.Log("CGDisplayManager: Update - Dialogue ended or not present. Advancing CG frame."); // Debug
                DisplayNextCGFrame(); // สั่งให้ CGDisplayManager แสดง CG Frame ถัดไป
            }
        }
    }
}