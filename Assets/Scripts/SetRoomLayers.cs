using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine;

public class SetRoomLayers : MonoBehaviour
{
    MRUKAnchor[] anchors;
    bool hasAnchors = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchors = FindObjectsByType<MRUKAnchor>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (anchors.Length == 0)
        {
            anchors = FindObjectsByType<MRUKAnchor>();
        } else if (!hasAnchors) {
            hasAnchors = true;
            setLayers();
        }
    }

    public void setLayers()
    {
        foreach (MRUKAnchor anchor in anchors)
        {
            if (anchor.gameObject.name == "FLOOR")
            {
                anchor.gameObject.layer = LayerMask.NameToLayer("Floor");
            }
            else
            {
                anchor.gameObject.layer = LayerMask.NameToLayer("Wall");
            }

        }
    }
}
