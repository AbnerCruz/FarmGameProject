using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;


public class FarmClass : BuildBaseClass
{
    GameManager manager; //Game manager
    public FarmObject farm_object; //Scriptable object
    
    //Mature Timer
    public float farm_initial_mature_timer; //Initial mature timer
    public float farm_current_mature_timer; //Current mature timer

    //Drops
    int farm_drop_quantity;
    int farm_drop_chance;
    public FarmObject.FarmRewardType farm_reward;

    //Accumulated points
    public int farm_current_storage;
    int farm_storage_limit;

    public FarmClass(FarmObject farm_object, GameManager manager) : base((int)farm_object.build_type, farm_object){
        this.manager = manager;
        this.farm_initial_mature_timer = farm_object.farm_base_mature_timer;
        this.farm_current_mature_timer = farm_object.farm_base_mature_timer;
        this.farm_storage_limit = farm_object.farm_level * farm_object.farm_base_storage;
        this.farm_drop_quantity = farm_object.farm_level;
        this.farm_drop_chance = farm_object.farm_drop_chance;
        this.farm_reward = farm_object.farm_reward;
    }

    public void  Timer(){
        farm_current_mature_timer -= Time.deltaTime;
        if(farm_current_mature_timer < 0f){
            RewardFarm();
            farm_current_mature_timer = farm_initial_mature_timer;
        }
    }
    public void RewardFarm(){
        if(Random.Range(0,100) >= (100-farm_drop_chance) &&  (farm_current_storage + farm_drop_quantity) <= farm_storage_limit){
            farm_current_storage += farm_drop_quantity;
            manager.UpdateTextAccumulatedPoints();
        }
    }
    public void FarmCollect(FarmClass farm){
        if(farm.farm_current_storage  != 0){
            switch(farm.farm_reward){
                case FarmObject.FarmRewardType.Rocks:
                    manager.player.player_rocks += farm_current_storage;
                break;
                case FarmObject.FarmRewardType.Crystal:
                    manager.player.player_rocks += UnityRandom.Range(1,farm_current_storage/2);
                    manager.player.player_crystals += farm_current_storage;
                break;
            }
            farm_current_storage = 0;
            manager.audioManager.item_drop_audio.Play();
            manager.UpdateTextAccumulatedPoints();
        }
    }
}                                                                                           
