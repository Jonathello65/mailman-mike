using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] private float throwForce = 10.0f;
    private Rigidbody m_rigidbody;
    public AudioSource hitSound;
    private float soundTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Applies forward momentum and rotational force to package when spawned to simulate throw
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.AddForce((transform.forward * throwForce) + (transform.up * 2.5f), ForceMode.Impulse);
        m_rigidbody.AddTorque(50, 0, 10);

        // Disappear after 10 seconds
        Destroy(gameObject, 10);

        hitSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        soundTimer -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Confirm delivery when package is tossed into a delivery zone
        if (other.CompareTag("DeliveryZone"))
        {
            other.GetComponent<DeliveryZone>().ConfirmDelivery();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Plays sound on collision, uses timer so sound doesn't play multiple times at once
        if (soundTimer <= 0)
        {
            hitSound.Play();
            soundTimer = 0.25f;
        }
        
    }
}
