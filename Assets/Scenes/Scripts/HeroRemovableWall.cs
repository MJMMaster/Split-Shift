using UnityEngine;

public class HeroRemovableWall : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public struct WallSaveData
    {
        public bool isActive;
    }

    private SaveableObject saveable;

    private void Awake()
    {
        saveable = GetComponent<SaveableObject>();
    }

    private void Start()
    {
        if (GameManager.Instance == null) return;

        string id = saveable.UniqueID;

        if (GameManager.Instance.IsObjectComponentSaved(id, nameof(HeroRemovableWall)))
        {
            var data = (WallSaveData)GameManager.Instance
                .GetObjectComponentState(id, nameof(HeroRemovableWall));

            RestoreState(data);
        }
    }

    // =============================
    //        SAVE SYSTEM
    // =============================
    public object CaptureState()
    {
        return new WallSaveData
        {
            isActive = gameObject.activeSelf
        };
    }

    public void RestoreState(object state)
    {
        WallSaveData data = (WallSaveData)state;
        gameObject.SetActive(data.isActive);
    }

public string GetUniqueID()
    {
        return saveable.UniqueID;
    }
}