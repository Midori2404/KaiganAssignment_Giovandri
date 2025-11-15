using System.Collections.Generic;
using UnityEngine;

public class AvatarCustomizeManager : MonoBehaviour
{
    [Header("Avatar Setup")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform headAccessoriesSlot;
    [SerializeField] private Transform rootBone;
    [SerializeField] private SkinnedMeshRenderer baseBodyRenderer;

    // Track Clothing
    private Dictionary<ClothingCategory, SkinnedMeshRenderer> activeRenderers = new();

    // Track Hidden Body Parts by Category
    private Dictionary<ClothingCategory, List<GameObject>> hiddenPartsByCategory = new();

    // Track Hair
    private Dictionary<ClothingCategory, GameObject> activeHairObjects = new();

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
    }

    public void EquipClothing(ClothingDataSO newClothing)
    {
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
}
