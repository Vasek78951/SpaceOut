using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CableManager : MonoBehaviour
{

    private Dictionary<Vector2Int, CablePiece> cableGrid = new Dictionary<Vector2Int, CablePiece>();
    private Dictionary<int, List<CablePiece>> networks = new Dictionary<int, List<CablePiece>>();
    private HashSet<Vector2Int> poweredCables = new HashSet<Vector2Int>();
    public GameObject prefab;

    public void InstantiateCabel(Vector2Int position, CablePiece newCable)
    {
        if (newCable == null)
        {
            Debug.LogError("Error: newCable is null.");
            return;
        }

        cableGrid[position] = newCable;
    }


    public List<CablePiece> CheckAround(CablePiece cable)
    {
        Vector2Int pos = cableGrid.FirstOrDefault(x => x.Value == cable).Key;
        List<CablePiece> founded = new List<CablePiece>();
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down
        };

        foreach (Vector2Int direction in directions)
        {
            if(cableGrid.ContainsKey(pos + direction))
            {
                Debug.Log("found");
                founded.Add(cableGrid[pos + direction]);
            }
        }
        return founded;
        
    }

    public void PowerAround(Vector2Int pos)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down
        };

        foreach (Vector2Int direction in directions)
        {
            if (cableGrid.ContainsKey(pos + direction))
            {
                Debug.Log("found");
                cableGrid[pos + direction].SetPowered();
            }
        }
    }

    public void separate(List<CablePiece> cabels)
    {
        foreach (CablePiece piece in cabels)
        {
            List<CablePiece> foundedInbranche = CheckAround(piece);
            for(int i = 0; i < cabels.Count; i++)
            {
                if (foundedInbranche.Contains(cabels[i]))
                {
                    cabels.Remove(cabels[i]);
                }
            }
            if(cabels.Count > 0)
            {
                networks.Add(getUniqueIndex(), foundedInbranche);
            }
        }
    }

    public int getUniqueIndex()
    {
        for(int i = 0; i < networks.Count + 1; i++)
        {
            if (networks[i] == null)
            {
                return i;
            }
        }
        return 0;
    }
}
