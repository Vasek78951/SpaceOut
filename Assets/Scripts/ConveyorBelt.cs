using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ConveyorBelt : MonoBehaviour
{
    public float forwardSpeed;
    public float middleSpeed;
    public Transform middleTransform;
    public Transform destinationTransform;

    public void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("triggering");
        if (collision.gameObject.layer == 11)
        {
            Vector2 objectPos = collision.transform.position;
            Vector2 directionMiddle = new Vector2(middleTransform.position.x, middleTransform.position.y) - objectPos;
            Vector2 directionDestination = new Vector2(destinationTransform.position.x, destinationTransform.position.y) - objectPos;
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.MovePosition(rb.position + (directionMiddle * middleSpeed) * Time.deltaTime);
            rb.MovePosition(rb.position + (directionDestination.normalized * forwardSpeed) * Time.deltaTime);
            //Vector2 direction;
            //if(directionDestination.x > directionDestination.y)
            //{

            //    direction = new Vector2(0, directionMiddle.y) * middleSpeed;
            //    if(directionMiddle != Vector2.zero)
            //        direction += new Vector2(directionDestination.x, 0).normalized * forwardSpeed;
            //    rb.MovePosition(rb.position + direction);

            //}
            //else
            //{

            //    direction = new Vector2(directionMiddle.x, 0) * middleSpeed);
            //    if (directionMiddle != Vector2.zero)
            //        direction += new Vector2(0, directionDestination.y).normalized * forwardSpeed;
            //    rb.MovePosition(rb.position + direction);
            //}
        }
    }


}
