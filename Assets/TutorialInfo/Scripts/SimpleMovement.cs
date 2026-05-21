using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public Transform head;
    public float speed = 2f;

    private bool moving = false;
    private bool forward = false;
    private bool backward = false;
    private bool left = false;
    private bool right = false;

    void Update()
    {
        if (moving && forward)
        {
            Vector3 forward = head.forward;
            forward.y = 0f;

            transform.position += forward.normalized * speed * Time.deltaTime;
        }
        if (moving && backward)
        {
            Vector3 forward = -head.forward;
            forward.y = 0f;

            transform.position += forward.normalized * speed * Time.deltaTime;
        }
        if (moving && left)
        {
            Vector3 forward = -head.right;
            forward.y = 0f;

            transform.position += forward.normalized * speed * Time.deltaTime;
        }
        if (moving && right)
        {
            Vector3 forward = head.right;
            forward.y = 0f;

            transform.position += forward.normalized * speed * Time.deltaTime;
        }
    }

    public void StartMovingForward()
    {
        moving = true;
        forward = true;
        backward = false;
        left = false;
        right = false;
    }
    public void StartMovingBackward()
    {
        moving = true;
        forward = false;
        backward = true;
        left = false;
        right = false;
    }
    public void StartMovingLeft()
    {
        moving = true;
        forward = false;
        backward = false;
        left = true;
        right = false;
    }
    public void StartMovingRight()
    {
        moving = true;
        forward = false;
        backward = false;
        left = false;
        right = true;
    }

    public void StopMoving()
    {
        moving = false;
        forward = false;
        backward = false;
        left = false;
        right = true;
    }
}
