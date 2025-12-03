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

    private SaveableObject saveableObject; // reference for unique ID

    private void Awake()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(0, openHeight, 0);

        doorRenderer = GetComponent<Renderer>();
        saveableObject = GetComponent<SaveableObject>();
        if (saveableObject == null)
            Debug.LogError($"{name} is missing SaveableObject!");
        UpdateDoorColor();
    }

    // -------------------------------
    // DOOR LOGIC
    // -------------------------------

    public virtual void ToggleDoor()
    {
        if (isLocked) return;

        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    public void OpenDoor()
    {
        if (isLocked) return;

        isOpen = true;
        Debug.Log($"{name} opened!");

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoorRoutine(openPosition));

        UpdateDoorColor();

        if (autoClose)
            StartCoroutine(AutoCloseRoutine());
    }

    public void CloseDoor()
    {
        isOpen = false;
        Debug.Log($"{name} closed!");

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoorRoutine(closedPosition));

        UpdateDoorColor();
    }

    private IEnumerator MoveDoorRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime);

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
    }

    // -------------------------------
    // SAVE SYSTEM
    // -------------------------------

    public string GetUniqueID()
    {
        return saveableObject.UniqueID;
    }

    public object CaptureState()
    {
        return new DoorSaveData
        {
            isOpen = this.isOpen,
            isLocked = this.isLocked,
            position = transform.position
        };
    }

    private void SetDoorInstant(bool open)
    {
        isOpen = open;

        Vector3 target = open ? openPosition : closedPosition;

        // Snap instantly
        transform.position = target;

        UpdateDoorColor();
    }
    public void RestoreState(object state)
    {
        var data = (DoorSaveData)state;

        isOpen = data.isOpen;
        isLocked = data.isLocked;
        transform.position = data.position;

        SetDoorInstant(isOpen);
    }
}

[Serializable]
public struct DoorSaveData
{
    public bool isOpen;
    public bool isLocked;
    public Vector3 position;
}