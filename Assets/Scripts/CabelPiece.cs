using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CablePiece : MonoBehaviour
{
    public bool isPowered;
    private CableManager manager;
    private SpriteRenderer spriteRenderer;
    private Raft raft;

    private void Awake()
    {
        manager = FindAnyObjectByType<CableManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        raft = FindAnyObjectByType<Raft>();
    }
    public void SetPowered()
    {
        isPowered = true;
        spriteRenderer.color = Color.yellow;
        Debug.Log("color: " + spriteRenderer.color);
        var around = manager.CheckAround(this);
        if (around == null || around.Count <= 0)
        {
            return;
        }
        foreach (CablePiece c in around)
        {
            if (!c.isPowered)
            {
                c.SetPowered();
                PowerConsumers(true);
            }
        }
    }
    private void PowerConsumers(bool value)
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
            if (raft.placedBuildings.Contains(new Vector2Int((int)transform.position.x, (int)transform.position.y) + direction))
            {
                Collider2D collider = Physics2D.OverlapBox(new Vector2Int((int)transform.position.x, (int)transform.position.y) + direction, new Vector2(0.5f, 0.5f), 0);
                var consumer = collider.GetComponent<TimeProccesingObject>();
                if (consumer != null)
                {
                    consumer.isPowered = value;
                }
            }
        }
    }

    public void SetUnpowered()
    {
        var around = manager.CheckAround(this);
        if(around == null || around.Count <= 0)
        {
            isPowered = false;
            spriteRenderer.color = Color.gray;
            PowerConsumers(false);
            return;
        }
        foreach( CablePiece c in around)
        {
            if (c.isPowered)
            {
                SetPowered();
                return;
            }
        }
    }

    private void OnDestroy()
    {
        var around = manager.CheckAround(this);
        if (around == null || around.Count <= 0)
        {
            return;
        }
        foreach (CablePiece c in around)
        {
            if (c.isPowered)
            {
                c.SetUnpowered();
            }
        }
    }

    private void UpdateVisuals()
    {
        // This method can change the sprite or rotation based on Connections
    }
}

