using UnityEngine;

public class PointAndPick : MonoBehaviour
{
    public OVRHand hand;
    public OVRSkeleton skeleton;
    public LayerMask surfaceMask; // 'Surface' layer from the MRUK code
    public Transform agentTarget;
    void Update()
    {
        if (!hand.IsTracked || hand.HandConfidence != OVRHand.TrackingConfidence.High) return;
        var wrist = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;
        var tip = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform;
        Vector3 dir = (tip.position - wrist.position).normalized;
        if (Physics.Raycast(tip.position, dir, out RaycastHit hit, 10f, surfaceMask))
            agentTarget.position = hit.point;
        // OVRHand.PointerPose is more stable than wrist-to-index pointing
    }
}
