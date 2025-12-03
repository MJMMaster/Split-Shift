using UnityEngine;

public class SaveableObject : MonoBehaviour
{
    [SerializeField] private string uniqueID = System.Guid.NewGuid().ToString();

    public string UniqueID => uniqueID;
}