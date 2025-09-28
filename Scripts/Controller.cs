using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotateSpeed = 120f;
    public Transform cam;
    

    public float bobFrequency = 6f;    // how fast it bobs
    public float bobAmplitude = 0.05f; // how high it bobs
    private float bobTimer = 0f;
    private Vector3 camStartLocalPos;

    void Start() {
        camStartLocalPos = cam.localPosition;
    }

    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.forward * v + transform.right * h;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;


        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX * rotateSpeed * Time.deltaTime);

        if (move.magnitude > 0.1f) {
            bobTimer += Time.deltaTime * bobFrequency;
            float newY = camStartLocalPos.y + Mathf.Sin(bobTimer) * bobAmplitude;
            cam.localPosition = new Vector3(camStartLocalPos.x, newY, camStartLocalPos.z);
        } else {
            bobTimer = 0f;
            cam.localPosition = Vector3.Lerp(cam.localPosition, camStartLocalPos, Time.deltaTime * 5f);
        }
    }

     
    
}
