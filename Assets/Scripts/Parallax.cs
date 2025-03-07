using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    [SerializeField] float animationSpeed = 1f;
    [SerializeField] Material[] materials; // Array of materials
    [SerializeField] float changeInterval = 45f; // Time in seconds for material change

    private int currentMaterialIndex = 0;
    private float timer = 0f;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (materials.Length > 0)
        {
            meshRenderer.material = materials[currentMaterialIndex];
        }
    }

    private void Update()
    {
        // Update the texture offset for parallax effect
        meshRenderer.material.mainTextureOffset += new Vector2(animationSpeed * Time.deltaTime, 0);

        // Update timer and change material if necessary
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f; // Reset timer
            ChangeMaterial();
        }
    }

    private void ChangeMaterial()
    {
        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length; // Loop through materials
        meshRenderer.material = materials[currentMaterialIndex];
    }

    public void ResetMaterial()
    {
        currentMaterialIndex = 0; // Reset index to 0
        if (materials.Length > 0)
        {
            meshRenderer.material = materials[currentMaterialIndex]; // Set to the first material
        }
        meshRenderer.material.mainTextureOffset = Vector2.zero; // Reset texture offset
    }
}
