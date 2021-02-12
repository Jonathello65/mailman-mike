using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    public Text deliveriesText;
    public GameObject gameCompleteText;
    public AudioSource timeRunningOut;
    [SerializeField] private float time = 60.0f;
    private List<DeliveryZone> deliveryZones = new List<DeliveryZone>();
    private int numberOfZones;
    private int deliveriesMade = 0;
    public bool levelFinished = false;
    private SoundEffects sfx;

    void Start()
    {
        deliveryZones = GameObject.FindObjectsOfType<DeliveryZone>().ToList();
        numberOfZones = deliveryZones.Count;
        UpdatePackageText();

        sfx = FindObjectOfType<SoundEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        // Updates timer until time hits 0 or all packages are delivered
        if (!levelFinished)
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
                timerText.text = FormatTime(time);

                // Start playing timer sound effect when 5 seconds are left
                if (!timeRunningOut.isPlaying && time <= 5)
                {
                    timeRunningOut.Play();
                }
            }
            else
            {
                // Ran out of time, restart level
                timerText.text = "TIME OVER!";
                sfx.PlaySound("OutOfTime");
                timeRunningOut.Stop();
                StartCoroutine(RestartLevel());
            }
        }
    }

    string FormatTime(float time)
    {
        // formats time as minutes:seconds
        string minutes = Mathf.Floor((Mathf.Ceil(time) / 60)).ToString();
        string seconds = (Mathf.Ceil(time) % 60).ToString("00");

        return minutes + ":" + seconds;
    }

    public void UpdatePackages()
    {
        // Tracks packages delivered, completing the level when all deliveries are made
        deliveriesMade++;
        UpdatePackageText();
        if (deliveriesMade == numberOfZones && !levelFinished)
            StartCoroutine(CompleteLevel());
    }

    void UpdatePackageText()
    {
        // Displays number of packages delivered in UI
        deliveriesText.text = "Deliveries: " + deliveriesMade + "/" + numberOfZones;
    }

    IEnumerator CompleteLevel()
    {
        // Moves to next level when all packages are delivered
        levelFinished = true;
        timerText.text = "Level Complete!";
        sfx.PlaySound("LevelFinish");
        timeRunningOut.Stop();
        yield return new WaitForSeconds(3);

        // Moves to next level if available, otherwise game is complete and returns to menu
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextScene)
            SceneManager.LoadScene(nextScene);
        else
        {
            // Game complete
            gameCompleteText.SetActive(true);
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(0);
        }
            
    }

    IEnumerator RestartLevel()
    {
        // Restarts level if timer runs out before deliveries are finished
        levelFinished = true;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
