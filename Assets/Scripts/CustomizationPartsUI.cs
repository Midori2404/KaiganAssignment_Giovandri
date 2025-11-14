using UnityEngine;

public class CustomizationPartsUI : MonoBehaviour
{

    [Header("Female UI Panels")]
    [SerializeField] private GameObject femaleOutfitPanel;
    [SerializeField] private GameObject femaleHairPanel;
    [SerializeField] private GameObject femaleTopPanel;
    [SerializeField] private GameObject femaleBottomPanel;
    [SerializeField] private GameObject femaleshoesPanel;

    [Header("Male UI Panels")]
    [SerializeField] private GameObject maleOutfitPanel;
    [SerializeField] private GameObject maleHairPanel;
    [SerializeField] private GameObject maleTopPanel;
    [SerializeField] private GameObject maleBottomPanel;
    [SerializeField] private GameObject maleshoesPanel;

    private void OnEnable()
    {
        ShowCategory("Outfit");
    }

    public void ShowCategory(string category)
    {
        if (CharacterCustomizationUI.currentGender == "Female")
        {
            femaleOutfitPanel.SetActive(false);
            femaleHairPanel.SetActive(false);
            femaleTopPanel.SetActive(false);
            femaleBottomPanel.SetActive(false);
            femaleshoesPanel.SetActive(false);

            switch (category)
            {
                case "Outfit": femaleOutfitPanel.SetActive(true); break;
                case "Hair": femaleHairPanel.SetActive(true); break;
                case "Top": femaleTopPanel.SetActive(true); break;
                case "Bottom": femaleBottomPanel.SetActive(true); break;
                case "Shoes": femaleshoesPanel.SetActive(true); break;
            }
        }

        if (CharacterCustomizationUI.currentGender == "Male")
        {
            maleOutfitPanel.SetActive(false);
            maleHairPanel.SetActive(false);
            maleTopPanel.SetActive(false);
            maleBottomPanel.SetActive(false);
            maleshoesPanel.SetActive(false);

            switch (category)
            {
                case "Outfit": maleOutfitPanel.SetActive(true); break;
                case "Hair": maleHairPanel.SetActive(true); break;
                case "Top": maleTopPanel.SetActive(true); break;
                case "Bottom": maleBottomPanel.SetActive(true); break;
                case "Shoes": maleshoesPanel.SetActive(true); break;
            }
        }
    }
}
