
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnePlayer : MenuOption {

    internal override void DoOption() {
        SceneManager.LoadScene(GameManager.RandomizeScene());
    }

    internal override void Selected() {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    internal override void Deselected() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

}
