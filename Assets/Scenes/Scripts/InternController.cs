using UnityEngine;

public class InternController : MonoBehaviour
{
    [Header("References")]
    public InternPawn pawn;


    private float horizontalInput;

    private void Update()
    {
        if (pawn == null) return;

        // --- Movement Input (keyboard or joystick) ---
        horizontalInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (pawn == null) return;

        pawn.Move(new Vector3(horizontalInput, 0f, 0f));
    }
}