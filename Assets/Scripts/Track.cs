using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {

    // The width of the line
    private float width = .85f;

    // Tutorial has length 9.
    private int generateDistance = 10;

    public bool spawnPowerups = false;
    // How often the powerups should spawn.
    public int powerUpFrequency = 10;
    public GameObject powerup;
    // A list of all spawned powerups.
    private List<GameObject> powerups;

    // Since we remove part of the track, we need
    // to know how many we've removed so that we can return the
    // correct node to the player.
    private int nodeOffset = 0;

    // The center from which we calculate the generator distance from.
    public Vector2 center = new Vector2(0, 0);

    // The color of the track.
    public Color color = new Color(1, 1, 1, 0);

    private int numCapVertices = 1;

    private List<Vector2> track;

    // Keeps track of the total length of the track including removed pieces.
    private int trackLength = 0;

    /**
     * Return the lenght of the track.
     */
    public int Length()
    {
        if (track == null)
        {
            return 0;
        }
        return track.Count;
    }

    /**
     * Returns the world position for the specified node.
     * */
    public Vector2 Node(int position)
    {
        if (track == null) return Vector2.zero;
        position -= nodeOffset;
        if (position > Length() - 1)
            position = Length() - 1;
        return track[position];
    }

    void DrawTrack()
    {
        var lineRend = this.GetComponent<LineRenderer>();
        lineRend.numCapVertices = numCapVertices;
        lineRend.numCornerVertices = numCapVertices;
        lineRend.material = new Material(Shader.Find("Trak/TrackShader"));
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0, width);
        curve.AddKey(1, width);
        lineRend.widthCurve = curve;
        lineRend.gameObject.SetActive(true);

        lineRend.positionCount = track.Count;

        for (var i=0; i < track.Count; i++)
        {
            lineRend.SetPosition(i, track[i]);
        }
    }

    void DefaultTrack()
    {
        track.Add(new Vector2(0, 0));
        track.Add(new Vector2(1, 0));
        track.Add(new Vector2(2, 0));
        track.Add(new Vector2(2, 1));
        track.Add(new Vector2(2, 2));
        track.Add(new Vector2(1, 2));
        track.Add(new Vector2(1, 1));
        track.Add(new Vector2(2, 1));
        track.Add(new Vector2(3, 1));
        track.Add(new Vector2(4, 1));
        track.Add(new Vector2(4, 2));
        track.Add(new Vector2(5, 2));
        trackLength = track.Count;
    }

    delegate Vector2[] GenerateTrack();

    Vector2[] GenerateForward()
    {
        var curr = track[track.Count - 1];
        return new Vector2[] { curr + new Vector2(1, 0) };
    }

    Vector2[] GenerateUp()
    {
        var curr = track[track.Count - 1];
        return new Vector2[] { curr + new Vector2(0, 1) };
    }

    Vector2[] GenerateDown()
    {
        var curr = track[track.Count - 1];
        return new Vector2[] { curr + new Vector2(0, -1) };
    }

    Vector2[] GenerateLoopUp()
    {
        var curr = track[track.Count - 1];
        return new Vector2[] {
            curr + new Vector2(0, 1),
            curr + new Vector2(0, 2),
            curr + new Vector2(-1, 2),
            curr + new Vector2(-1, 1),
            curr + new Vector2(0, 1),
            curr + new Vector2(1, 1),
        };
    }

    Vector2[] GenerateLoopDown()
    {
        var curr = track[track.Count - 1];
        return new Vector2[] {
            curr + new Vector2(0, -1),
            curr + new Vector2(0, -2),
            curr + new Vector2(-1, -2),
            curr + new Vector2(-1, -1),
            curr + new Vector2(0, -1),
            curr + new Vector2(1, -1),
        };
    }

    /**
     * Generates a piece for the track. This piece could be one of:
     *      - Move Forward
     *      - Move Up
     *      - Move Down
     *      - A loop going UP
     *      - A loop going DOWN
     *      
     * The generated piece will then be returned.
     * The piece will have the correct coordinates and will not have to be
     * modified.
     */
    Vector2[] GenerateNode()
    {
        var curr = track[track.Count - 1]; // (6, 0)
        var prev = track[track.Count - 2]; // (6, 1)
        var dir = curr - prev;             // (0, -1)

        var UP = new Vector2(0, 1);
        var DOWN = new Vector2(0, -1);

        var options = new List<GenerateTrack>();
        options.Add(GenerateForward);
        if (dir.Equals(DOWN))
            options.Add(GenerateUp);
        if (dir.Equals(UP))
            options.Add(GenerateDown);
        options.Add(GenerateLoopUp);
        options.Add(GenerateLoopDown);

        var move = options[Random.Range(0, options.Count)];

        return move();
    }

    /**
     * Consumes any powerups located at the center of the track.
     */
    public void ConsumePowerup(Player player)
    {
        Debug.Log("Hello");
        if (spawnPowerups && powerups.Count > 0)
        {
            GameObject p = powerups[0];
            var pos3 = p.transform.position;
            var pos = new Vector2(pos3.x, pos3.y);
            if (pos.Equals(center))
            {
                var s = p.GetComponent<Powerup>();
                s.Consume(player);
                powerups.RemoveAt(0);
            }
        }
    }

    void SpawnPowerup()
    {
        if (trackLength % powerUpFrequency == 0)
        {
            var coords = Last();
            var p = Instantiate(powerup, coords, Quaternion.identity);
            powerups.Add(p);
        }
    }

    Vector2 Add(Vector2[] nodes)
    {
        foreach(var node in nodes)
        {
            track.Add(node);
            trackLength = track.Count;
            SpawnPowerup();
        }
        return nodes[nodes.Length - 1];
    }

    void GenerateTutorial()
    {
        track.Add(new Vector2(0, 0));
        Add(GenerateForward());
        Add(GenerateForward());
        Add(GenerateUp());
        Add(GenerateForward());
        Add(GenerateDown());
        Add(GenerateDown());
        Add(GenerateForward());
        Add(GenerateForward());
        Add(GenerateLoopDown());
        Add(GenerateForward());
        Add(GenerateLoopUp());
        Add(GenerateForward());
    }

    Vector2 First()
    {
        var first = track[0];
        return first;
    }

    Vector2 Last()
    {
        var last = track[track.Count - 1];
        return last;
    }

    bool InsideGeneratorDistance(Vector2 node)
    {
        node -= center;
        var distance = Mathf.Max(Mathf.Abs(node.x), Mathf.Abs(node.y));
        return distance < generateDistance;
    }

    bool LastInsideGeneratorDistance()
    {
        return InsideGeneratorDistance(Last());
    }

    bool FirstOutsideGeneratorDistance()
    {
        return !InsideGeneratorDistance(First());
    }

    void RemoveFirst()
    {
        track.RemoveAt(0);
        nodeOffset += 1;
    }

    /**
     * Generates a track until it hits the generator distance.
     * Returns true if the track was changed.
     */
    bool GenerateToDistance()
    {
        bool changed = false;
        while (LastInsideGeneratorDistance())
        {
            var nodes = GenerateNode();
            Add(nodes);
            changed = true;
        }
        return changed;
    }

    void CreateTrack()
    {
        track = new List<Vector2>();

        GenerateTutorial();
        GenerateToDistance();
    }

	// Use this for initialization
	void Start () {
        if (spawnPowerups)
            powerups = new List<GameObject>();
        CreateTrack();
        DrawTrack();
    }

    // Update is called once per frame
    void Update () {
        bool changed = false;
        while (FirstOutsideGeneratorDistance())
        {
            RemoveFirst();
            changed = true;
        }
        changed |= GenerateToDistance();
        if (changed)
        {
            DrawTrack();
        }
    }
}
