using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField, Tooltip("A list of MenuOption components from UI GameObjects")]
    private List<MenuOption> menuOptions;

    [SerializeField]
    private Text highLevel;

    [SerializeField]
    private Text highScore;

    [SerializeField]
    private Text lastScoreP1;

    [SerializeField]
    private Text lastScoreP2;

    private MenuOption currentlySelected;//The currently selected MonoBehaviour that extends the abstract MenuOption class

    [SerializeField]
    private bool transitionDone;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool hasPlayedMenuTransition;
    private bool skipped;

    private void Start() {

        highLevel.text = PlayerStats.HighLevel.ToString();
        highScore.text = PlayerStats.HighScore.ToString();

        lastScoreP1.text = PlayerStats.CurrentScoreP1.ToString();//CurrentScore is last score since it doesn't get cleared until calling GameManager.NewGame();
        lastScoreP2.text = PlayerStats.CurrentScoreP2.ToString();

        currentlySelected = menuOptions[0];
    }

    private void Update () {

        currentlySelected.Deselected();

        if (Input.GetKeyDown(KeyCode.UpArrow)) {

            int currentlySelectedIndex = menuOptions.IndexOf(currentlySelected);

            if (currentlySelectedIndex <= 0) {
                //currentlySelected = menuOptions[menuOptions.Count - 1];//Loop back around to the bottom of the options
            }
            else {
                currentlySelected = menuOptions[currentlySelectedIndex - 1];//Move selection upwards
            }

        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {

            int currentlySelectedIndex = menuOptions.IndexOf(currentlySelected);

            if (currentlySelectedIndex >= menuOptions.Count - 1) {
                //currentlySelected = menuOptions[0];//Loop back around to the top of the options
            }
            else {
                currentlySelected = menuOptions[currentlySelectedIndex + 1];//Move selection downwards
            }

        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
            if (hasPlayedMenuTransition || skipped) {
                currentlySelected.DoOption();
            }
            else {
                skipped = true;
                animator.SetTrigger("SkipTransition");
            }
        }

        currentlySelected.Selected();//tell (new?) option that it's selected

    }
}
