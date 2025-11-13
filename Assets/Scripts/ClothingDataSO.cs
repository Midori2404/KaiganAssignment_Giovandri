using UnityEngine;

[CreateAssetMenu(fileName = "ClothingDataSO", menuName = "Scriptable Objects/ClothingDataSO")]
public class ClothingDataSO : ScriptableObject
{
    public SkinnedMeshRenderer clothingPrefab;
    public string[] bodyPartsToHideName;

    public ClothingCategory category;
}
