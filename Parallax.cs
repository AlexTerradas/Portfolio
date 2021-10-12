using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    private float length, startPosition;
    public GameObject player;
    public float parallaxEffect;
    public float yPos;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float temp = (player.transform.position.x * (1 - parallaxEffect));
        float dist = (player.transform.position.x  *  parallaxEffect ) + 0.7f;

        transform.position = new Vector3(startPosition + dist, yPos, transform.position.z);

        if (temp > startPosition + length) startPosition += length;
        else if (temp < startPosition - length) startPosition -= length;

    }
}
