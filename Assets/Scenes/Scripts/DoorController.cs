using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SaveableObject))]
public class DoorController : MonoBehaviour, ISaveable
{
    [Header("Door State")]
    public bool isLocked = false;
    public bool isOpen = false;

    [Header("Door Movement")]
    public float openHeight = 3f;
    public float moveSpeed = 3f;

    [Header("Auto Close")]
    public bool autoClose = false;
    public float closeDelay = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine moveRoutine;
    private Renderer doorRenderer;

    private SaveableObject saveableObject;

    private void Awake()
    {
        saveableObject = GetComponent<SaveableObject>();
        if (saveableObject == null)
            Debug.LogError($"{name} is missing SaveableObject!");

        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);
        doorRenderer = GetComponent<Renderer>();
        UpdateDoorColor();
    }

    private void Start()
    {
        // Delay restore slightly to ensure all systems initialize
        StartCoroutine(RestoreStateDelayed());
    }

    private IEnumerator RestoreStateDelayed()
    {
        yield return null; // wait one frame

        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        object savedState = GameManager.Instance?.GetObjectComponentState(saveableObject.UniqueID, GetType().Name, sceneName);

        if (savedState != null)
        {
            RestoreState(savedState);
        }
        else
        {
            // No saved state: ensure door starts closed
            SetDoorInstant(false);
        }
    }

    // -------------------------------
    // Door logic
    // -------------------------------
    public void ToggleDoor()
    {
        if (isLocked) return;

        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    public void OpenDoor()
    {
        if (isLocked || isOpen) return;

        isOpen = true;
        Debug.Log($"{name} opened!");
        MessageDisplay.Instance?.ShowMessage($"{name} opened!");


        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoorRoutine(openPosition));

        UpdateDoorColor();
        GameManager.Instance?.SaveSceneState();

        if (autoClose)
            StartCoroutine(AutoCloseRoutine());
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;
        Debug.Log($"{name} closed!");
        MessageDisplay.Instance?.ShowMessage($"{name} closed!");


        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoorRoutine(closedPosition));

        UpdateDoorColor();
        GameManager.Instance?.SaveSceneState();
    }

    private IEnumerator MoveDoorRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = target;
    }

    private IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(closeDelay);
        CloseDoor();
    }

    private void UpdateDoorColor()
    {
        if (doorRenderer != null)
            doorRenderer.material.color = isOpen ? Color.green : Color.red;
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log($"{name} unlocked!");
        MessageDisplay.Instance?.ShowMessage($"{name} unlocked!");
        GameManager.Instance?.SaveSceneState();
    }

    private void SetDoorInstant(bool open)
    {
        isOpen = open;
        transform.position = open ? openPosition : closedPosition;
        UpdateDoorColor();
    }

    // -------------------------------
    // ISaveable Implementation
    // -------------------------------
    public string GetUniqueID() => saveableObject.UniqueID;

    public object CaptureState()
    {
        return new DoorSaveData
        {
            isOpen = this.isOpen,
            isLocked = this.isLocked,
            position = transform.position
        };
    }

    public void RestoreState(object state)
    {
        if (state is DoorSaveData data)
        {
            isLocked = data.isLocked;

            // Only open if allowed
            isOpen = data.isOpen;
            SetDoorInstant(isOpen);
        }
        else
        {
            Debug.LogWarning($"DoorController.RestoreState received invalid state: {state?.GetType()}");
            SetDoorInstant(false); // fallback
        }
    }
}

[Serializable]
public struct DoorSaveData
{
    public bool isOpen;
    public bool isLocked;
    public Vector3 position;
}