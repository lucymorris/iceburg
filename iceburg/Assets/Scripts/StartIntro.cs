using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StartIntro : MonoBehaviour
{
    public string playSceneName;
    public GameObject startScreen;

    PlayableDirector timeline;
    bool started = false;

    void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        startScreen.SetActive(true);
    }

    void Update()
    {
        if (timeline.state != PlayState.Playing)
        {
            if (!started)
            {
                if (Input.GetButtonDown("A_1") || Input.GetButtonDown("Interact"))
                {
                    started = true;
                    startScreen.SetActive(false);
                    timeline.Play();
                }
            }
            else
            {
                SceneManager.LoadScene(playSceneName);
            }
        }
    }
}