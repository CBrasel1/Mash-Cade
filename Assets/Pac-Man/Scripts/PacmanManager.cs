using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// All code sections that are commented out are able to be uncommented to play a realistic single
// player game with ui and lives

[DefaultExecutionOrder(-100)]
public class PacmanManager : MonoBehaviour
{
    public static PacmanManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;
    [SerializeField] private Pacman pacman;
    [SerializeField] private Transform pellets;
    // [SerializeField] private Text gameOverText;
    // [SerializeField] private Text scoreText;
    // [SerializeField] private Text livesText;

    public int score { get; private set; } = 0;

    private int ghostMultiplier = 1;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (GameManager.lives <= 0 && Input.anyKeyDown) {
            NewGame();
        }
    }

    private void NewGame()
    {
        // SetScore(0);
        NewRound();
    }

    private void NewRound()
    {
        // gameOverText.enabled = false;

        foreach (Transform pellet in pellets) {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
    }

    private void GameOver()
    {
        // gameOverText.enabled = true;

        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].gameObject.SetActive(false);
        }

        pacman.gameObject.SetActive(false);
    }

    // private void SetLives(int lives)
    // {
    //     this.lives = lives;
    //     livesText.text = "x" + lives.ToString();
    // }

    // private void SetScore(int score)
    // {
    //     this.score = score;
    //     scoreText.text = score.ToString().PadLeft(2, '0');
    // }

    public void PacmanEaten()
    {
        pacman.DeathSequence();

        for(int i = 0; i < ghosts.Length; i++) {
            ghosts[i].chase.Disable();
            ghosts[i].scatter.Enable();
        }

        GameManager.LoseALife();

        if (GameManager.lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        // SetScore(score + points);

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        // SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
            pacman.gameObject.SetActive(false);
            
            string nextScene = GameManager.RandomizeScene();
            if(nextScene == "") {
                SceneManager.LoadScene("Win");
            } else {

                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf) {
                return true;
            }
        }

        return false;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }

}
