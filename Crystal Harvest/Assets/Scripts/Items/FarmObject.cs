using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName="FarmObject", menuName="Items/New Farm")]
public class FarmObject : Item  
{
    public FarmRewardType farm_reward;
    public enum FarmRewardType{
        Rocks,
        Crystal,
        Energy
    }
    public int farm_level;
    public float farm_base_mature_timer;
    public int farm_drop_chance;
    public int farm_base_storage;
}
