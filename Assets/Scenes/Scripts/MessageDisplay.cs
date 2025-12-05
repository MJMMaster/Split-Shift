using TMPro;
using UnityEngine;
using System.Collections;

public class MessageDisplay : MonoBehaviour
{
    public static MessageDisplay Instance;
    public TextMeshProUGUI messageText;
    public float displayTime = 2f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void ShowMessage(string message)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DisplayRoutine(message));
    }

    private IEnumerator DisplayRoutine(string message)
    {
        messageText.text = message;
        messageText.enabled = true;
        yield return new WaitForSeconds(displayTime);
        messageText.enabled = false;
    }
}