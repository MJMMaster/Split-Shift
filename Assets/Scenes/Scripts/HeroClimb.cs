using UnityEngine;

[RequireComponent(typeof(HeroPawn))]
public class HeroClimb : MonoBehaviour
{
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private Rigidbody rb;
    private HeroPawn heroPawn;
    private Collider currentLadder;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        heroPawn = GetComponent<HeroPawn>();
    }

    private void Update()
    {
        if (isClimbing)
        {
            float vertical = Input.GetAxis("Vertical");
            rb.linearVelocity = new Vector3(0, vertical * climbSpeed, 0);

            // Jump off ladder
            if (Input.GetButtonDown("Jump"))
            {
                isClimbing = false;
                heroPawn.PrimaryAction(); // use your jump method
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
            currentLadder = other;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentLadder)
        {
            isClimbing = false;
            rb.useGravity = true;
            currentLadder = null;
        }
    }
}