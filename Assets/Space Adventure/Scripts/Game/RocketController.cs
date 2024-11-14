using System.Collections;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Range(0, 9.5f)]
    public float speed = 8.0f;
    [Range(0, 2.0f)]
    public float changeLanesSpeed = 2.0f;

    public Transform parts;
    public GameObject crashedParticles;

    private int lane = 0;
    private bool changingLanes;
    private float duration;
    private Vector3 startPos, endPos;
    private bool paused;

    private Animation anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = this.GetComponent<Animation>();
        audioSource = this.GetComponent<AudioSource>();
        LoadRocket();
        UpdatePosition();
        UpdateObstaclesSpeed(speed);
    }

    void Update()
    {
        if (!paused)
        {
            // Check if changing lanes
            changingLanes = transform.position != endPos;

            // If user presses A or Left Arrow.
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }

            // If user presses D or Right Arrow.
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }

            // If changing lanes, move the rocket smoothly
            if (changingLanes)
            {
                if (changeLanesSpeed > 0)
                {
                    duration += Time.deltaTime / ((2 - changeLanesSpeed) / 10);
                    transform.position = Vector3.Lerp(startPos, endPos, duration);
                }
            }
        }
    }

    public void MoveLeft()
    {
        if (lane > -2)
        {
            lane--;
            anim.Play("Move-Left");
            audioSource.Play();
            UpdatePosition();
        }
    }

    public void MoveRight()
    {
        if (lane < 2)
        {
            lane++;
            anim.Play("Move-Right");
            audioSource.Play();
            UpdatePosition();
        }
    }

    private void LoadRocket()
    {
        foreach (Transform part in parts)
        {
            if (part.name != "Base")
            {
                bool partAdded = PlayerPrefs.GetInt("PartAdded-" + part.name, 0) == 1;
                part.gameObject.SetActive(partAdded);
            }
        }
    }

    private void UpdatePosition()
    {
        duration = 0;
        startPos = transform.position;
        endPos = new Vector3(lane, transform.position.y, transform.position.z);
    }

    private void UpdateObstaclesSpeed(float obstaclesSpeed)
    {
        ObstaclesLine.speed = obstaclesSpeed;
    }

    public void Pause()
    {
        paused = true;
        UpdateObstaclesSpeed(0);
    }

    public void Resume()
    {
        paused = false;
        UpdateObstaclesSpeed(speed);
    }

    public void Crashed()
    {
        Pause();
        crashedParticles.SetActive(true);

#if UNITY_ANDROID || UNITY_IOS
        if (Settings.GetSetting("Vibration"))
        {
            Handheld.Vibrate();
        }
#endif
    }
}
