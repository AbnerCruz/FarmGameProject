using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//Player Save Options
[System.Serializable]
class PlayerData{
    public string player_nickname; //Player's nickname
    public float player_money; //Player's money
    public float player_energy; //Player's energy
    public float player_rocks; //Player's rocks
    public float player_crystals; //Player's crystals
    public int player_tiles; //Player's tiles
    public float player_played_time; //Player's played time
    public PlayerInventory player_inventory; //Player's inventory
    public GridClass world_grid; //Save the world grid
}
//Player inventory, storage the player's items ID and quantity
[System.Serializable]
public class PlayerInventory
{
    public int[] item_slots; //Player's items ID
    public int[] item_amount; //Player's items quantity
}
public class Player : MonoBehaviour
{
    public GameManager manager; //Game manager
    public string player_nickname; //Player's nickname
    public float played_time; //Player's played time
    public float player_money; //Player's money
    public float player_energy; //Player's energy
    public float player_rocks; //Player's rocks
    public float player_crystals; //Player's crystals
    public int player_tiles; //Player's tiles
    public float reward_initial_timer; //Player's reward timer fixed
    public float reward_timer; //Player's current reward timer
    public PlayerInventory inventory = new PlayerInventory(); //Player's inventory
    public GameObject player_hotbar; //Player's hotbar reference

//***UNITY METHODS***//
    void Awake(){
        //Inventory initialization 
        inventory.item_slots = new int[9];
        inventory.item_amount = new int[9];

        //Initial player data
        player_tiles = 1;
        played_time = 0f;
    }

//***INVENTORY SYSTEM***//
    //Add item to inventory
    public void NewItemToInventory(Item selected_item, int quantity){
        for(int i = 0; i < inventory.item_slots.Length; i++){
            if(inventory.item_slots[i] == 0 || inventory.item_slots[i] == selected_item.item_id){
                inventory.item_slots[i] = selected_item.item_id;
                inventory.item_amount[i] += quantity;
                UpdateHotBar(); //Update hotbar after selling item
                break;
            }
        }
    }

    //Remove item from inventory
    public void SellFunction(Item selected_item, int quantity){
        for(int i = 0; i < inventory.item_slots.Length; i++){
            if(inventory.item_slots[i] == selected_item.item_id){
                if(inventory.item_amount[i] >= quantity){
                    inventory.item_amount[i] -= quantity;
                    manager.player.player_money += selected_item.item_price * quantity;
                    //No items
                    if(inventory.item_amount[i] <= 0){
                        inventory.item_slots[i] = 0;
                        player_hotbar.GetComponent<HotBarInventoryScript>().hotbar_inventory[i] = null;
                    }
                    UpdateHotBar(); //Update hotbar after selling item
                }
                break;
            }
        }
    }

    //Decrease quantity of item in inventory
    public void InventoryDecreaseItem(Item item){
        for(int i = 0; i < inventory.item_amount.Length; i++){
            if(inventory.item_slots[i] == item.item_id){
                inventory.item_amount[i]--;
                if(inventory.item_amount[i] <= 0){
                    inventory.item_slots[i] = 0;
                    player_hotbar.GetComponent<HotBarInventoryScript>().hotbar_inventory[i] = null;
                }
            }
        }
    }

    //Update hotbar items :)
    public void UpdateHotBar(){
        Item[] hotbar = player_hotbar.GetComponent<HotBarInventoryScript>().hotbar_inventory;
        foreach(Item item in manager.manager_item_list.all_items){
            if(Array.Exists(inventory.item_slots, element => element == item.item_id)){ //If player has this item in inventory
                for(int i = 0; i < inventory.item_slots.Length; i++){
                    if(item.item_id == inventory.item_slots[i]){
                        if(item.item_type == Item.ItemType.Build){
                            hotbar[i] = item;
                        }
                        else if(item.item_id == 0){
                            hotbar[i] = null;
                        }
                    }
                }  
            }
            //else if(){} Possibility to change material storage system
        }
    }
}
