using UnityEngine;

/// <summary>
/// Spawns a grid of randomized avatar models for preview/testing.
/// Each avatar is assigned random clothing based on its gender.
/// </summary>
public class CharacterSpawner : MonoBehaviour
{
    [Header("Avatar Prefabs")]
    [SerializeField] private GameObject maleAvatarPrefab;
    [SerializeField] private GameObject femaleAvatarPrefab;

    [Header("Clothing Lists")]
    [SerializeField] private ClothingListSO maleClothingListSO;
    [SerializeField] private ClothingListSO femaleClothingListSO;

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private float spacing = 2f;

    private int avatarsToSpawn = 0;

    /// <summary>
    /// Called by UI input field to set how many avatars should be spawned.
    /// </summary>
    public void SetAvatarCount(string countText)
    {
        if (int.TryParse(countText, out int value))
            avatarsToSpawn = Mathf.Max(0, value);
    }

    /// <summary>
    /// Spawns a new grid of randomized characters.
    /// </summary>
    public void SpawnAvatars()
    {
        ClearPrevious();

        for (int i = 0; i < avatarsToSpawn; i++)
        {
            int x = i % gridWidth;
            int z = i / gridWidth;

            Vector3 pos = transform.position + new Vector3(x * spacing, 0, z * spacing);

            GameObject modelPrefab = (Random.value > 0.5f) ? maleAvatarPrefab : femaleAvatarPrefab;
            GameObject avatar = Instantiate(modelPrefab, pos, Quaternion.identity, transform);

            Animator animator = avatar.GetComponent<Animator>();
            animator.SetBool("isWalking", true);

            ModelReferenceBinder binder = avatar.GetComponent<ModelReferenceBinder>();
            ClothingListSO clothingList =
                (binder.gender == Gender.Male) ? maleClothingListSO : femaleClothingListSO;

            AvatarCustomizeManager.Instance.RandomizeForAvatar(avatar, clothingList);
        }
    }

    private void ClearPrevious()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }
}
