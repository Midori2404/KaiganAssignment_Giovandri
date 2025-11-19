using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClothingButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    private ClothingDataSO clothingData;

    public void Setup(ClothingDataSO data)
    {
        clothingData = data;
        label.text = data.name;
    }

    public void OnClick()
    {
        AvatarCustomizeManager.Instance.EquipClothing(clothingData);
    }
}
