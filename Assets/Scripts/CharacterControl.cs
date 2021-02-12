using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour 
{
    public GameObject package;
    public GameObject envelope;
    public SoundEffects sfx;
    public GameManager manager;
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private float m_animatorSpeed = 1;
    [SerializeField] private float m_throwCooldown = 1.0f;
    [SerializeField] private float m_altThrowCooldown = 0.5f;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;

    private float m_jumpTimeStamp = -1.0f;
    private float m_minJumpInterval = 0.25f;
    private bool m_hasDoubleJump = true;
    private float m_throwTimeStamp = -1.0f;
    private float m_altThrowTimeStamp = -1.0f;
    private float m_stepTime = 0;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();
    private Vector3 m_startingPos;
    private string[] m_footsteps = new string[] {"Footstep1", "Footstep2", "Footstep3", "Footstep4"};
    System.Random r = new System.Random();

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    void Start()
    {
        // Saves player starting position
        m_startingPos = transform.position;

        sfx = FindObjectOfType<SoundEffects>();
        manager = FindObjectOfType<GameManager>();
    }

	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        if (!manager.levelFinished)
        {
            Movement();
            Throwing();
        }
        else
        {
            m_animator.SetFloat("MoveSpeed", 0);
        }
        
        // Quits to menu when player presses ESC
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Main_Menu");

        m_wasGrounded = m_isGrounded;
        m_stepTime -= Time.deltaTime;
    }

    // Function for moving forward, backward, left, and right
    private void Movement()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float turn = Input.GetAxis("Mouse X");

        bool run = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) 
        {
            if (!run) 
            { 
                v *= m_walkScale;
                h *= m_walkScale; 
            }
            else 
            { 
                v *= m_backwardRunScale;
                h *= m_backwardRunScale; 
            }
        } 
        else if (!run)
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        HandleCollision();

        // Footstep sound effects
        if (((m_currentH < -0.1f || m_currentH > 0.1f) || (m_currentV < -0.1f || m_currentV > 0.1f)) && m_stepTime <= 0 && m_isGrounded)
        {
            sfx.PlaySound(m_footsteps[r.Next(m_footsteps.Length)]);
            //Debug.Log(m_footsteps[r.Next(m_footsteps.Length)]);
            if (run)
                m_stepTime = 0.25f;
            else if (v < 0 || Mathf.Abs(h) > v)
                m_stepTime = 0.3f;
            else
                m_stepTime = 0.4f;
        }

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.position += transform.right * m_currentH * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);

        if (v != 0)
            m_animator.SetFloat("MoveSpeed", m_currentV);
        else
            m_animator.SetFloat("MoveSpeed", -Mathf.Abs(m_currentH));
        
        m_animator.speed = m_animatorSpeed;

        JumpingAndLanding();
    }

    // Function for jumping, double jumping, and landing
    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            sfx.PlaySound("Jump1");
        }

        if (!m_isGrounded && !m_wasGrounded && m_hasDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidBody.velocity = Vector3.zero;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            m_hasDoubleJump = false;
            sfx.PlaySound("Jump2");
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
            m_hasDoubleJump = true;
            sfx.PlaySound(m_footsteps[r.Next(m_footsteps.Length)]);
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    // Function for throwing packages and envelopes
    private void Throwing()
    {
        bool throwCooldownOver = (Time.time - m_throwTimeStamp) >= m_throwCooldown;
        bool altThrowCooldownOver = (Time.time - m_altThrowTimeStamp) >= m_altThrowCooldown;

        if (throwCooldownOver && Input.GetMouseButtonDown(0))
        {
            m_throwTimeStamp = Time.time;
            Instantiate(package, transform.position + transform.forward / 3 + transform.up / 2, Camera.main.transform.rotation);
            sfx.PlaySound("Throw1");
        }

        if (altThrowCooldownOver && Input.GetMouseButtonDown(1))
        {
            m_altThrowTimeStamp = Time.time;
            Instantiate(envelope, transform.position + transform.forward / 3 + transform.up / 2, Camera.main.transform.rotation);
            sfx.PlaySound("Throw2");
        }
    }

    // Casts out rays from head, body, and feet in the direction of movement to detect any collisions and sets velocity to 0 if player would collide with object
    void HandleCollision()
    {
        Vector3 vertical = transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        Vector3 horizontal = transform.right * m_currentH * m_moveSpeed * Time.deltaTime;
        Vector3 movementDirection = vertical + horizontal;
        
        Vector3 feet = transform.position + new Vector3(0, 0.1f, 0);
        Vector3 body = feet + new Vector3(0, 0.45f, 0);
        Vector3 head = feet + new Vector3(0, 0.9f, 0);
        
        RaycastHit hit;
        if (Physics.Raycast(feet, movementDirection, out hit, 0.35f))
        {
            m_currentH = 0;
            m_currentV = 0;
        }
        else if (Physics.Raycast(body, movementDirection, out hit, 0.35f))
        {
            m_currentH = 0;
            m_currentV = 0;
        }
        else if (Physics.Raycast(head, movementDirection, out hit, 0.35f))
        {
            m_currentH = 0;
            m_currentV = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Moves player back to starting position if they fall off map
        if (other.CompareTag("Boundary"))
        {
            transform.position = m_startingPos;
        }
    }
}
