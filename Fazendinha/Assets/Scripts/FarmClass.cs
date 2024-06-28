using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FarmClass : BuildBaseClass
{
    //Management
    GameManager manager;
    public FarmObject farm_object;
    
    //Mature Timer
    public float farm_initial_mature_timer;
    public float farm_mature_timer;

    //Drops
    int farm_drop_quantity;
    int farm_drop_chance;
    public FarmObject.FarmReward farm_reward;

    //Accumulated points
    public int farm_stock;
    int farm_stock_limit;

    public FarmClass(FarmObject farm_object, GameManager manager) : base((int)farm_object.build_type, farm_object){
        this.manager = manager;
        this.farm_initial_mature_timer = farm_object.farm_mature_timer;
        this.farm_mature_timer = farm_object.farm_mature_timer;
        this.farm_stock_limit = farm_object.farm_stock_limit;
        this.farm_drop_quantity = farm_object.farm_level;
        this.farm_drop_chance = farm_object.farm_drop_chance;
        this.farm_reward = farm_object.farm_reward;
    }

    public void  Timer(){
        farm_mature_timer -= Time.deltaTime;
        if(farm_mature_timer < 0f){
            RewardFarm(); // FUNCTION HERE
            farm_mature_timer = farm_initial_mature_timer;
        }
    }
    public void RewardFarm(){
        if(Random.Range(0,100) >= 100-farm_drop_chance &&  farm_stock < farm_stock_limit){
            farm_stock += farm_drop_quantity;
            manager.UpdateTextAccumulatedPoints();
        }
    }
}                                                                                           
