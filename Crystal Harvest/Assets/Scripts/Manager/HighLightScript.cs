using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightScript : MonoBehaviour
{
    void OnMouseEnter(){
        GetComponent<SpriteRenderer>().color = new Color(102,178,255,0.8f);
    }
    void OnMouseExit(){
        GetComponent<SpriteRenderer>().color = new Color(0,0,0,0f);
    }
}
