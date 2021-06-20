using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

    private Transform[] children;

    // How far apart the children should be
    public float distance = 0.1f;
    // How large the children are.
    public float scale = 0.1f;

    // The speed at which the children rotates.
    public float speed = 200;

    // The direction that the children rotates.
    // 1 = counter-clockwise
    // -1 = clockwise
    private float direction = -1;

    private PowerupAction action;

    delegate Color PowerupAction(Player player, bool getColor);

    Color PowerupActionHeal(Player player, bool getColor = false)
    {
        if (!getColor)
        {
            player.AddLife();
        }
        return GetColor(118, 255, 0);
    }

    static Color GetColor(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    void SetPowerupType()
    {
        var options = new List<PowerupAction>();
        options.Add(PowerupActionHeal);

        action = options[Random.Range(0, options.Count)];
    }

    void ColorChildren()
    {
        foreach (Transform child in children)
        {
            ColorChild(child);
        }
    }

    void ColorChild(Transform child)
    {
        var surface = child.GetComponent<MeshRenderer>();
        var mat = surface.material;

        var color = action(null, true);

        mat.SetColor("_Color", color);
        surface.material = mat;
    }

    void GetChildren()
    {
        children = new Transform[2];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }

    void RotateChildren()
    {
        foreach (Transform child in children)
        {
            child.RotateAround(this.transform.position, new Vector3(0, 0, direction), speed * Time.deltaTime);
        }
    }

    public void Consume(Player player)
    {
        action(player, false);
        Destroy(this.gameObject);
    }

    void MoveChildren()
    {
        children[0].position += new Vector3(0, distance, 0);
        children[1].position += new Vector3(0, -distance, 0);
    }

    void ScaleChildren()
    {
        foreach (Transform child in children)
        {
            child.localScale = new Vector3(scale, scale, scale);
        }
    }

    // Use this for initialization
    void Start () {
        GetChildren();
        MoveChildren();
        ScaleChildren();
        SetPowerupType();
        ColorChildren();
	}
	
	// Update is called once per frame
	void Update () {
        RotateChildren();
	}
}
