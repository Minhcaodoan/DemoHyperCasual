using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool hasHit = 0 == 1;
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        // Kiểm tra nếu va chạm với chướng ngại vật có tag "Obstacle"
        if (other.CompareTag("Obstacle"))
        {
            hasHit = true;
            Debug.Log("Đã va chạm với chướng ngại vật!");

            // 1. Dừng di chuyển nhân vật (gọi script movement)
            GetComponent<PlayerController>().enabled = false;

            // 2. Kích hoạt animation thua cuộc
            GetComponent<Animator>().SetTrigger("Hit");
            GetComponent<Animator>().SetBool("Dead", true);

            // 3. Gọi Game Manager để hiện bảng Game Over sau 1-2 giây
            // GameManager.Instance.GameOver();
        }
    }
}
