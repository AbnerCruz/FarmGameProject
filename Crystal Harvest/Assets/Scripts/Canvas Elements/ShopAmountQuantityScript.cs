using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopAmountQuantityScript : MonoBehaviour
{
    //References
    public GameManager manager; //Game manager
    public Slider slider;
    public Image price_icon; //Maybe change this with a better UI with a new craft system?
    public Item selected_object;
    public Text max_quantity_text;
    public Text current_quantity_text;
    public Text price_text;

    //Quantity manager 
    int max_quantity; //Max quantity player can buy or sell
    int current_quantity; //Current quantity player has selected
    int limit_quantity; //Limit quantity player can buy or sell in one pack
    public float price;

    //Support variables for quantity management
    public int increment_value; //Click option to increase quantity
    public int decrement_value; //Click option to decrease quantity

    //Buy or Sell var
    public bool buy_or_sell; //Alternate buy or sell mode

    void Awake(){
        limit_quantity = 1000;
    }
    void Update(){
        if(selected_object != null){
            if(buy_or_sell){
                switch(selected_object.item_type){
                    case Item.ItemType.Build:
                        switch(selected_object.price_type){
                            case Item.PriceType.Rocks:
                                max_quantity = Mathf.FloorToInt(manager.player.player_rocks/selected_object.item_price);
                            break;
                            case Item.PriceType.Crystal:
                                max_quantity = Mathf.FloorToInt(manager.player.player_crystals/selected_object.item_price);
                            break;
                            default:
                                max_quantity = Mathf.FloorToInt(manager.player.player_money/selected_object.item_price);
                            break;
                        }
                    break;
                    case Item.ItemType.Material: //Unique buyable material is tile
                        switch(selected_object.price_type){
                            default:
                                max_quantity = Mathf.FloorToInt(manager.player.player_money/selected_object.item_price);
                            break;
                        }
                    break;
                }
                if(max_quantity > limit_quantity){
                    max_quantity = limit_quantity;
                }
                price = selected_object.item_price * current_quantity;
                current_quantity = Mathf.FloorToInt(max_quantity * slider.value) + increment_value + decrement_value;
                if(current_quantity > max_quantity){
                    current_quantity = max_quantity;
                }
            }
            else{
                int var_quantity = 0;
                if(!(selected_object.item_name == "Tile")){
                    switch(selected_object.item_type){
                        case Item.ItemType.Material:
                            switch(selected_object.material_type){
                                case Item.MaterialType.Rocks:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_rocks);
                                break;
                                case Item.MaterialType.Crystal:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_crystals);
                                break;
                                case Item.MaterialType.Energy:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_energy);
                                break;
                            }
                        break;
                        case Item.ItemType.Build:
                            for(int i = 0; i < manager.player.inventory.item_slots.Length; i++){
                                if(manager.player.inventory.item_slots[i] == selected_object.item_id){
                                    var_quantity = manager.player.inventory.item_amount[i];
                                    break;
                                }
                                else{
                                    var_quantity = 0;
                                }
                            }
                        break;
                    }
                }
                else{
                    var_quantity = Mathf.FloorToInt(manager.player.player_tiles);
                }
                max_quantity = var_quantity;
                price = selected_object.item_price * current_quantity;
                current_quantity = Mathf.FloorToInt(max_quantity * slider.value) + increment_value + decrement_value;
                if(current_quantity > max_quantity){
                    current_quantity = max_quantity;
                }
            }
        }

        current_quantity_text.text = current_quantity.ToString();
        max_quantity_text.text = max_quantity.ToString();
        price_text.text = price.ToString();
    }
    public void IncrementFunction(){
        if(current_quantity < max_quantity){
            increment_value++;
        }
    }
    public void DecrementFunction(){
        if(current_quantity > 0){
            decrement_value--;
        }
    }
    public void ClearCrements(){
        increment_value = 0;
        decrement_value = 0;
    }
    public void ConfirmedButtonFunction(){
        if(buy_or_sell){
            if(current_quantity > 0){
                manager.shop_manager.Buying(selected_object, current_quantity);
            }
        }
        else{
            if(current_quantity > 0){
                manager.shop_manager.NewSell(selected_object, current_quantity);
            }
        }
        slider.value = 0f;
        ClearCrements();
        manager.shop_amount_bool = false;
        buy_or_sell = false;
    }
    public void CancellingBuy(){
        slider.value = 0f;
        ClearCrements();
        manager.item_to_shop = null;
        manager.shop_amount_bool = false;
    }
}