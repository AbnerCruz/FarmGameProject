using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBarInventoryScript : MonoBehaviour
{
    //References
    public GameManager manager; //Manager

    //Arrays
    public Item[] hotbar_inventory; //Player inventory in Items conversion
    public Image[] hud_inventory_slots; //Visual items slots in hotbar
    public Text[] hud_item_amount; //Item quantity
    public GameObject[] hud_hotbar_slots; //Visual slots in HUD

    void Start(){
        hotbar_inventory = new Item[manager.player.inventory.item_slots.Length];
    }
    
    void Update(){
        HudContoller();
    }

    //Control HUD informations
    void HudContoller(){
        for(int i = 0; i < hud_inventory_slots.Length; i++){
            Color current_color = hud_inventory_slots[i].color;
            //Player have items in this slot
            if(hotbar_inventory[i] != null){
                //Update item sprite
                hud_inventory_slots[i].sprite = hotbar_inventory[i].item_sprite;
            }
            else{
                //No item in this slot
                hud_inventory_slots[i].sprite = null;
            }

            //Hide slot if no item in it
            if(hud_inventory_slots[i].sprite == null){
                hud_inventory_slots[i].color = new Color(hud_inventory_slots[i].color.r, hud_inventory_slots[i].color.g,hud_inventory_slots[i].color.b, 0f);
            }
            //Show slot if item in it
            else{
                hud_inventory_slots[i].color = new Color(current_color.r,current_color.g,current_color.b,1f);
            }
            //Update item quantity
            hud_item_amount[i].text = manager.player.inventory.item_amount[i].ToString();
        }
    }

    //I don't like this code. This just control the selected hotbar opacity. I want a sprite instead of opacity...
    public void HotBarSelection(int hotbar){
        Color current_color = hud_hotbar_slots[hotbar-1].GetComponent<Image>().color;
        for(int i = 0;i < hud_hotbar_slots.Length; i++){
            hud_hotbar_slots[i].GetComponent<Image>().color = new Color(current_color.r,255,current_color.b,0f);
        }
        hud_hotbar_slots[hotbar-1].GetComponent<Image>().color = new Color(current_color.r,255,current_color.b,0.15f);
    }
}
