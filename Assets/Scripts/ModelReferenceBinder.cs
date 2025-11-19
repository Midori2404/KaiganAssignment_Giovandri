using UnityEngine;

/// <summary>
/// Stores references to bones, head slot, base body renderer, and body parts. This keeps models modular and avoids hard-coding.
/// </summary>
public class ModelReferenceBinder : MonoBehaviour
{
    [Header("Select Gender")]
    public Gender gender;

    [Header("Required References")]
    public Transform rootBone;
    public Transform headAccessoriesSlot;
    public SkinnedMeshRenderer baseBodyRenderer;
    public SkinnedMeshRenderer combinedMeshRenderer;

    [System.Serializable]
    public class BodyPartEntry
    {
        public string partName;
        public SkinnedMeshRenderer renderer;
    }

    [Header("Body Parts List")]
    public BodyPartEntry[] bodyParts;
}

public enum Gender
{
    Male,
    Female
}