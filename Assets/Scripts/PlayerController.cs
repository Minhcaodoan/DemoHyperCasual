using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int targetLane = 1; // 0: Trái, 1: Giữa, 2: Phải
    private float laneDistance = 2.5f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float forwardSpeed = 10f;

    private CharacterController controller;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Khi game bắt đầu, bật trạng thái chạy (Moving = true)
        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }
    }

    private void Update()
    {
        Vector3 moveDir = Vector3.forward * forwardSpeed;

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
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (animator != null)
                {
                    // Dùng SetBool "Jumping" hoặc SetTrigger tùy theo thiết lập mũi tên chuyển trạng thái của asset
                    animator.SetBool("Jumping", true);
                }
            }
            else
            {
                // Sau khi nhảy xong có thể tắt đi (hoặc để Transition tự động thoát state)
                if (animator != null)
                {
                    animator.SetBool("Jumping", false);
                }
            }

            // 3. Trượt (Phím S hoặc Mũi tên xuống)
            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                if (animator != null)
                {
                    animator.SetBool("Sliding", true);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("Sliding", false);
                }
            }
        }

        // Tính toán vị trí mục tiêu theo trục X
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (targetLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (targetLane == 2)
            targetPosition += Vector3.right * laneDistance;

        // Di chuyển mượt mà về làn mục tiêu (Lerp)
        Vector3 diff = targetPosition - transform.position;
        moveDir.x = diff.x * moveSpeed;

        controller.Move(moveDir * Time.deltaTime);
    }
}