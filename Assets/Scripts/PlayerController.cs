using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Walking Sprites (3 per direction)")]
    public Sprite[] walkUpSprites;    // ใส่เฟรม 0,1,2
    public Sprite[] walkDownSprites;  // ใส่เฟรม 3,4,5
    public Sprite[] walkLeftSprites;  // ใส่เฟรม 6,7,8
    public Sprite[] walkRightSprites; // ใส่เฟรม 9,10,11

    [Header("Animation Settings")]
    public float frameDuration = 0.2f; // เวลาเปลี่ยนเฟรม

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private Vector2 lastMove; // ทิศทางล่าสุดที่เคยเดิน
    private float animationTimer;
    private int frameIndex; // ตำแหน่งเฟรมในแต่ละทิศ

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastMove = Vector2.down; // เริ่มต้นหันหน้าลง
        frameIndex = 1; // เฟรมกลาง
    }

    void Update()
    {
        // รับอินพุตจากผู้เล่น
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // ถ้ามีการขยับ → บันทึกทิศล่าสุด
        if (movement != Vector2.zero)
        {
            lastMove = movement;
            AnimateWalk(); // เล่นแอนิเมชันเดิน
        }
        else
        {
            ShowIdle(); // หยุดเดิน → แสดงเฟรมกลาง
        }
    }

    void FixedUpdate()
    {
        // เคลื่อนตัวละคร
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void AnimateWalk()
    {
        animationTimer += Time.deltaTime;

        // เปลี่ยนเฟรมเมื่อครบเวลา
        if (animationTimer >= frameDuration)
        {
            animationTimer = 0f;
            frameIndex = (frameIndex + 1) % 3;

            // เลือก Sprite ตามทิศ
            if (lastMove.y > 0)
                spriteRenderer.sprite = walkUpSprites[frameIndex];
            else if (lastMove.y < 0)
                spriteRenderer.sprite = walkDownSprites[frameIndex];
            else if (lastMove.x > 0)
                spriteRenderer.sprite = walkRightSprites[frameIndex];
            else if (lastMove.x < 0)
                spriteRenderer.sprite = walkLeftSprites[frameIndex];
        }
    }

    void ShowIdle()
    {
        // หยุดเดิน → ใช้เฟรมกลาง (index 1)
        frameIndex = 1;

        if (lastMove.y > 0)
            spriteRenderer.sprite = walkUpSprites[frameIndex];
        else if (lastMove.y < 0)
            spriteRenderer.sprite = walkDownSprites[frameIndex];
        else if (lastMove.x > 0)
            spriteRenderer.sprite = walkRightSprites[frameIndex];
        else if (lastMove.x < 0)
            spriteRenderer.sprite = walkLeftSprites[frameIndex];
    }
}
