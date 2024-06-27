using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Item[] item_slots;
    public int[] item_amount;

    void Awake(){
        item_slots = new Item[9];
        item_amount = new int[9];

    }

    //FakeBuyFunction
    public void BuyFunction(Item selected_item, int quantity){
        for(int i = 0; i < item_slots.Length; i++){
            if(item_slots[i] == null || item_slots[i] == selected_item){
                item_slots[i] = selected_item;
                item_amount[i] += quantity;
                break;
            }
        }
    }
    public void SellFunction(Item selected_item, int quantity){
        for(int i = 0; i < item_slots.Length; i++){
            if(item_slots[i] == selected_item){
                if(item_amount[i] >= quantity){
                    item_amount[i] -= quantity;
                    GameObject.Find("GameManager").GetComponent<GameManager>().player.player_money += selected_item.item_price * quantity;
                }
                else{
                    //Function Does not have quantity??
                }
                break;
            }
        }
    }
    public void CreateBuildDecrease(Item selected_build){
        for(int i = 0; i < item_amount.Length; i++){
            if(item_slots[i] == selected_build){
                item_amount[i]--;
            }
        }
    }
    public void RemoveBuildIncrease(Item selected_build){
        BuyFunction(selected_build, 1);
    }

    void Update(){
        for(int i = 0; i < item_slots.Length; i++){
            if(item_amount[i] <= 0){
                item_slots[i] = null;
            }
        }
    }

}
