using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MenuOption
{
    internal override void Deselected()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    internal override void DoOption()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene("Instructions");
        }
    }

    internal override void Selected()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
