using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates clothing selection buttons for the chosen category based on the currently selected gender.
/// </summary>
public class CustomizationPartsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject clothingButtonPrefab;

    [Header("Clothing Lists")]
    [SerializeField] private ClothingListSO maleClothingList;
    [SerializeField] private ClothingListSO femaleClothingList;

    private List<GameObject> spawnedButtons = new();

    private void OnEnable()
    {
        ShowCategory("Outfit");
    }

    /// <summary>
    /// Displays all clothing items belonging to the selected category, clearing old buttons and spawning new ones.
    /// </summary>
    public void ShowCategory(string category)
    {
        // Clear Buttons
        foreach (var obj in spawnedButtons)
            Destroy(obj);
        spawnedButtons.Clear();

        ClothingListSO activeList = CharacterCustomizationArea.currentGender == "Male" ? maleClothingList : femaleClothingList;

        if (!System.Enum.TryParse(category, out ClothingCategory categoryEnum))
            return;

        // Get Clothing List
        List<ClothingDataSO> clothes = activeList.GetClothingByCategory(categoryEnum);
        if (clothes == null || clothes.Count == 0)
        {
            Debug.Log($"No clothing found for {category}");
            return;
        }

        // Instantiate Buttons
        foreach (var clothing in clothes)
        {
            GameObject btn = Instantiate(clothingButtonPrefab, buttonParent);
            ClothingButtonUI buttonUI = btn.GetComponent<ClothingButtonUI>();
            buttonUI.Setup(clothing);

            spawnedButtons.Add(btn);
        }
    }
}
