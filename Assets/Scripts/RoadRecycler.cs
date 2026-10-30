using UnityEngine;

public class RoadRecycler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] roads;

    [Header("Settings")]
    [SerializeField] private float roadLength = 200f;    // Chiều dài thực tế của 1 đoạn đường

    private float totalRoadLength;

    private void Start()
    {
        // Tự động tính tổng chiều dài chuỗi đường dựa trên số lượng đoạn đường bạn gán
        totalRoadLength = roadLength * roads.Length;
    }

    private void Update()
    {
        foreach (Transform road in roads)
        {
            // Nếu Player chạy vượt qua tâm toạn độ đoạn đường này một khoảng bằng 1/2 chiều dài đoạn đường
            if (player.position.z > road.position.z + roadLength/2 + 20)
            {
                // Dịch chuyển đoạn đường này nhảy vọt lên trên cùng, nối tiếp phía trước
                Vector3 newPos = road.position;
                newPos.z += totalRoadLength;
                road.position = newPos;
            }
        }
    }
}