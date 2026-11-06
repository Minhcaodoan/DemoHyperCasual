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

    [Header("Touch / Swipe Settings (Mobile)")]
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    [SerializeField] private float swipeThreshold = 50f; // Khoảng cách tối thiểu để tính là một cú vuốt

    private CharacterController controller;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Khi game bắt đầu, bật trạng thái chạy (Moving = true)
        animator?.SetBool("Moving", true);

        if (capsuleCollider != null)
        {
            originalHeight = capsuleCollider.height;
        }
    }

    private void Update()
    {
        UIManager.Instance?.UpdateScoreUI();

        // 1. Xử lý phím bấm bàn phím (PC)
        HandleKeyboardInput();

        // 2. Xử lý thao tác vuốt màn hình (Mobile)
        HandleTouchInput();

        // Xử lý thời gian trượt
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }

        // 3. Xử lý Trọng lực và Nhảy (ĐÃ SỬA LỖI GHI ĐÈ)
        if (controller.isGrounded)
        {
            animator?.SetBool("Jumping", false);

            // Nếu đang đứng trên đất và không vừa mới nhảy thì giữ vận tốc âm nhẹ để bám đất
            if (verticalVelocity < 0)
            {
                verticalVelocity = -1f;
            }

            // Kiểm tra lệnh Nhảy ngay tại đây khi đang chạm đất
            if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame))
            {
                Jump();
            }
        }
        else
        {
            // Khi đang ở trên không, chịu tác động của trọng lực
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Tính vị trí mục tiêu theo trục X (để chuyển làn mượt)
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (targetLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (targetLane == 2)
            targetPosition += Vector3.right * laneDistance;

        Vector3 diff = targetPosition - transform.position;

        // Tổng hợp vector di chuyển
        Vector3 moveDir = new Vector3(diff.x * moveSpeed, verticalVelocity, forwardSpeed);

        // Thực hiện di chuyển
        controller.Move(moveDir * Time.deltaTime);
    }

    // --- HÀM XỬ LÝ PHÍM BÀN PHÍM (PC) ---
    private void HandleKeyboardInput()
    {
        if (Keyboard.current == null) return;

        // Chuyển làn Trái / Phải
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            MoveLeftLane();
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            MoveRightLane();
        }

        // Nhảy (Phím Space)
        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && controller.isGrounded)
        {
            Jump();
        }

        // Trượt (Phím S hoặc Mũi tên xuống)
        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            StartSlide();
        }
    }

    // --- HÀM XỬ LÝ VUỐT MÀN HÌNH (MOBILE) ---
    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        // Khi ngón tay chạm xuống màn hình
        if (touch.press.wasPressedThisFrame)
        {
            touchStartPosition = touch.position.ReadValue();
        }

        // Khi ngón tay nhấc khỏi màn hình
        if (touch.press.wasReleasedThisFrame)
        {
            touchEndPosition = touch.position.ReadValue();
            AnalyzeSwipe();
        }
    }

    private void AnalyzeSwipe()
    {
        Vector2 swipeDelta = touchEndPosition - touchStartPosition;

        // Nếu khoảng cách vuốt quá ngắn thì bỏ qua (tránh nhầm lẫn với chạm nhẹ)
        if (swipeDelta.magnitude < swipeThreshold) return;

        // So sánh hướng vuốt theo trục Ngang (X) hay Dọc (Y) chiếm ưu thế hơn
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            // Vuốt Ngang -> Chuyển làn
            if (swipeDelta.x > 0)
            {
                MoveRightLane(); // Vuốt phải
            }
            else
            {
                MoveLeftLane();  // Vuốt trái
            }
        }
        else
        {
            // Vuốt Dọc -> Nhảy hoặc Trượt
            if (swipeDelta.y > 0)
            {
                if (controller.isGrounded) Jump(); // Vuốt lên -> Nhảy
            }
            else
            {
                StartSlide(); // Vuốt xuống -> Trượt
            }
        }
    }

    // --- CÁC HÀM HÀNH ĐỘNG CHUNG ---
    private void MoveLeftLane()
    {
        targetLane--;
        if (targetLane < 0) targetLane = 0;
    }

    private void MoveRightLane()
    {
        targetLane++;
        if (targetLane > 2) targetLane = 2;
    }

    private void Jump()
    {
        Debug.Log("dd");
        AudioManager.Instance?.PlaySFX("JumpSound");
        verticalVelocity = jumpForce;
        animator?.SetBool("Jumping", true);
    }

    private void StartSlide()
    {
        if (isSliding) return; // Đang trượt rồi thì không gọi lại nữa

        isSliding = true;
        slideTimer = slideDuration;

        // Thu nhỏ chiều cao của CharacterController khi trượt để chui qua vật cản thấp
        if (capsuleCollider != null)
        {
            capsuleCollider.height = 0.5f;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y / 2f, capsuleCollider.center.z);
        }
        animator?.SetBool("Sliding", true);
    }

    private void StopSlide()
    {
        isSliding = false;

        // Khôi phục lại kích thước collider ban đầu
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y * 2f, capsuleCollider.center.z);
        }
        animator?.SetBool("Sliding", false);
    }
}