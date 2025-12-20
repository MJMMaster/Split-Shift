using UnityEngine;

public class Brick : MonoBehaviour
{
    private void Start()
    {
        MinigameManager.Instance.RegisterBrick();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            MinigameManager.Instance.BrickDestroyed();
            Destroy(gameObject);
        }
    }
}