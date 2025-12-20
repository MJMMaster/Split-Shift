using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float xLimit = 7f;

    [HideInInspector]
    public float moveDirection; // used by the ball

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        moveDirection = input;

        Vector3 pos = transform.position;
        pos.x += input * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        transform.position = pos;
    }
}