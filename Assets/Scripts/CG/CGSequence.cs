using UnityEngine;

// ทำให้สามารถเห็นและแก้ไขได้ใน Inspector ของ Unity
[System.Serializable]
public class CGSequence
{
    public CGFrame[] cgFrames; // อาร์เรย์ของ CGFrame เพื่อเก็บลำดับภาพ CG และ Dialogue
}