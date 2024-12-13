using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static List<string> sceneNameList = new List<string>{ "DigDug", "Pac-Man", "DonkeyKong" };
    public static List<string> characterList = new List<string>{ "DigDug", "Pac-Man", "Mario"};
    public static int lives = 3;

    public static string RandomizeScene() {
        if(sceneNameList.Count == 0) {
            return "Win";
        } else {
            int index = Random.Range(0, sceneNameList.Count);
            string scene = sceneNameList[index].ToString();
            sceneNameList.RemoveAt(index);
            return scene;
        }
    }

    public static int RandomizeCharacter() {
        int index = Random.Range(0, characterList.Count);
        characterList.Remove(characterList[index]);
        return index;
    }

    public static void LoseALife() {
        lives--;
    }

    public static void Reset() {
        sceneNameList = new List<string>{ "DigDug", "Pac-Man", "DonkeyKong" };
        characterList = new List<string>{ "DigDug", "Pac-Man", "Mario"};
        lives = 3;
    }
}
