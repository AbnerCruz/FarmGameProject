using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopAmountQuantityScript : MonoBehaviour
{
    GameManager manager;
    [SerializeField] int max_quantity;
    int limit_quantity;
    [SerializeField] int current_quantity;
    public Item selected_object;
    public Text current_quantity_text;
    public Text max_quantity_text;
    public float price;
    public Text price_text;
    public Slider slider;
    public int increment_value;
    public int decrement_value;
    public Image price_icon;

    //Buy or Sell var
    public bool buy_or_sell;

    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        limit_quantity = 1000;
    }
    void Update(){
        if(selected_object != null){
            if(buy_or_sell){
                switch(selected_object.item_type){
                    case Item.ItemType.Build:
                    switch(selected_object.price_type){
                        case Item.PriceType.Ore:
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
                    case Item.ItemType.Material:
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
                            switch(selected_object.price_type){
                                case Item.PriceType.Ore:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_rocks);
                                    break;
                                case Item.PriceType.Crystal:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_crystals);
                                    break;
                                case Item.PriceType.Money:
                                    var_quantity = Mathf.FloorToInt(manager.player.player_energy);
                                    break;
                            }
                        break;
                        case Item.ItemType.Build:
                            for(int i = 0; i < manager.player.inventory.item_slots.Length; i++){
                                if(manager.player.inventory.item_slots[i] == selected_object){
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
                manager.Buying(current_quantity);
            }
        }
        else{
            if(current_quantity > 0){
                manager.Selling(current_quantity);
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
