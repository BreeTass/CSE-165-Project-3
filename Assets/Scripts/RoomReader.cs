using UnityEngine;
using Meta.XR.MRUtilityKit;
public class RoomReader : MonoBehaviour
{
    void OnEnable()
    {
        MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }
    void OnSceneLoaded()
    {
        var room = MRUK.Instance.GetCurrentRoom();
        var floor = room.FloorAnchors; // MRUKAnchor with MRUKPlane
        foreach (var wall in room.WallAnchors)
        {
            wall.gameObject.layer = LayerMask.NameToLayer("Surface");
            Debug.Log(wall);
        }
    }
}