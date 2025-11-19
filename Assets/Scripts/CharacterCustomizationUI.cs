using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles entering the customization area, opening the UI, spawning the selected gender model, and switching cameras.
/// </summary>
public class CharacterCustomizationArea : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject genderPanel;
    [SerializeField] private GameObject avatarCustomizeDisplay;

    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera customizeCamera;

    [Header("Model Spawn Points")]
    [SerializeField] private Transform modelSpawnPoint;

    public GameObject maleModelPrefab;
    public GameObject femaleModelPrefab;
    public GameObject currentModel;

    public static string currentGender;

    private bool playerInRange = false;
    private bool isCustomizing = false;

    void Start()
    {
        genderPanel.SetActive(false);
        avatarCustomizeDisplay.SetActive(false);

        customizeCamera.gameObject.SetActive(false);
        gameplayCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (playerInRange && Input.GetKey(KeyCode.F) && !isCustomizing)
        {
            OpenCustomization();
        }
    }

    public void OpenCustomization()
    {
        isCustomizing = true;

        gameplayCamera.gameObject.SetActive(false);
        customizeCamera.gameObject.SetActive(true);

        genderPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Spawns the selected gender model and opens the customization UI.
    /// </summary>
    public void PickGender(string gender)
    {
        if (currentModel != null)
            Destroy(currentModel);

        // Spawn selected model
        if (gender == "Female")
        {
            currentGender = "Female";
            currentModel = Instantiate(femaleModelPrefab, modelSpawnPoint);
        }
        if (gender == "Male")
        {
            currentGender = "Male";
            currentModel = Instantiate(maleModelPrefab, modelSpawnPoint);
        }

        genderPanel.SetActive(false);
        avatarCustomizeDisplay.SetActive(true);
        AvatarCustomizeManager.Instance.InitializeModel(currentModel);
    }

    public void BackToGenderSelection()
    {
        currentGender = null;
        avatarCustomizeDisplay.SetActive(false);

        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
        }

        genderPanel.SetActive(true);
    }

    public void CloseCustomization()
    {
        currentGender = null;
        isCustomizing = false;

        genderPanel.SetActive(false);
        avatarCustomizeDisplay.SetActive(false);

        gameplayCamera.gameObject.SetActive(true);
        customizeCamera.gameObject.SetActive(false);

        if (currentModel != null)
            Destroy(currentModel);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
