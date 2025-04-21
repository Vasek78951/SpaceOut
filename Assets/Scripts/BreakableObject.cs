using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public float maxHealth = 5f;
    public float currentHealth;
    public ObjectType objectType;  // Type of breakable object

    private Renderer objectRenderer; // Reference to the object's renderer
    private Color originalColor; // Stores the original color

    private void Start()
    {
        currentHealth = maxHealth;
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color; // Save the original color
        }
    }

    public void TakeDamage(float baseDamage, ItemTypeData itemTypeData)
    {
        float multiplier = (itemTypeData != null) ? itemTypeData.GetDamageMultiplier(objectType) : 1;
        float totalDamage = baseDamage * multiplier;

        currentHealth -= totalDamage;
        Debug.Log($"{gameObject.name} took {totalDamage} damage ({baseDamage} base * {multiplier}x multiplier). Remaining health: {currentHealth}");

        if (objectRenderer != null)
        {
            StartCoroutine(FlashRed()); // Start the color change effect
        }

        if (currentHealth <= 0)
        {
            BreakObject();
        }
    }

    private IEnumerator FlashRed()
    {
        objectRenderer.material.color = Color.red; // Change to red
        yield return new WaitForSeconds(0.1f); // Wait for half a second
        objectRenderer.material.color = originalColor; // Revert to the original color
    }

    private void BreakObject()
    {
        Debug.Log($"{gameObject.name} is destroyed!");
        Destroy(gameObject);
    }
}
