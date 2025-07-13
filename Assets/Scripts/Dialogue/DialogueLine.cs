using UnityEngine;

// ทำให้สามารถเห็นและแก้ไขได้ใน Inspector ของ Unity
[System.Serializable]
public class DialogueLine
{
    // [TextArea(3, 10)] ทำให้ช่องกรอกข้อความใน Inspector มีหลายบรรทัด
    [TextArea(3, 10)]
    public string sentence; // ข้อความบทสนทนา
    public string characterName; // ชื่อตัวละครที่พูด
    public Sprite characterSprite; // รูปภาพ (Sprite) ของตัวละคร (เช่น ใบหน้า)
}