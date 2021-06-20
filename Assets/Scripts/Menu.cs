using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Color background = GetColor(110, 181, 102);

    // Use this for initialization
    void Start () {
        SetBackground();
	}
	
	// Update is called once per frame
	void Update () {

    }

    static Color GetColor(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f, 1);
    }

    public void LoadNormal()
    {
        SceneManager.LoadScene("Normal");
    }

    public void LoadPlayground()
    {
        SceneManager.LoadScene("Playground");
    }

    public void LoadLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    Color BackgroundHint()
    {
        var color = background;
        var v = 1.085f;
        color.r = Mathf.Min(1, color.r * v);
        color.g = Mathf.Min(1, color.g * v);
        color.b = Mathf.Min(1, color.b * v);
        return color;
    }

    void SetBackground()
    {
        var skybox = Camera.main.GetComponent<Skybox>();
        var mat = skybox.material;
        mat.SetColor("_Color", BackgroundHint());
        mat.SetColor("_Color2", background);
    }
}
