using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SoundEffects sfx;
    [SerializeField] private float movementTime = 1.0f;
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private int health = 5;
    private int maxHealth;
    private float switchTimer = 0.0f;
    private bool movingLeft = true;
    private Renderer enemyModel;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        enemyModel = transform.GetChild(0).gameObject.GetComponent<Renderer>();
        sfx = FindObjectOfType<SoundEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveLeftAndRight();
    }

    void MoveLeftAndRight()
    {
        // Moves back and forth going to the left and right at a constant speed 
        switchTimer += Time.deltaTime;
        if (switchTimer >= movementTime)
        {
            movingLeft = !movingLeft;
            switchTimer = 0.0f;
        }

        if (movingLeft)
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage()
    {
        // Takes damage when hit by envelope, changing color to be more black until health is gone and enemy is destroyed
        health--;
        float col = ((float)health / (float)maxHealth);
        enemyModel.material.color = new Color(col, col, col, 0);
        if (health <= 0)
        {
            sfx.PlaySound("EnemyDestroy");
            Destroy(gameObject);
        }
    }
}
