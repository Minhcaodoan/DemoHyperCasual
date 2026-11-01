using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    private int targetLane = 1; // 0: Trái, 1: Giữa, 2: Phải
    private float laneDistance = 2.5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float forwardSpeed = 10f;

    [Header("Jump & Gravity Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;
    private float verticalVelocity; // Vận tốc trục Y (nhảy và trọng lực)

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 2f; // Thời gian thực hiện trượt
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalHeight;
    private CapsuleCollider capsuleCollider;

    private CharacterController controller;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Khi game bắt đầu, bật trạng thái chạy (Moving = true)
        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }

        if (capsuleCollider != null)
        {
            originalHeight = capsuleCollider.height;
        }
    }

    private void Update()
    {
        // Xử lý nhận phím (New Input System)
        if (Keyboard.current != null)
        {
            // 1. Chuyển làn Trái / Phải
            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                targetLane--;
                if (targetLane < 0) targetLane = 0;
            }
            else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                targetLane++;
                if (targetLane > 2) targetLane = 2;
            }

            // 2. Nhảy (Phím Space)
            if (controller.isGrounded)
            {
                verticalVelocity = -1f; // Giữ nhân vật bám sát mặt đường khi đứng/chạy trên đất
                if (animator != null)
                {
                    animator.SetBool("Jumping", false); // Chạm đất thì tắt animation nhảy
                }
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    Jump();
                }
            }
            else
            {
                Debug.Log("hehe");
                // Khi ở trên không, liên tục bị trọng lực kéo xuống từng khung hình
                verticalVelocity -= gravity * Time.deltaTime;
            }

            // 3. Trượt (Phím S hoặc Mũi tên xuống)
            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                StartSlide();
            }
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0)
                {
                    StopSlide();
                }
            }
        }

        // Tính vị trí mục tiêu theo trục X (để chuyển làn mượt)
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (targetLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (targetLane == 2)
            targetPosition += Vector3.right * laneDistance;

        Vector3 diff = targetPosition - transform.position;

        // Tổng hợp vector di chuyển (Tiến về trước + Sang ngang + Lên/Xuống do nhảy/trọng lực)
        Vector3 moveDir = new Vector3(diff.x * moveSpeed, verticalVelocity, forwardSpeed);

        // Thực hiện di chuyển duy nhất 1 lần trong khung hình
        controller.Move(moveDir * Time.deltaTime);
    }

    private void Jump()
    {
        verticalVelocity = jumpForce;
        if (animator != null)
        {
            // Dùng SetBool "Jumping" hoặc SetTrigger tùy theo thiết lập mũi tên chuyển trạng thái của asset
            animator.SetBool("Jumping", true);
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        Debug.Log("hihi");
        // Thu nhỏ chiều cao của CharacterController khi trượt để chui qua vật cản thấp
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalHeight / 2f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y / 2f, capsuleCollider.center.z);
        }
        if (animator != null)
        {
            animator.SetBool("Sliding", true);
        }
    }

    private void StopSlide()
    {
        isSliding = false;

        Debug.Log("haha");
        // Khôi phục lại kích thước collider ban đầu
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y * 2f, capsuleCollider.center.z);
        }

        if (animator != null)
        {
            animator.SetBool("Sliding", false);
        }
    }
}