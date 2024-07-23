using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInventory
{
    public int[] item_slots;
    public int[] item_amount;
}
public class Player : MonoBehaviour
{
    GameManager manager;
    public string nickname;
    public float played_time;
    public float player_money;
    public float player_energy;
    public float player_rocks;
    public float player_crystals;
    public int player_tiles;
    public float reward_initial_timer;
    public float reward_timer;
    public PlayerInventory inventory = new PlayerInventory();
    public GameObject player_hotbar;



    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        inventory.item_slots = new int[9];
        inventory.item_amount = new int[9];
        manager.player = this;
        player_tiles = 1;
        played_time = 0f;
    }
    public void BuyFunction(Item selected_item, int quantity){
        for(int i = 0; i < inventory.item_slots.Length; i++){
            if(inventory.item_slots[i] == 0 || inventory.item_slots[i] == selected_item.item_id){
                inventory.item_slots[i] = selected_item.item_id;
                inventory.item_amount[i] += quantity;
                break;
            }
        }
        UpdateHotBar();
    }
    public void SellFunction(Item selected_item, int quantity){
        for(int i = 0; i < inventory.item_slots.Length; i++){
            if(inventory.item_slots[i] == selected_item.item_id){
                if(inventory.item_amount[i] >= quantity){
                    inventory.item_amount[i] -= quantity;
                    GameObject.Find("GameManager").GetComponent<GameManager>().player.player_money += selected_item.item_price * quantity;
                }
                else{
                    //Function Does not have quantity??
                }
                break;
            }
        }
        UpdateHotBar();
    }
    public void CreateBuildDecrease(Item selected_build){
        for(int i = 0; i < inventory.item_amount.Length; i++){
            if(inventory.item_slots[i] == selected_build.item_id){
                inventory.item_amount[i]--;
            }
        }
        UpdateHotBar();
    }
    public void RemoveBuildIncrease(Item selected_build){
        BuyFunction(selected_build, 1);
    }

    void Update(){
        for(int i = 0; i < inventory.item_slots.Length; i++){
            if(inventory.item_amount[i] <= 0){
                inventory.item_slots[i] = 0;
                player_hotbar.GetComponent<HotBarInventoryScript>().hotbar_inventory[i] = null;
                UpdateHotBar();
            }
        }
    }
    public void UpdateHotBar(){
        Item[] hotbar = player_hotbar.GetComponent<HotBarInventoryScript>().hotbar_inventory;
        for(int i = 0; i < inventory.item_slots.Length; i++){
            foreach(Item item in manager.item_list.items){
                if(item.item_id == inventory.item_slots[i]){
                    hotbar[i] = item;
                }
                else if(item.item_id == 0){
                    hotbar[i] = null;
                }
            }     
        }
    }
}
