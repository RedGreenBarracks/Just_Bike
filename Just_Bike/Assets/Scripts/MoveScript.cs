using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public float acceleration = 2.0f;
    public float rotateSpeed = 50.0f;
    private float currentSpeed = 0.0f;

    void Update()
    {
        float targetSpeed = Input.GetAxis("Vertical") * maxSpeed;
        float rotate = Input.GetAxis("Horizontal");

        if (Mathf.Abs(targetSpeed) > Mathf.Epsilon)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.deltaTime);
        }

        Vector3 movement = new Vector3(0, 0, currentSpeed);

        transform.position += transform.rotation * movement * Time.deltaTime;
        transform.Rotate(0, rotate * rotateSpeed * Time.deltaTime, 0);
    }
}