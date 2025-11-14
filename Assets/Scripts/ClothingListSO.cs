using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClothingListSO", menuName = "Scriptable Objects/ClothingListSO")]
public class ClothingListSO : ScriptableObject
{

    [Header("Available Clothing Per Category")]
    public List<ClothingDataSO> hairClothes = new();
    public List<ClothingDataSO> topClothes = new();
    public List<ClothingDataSO> bottomClothes = new();
    public List<ClothingDataSO> shoeClothes = new();
    public List<ClothingDataSO> outfitClothes = new();

    public List<ClothingCategory> GetAllCategories()
    {
        return new List<ClothingCategory>
        {
            ClothingCategory.Hair,
            ClothingCategory.Top,
            ClothingCategory.Bottom,
            ClothingCategory.Shoes,
            ClothingCategory.Outfit
        };
    }

    public List<ClothingDataSO> GetClothingByCategory(ClothingCategory category)
    {
        return category switch
        {
            ClothingCategory.Hair => hairClothes,
            ClothingCategory.Top => topClothes,
            ClothingCategory.Bottom => bottomClothes,
            ClothingCategory.Shoes => shoeClothes,
            ClothingCategory.Outfit => outfitClothes,
            _ => null
        };
    }

    public ClothingDataSO GetRandomClothing(ClothingCategory category)
    {
        List<ClothingDataSO> list = GetClothingByCategory(category);
        if (list == null || list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }
}

public enum ClothingCategory
{
    Hair,
    Top,
    Bottom,
    Shoes,
    Outfit
}