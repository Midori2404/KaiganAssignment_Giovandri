using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCustomizationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject genderPanel;
    [SerializeField] private GameObject avatarCustomizeDisplay;
    [SerializeField] private GameObject maleCatalogue;
    [SerializeField] private GameObject femaleCatalogue;

    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera customizeCamera;

    [Header("Model Spawn Points")]
    [SerializeField] private Transform modelSpawnPoint;

    public GameObject maleModelPrefab;
    public GameObject femaleModelPrefab;
    public GameObject currentModel;

    public static string currentGender;

    private AvatarCustomizeManager avatarCustomizeManager;
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

    public void PickGender(string gender)
    {
        if (currentModel != null)
            Destroy(currentModel);

        // Spawn selected model
        if (gender == "Female")
        {
            currentGender = "Female";
            currentModel = Instantiate(femaleModelPrefab, modelSpawnPoint);
            femaleCatalogue.SetActive(true);
            maleCatalogue.SetActive(false);

            avatarCustomizeManager = femaleCatalogue.GetComponent<AvatarCustomizeManager>();
        }
        if (gender == "Male")
        {
            currentGender = "Male";
            currentModel = Instantiate(maleModelPrefab, modelSpawnPoint);
            maleCatalogue.SetActive(true);
            femaleCatalogue.SetActive(false);

            avatarCustomizeManager = maleCatalogue.GetComponent<AvatarCustomizeManager>();
        }

        // Switch panels
        genderPanel.SetActive(false);
        avatarCustomizeDisplay.SetActive(true);
        avatarCustomizeManager.InitializeVariables();
    }

    public void BackToGenderSelection()
    {
        currentGender = null;
        avatarCustomizeDisplay.SetActive(false);
        maleCatalogue.SetActive(false);
        femaleCatalogue.SetActive(false);

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
