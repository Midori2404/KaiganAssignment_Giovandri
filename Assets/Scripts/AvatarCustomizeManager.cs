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

    private void Start()
    {
    }

    public void EquipClothing(ClothingDataSO newClothing)
    {
        // Restore previously hidden parts
        if (hiddenPartsByCategory.TryGetValue(newClothing.category, out var previouslyHidden))
        {
            foreach (var part in previouslyHidden)
                if (part != null)
                    part.SetActive(true);
            hiddenPartsByCategory.Remove(newClothing.category);
        }

        // Remove old clothing
        if (activeRenderers.TryGetValue(newClothing.category, out var oldRenderer))
        {
            if (oldRenderer != null)
                Destroy(oldRenderer.gameObject);
        }

        // Determine parent based on clothing type
        Transform parentSlot = (newClothing.category == ClothingCategory.Hair) ? headAccessoriesSlot : playerBody;

        // Spawn new clothing
        SkinnedMeshRenderer newRenderer = Instantiate(newClothing.clothingPrefab, parentSlot);
        newRenderer.bones = baseBodyRenderer.bones;
        newRenderer.rootBone = rootBone;

        // Hide body parts based on name lookup
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
}
