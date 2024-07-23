using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButtonController : MonoBehaviour
{
    GameManager manager;

    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update(){
        if(manager.create_build){
            manager.create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,1f);
        }
        else{
            manager.create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,0.2f);
        }
    }
}
