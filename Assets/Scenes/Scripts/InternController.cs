using UnityEngine;

public class InternController : MonoBehaviour
{
    public InternPawn pawn;

    private float horizontalInput;

    private void Update()
    {
        // --- Movement Input ---
        // Supports joystick or keyboard
        horizontalInput = Input.GetAxis("Horizontal");

        // --- Interact Input ---
        if (Input.GetButtonDown("Submit"))
        {
            pawn.Interact();
        }
    }

    private void FixedUpdate()
    {
        pawn.Move(new Vector3(horizontalInput, 0f, 0f));
    }
}