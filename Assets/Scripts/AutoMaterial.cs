using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMaterial : MonoBehaviour
{
    public SkinnedMeshRenderer SkinnedMeshRenderer;
    public MeshRenderer[] renderers;
    public Material setMaterial;
    void Start()
    {
        MaterialChange(setMaterial, gameObject);
    }

    public void MaterialChange(Material material, GameObject gameObject) {
        SkinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true);
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);

        List<Material> materials = new List<Material>();
        foreach (MeshRenderer renderer in renderers) {
            if (renderer.gameObject.tag != "IgnoreMaterial") {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                    materials.Add(material);
                }
                renderer.SetMaterials(materials);
            } else {
                renderer.materials[0] = material;
            }
        }
        materials.Clear();

        for (int i = 0; i < SkinnedMeshRenderer.sharedMaterials.Length; i++) {
            materials.Add(material);
        }
        SkinnedMeshRenderer.SetMaterials(materials);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("N")) {
            
        }
    }
}
