using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DigdugManager : MonoBehaviour {//For handling play sessions

    [SerializeField]
    private GameObject readyDialog;

    [SerializeField]
    private GameObject gameOverDialog;

    [SerializeField]
    private GameObject playerDialogHeader;

    [SerializeField]
    private Text playerDialogNumber;

    [SerializeField]
    private GameObject statsSidebar;

    [SerializeField]
    private Text sidebarPlayerOneScore;

    [SerializeField]
    private Text sidebarPlayerTwoScore;

    [SerializeField]
    private Text sidebarLevelNumber;

    [SerializeField]
    private Text sidebarHighScore;

    [SerializeField]
    private GameObject sidebarPlayerOneLivesParent;

    [SerializeField]
    private GameObject sidebarPlayerTwoLivesParent;

    [SerializeField]
    private GameObject[] sidebarPlayerOneLives;

    [SerializeField]
    private GameObject[] sidebarPlayerTwoLives;

    private Queue dialogQueue;

    internal enum GameMode { OnePlayer, TwoPlayer }

    private GameMode currentGameMode;
    public GameManager gameManager;

    private int playerTwoLives = 3;

    internal bool PlayerOneTurn { get; private set; } = true;

    private bool gameInProgress = false;

    private object currentDialogInProgress;

    private int levelCount;

    public int LevelCount {
        get { return levelCount; }
        set {
            levelCount = value;
            PlayerStats.CurrentLevel = value;
        }
    }

    private int sceneCounter;

    private static bool created;

    // internal bool HasGameEnded {
        // get {
        //     return GameManager.lives <= 0 && currentGameMode == GameMode.OnePlayer || playerTwoLives <= 0 && GameManager.lives <= 0;
        // }
    // }


    private void Awake() {
        if (!created) {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }

        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        // dialogQueue = new Queue();
        // gameManager = FindObjectOfType<GameManager>();
        // sidebarHighScore.text = PlayerStats.HighScore.ToString();
    }

    private void Update() {

        if (gameInProgress) {

            // UpdateSidebar();

        }
        else {
            statsSidebar.gameObject.SetActive(false);
        }

        // if (currentDialogInProgress == null && dialogQueue.Count >= 1) {

        //     // currentDialogInProgress = dialogQueue.Dequeue();

        //     if (currentDialogInProgress is ReadyDialog) {
                // ReadyDialog dialog = currentDialogInProgress as ReadyDialog;
                // dialog.DoDialog();
        //     }
        //     else {
                // GameOverDialog dialog = currentDialogInProgress as GameOverDialog;
                // dialog.DoDialog();
        //     }
        // }
    }

    // private void UpdateSidebar() {

    //     statsSidebar.SetActive(true);

    //     //count upwards a lil per frame when they get points for a cool effect 
    //     int highscore = Int32.Parse(sidebarHighScore.text);
    //     if (highscore < PlayerStats.HighScore - 4) {
    //         sidebarHighScore.text = (highscore + 4).ToString();
    //     }else if (highscore < PlayerStats.HighScore) {
    //         sidebarHighScore.text = PlayerStats.HighScore.ToString();//if we're less than the actual high score but not quite ready for an addition of X, just set it to highscore
    //     }

    //     int scoreP1 = Int32.Parse(sidebarPlayerOneScore.text);
    //     if (scoreP1 < PlayerStats.CurrentScoreP1 - 4) {
    //         sidebarPlayerOneScore.text = (scoreP1 + 4).ToString();
    //     }
    //     else if (scoreP1 < PlayerStats.CurrentScoreP1) {
    //         sidebarPlayerOneScore.text = PlayerStats.CurrentScoreP1.ToString();//if we're less than the actual high score but not quite ready for an addition of X, just set it to highscore
    //     }

    //     int scoreP2 = Int32.Parse(sidebarPlayerTwoScore.text);
    //     if (scoreP2 < PlayerStats.CurrentScoreP2 - 4) {
    //         sidebarPlayerTwoScore.text = (scoreP2 + 4).ToString();
    //     }
    //     else if (scoreP2 < PlayerStats.CurrentScoreP2) {
    //         sidebarPlayerTwoScore.text = PlayerStats.CurrentScoreP2.ToString();//if we're less than the actual high score but not quite ready for an addition of X, just set it to highscore
    //     }

    //     sidebarLevelNumber.text = levelCount.ToString();

    //     if (PlayerOneTurn) {

    //         sidebarPlayerTwoLivesParent.SetActive(false);
    //         sidebarPlayerOneLivesParent.SetActive(true);

    //         switch (playerOneLives) {

    //             case 0:
    //                 sidebarPlayerOneLives[0].SetActive(false);
    //                 sidebarPlayerOneLives[1].SetActive(false);
    //                 sidebarPlayerOneLives[2].SetActive(false);
    //                 break;
    //             case 1:
    //                 sidebarPlayerOneLives[0].SetActive(true);
    //                 sidebarPlayerOneLives[1].SetActive(false);
    //                 sidebarPlayerOneLives[2].SetActive(false);
    //                 break;
    //             case 2:
    //                 sidebarPlayerOneLives[0].SetActive(true);
    //                 sidebarPlayerOneLives[1].SetActive(true);
    //                 sidebarPlayerOneLives[2].SetActive(false);
    //                 break;
    //                     case 3:
    //                 sidebarPlayerOneLives[0].SetActive(true);
    //                 sidebarPlayerOneLives[1].SetActive(true);
    //                 sidebarPlayerOneLives[2].SetActive(true);
    //                 break;
    //         }
    //     }
    //     else {
    //         sidebarPlayerOneLivesParent.SetActive(false);
    //         sidebarPlayerTwoLivesParent.SetActive(true);

    //         switch (playerTwoLives) {

    //             case 0:
    //                 sidebarPlayerTwoLives[0].SetActive(false);
    //                 sidebarPlayerTwoLives[1].SetActive(false);
    //                 sidebarPlayerTwoLives[2].SetActive(false);
    //                 break;
    //             case 1:
    //                 sidebarPlayerTwoLives[0].SetActive(true);
    //                 sidebarPlayerTwoLives[1].SetActive(false);
    //                 sidebarPlayerTwoLives[2].SetActive(false);
    //                 break;
    //             case 2:
    //                 sidebarPlayerTwoLives[0].SetActive(true);
    //                 sidebarPlayerTwoLives[1].SetActive(true);
    //                 sidebarPlayerTwoLives[2].SetActive(false);
    //                 break;
    //             case 3:
    //                 sidebarPlayerTwoLives[0].SetActive(true);
    //                 sidebarPlayerTwoLives[1].SetActive(true);
    //                 sidebarPlayerTwoLives[2].SetActive(true);
    //                 break;
    //         }

    //     }

    // }

    public void OnDeath() {
        GameManager.LoseALife();

        if (GameManager.lives <= 0) {
            SceneManager.LoadScene("GameOver");
        } else {
            foreach (EnemyBehaviour enemy in FindObjectsOfType<EnemyBehaviour>()) {
                enemy.ResetBehaviour();
            }
            PlayerController player = FindObjectOfType<PlayerController>();
            player.transform.position = player.StartPosition;
        }
    }

    private void NextTurn() {

        foreach (EnemyBehaviour enemy in FindObjectsOfType<EnemyBehaviour>()) {
            enemy.ResetBehaviour();
        }
        PlayerController player = FindObjectOfType<PlayerController>();
        player.transform.position = player.StartPosition;

        if (GameManager.lives <= 0) {
            EndGame();
        }

        // if (currentGameMode == GameMode.OnePlayer) {
        //     PlayerOneTurn = true;
        // }
        // else {

        //     if (PlayerOneTurn) {
        //         if (playerTwoLives >= 1) {
        //             PlayerOneTurn = false;
        //         }
        //     }
        //     else {
        //         if (GameManager.lives >= 1) {
        //             PlayerOneTurn = true;
        //         }
        //     }
        // }

        // if (!HasGameEnded) {
        //     dialogQueue.Enqueue(new ReadyDialog(3f, this));
        // }

    }

    internal void SetPaused(bool paused) {
        // foreach (EnemyBehaviour enemy in FindObjectsOfType<EnemyBehaviour>()) {
        //     enemy.Paused = paused;
        // }
        // FindObjectOfType<PlayerController>().Paused = paused;
    }

    internal void NewGame(GameMode gameMode) {

        // PlayerStats.ClearRecentStats();

        // if (!gameInProgress) {

        //     currentGameMode = gameMode;
        //     gameInProgress = true;
        //     NextLevel();

        // }

    }

    private void NextLevel() {

        string nextScene = GameManager.RandomizeScene();
        if(nextScene == "") {
            SceneManager.LoadScene("Win");
        } else {

            SceneManager.LoadScene(nextScene);
        }

        // if (!HasGameEnded) {
        //     dialogQueue.Enqueue(new ReadyDialog(3f, this));
        // }

    }

    private void ReverseGameOverDialog() {
        // SetGameOverDialog(false);
        // SetPaused(false);
        // currentDialogInProgress = null;

        // if (HasGameEnded) {
        //     EndGame();
        // }
    }

    private void ReverseReadyDialog() {
        // SetReadyDialog(false);
        // SetPaused(false);
        // currentDialogInProgress = null;
    }

    private void ReverseWinDialog() {
        // SetPaused(false);
        // currentDialogInProgress = null;
        // NextLevel();
    }

    private void GameOver() {
        // dialogQueue.Enqueue(new GameOverDialog(3f, this));
    }

    private void EndGame() {
        SceneManager.LoadScene("GameOver");
    }

    private void SetReadyDialog(bool enabled) {

        // int currentPlayerNumber;

        // if (PlayerOneTurn)
        //     currentPlayerNumber = 1;
        // else
        //     currentPlayerNumber = 2;

        // playerDialogNumber.text = currentPlayerNumber.ToString();
        // playerDialogHeader.SetActive(enabled);
        // readyDialog.SetActive(enabled);
    }

    private void SetGameOverDialog(bool enabled) {

        int currentPlayerNumber;

        if (PlayerOneTurn)
            currentPlayerNumber = 1;
        else
            currentPlayerNumber = 2;

        playerDialogNumber.text = currentPlayerNumber.ToString();
        // playerDialogHeader.SetActive(enabled);
        // gameOverDialog.SetActive(enabled);

    }

    private int EnemiesStillAlive() {
        int enemies = 0;
        foreach (EnemyBehaviour enemy in FindObjectsOfType<EnemyBehaviour>()) {
            if (!enemy.isDying) {
                enemies++;
            }
        }
        return enemies;
    }

    internal void OnEnemyDeath() {
        if (EnemiesStillAlive() <= 0) {
            NextLevel();
        }
    }

    private class ReadyDialog {

        // private float displayDuration;
        // private DigdugManager gameManager;

        // public ReadyDialog(float displayDuration, DigdugManager gameManager) {
        //     this.displayDuration = displayDuration;
        //     this.gameManager = gameManager;
        // }

        // internal void DoDialog() {
        //     gameManager.SetPaused(true);
        //     gameManager.SetReadyDialog(true);
        //     gameManager.Invoke(nameof(ReverseReadyDialog), displayDuration);
        // }

    }

    private class WinDialog {

        // private float displayDuration;
        // private DigdugManager gameManager;

        // public WinDialog(float displayDuration, DigdugManager gameManager) {
        //     this.displayDuration = displayDuration;
        //     this.gameManager = gameManager;
        // }

        // internal void DoDialog() {
        //     gameManager.SetPaused(true);
        //     gameManager.Invoke(nameof(ReverseWinDialog), displayDuration);
        // }

    }

    private class GameOverDialog {

        // private float displayDuration;
        // private DigdugManager gameManager;

        // public GameOverDialog(float displayDuration, DigdugManager gameManager) {
        //     this.displayDuration = displayDuration;
        //     this.gameManager = gameManager;
        // }

        // internal void DoDialog() {
        //     gameManager.SetPaused(true);
        //     gameManager.SetGameOverDialog(true);
        //     gameManager.Invoke(nameof(ReverseGameOverDialog), displayDuration);
        // }
    }
}
