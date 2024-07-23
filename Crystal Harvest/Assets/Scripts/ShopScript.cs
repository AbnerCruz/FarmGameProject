using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    GameManager manager;
    public Text player_points_text;
    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void SelectingItemToShop(Item item){
        manager.item_to_shop = item;
    }
}
