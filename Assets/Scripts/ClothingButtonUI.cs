using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI button representing a single clothing item.
/// Displays its name and equips the item when clicked.
/// </summary>
public class ClothingButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    private ClothingDataSO clothingData;

    /// <summary>
    /// Initializes the button with the given clothing data.
    /// </summary>
    public void Setup(ClothingDataSO data)
    {
        clothingData = data;
        label.text = data.name;
    }

    /// <summary>
    /// Called by the UI Button component.
    /// Equips the selected clothing item.
    /// </summary>
    public void OnClick()
    {
        AvatarCustomizeManager.Instance.EquipClothing(clothingData);
    }
}
