using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Envelope : MonoBehaviour
{
    [SerializeField] private float speed = 50.0f;
    [SerializeField] private float lifetime = 3.0f;
    private bool hitObject = false;
    private Vector3 throwDirection;
    private AudioSource hitSound;

    void Start()
    {
        throwDirection = transform.forward;
        hitSound = GetComponent<AudioSource>();
        Destroy(gameObject, 20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Keep moving/rotating envelope until a hit is detected
        if (!hitObject)
        {
            transform.Translate(throwDirection * Time.deltaTime * speed, Space.World);
            transform.Rotate(0, 5, 0, Space.Self);
            DetectHit();
            lifetime -= Time.deltaTime;
        }

        // If no collision detected after lifetime has expired, destroy object
        if (lifetime <= 0 && !hitObject)  
            Destroy(gameObject);
    }

    void DetectHit()
    {
        // Casts ray out in direction of movement
        RaycastHit hit;
        if (Physics.Raycast(transform.position, throwDirection, out hit, 1.0f))
        {
            // If a hit is detected, stick envelope to hit object and damage it if it's an enemy
            transform.position = hit.point;
            hitObject = true;
            GameObject hitObj = hit.transform.gameObject;
            hitSound.Play();

            if (hitObj.CompareTag("Enemy"))
            {
                hitObj.GetComponent<Enemy>().TakeDamage();
                transform.parent = hitObj.transform;
            }
        }
    }
}
