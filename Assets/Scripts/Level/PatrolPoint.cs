using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public Vector3 Position;
    public int Priority = 1;

    private void Awake()
    {
        Position = transform.position;
    }
}
