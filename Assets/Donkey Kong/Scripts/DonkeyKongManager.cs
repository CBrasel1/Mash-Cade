using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class DonkeyKongManager : MonoBehaviour
{
    public static DonkeyKongManager Instance { get; private set; }
    [SerializeField] private Player mario;

    private const int NUM_LEVELS = 1;

    public int level { get; private set; } = 1;
    public int score { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void NewGame()
    {
        score = 0;

        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("DonkeyKong");
    }

    public void LevelComplete()
    {
        score += 1000;
        string nextScene = GameManager.RandomizeScene();
        if(nextScene == "") {
            SceneManager.LoadScene("Win");
        } else {

            SceneManager.LoadScene(nextScene);
        }
    }

    public void Death() {
        GameManager.LoseALife();

        if (GameManager.lives <= 0) {
            SceneManager.LoadScene("GameOver");
        } else {
            SceneManager.LoadScene("DonkeyKong");
        }
    }

    public void LevelFailed()
    {
        Invoke(nameof(Death), 3f);
    }

}
