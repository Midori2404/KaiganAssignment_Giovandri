using UnityEngine;

public class TestInstantiatteMesh : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer originalSkinMesh;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererPrefab;
    [SerializeField] private Transform rootBone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SkinnedMeshRenderer spawnedSkinnedMeshRenderer = Instantiate(skinnedMeshRendererPrefab, transform);
        spawnedSkinnedMeshRenderer.bones = originalSkinMesh.bones;
        spawnedSkinnedMeshRenderer.rootBone = rootBone;

        foreach (Transform bone in originalSkinMesh.bones)
        {
            Debug.Log(bone);
        }
    }

    
}
