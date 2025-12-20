using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            HeroHealth hp = other.GetComponent<HeroHealth>();
            if (hp == null) return;

            // Small offset to avoid collision issues
            Vector3 safePos = other.transform.position + Vector3.up * 0.1f;

            // Save checkpoint using your manager
            CheckpointManager.Instance.SaveCheckpoint(safePos, hp.currentHealth);
            used = true;

            Debug.Log("Activated checkpoint!");
            MessageDisplay.Instance?.ShowMessage("Activated checkpoint!");
        }
    }
}