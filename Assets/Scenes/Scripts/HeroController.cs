using UnityEngine;

public class HeroController : MonoBehaviour
{
    private HeroPawn pawn;

    private void Start()
    {
        pawn = GetComponent<HeroPawn>();

        if (pawn == null)
            Debug.LogError("HeroController requires a HeroPawn on the same GameObject.");
    }

    private void Update()
    {
        if (pawn == null) return;

        // Horizontal movement
        float moveX = Input.GetAxis("Horizontal");
        pawn.Move(new Vector2(moveX, 0));

        // Primary (jump or ability)
        if (Input.GetButtonDown("Submit"))
        {
            pawn.PrimaryAction();
        }

        // Secondary ability (Fire1)
        if (Input.GetButtonDown("Fire1"))
        {
            pawn.SecondaryAction();
        }
    }
}