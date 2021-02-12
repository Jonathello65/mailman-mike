using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private bool packageDelivered = false;
    private GameManager manager;
    private SoundEffects sfx;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        sfx = FindObjectOfType<SoundEffects>();
    }

    public void ConfirmDelivery()
    {
        // When player throws a package into the delivery zone, change color and confirm delivery in game manager
        if (!packageDelivered && !manager.levelFinished)
        {
            packageDelivered = true;
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0.11f, 0.47f);
            manager.UpdatePackages();
            sfx.PlaySound("PackageDelivered");
        }
    }
}
