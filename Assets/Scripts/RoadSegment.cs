using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public static RoadSegment Instance;
    [System.Serializable]
    public struct SpawnInfo
    {
        public Transform spawnPoint; // Vị trí xuất hiện
        public string obstacleTag;   // Tag của loại vật cản tương ứng trong ObjectPooler
    }

    public SpawnInfo[] spawnInfos; // Danh sách các điểm mốc và tag của chúng

    private void Awake()
    {
        Instance = this;
    }

    // Hàm gọi khi đoạn đường được tái sử dụng
    public void ResetRoad()
    {
        foreach (SpawnInfo info in spawnInfos)
        {
            // Xác suất xuất hiện (ví dụ: 60% xuất hiện, 40% bỏ trống)
            if (Random.value > 0.4f && info.spawnPoint != null)
            {
                // Gọi vật cản từ kho dựa vào tag riêng của điểm mốc đó
                GameObject obstacle = ObjectPooler.Instance.SpawnFromPool(info.obstacleTag, info.spawnPoint.position, info.spawnPoint.rotation);

                obstacle?.transform.SetParent(this.transform); // Đặt làm con để di chuyển cùng đoạn đường
            }
        }
    }
}