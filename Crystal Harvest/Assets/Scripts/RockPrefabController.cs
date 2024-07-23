using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPrefabController : MonoBehaviour
{
    float life_time;
    void Start(){
        life_time = Random.Range(0.3f,1f);
    }
    void Update(){
        life_time -= Time.deltaTime;
        if(life_time <= 0){
            Destroy(this.gameObject);
        }
    }
}
