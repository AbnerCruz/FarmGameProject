using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBarInventoryScript : MonoBehaviour
{
    GameManager manager;
    public Item[] player_inventory_slots;
    public Image[] hud_inventory_slots;
    public Text[] hud_item_amount;
    public GameObject[] hud_hotbar_slots;

    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Start(){
        player_inventory_slots = new Item[manager.player.inventory.item_slots.Length];
    }
    void Update(){
        for(int i = 0; i < manager.player.inventory.item_slots.Length; i++){
            Color current_color = hud_inventory_slots[i].color;
            if(manager.player.inventory.item_slots[i] != null){
                if(player_inventory_slots[i] == null){
                    player_inventory_slots[i] = manager.player.inventory.item_slots[i];
                    hud_inventory_slots[i].sprite = manager.player.inventory.item_slots[i].item_sprite;
                }
            }
            else{
                hud_inventory_slots[i].sprite = null;
                player_inventory_slots[i] = null;
            }
            if(hud_inventory_slots[i].sprite == null){
                hud_inventory_slots[i].color = new Color(hud_inventory_slots[i].color.r, hud_inventory_slots[i].color.g,hud_inventory_slots[i].color.b, 0f);
            }
            else{
                hud_inventory_slots[i].color = new Color(current_color.r,current_color.g,current_color.b,1f);
            }
            hud_item_amount[i].text = manager.player.inventory.item_amount[i].ToString();
        }
    }

    public void HotBarSelection(int hotbar){
        Color current_color = hud_hotbar_slots[hotbar-1].GetComponent<Image>().color;
        for(int i = 0;i < hud_hotbar_slots.Length; i++){
            hud_hotbar_slots[i].GetComponent<Image>().color = new Color(current_color.r,255,current_color.b,0f);
        }
        hud_hotbar_slots[hotbar-1].GetComponent<Image>().color = new Color(current_color.r,255,current_color.b,0.15f);
    }
}
