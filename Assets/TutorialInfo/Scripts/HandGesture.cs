using UnityEngine;

public class HandGesture : MonoBehaviour
{
    public OVRHand hand;
    public OVRSkeleton skeleton;

    public Transform agentTarget;
    public LayerMask floorMask;
    public LineRenderer lineRenderer;

    public FollowMovement agentMovement;

    public float rayDistance = 10f;
    public float minPinchStrength = 0.65f;
    public float smoothing = 12f;

    public float closedDotThreshold = 0.6f;

    private bool wasPinching = false;
    private Quaternion directionOffset = Quaternion.identity;

    void Update()
    {
        if (!hand.IsTracked ||
            hand.HandConfidence != OVRHand.TrackingConfidence.High)
        {
            lineRenderer.enabled = false;
            wasPinching = false;
            return;
        }

        float indexPinch =
            hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        bool indexPinching = indexPinch > minPinchStrength;

        if (!indexPinching)
        {
            lineRenderer.enabled = false;
            wasPinching = false;
            return;
        }

        bool middleClosed = FingerClosed(
            OVRSkeleton.BoneId.XRHand_MiddleProximal,
            OVRSkeleton.BoneId.XRHand_MiddleDistal,
            OVRSkeleton.BoneId.XRHand_MiddleTip
        );

        bool ringClosed = FingerClosed(
            OVRSkeleton.BoneId.XRHand_RingProximal,
            OVRSkeleton.BoneId.XRHand_RingDistal,
            OVRSkeleton.BoneId.XRHand_RingTip
        );

        bool pinkyClosed = FingerClosed(
            OVRSkeleton.BoneId.XRHand_LittleProximal,
            OVRSkeleton.BoneId.XRHand_LittleDistal,
            OVRSkeleton.BoneId.XRHand_LittleTip
        );

        bool running = middleClosed && ringClosed && pinkyClosed;

        agentMovement.SetRunning(running);

        Transform wrist =
            skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_Wrist].Transform;

        Transform tip =
            skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_IndexTip].Transform;

        Vector3 fingerDir =
            (tip.position - wrist.position).normalized;

        if (!wasPinching)
        {
            Vector3 targetDir =
                (agentTarget.position - tip.position).normalized;

            directionOffset =
                Quaternion.FromToRotation(fingerDir, targetDir);

            wasPinching = true;
        }

        Vector3 rayDir =
            directionOffset * fingerDir;

        Ray ray = new Ray(tip.position, rayDir);

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, tip.position);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, floorMask))
        {
            lineRenderer.SetPosition(1, hit.point);

            agentTarget.position = Vector3.Lerp(
                agentTarget.position,
                hit.point,
                Time.deltaTime * smoothing
            );
        }
        else
        {
            lineRenderer.SetPosition(
                1,
                tip.position + rayDir * rayDistance
            );
        }
    }

    bool FingerClosed(
        OVRSkeleton.BoneId proximalId,
        OVRSkeleton.BoneId distalId,
        OVRSkeleton.BoneId tipId)
    {
        Transform proximal =
            skeleton.Bones[(int)proximalId].Transform;

        Transform distal =
            skeleton.Bones[(int)distalId].Transform;

        Transform tip =
            skeleton.Bones[(int)tipId].Transform;

        Vector3 a = (distal.position - proximal.position).normalized;
        Vector3 b = (tip.position - distal.position).normalized;

        float dot = Vector3.Dot(a, b);

        return dot < closedDotThreshold;
    }
}
