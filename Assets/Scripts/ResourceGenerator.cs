using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    public GameObject despawnArea;
    [Range(0, 30)] public float spawnRange;
    [Range(1, 5)] public float spawnTimeRange;
    public FloatingItem floatItemPrefab;
    public List<ItemSpawn> itemsToSpawn;

    [System.Serializable]
    public class ItemSpawn
    {
        public Item item;
        public float spawnChance;
    }
    private void Start()
    {
        StartCoroutine(SpawnItem());
    }
    private IEnumerator SpawnItem()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1, spawnTimeRange));

            Item itemToSpawn = GetRandomItem();
            if (itemToSpawn != null && itemToSpawn.worldPrefab != null)
            {
                float rndy = Random.Range(
                    -(despawnArea.transform.localScale.y / 2) + 1,
                     (despawnArea.transform.localScale.y / 2) - 1
                );
                Vector2 spawnPos = new Vector2(
                    (despawnArea.transform.localScale.x / 2) - 1,
                    rndy
                );

                // Spawn the FloatingItem
                FloatingItem floatItem = Instantiate(floatItemPrefab, spawnPos, Quaternion.identity);
                floatItem.InitialiseItem(itemToSpawn);
            }
        }
    }


    private Item GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (var item in itemsToSpawn)
        {
            totalWeight += item.spawnChance;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var item in itemsToSpawn)
        {
            cumulativeWeight += item.spawnChance;
            if (randomValue < cumulativeWeight)
            {
                return item.item;
            }
        }

        return null;
    }

}
