using UnityEngine;

public class RotateObj : MonoBehaviour
{
    public Transform target; // Assign the player or camera in Inspector


void LateUpdate()
    {
        if (target == null) return;

        // Always face target
        transform.LookAt(target);

        // Optional: lock rotation so sprite stays upright
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }


}
