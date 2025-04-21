using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float xScale;
    public float yScale;
    public void Awake()
    {
        Vector3 scale = new Vector3(xScale, yScale);
        transform.localScale = scale;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FloatingItem>() != null || collision.gameObject.GetComponent<DroppedItem>() != null)
        {
            Destroy(collision.gameObject);
        }
    }
}
