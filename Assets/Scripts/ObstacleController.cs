using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.z < Camera.main.transform.position.z - 10f)
        {
            gameObject.SetActive(false);
        }
    }
}