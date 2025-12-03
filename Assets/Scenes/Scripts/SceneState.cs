using UnityEngine;

public class SceneState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;

    public SceneState(Vector3 pos, Quaternion rot, Vector3 vel)
    {
        position = pos;
        rotation = rot;
        velocity = vel;
    }
}