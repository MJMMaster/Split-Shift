using UnityEngine;
using static UnityEditor.Progress;

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

            Vector3 safePos = other.transform.position + Vector3.up * 0.1f;

            CheckpointManager.Instance.SaveCheckpoint(safePos, hp.currentHealth);
            used = true;

            Debug.Log("Activated checkpoint!");
            MessageDisplay.Instance?.ShowMessage("Activated checkpoint!");

        }
    }
}