using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarCustomizeManager : MonoBehaviour
{
    [Header("Avatar Setup")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform headAccessoriesSlot;
    [SerializeField] private Transform rootBone;
    [SerializeField] private SkinnedMeshRenderer baseBodyRenderer;
    [SerializeField] private SkinnedMeshRenderer combinedMeshRenderer;

    // Track Clothing
    private Dictionary<ClothingCategory, SkinnedMeshRenderer> activeRenderers = new();

    // Track Hidden Body Parts by Category
    private Dictionary<ClothingCategory, List<GameObject>> hiddenPartsByCategory = new();

    // Track Hair
    private Dictionary<ClothingCategory, GameObject> activeHairObjects = new();

    // This will hold our final optimized mesh
    private SkinnedMeshRenderer combinedOutfitRenderer;

    [Header("References")]
    [SerializeField] private CharacterCustomizationUI characterCustomizationUI;

    private void Start()
    {
    }
    public void InitializeVariables()
    {
        playerBody = characterCustomizationUI.currentModel.transform;

        // Assign Variables From Spawned Model 
        ModelReferenceBinder binder = playerBody.GetComponent<ModelReferenceBinder>();
        rootBone = binder.rootBone;
        headAccessoriesSlot = binder.headAccessoriesSlot;
        baseBodyRenderer = binder.baseBodyRenderer;
        combinedMeshRenderer = binder.combinedMeshRenderer;
    }

    public void EquipClothing(ClothingDataSO newClothing)
    {
        BreakCombinedOutfit();

        // if Outfit
        if (newClothing.category == ClothingCategory.Outfit)
        {
            RemoveCategoryRenderer(ClothingCategory.Top);
            RemoveCategoryRenderer(ClothingCategory.Bottom);

            RestoreHiddenParts(ClothingCategory.Top);
            RestoreHiddenParts(ClothingCategory.Bottom);
        }
        // if either top or bottom
        else if (newClothing.category == ClothingCategory.Top || newClothing.category == ClothingCategory.Bottom)
        {
            RemoveCategoryRenderer(ClothingCategory.Outfit);

            RestoreHiddenParts(ClothingCategory.Outfit);
        }

        // Remove Old Hair
        if (newClothing.category == ClothingCategory.Hair)
        {
            RemoveHairObject();
        }
        else
        {
            RemoveCategoryRenderer(newClothing.category);
        }

        // Restore Previous Hidden Parts by Category
        RestoreHiddenParts(newClothing.category);

        // if Equipping Hair
        if (newClothing.category == ClothingCategory.Hair)
        {
            SpawnHairObject(newClothing);
        }
        else
        {
            SpawnSkinnedClothing(newClothing);
        }

        // Hide body parts
        List<GameObject> newlyHiddenParts = new();
        foreach (string partName in newClothing.bodyPartsToHideName)
        {
            Transform found = playerBody.Find(partName);
            if (found != null)
            {
                found.gameObject.SetActive(false);
                newlyHiddenParts.Add(found.gameObject);
            }
        }

        hiddenPartsByCategory[newClothing.category] = newlyHiddenParts;
    }

    #region SPAWN CLOTHINGS
    private void SpawnSkinnedClothing(ClothingDataSO clothing)
    {
        SkinnedMeshRenderer newRenderer = Instantiate(clothing.clothingPrefab, playerBody);
        newRenderer.bones = baseBodyRenderer.bones;
        newRenderer.rootBone = rootBone;

        activeRenderers[clothing.category] = newRenderer;
    }

    private void SpawnHairObject(ClothingDataSO clothing)
    {
        GameObject hairObj = Instantiate(clothing.hairPrefab, headAccessoriesSlot);
        activeHairObjects[ClothingCategory.Hair] = hairObj;
    }

    public void RandomizeOutfit(ClothingListSO list)
    {
        // 50/50 Chance
        bool useOutfit = Random.value > 0.5f;

        // Hair
        EquipClothing(list.GetRandomClothing(ClothingCategory.Hair));

        // Shoes
        EquipClothing(list.GetRandomClothing(ClothingCategory.Shoes));

        // Either Outfit or Top and Bottom
        if (useOutfit)
        {
            var outfit = list.GetRandomClothing(ClothingCategory.Outfit);
            EquipClothing(outfit);
        }
        else
        {
            var top = list.GetRandomClothing(ClothingCategory.Top);
            EquipClothing(top);

            var bottom = list.GetRandomClothing(ClothingCategory.Bottom);
            EquipClothing(bottom);
        }
    }

    #endregion

    #region REMOVE CLOTHINGS
    private void RemoveCategoryRenderer(ClothingCategory category)
    {
        if (activeRenderers.TryGetValue(category, out var renderer))
        {
            if (renderer != null)
                Destroy(renderer.gameObject);

            activeRenderers.Remove(category);
        }
    }

    private void RemoveHairObject()
    {
        if (activeHairObjects.TryGetValue(ClothingCategory.Hair, out GameObject hair))
        {
            if (hair != null)
                Destroy(hair);

            activeHairObjects.Remove(ClothingCategory.Hair);
        }
    }

    private void RestoreHiddenParts(ClothingCategory category)
    {
        if (hiddenPartsByCategory.TryGetValue(category, out var list))
        {
            foreach (var part in list)
                if (part != null)
                    part.SetActive(true);

            hiddenPartsByCategory.Remove(category);
        }
    }
    #endregion

    private void BreakCombinedOutfit()
    {
        // If the placeholder exists and is active, clear it.
        if (combinedMeshRenderer != null && combinedMeshRenderer.gameObject.activeInHierarchy)
        {
            // Clear the mesh so nothing is rendered from the placeholder
            combinedMeshRenderer.sharedMesh = null;
            combinedMeshRenderer.gameObject.SetActive(false);

            // Re-show all the individual, active renderers
            foreach (var renderer in activeRenderers.Values)
            {
                if (renderer != null)
                    renderer.gameObject.SetActive(true);
            }
        }
    }

    //public void FinalizeOutfit()
    //{
    //    // If we already have a combined mesh, or no parts to combine, exit.
    //    if (combinedOutfitRenderer != null || activeRenderers.Count == 0)
    //        return;

    //    // Hide all individual parts BEFORE combining
    //    foreach (var renderer in activeRenderers.Values)
    //    {
    //        if (renderer != null)
    //            renderer.gameObject.SetActive(false);
    //    }

    //    // --- Gather Meshes and Materials ---
    //    var combineInstances = new List<CombineInstance>();

    //    // --- FIX: This list MUST be built in the same order as the combineInstances ---
    //    var materials = new List<Material>();

    //    foreach (var renderer in activeRenderers.Values)
    //    {
    //        if (renderer == null || renderer.sharedMesh == null) continue;

    //        // --- FIX: We must loop through sub-meshes, not just the renderer ---
    //        for (int i = 0; i < renderer.sharedMesh.subMeshCount; i++)
    //        {
    //            // Safety check: ensure material exists for this sub-mesh
    //            if (i >= renderer.sharedMaterials.Length || renderer.sharedMaterials[i] == null)
    //                continue; // Skip this sub-mesh if it has no material

    //            // 1. Add the CombineInstance for this specific sub-mesh
    //            CombineInstance ci = new CombineInstance
    //            {
    //                mesh = renderer.sharedMesh,
    //                subMeshIndex = i,
    //                transform = playerBody.worldToLocalMatrix * renderer.transform.localToWorldMatrix
    //            };
    //            combineInstances.Add(ci);

    //            // 2. Add the corresponding material for that sub-mesh
    //            // This builds a 1-to-1 list in the correct order.
    //            materials.Add(renderer.sharedMaterials[i]);
    //        }
    //    }

    //    // If list is empty (e.g., all renderers were null or had no materials), abort.
    //    if (combineInstances.Count == 0)
    //    {
    //        Debug.LogWarning("No meshes found to combine. Aborting FinalizeOutfit.");

    //        // Re-show individual parts since we failed
    //        foreach (var renderer in activeRenderers.Values)
    //        {
    //            if (renderer != null)
    //                renderer.gameObject.SetActive(true);
    //        }
    //        return;
    //    }

    //    // --- Create and Configure Combined Object ---
    //    GameObject combinedObject = new GameObject("CombinedOutfit");
    //    combinedObject.transform.SetParent(playerBody);
    //    combinedObject.transform.localPosition = Vector3.zero;
    //    combinedObject.transform.localRotation = Quaternion.identity;



    //    // Check if the reference mesh is available for bind poses
    //    Mesh baseMesh = baseBodyRenderer.sharedMesh;
    //    if (baseMesh == null)
    //    {
    //        Debug.LogError("Base body mesh is missing! Cannot get Bind Poses.");
    //        return;
    //    }

    //    combinedOutfitRenderer = combinedObject.AddComponent<SkinnedMeshRenderer>();
    //    Mesh combinedMesh = new Mesh();
    //    combinedMesh.CombineMeshes(combineInstances.ToArray(), false, true);

    //    // 1. Assign Bind Poses: Get them from the base mesh
    //    combinedMesh.bindposes = baseMesh.bindposes; // Fix: Assign before setting sharedMesh

    //    // 2. Assign the mesh
    //    combinedOutfitRenderer.sharedMesh = combinedMesh;

    //    // 3. Assign the Bones: Get them from the base renderer
    //    combinedOutfitRenderer.bones = baseBodyRenderer.bones; // Ensure bone count matches bindpose count
    //    combinedOutfitRenderer.rootBone = rootBone;

    //    // --- NEW MANUAL BOUNDS CALCULATION FIX ---

    //    // 1. Get the vertex array from the combined mesh
    //    Vector3[] vertices = combinedMesh.vertices;
    //    if (vertices.Length > 0)
    //    {
    //        // 2. Initialize min and max vectors using the first vertex
    //        Vector3 min = vertices[0];
    //        Vector3 max = vertices[0];

    //        // 3. Iterate through all vertices to find the absolute minimum and maximum extent
    //        for (int i = 1; i < vertices.Length; i++)
    //        {
    //            Vector3 v = vertices[i];

    //            // Component-wise min
    //            min.x = Mathf.Min(min.x, v.x);
    //            min.y = Mathf.Min(min.y, v.y);
    //            min.z = Mathf.Min(min.z, v.z);

    //            // Component-wise max
    //            max.x = Mathf.Max(max.x, v.x);
    //            max.y = Mathf.Max(max.y, v.y);
    //            max.z = Mathf.Max(max.z, v.z);
    //        }

    //        // 4. Calculate the center and size from the min/max vectors
    //        Vector3 center = (min + max) * 0.5f;
    //        Vector3 size = max - min;

    //        // 5. Create and assign the new Bounds object
    //        Bounds finalBounds = new Bounds(center, size);
    //        combinedMesh.bounds = finalBounds;

    //        // Use the bounds setter from the SMR to trigger the necessary internal updates
    //        combinedOutfitRenderer.localBounds = finalBounds;
    //    }
    //    else
    //    {
    //        Debug.LogError("Combined mesh has zero vertices! Cannot set bounds.");
    //    }


    //    // --- FIX: Assign the materials list directly. DO NOT use Distinct() ---
    //    combinedOutfitRenderer.materials = materials.ToArray();
    //}

    public void FinalizeOutfit()
    {
        // 1. Initial Checks
        if (activeRenderers.Count == 0 || combinedMeshRenderer == null)
        {
            Debug.LogWarning("No clothing equipped or Placeholder not assigned.");
            return;
        }

        // 2. Prepare Lists and Combine
        var combineInstances = new List<CombineInstance>();
        var materials = new List<Material>();

        foreach (var renderer in activeRenderers.Values)
        {
            if (renderer == null || renderer.sharedMesh == null) continue;

            for (int i = 0; i < renderer.sharedMesh.subMeshCount; i++)
            {
                if (i >= renderer.sharedMaterials.Length || renderer.sharedMaterials[i] == null)
                    continue;

                CombineInstance ci = new CombineInstance
                {
                    mesh = renderer.sharedMesh,
                    subMeshIndex = i,
                    // Use the placeholder's worldToLocalMatrix as the reference point
                    transform = combinedMeshRenderer.transform.worldToLocalMatrix * renderer.transform.localToWorldMatrix
                };
                combineInstances.Add(ci);
                materials.Add(renderer.sharedMaterials[i]);
            }
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning("No valid submeshes found to combine. Skipping finalize.");
            return;
        }

        // --- 3. EXECUTE COMBINE ON PLACEHOLDER MESH DATA ---
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), false, true);

        // CRITICAL: Copy Bind Poses from the base mesh *to the new combined mesh*
        if (baseBodyRenderer.sharedMesh != null)
        {
            combinedMesh.bindposes = baseBodyRenderer.sharedMesh.bindposes;
        }

        // 4. Manual Bounds Calculation
        Vector3[] vertices = combinedMesh.vertices;
        if (vertices.Length > 0)
        {
            Vector3 min = vertices[0], max = vertices[0];
            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
            Bounds finalBounds = new Bounds((min + max) * 0.5f, max - min);

            // --- ASSIGNMENT TO PLACEHOLDER ---
            // This is now safe because the placeholder's bones/rootbone were set up manually.
            combinedMeshRenderer.sharedMesh = combinedMesh;
            combinedMeshRenderer.materials = materials.ToArray();
        }

        // 5. Final Visibility and Cleanup
        combinedMeshRenderer.updateWhenOffscreen = true;
        combinedMeshRenderer.gameObject.SetActive(true); // Ensure the placeholder is visible

        // Hide and remove tracking of individual items
        foreach (var renderer in activeRenderers.Values)
        {
            if (renderer != null)
            {
                Destroy(renderer.gameObject);
            }
        }
        activeRenderers.Clear();
    }

    public void OnConfirmButton()
    {
        FinalizeOutfit();
    }
}
