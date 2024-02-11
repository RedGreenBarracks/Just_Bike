using UnityEngine;

public class MoveScript : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float rotateSpeed = 50.0f;

    void Update()
    {
        float moveForward = Input.GetAxis("Vertical");
        float rotate = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(0, 0, moveForward);

        transform.position += transform.rotation * movement * moveSpeed * Time.deltaTime;
        transform.Rotate(0, rotate * rotateSpeed * Time.deltaTime, 0);
    }
}
