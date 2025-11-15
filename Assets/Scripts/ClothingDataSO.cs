using UnityEngine;

[CreateAssetMenu(fileName = "ClothingDataSO", menuName = "Scriptable Objects/ClothingDataSO")]
public class ClothingDataSO : ScriptableObject
{
    [Header("Clothing Prefabs (Select ONE depending on category)")]
    public GameObject hairPrefab;
    public SkinnedMeshRenderer clothingPrefab;

    [Header("Body Parts To Be Hidden")]
    public string[] bodyPartsToHideName;

    public ClothingCategory category;
}
