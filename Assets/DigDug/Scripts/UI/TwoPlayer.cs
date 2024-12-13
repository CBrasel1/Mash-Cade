
using UnityEngine;

public class TwoPlayer : MenuOption {

    internal override void DoOption() {
        Application.Quit();
    }

    internal override void Selected() {
        transform.GetChild(0).gameObject.SetActive(true);//GetChild(0) is the selection icon
    }

    internal override void Deselected() {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}