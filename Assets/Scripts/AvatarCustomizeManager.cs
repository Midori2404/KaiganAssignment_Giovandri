using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Controls equipping, hiding body parts, spawning clothing, and combining meshes for optimization.
/// </summary>
public class AvatarCustomizeManager : MonoBehaviour
{
    public static AvatarCustomizeManager Instance { get; private set; }

    [Header("Clothing Lists")]
    [SerializeField] private ClothingListSO maleClothingListSO;
    [SerializeField] private ClothingListSO femaleClothingListSO;

    private Transform playerBody;
    private Transform headAccessoriesSlot;
    private Transform rootBone;
    private SkinnedMeshRenderer baseBodyRenderer;
    private SkinnedMeshRenderer combinedMeshRenderer;

    private Dictionary<string, SkinnedMeshRenderer> bodyPartLookup;

    // Track Clothing
    private Dictionary<ClothingCategory, SkinnedMeshRenderer> activeRenderers = new();
    private Dictionary<ClothingCategory, List<GameObject>> hiddenPartsByCategory = new();
    private Dictionary<ClothingCategory, GameObject> activeHairObjects = new();
    private Dictionary<ClothingCategory, ClothingDataSO> equippedData = new Dictionary<ClothingCategory, ClothingDataSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Assigns model references and prepares dictionaries.
    /// </summary>
    public void InitializeModel(GameObject model)
    {
        playerBody = model.transform;

        ModelReferenceBinder binder = model.GetComponent<ModelReferenceBinder>();
        if (binder == null)
        {
            Debug.LogError("UI model missing ModelReferenceBinder.");
            return;
        }

        AssignBinder(binder);
        ResetDictionaries();
    }

    /// <summary>
    /// Binds the references from the model prefab.
    /// </summary>
    private void AssignBinder(ModelReferenceBinder binder)
    {
        rootBone = binder.rootBone;
        headAccessoriesSlot = binder.headAccessoriesSlot;
        baseBodyRenderer = binder.baseBodyRenderer;
        combinedMeshRenderer = binder.combinedMeshRenderer;

        bodyPartLookup = binder.bodyParts.ToDictionary(
            p => p.partName,
            p => p.renderer
        );
    }

    private void ResetDictionaries()
    {
        activeRenderers = new Dictionary<ClothingCategory, SkinnedMeshRenderer>();
        activeHairObjects = new Dictionary<ClothingCategory, GameObject>();
        hiddenPartsByCategory = new Dictionary<ClothingCategory, List<GameObject>>();
        equippedData = new Dictionary<ClothingCategory, ClothingDataSO>();
    }

    /// <summary>
    /// Equips a clothing item and handles hiding body parts,
    /// removing conflicting categories (e.g., Outfit replaces Top/Bottom),
    /// and spawning the correct renderer or hair object.
    /// </summary>
    public void EquipClothing(ClothingDataSO newClothing)
    {
        BreakCombinedOutfit();
        equippedData[newClothing.category] = newClothing;

        if (newClothing.category == ClothingCategory.Outfit)
        {
            RemoveCategoryRenderer(ClothingCategory.Top);
            RemoveCategoryRenderer(ClothingCategory.Bottom);

            RestoreHiddenParts(ClothingCategory.Top);
            RestoreHiddenParts(ClothingCategory.Bottom);
        }
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
        List<GameObject> hiddenList = new();
        foreach (string partName in newClothing.bodyPartsToHideName)
        {
            if (bodyPartLookup.TryGetValue(partName, out var part))
            {
                part.gameObject.SetActive(false);
                hiddenList.Add(part.gameObject);
            }
        }

        hiddenPartsByCategory[newClothing.category] = hiddenList;
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

        EquipClothing(list.GetRandomClothing(ClothingCategory.Hair));
        EquipClothing(list.GetRandomClothing(ClothingCategory.Shoes));

        // Either Outfit or Top and Bottom
        if (useOutfit)
        {
            EquipClothing(list.GetRandomClothing(ClothingCategory.Outfit));
        }
        else
        {
            EquipClothing(list.GetRandomClothing(ClothingCategory.Top));
            EquipClothing(list.GetRandomClothing(ClothingCategory.Bottom));
        }
    }

    public void RandomizeCurrentModel()
    {
        if (playerBody == null)
            return;

        ModelReferenceBinder binder = playerBody.GetComponent<ModelReferenceBinder>();
        ClothingListSO list = (binder.gender == Gender.Male) ? maleClothingListSO : femaleClothingListSO;

        RandomizeOutfit(list);
    }

    public void RandomizeForAvatar(GameObject avatar, ClothingListSO list)
    {
        InitializeModel(avatar);
        RandomizeOutfit(list);
        FinalizeOutfit();
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

    #region MESH COMBINING
    /// <summary>
    /// Combines all equipped clothing SkinnedMeshes into a single renderer to reduce draw calls. 
    /// Used for crowd spawning or finalizing the avatar.
    /// </summary>
    public void FinalizeOutfit()
    {
        if (activeRenderers.Count == 0 || combinedMeshRenderer == null)
            return;

        var combineInstances = new List<CombineInstance>();
        var finalBindposes = new List<Matrix4x4>();
        var materials = new List<Material>();
        var finalBones = new List<Transform>();

        foreach (var renderer in activeRenderers.Values)
        {
            Mesh mesh = renderer.sharedMesh;
            if (mesh == null) continue;

            // Add Instances
            for (int s = 0; s < mesh.subMeshCount; s++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.subMeshIndex = s;
                ci.transform = combinedMeshRenderer.transform.worldToLocalMatrix *
                               renderer.transform.localToWorldMatrix;

                combineInstances.Add(ci);

                materials.Add(renderer.sharedMaterials[s]);
            }

            foreach (var bp in mesh.bindposes)
                finalBindposes.Add(bp);

            foreach (var bone in renderer.bones)
                finalBones.Add(bone);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), false, true);

        combinedMesh.bindposes = finalBindposes.ToArray();
        combinedMeshRenderer.bones = finalBones.ToArray();

        combinedMeshRenderer.sharedMesh = combinedMesh;
        combinedMeshRenderer.materials = materials.ToArray();
        combinedMeshRenderer.updateWhenOffscreen = true;

        combinedMeshRenderer.gameObject.SetActive(true);

        // Destroy originals
        foreach (var r in activeRenderers.Values)
            Destroy(r.gameObject);

        activeRenderers.Clear();
    }

    private void BreakCombinedOutfit()
    {
        // If the placeholder exists and is active, clear it.
        if (combinedMeshRenderer != null && combinedMeshRenderer.gameObject.activeInHierarchy)
        {
            // Clear the mesh so nothing is rendered from the placeholder
            combinedMeshRenderer.sharedMesh = null;
            combinedMeshRenderer.gameObject.SetActive(false);

            RebuildEquippedClothings();
        }
    }

    private void RebuildEquippedClothings()
    {
        foreach (var data in equippedData)
        {
            ClothingCategory category = data.Key;
            ClothingDataSO clothing = data.Value;

            if (clothing == null) continue;

            if (category == ClothingCategory.Hair)
                continue;

            SpawnSkinnedClothing(clothing);
        }
    }

    #endregion
}
