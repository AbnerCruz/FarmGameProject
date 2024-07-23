using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBarInventoryScript : MonoBehaviour
{
    GameManager manager;
    public Item[] hotbar_inventory;
    public Image[] hud_inventory_slots;
    public Text[] hud_item_amount;
    public GameObject[] hud_hotbar_slots;

    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Start(){
        hotbar_inventory = new Item[manager.player.inventory.item_slots.Length];
    }
    void Update(){
        for(int i = 0; i < hud_inventory_slots.Length; i++){
            Color current_color = hud_inventory_slots[i].color;
            if(hotbar_inventory[i] != null){
                hud_inventory_slots[i].sprite = hotbar_inventory[i].item_sprite;
            }
            else{
                hud_inventory_slots[i].sprite = null;
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
