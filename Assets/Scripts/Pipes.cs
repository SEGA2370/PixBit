using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public float speed = 5f;
    private float leftEdge;

    private void Start()
    {
        if (Camera.main != null)
        {
            leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 6f;
        }
    }

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < leftEdge)
        {
           Destroy(gameObject);
        }
    }
}
