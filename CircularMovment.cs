using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovment : MonoBehaviour
{
    [SerializeField]
    public Transform rotationCenter;

    [SerializeField]
    public float rotationRadius = 2f;

    [SerializeField]
    public float angularSpeed = 2f;
    
    public float X, Y, angle = 0f;

    // Update is called once per frame
    void Update()
    {
        X = rotationCenter.position.x + Mathf.Cos(angle) * rotationRadius;
        Y = rotationCenter.position.y + Mathf.Sin(angle) * rotationRadius;

        transform.position = new Vector2(X, Y);

        angle = angle + Time.deltaTime * angularSpeed;

        if (angle >= 360f) angle = 0f;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }

}
