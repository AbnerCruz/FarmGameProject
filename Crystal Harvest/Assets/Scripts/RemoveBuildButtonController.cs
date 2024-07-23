using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveBuildButtonController : MonoBehaviour
{
    GameManager manager;

    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if(manager.remove_build){
            manager.remove_create_build_UI_button.GetComponent<Image>().color = new Color(255,0,0,1f);
        }
        else{
            manager.remove_create_build_UI_button.GetComponent<Image>().color = new Color(255,0,0,0.2f);
        }
    }
}
