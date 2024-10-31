using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parralaxEffect;
    private float length;
    private float xPosition;
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parralaxEffect);

        float distanceToMove = cam.transform.position.x * parralaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if (distanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
    }
}
