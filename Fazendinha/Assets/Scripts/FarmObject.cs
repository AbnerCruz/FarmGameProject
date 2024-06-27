using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName="NewFarm", menuName="Farm")]
public class FarmObject : Item  
{
    public FarmReward farm_reward;
    public enum FarmReward{
        Rocks,
        Energy
    }
    public int farm_level;
    public float farm_mature_timer;
    public int farm_drop_chance;
    public int farm_stock_limit;
}
