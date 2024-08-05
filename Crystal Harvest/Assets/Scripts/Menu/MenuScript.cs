using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject load_button;
    public GameObject new_game_button;
    public Text game_version_text;
    public string save_name;
    public string game_version;
    public bool tutorial;
    public GameObject tutorial_container;
    void Start(){
        game_version_text.text = "Version "+game_version;
    }
    void Update(){
        if(File.Exists(Application.persistentDataPath + "/"+ save_name +"_player.data") && File.Exists(Application.persistentDataPath + "/"+ save_name +"_world.data")){
            load_button.SetActive(true);
        }
        else{
            load_button.SetActive(false);
        }
    }
      IEnumerator MyRoutine(int indexScene, int load_option){
        yield return new WaitForSeconds(0.6f); //Confirm song play time before load
        PlayerPrefs.SetInt("LoadOption", load_option);
        PlayerPrefs.SetString("SaveName", save_name);
        SceneManager.LoadScene(indexScene);
    }
    //If player have a saved game and select to load it
    public void LoadSave(int indexScene){
        load_button.GetComponent<AudioSource>().Play();
        StartCoroutine(MyRoutine(indexScene,1));
    }

    //If player select to create a new save
    public void NewSave(int indexScene){
        new_game_button.GetComponent<AudioSource>().Play();
        StartCoroutine(MyRoutine(indexScene,0));
    }

    //Quit game
    public void QuitGame(){
        Application.Quit();
    }

    //This is shit, i will just delete it and do another in the future...
    public void TutorialController(){
        tutorial = !tutorial;
        if(tutorial){
            tutorial_container.SetActive(true);
        }
        else{
            tutorial_container.SetActive(false);
        }
    }
}
