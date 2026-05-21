using UnityEngine;
using UnityEngine.AI;

public class FollowMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public Transform target;

    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;

    private bool running = false;

    public void SetRunning(bool isRunning)
    {
        running = isRunning;
        if (running)
        {
            agent.speed = runSpeed;
        }
        else
        {
            agent.speed = walkSpeed;
        }
    }

    void Update()
    {
        if (target == null || !agent.isOnNavMesh)
        {
            return;
        }

        agent.SetDestination(target.position);

        bool moving = agent.velocity.magnitude > 0.05f;

        animator.SetBool("Walking", moving);
        animator.SetBool("Running", moving && running);
    }
}
