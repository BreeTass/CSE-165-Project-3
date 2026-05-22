using System.Collections;
using Meta.XR.MRUtilityKit;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SetRoomLayers : MonoBehaviour
{
    public GameObject floorPlane;
    public NavMeshSurface surface;
    public NavMeshAgent agent;

    public Material wallMaterial;
    public Material ceilingMaterial;

    private bool setupDone = false;

    void Update()
    {
        if (setupDone)
            return;

        MRUKAnchor[] anchors = FindObjectsByType<MRUKAnchor>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        if (anchors.Length == 0)
            return;

        MRUKAnchor floorAnchor = null;

        foreach (MRUKAnchor anchor in anchors)
        {
            string name = anchor.gameObject.name.ToUpper();

            if (name.Contains("FLOOR"))
            {
                floorAnchor = anchor;
            }

            if (name.Contains("WALL"))
            {
                SetupWall(anchor.gameObject);
            }

            if (name.Contains("CEILING"))
            {
                SetLayerRecursively(
                    anchor.gameObject,
                    LayerMask.NameToLayer("Wall")
                );

                ApplyMaterialRecursively(
                    anchor.gameObject,
                    ceilingMaterial
                );
            }
        }

        if (floorAnchor == null)
            return;

        SetupFloorFromWalls(floorAnchor, anchors);

        surface.BuildNavMesh();

        if (agent != null &&
            NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        setupDone = true;
    }

    void SetupFloorFromWalls(MRUKAnchor floorAnchor, MRUKAnchor[] anchors)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (MRUKAnchor anchor in anchors)
        {
            if (!anchor.gameObject.name.ToUpper().Contains("WALL"))
                continue;

            Vector3 p = anchor.transform.position;

            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minZ = Mathf.Min(minZ, p.z);
            maxZ = Mathf.Max(maxZ, p.z);
        }

        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            floorAnchor.transform.position.y,
            (minZ + maxZ) / 2f
        );

        float width = maxX - minX;
        float depth = maxZ - minZ;

        floorPlane.transform.position = center;
        floorPlane.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        floorPlane.transform.localScale = new Vector3(
            width / 10f,
            1f,
            depth / 10f
        );

        floorPlane.layer = LayerMask.NameToLayer("Floor");
    }

    void SetupWall(GameObject wall)
    {
        SetLayerRecursively(wall, LayerMask.NameToLayer("Wall"));
        ApplyMaterialRecursively(wall, wallMaterial);

        if (wall.GetComponent<Collider>() == null)
        {
            BoxCollider box = wall.AddComponent<BoxCollider>();
            box.size = new Vector3(0.1f, 2f, 3f);
            box.center = Vector3.zero;
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void ApplyMaterialRecursively(GameObject obj, Material material)
    {
        if (material == null)
            return;

        Renderer r = obj.GetComponent<Renderer>();

        if (r != null)
            r.material = material;

        foreach (Transform child in obj.transform)
        {
            ApplyMaterialRecursively(child.gameObject, material);
        }
    }
}
