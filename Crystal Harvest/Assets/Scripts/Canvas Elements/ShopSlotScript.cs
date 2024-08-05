using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotScript : MonoBehaviour
{
    public Item item;
    public Image item_icon;
    public Text item_name;

    void Start(){
        item_icon.sprite = item.item_sprite;
        item_name.text = item.item_name;
    }
}
