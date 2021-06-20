using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // If it is possible to control the player.
    public bool playable = true;

    // If the player can die or not.
    public bool invulnerable = false;

    // The two anchors of the player.
    public Transform[] anchor = new Transform[2];

    public GameObject score;

    // The amount of lives the player has.
    public int lives = 3;
    private int maxLives;

    private bool dead = false;

    private Color[] lifeColors = new Color[]
    {
        GetColor(44),
        GetColor(243, 73, 77),
        GetColor(243, 161, 73),
        GetColor(110, 181, 102),
    };

    // The speed at which the player rotates.
    public float speed = 200;
    private float speedSet = 200;

    // The speed we should increase with after each "level".
    private float speedIncrease = 2;

    // How close to a node the player has to be
    // to be allowed to move to it.
    private float accuracy = 0.35f;

    // The direction that the player rotates.
    // 1 = counter-clockwise
    // -1 = clockwise
    private int direction = -1;

    // Set to 0 if the first anchor is anchored
    // or 1 if the second one is.
    private int connected = 0;

    // The track or "map" that we are currently playing on.
    private Track track;

    // The current node in the track that we are on.
    private int current = 0;

    private Transform cameraPosition;

    static Color GetColor(float v)
    {
        return GetColor(v, v, v);
    }

    static Color GetColor(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    void RotateAnchor()
    {
        var l = AnchorLocked();
        var f = AnchorFree();

        speed = Mathf.Lerp(speed, speedSet, 5 * Time.deltaTime);
        f.RotateAround(l.position, new Vector3(0, 0, direction), speed * Time.deltaTime);
    }

    Transform AnchorLocked()
    {
        return anchor[connected];
    }

    Transform AnchorFree()
    {
        return anchor[connected == 1 ? 0 : 1];
    }

    bool MoveInput()
    {
        bool move = Input.GetKeyDown("space");
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                move = true;
                break;
            }
        }
        return move;
    }

    void GetTrack()
    {
        track = GameObject.FindGameObjectsWithTag("Track")[0].GetComponent<Track>();
    }

    /**
     * Returns the color that matches the remaining lives.
     */
    Color LifeColor()
    {
        int left = lives;
        left = Mathf.Min(lifeColors.Length - 1, left);
        left = Mathf.Max(left, 0);
        return lifeColors[left];
    }

    Color LifeColorHint()
    {
        var color = LifeColor();
        var v = 1.085f;
        color.r = Mathf.Min(1, color.r * v);
        color.g = Mathf.Min(1, color.g * v);
        color.b = Mathf.Min(1, color.b * v);
        return color;
    }

    void ShakeCamera()
    {
        var camera = Camera.main.GetComponent<CameraControl>();
        camera.Shake(.1f, 10, 5f);
    }

    void GetCameraPosition()
    {
        cameraPosition = GameObject.FindGameObjectWithTag("CameraPosition").transform;
    }

    void MoveCamera()
    {
        var pos = track.Node(current);
        cameraPosition.position = Vector3.Lerp(cameraPosition.position, pos, 5 * Time.deltaTime);

        var skybox = Camera.main.GetComponent<Skybox>();
        var mat = skybox.material;

        var color1 = mat.GetColor("_Color");
        var color2 = mat.GetColor("_Color2");

        // Set camera color to match lives.
        color1 = Color.Lerp(color1, LifeColorHint(), 5 * Time.deltaTime);
        color2 = Color.Lerp(color2, LifeColor(), 5 * Time.deltaTime);

        mat.SetColor("_Color", color1);
        mat.SetColor("_Color2", color2);
        skybox.material = mat;
    }

    void UpdateScore()
    {
        score.GetComponent<Text>().text = "" + current;
    }

    /**
     * Checks if the player is close enough to move to the next
     * node, if it is the player will be moved.
     * If not, well... that remains to be seen.
     */
    void Move()
    {
        var next = track.Node(current + 1);
        var pos = AnchorFree().position;
        var dist = Vector2.Distance(pos, next);

        if (dist < accuracy)
        {
            // Set the other anchor as locked.
            MoveTo(current + 1);
            
            speedSet += speedIncrease;
            RemoveAndResumeTrails();
            UpdateScore();
        }
        else
        {
            if (current > 1)
                RemoveLife();
            ShakeCamera();
        }
    }

    void RemoveLife()
    {
        /*
        * Have world be green at first.
        * When you lose a life the world changes color.
        * At the end it will be red?
        * Fail again and you die?
        */
        lives -= 1;
        if (lives == 0 && !invulnerable)
        {
            // TODO: Die.
            dead = true;
        }
    }

    public void AddLife()
    {
        lives += 1;
        if (lives > maxLives) lives = maxLives;
    }

    void RemoveAndResumeTrails()
    {
    }

    void PauseTrail()
    {
        var locked = AnchorFree();
        var script = locked.gameObject.GetComponent<TimedTrailRenderer>();
    }

    void MoveTo(int target)
    {
        var next = track.Node(target);

        connected = connected == 1 ? 0 : 1;
        current = target;

        var locked = AnchorLocked();

        locked.position = next;
        track.center = next;

        track.ConsumePowerup(this);
    }

    bool BackInput()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Use this for initialization
    void Start () {
        maxLives = lives;
        if (playable)
        {
            GetCameraPosition();
            GetTrack();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (BackInput())
        {
            LoadMenu();
        }
        if (!playable)
        {
            if (MoveInput()) ShakeCamera();
            RotateAnchor();
        }
        else
        {
            if (!dead)
            {
                // Spin the player.
                RotateAnchor();

                // See if we want to try and move the player.
                if (MoveInput())
                {
                    // Try to move the player.
                    Move();
                }
            }
            else
            {
                if (MoveInput())
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    PauseTrail();
                }
            }
            MoveCamera();
        }
    }
}
