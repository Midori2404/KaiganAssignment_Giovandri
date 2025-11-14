using System.Collections.Generic;
using UnityEngine;

public class AvatarCustomizeManager : MonoBehaviour
{
    [Header("Avatar Setup")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform headAccessoriesSlot;
    [SerializeField] private Transform rootBone;
    [SerializeField] private SkinnedMeshRenderer baseBodyRenderer;

    private Dictionary<ClothingCategory, SkinnedMeshRenderer> activeRenderers = new();
    private Dictionary<ClothingCategory, List<GameObject>> hiddenPartsByCategory = new();

    [Header("References")]
    [SerializeField] private CharacterCustomizationUI characterCustomizationUI;

    private void Start()
    {
    }
    public void InitializeVariables()
    {
        playerBody = characterCustomizationUI.currentModel.transform;

        ModelReferenceBinder binder = playerBody.GetComponent<ModelReferenceBinder>();
        rootBone = binder.rootBone;
        headAccessoriesSlot = binder.headAccessoriesSlot;
        baseBodyRenderer = binder.baseBodyRenderer;
    }

    public void EquipClothing(ClothingDataSO newClothing)
    {
        // --- HANDLE OUTFIT SPECIFIC LOGIC ---
        if (newClothing.category == ClothingCategory.Outfit)
        {
            // Remove Top + Bottom renderers
            RemoveCategoryRenderer(ClothingCategory.Top);
            RemoveCategoryRenderer(ClothingCategory.Bottom);

            // Restore hidden parts from Top + Bottom
            RestoreHiddenParts(ClothingCategory.Top);
            RestoreHiddenParts(ClothingCategory.Bottom);
        }
        else if (newClothing.category == ClothingCategory.Top || newClothing.category == ClothingCategory.Bottom)
        {
            // Remove Outfit renderer if exists
            RemoveCategoryRenderer(ClothingCategory.Outfit);

            // Restore hidden parts for Outfit
            RestoreHiddenParts(ClothingCategory.Outfit);
        }

        RestoreHiddenParts(newClothing.category);
        RemoveCategoryRenderer(newClothing.category);

        Transform parentSlot = (newClothing.category == ClothingCategory.Hair) ? headAccessoriesSlot : playerBody;

        // Spawn new clothing
        SkinnedMeshRenderer newRenderer = Instantiate(newClothing.clothingPrefab, parentSlot);
        newRenderer.bones = baseBodyRenderer.bones;
        newRenderer.rootBone = rootBone;

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
            else
            {
                Debug.LogWarning($"Body part '{partName}' not found under playerBody!");
            }
        }

        // Update tracking dictionaries
        hiddenPartsByCategory[newClothing.category] = newlyHiddenParts;
        activeRenderers[newClothing.category] = newRenderer;

    }

    public void RandomizeOutfit(ClothingListSO list)
    {
        foreach (var category in list.GetAllCategories())
        {
            var randomItem = list.GetRandomClothing(category);
            EquipClothing(randomItem);
        }
    }

    private void RemoveCategoryRenderer(ClothingCategory category)
    {
        if (activeRenderers.TryGetValue(category, out var renderer))
        {
            if (renderer != null)
                Destroy(renderer.gameObject);

            activeRenderers.Remove(category);
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
}
