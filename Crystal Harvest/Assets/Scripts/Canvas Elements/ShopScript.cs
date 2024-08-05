using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    [Header("References")]
    public GameManager manager; //Game manager object
    public GameObject shop_panel; //Shop panel object
    public GameObject shop_amount_screen; //Activate item quantity to buy panel

//***SHOP SYSTEM***//
    //Select item to shop
    public void SelectingItemToShop(Item item){
        manager.item_to_shop = item;
    }

    //Shop state controller
    public void OpenOrCloseShop(GameManager manager){
        if(manager.pause == false){
            manager.GUI_state =! manager.GUI_state;
            manager.shop_manager.gameObject.SetActive(manager.GUI_state);
            if(!manager.GUI_state){
                manager.shopping = false;
                manager.item_to_shop = null;
            }
            else{
                manager.shopping = true;
            }
        }
    }

    //Shop panel status controller
    public void ShopPanelStatus(){
        if(manager.item_to_shop == null){
            manager.shop_amount_bool = false;
            shop_panel.SetActive(false);
        }
        else{
            shop_panel.SetActive(true);
        }
        
    }

    public void ShopPanelController(Item item){
        if(Input.GetKeyDown(KeyCode.Escape)){
            OpenOrCloseShop(manager);
        }
        if(item != null){
            ShopPanelScript current_panel = shop_panel.GetComponent<ShopPanelScript>();
            current_panel.selected_item_sprite.GetComponent<Image>().sprite = item.item_sprite;
            current_panel.selected_item_name.GetComponent<Text>().text = item.item_name;
            current_panel.selected_item_description.GetComponent<Text>().text = item.item_description;
            current_panel.selected_item_price.GetComponent<Text>().text = ": " + item.item_price.ToString();
            if((item.material_type == Item.MaterialType.none)){
                switch(item.price_type){
                    case Item.PriceType.Rocks:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = manager.rock_item_prefab.GetComponent<SpriteRenderer>().sprite;//Crystal
                    break;
                    case Item.PriceType.Money:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = manager.money_material_prefab.GetComponent<SpriteRenderer>().sprite;//Money
                    break;
                    case Item.PriceType.Crystal:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = manager.crystal_item_prefab.GetComponent<SpriteRenderer>().sprite;
                    break;
                }
            }
            else{
                current_panel.selected_item_price_icon.GetComponent<Image>().sprite = manager.money_material_prefab.GetComponent<SpriteRenderer>().sprite;//Money
            }
        }
    }

    //Buying Function
    public void Buying(Item item, int quantity){
        if(!(item.item_type == Item.ItemType.Material)){
            switch(item.price_type){
                case Item.PriceType.Money:
                    if(manager.player.player_money >= item.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item, quantity);
                        manager.player.player_money -= item.item_price * quantity;
                    }
                    else{
                        //NO MONEY TO BUY
                    }
                break;
                case Item.PriceType.Rocks:
                    if(manager.player.player_rocks >= item.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item, quantity);
                        manager.player.player_rocks -= item.item_price * quantity;
                    }
                    else{
                        //NO ROCKS TO
                    }
                break;
                case Item.PriceType.Crystal:
                    if(manager.player.player_crystals >= item.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item, quantity);
                        manager.player.player_crystals -= item.item_price * quantity;
                    }
                    else{
                        //NO CRYSTALS TO BUY FUNCTIONS
                    }
                break;
            }
        }
        else{
            //Unique buyable material
            if(item.item_name == "Tile"){
                manager.player.player_tiles += quantity;
                manager.player.player_money -= item.item_price * quantity;
            }
            else{
                //Debug.Log("Cannot buy materials");
            }
        }
        
    }

    public void NewSell(Item item, int quantity){
        if(item != null){
            switch(item.item_type){
                case Item.ItemType.Build:
                    manager.player.SellFunction(item, quantity);
                break;
                case Item.ItemType.Material:
                    switch(item.material_type){
                        case Item.MaterialType.Rocks:
                            if(manager.player.player_rocks >= quantity){
                                manager.player.player_rocks -= quantity;
                                manager.player.player_money += item.item_price * quantity;
                            }
                        break;
                        case Item.MaterialType.Crystal:
                            if(manager.player.player_crystals >= quantity){
                                manager.player.player_crystals -= quantity;
                                manager.player.player_money += item.item_price * quantity;
                            }
                        break;
                        case Item.MaterialType.Energy:
                            if(manager.player.player_energy >= quantity){
                                manager.player.player_energy -= quantity;
                                manager.player.player_money += item.item_price * quantity;
                            }
                        break;
                        case Item.MaterialType.Tile:
                            if(manager.player.player_tiles >= quantity){
                                manager.player.player_tiles -= quantity;
                                manager.player.player_money += item.item_price * quantity;
                            }
                        break;
                    }
                break;
            }
        }
    }

    //Add a new item to inventory [Possible security issues]
    public void NewItemToInventory(Item item, int quantity){
        if(item != null){
            manager.player.NewItemToInventory(item, quantity);
        }
    }

    //Button function to open amount selection to buy
    public void SelectAmountBuyingPanelFunction(){
        Item item = manager.item_to_shop;
        if(item.item_type != Item.ItemType.Material){
            if(VerifyItemPriceType(item)){
                manager.shop_amount_bool = true;
                shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
            }
            else{
                //Debug.Log($"No {item.price_type} to buy");
            }
        }
        else{
            if(item.item_name == "Tile"){
                if(VerifyItemPriceType(item)){
                    manager.shop_amount_bool = true;
                    shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
                }
                else{
                    //Debug.Log("No money to buy");
                }
            }
            else{
                //Debug.Log("Cannot buy materials");
            }
        }
    }

    //Verify item price type and return true or false if player can buy
    bool VerifyItemPriceType(Item item){
        switch(item.price_type){
            default:
                return false;
            case Item.PriceType.Money:
                if(manager.player.player_money >= item.item_price){
                    return true;
                }
                else{
                    return false;
                }
            case Item.PriceType.Crystal:
                if(manager.player.player_crystals >= item.item_price){
                    return true;
                }
                else{
                    return false;
                }
            case Item.PriceType.Rocks:
                if(manager.player.player_rocks >= item.item_price){
                    return true;
                }
                else{
                    return false;
                }
        }
    }

    //Open amount selection panel to sell
    public void SelectAmountSellingPanelFunction(){
        Item item = manager.item_to_shop;
        //Verify if player has the item to sell
        switch(item.item_type){
            default:
                //Debug.Log("No item to sell");
            break;
        //***MATERIAL SELL***//
            case Item.ItemType.Material:
                switch(item.material_type){
                    default:
                        //Debug.Log("Material type not recognized");
                    break;
                    case Item.MaterialType.Rocks:
                        if(manager.player.player_rocks > 0){
                            manager.shop_amount_bool = true;
                            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;    
                        }
                        else{
                            //Debug.Log("No rocks to sell");
                        }
                    break;
                
                    case Item.MaterialType.Crystal:
                        if(manager.player.player_crystals > 0){
                            manager.shop_amount_bool = true;
                            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;    
                        }
                        else{
                            //Debug.Log("No crystals to sell");
                        }
                    break;
                    case Item.MaterialType.Energy:
                        if(manager.player.player_energy > 0){
                            manager.shop_amount_bool = true;
                            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;    
                        }
                        else{
                            //Debug.Log("No energy to sell");
                        }
                    break;
                    case Item.MaterialType.Tile:
                        if(manager.player.player_tiles > 0){
                            manager.shop_amount_bool = true;
                            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;    
                        }
                        else{
                            //Debug.Log("No tile to sell");
                        }
                    break;
                }
            break;

        //***BUILD SELL***//
            case Item.ItemType.Build:
                if(manager.player.inventory.item_slots.Contains(item.item_id)){
                    foreach(int i in manager.player.inventory.item_slots){
                        if(i == item.item_id){
                            manager.shop_amount_bool = true;
                            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;
                            break;
                        }
                    }   
                }
                else{
                    //Debug.Log($"{item.item_name} not found in inventory");
                }
            break;
        }
    }
}
