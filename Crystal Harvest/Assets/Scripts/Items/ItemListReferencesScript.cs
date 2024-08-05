using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemListReferencesScript : MonoBehaviour
{
    public List<Item> all_items;
    public List<BuildBaseClass> all_builds;

    void Awake(){
        //Items Configuration
        int id_counter = 0;
        foreach(Item current_item in all_items){
            //ItemsID Configuration
            id_counter++;
            ItemId(current_item,id_counter);

            //Identify all builds in game
            if(current_item.item_type == Item.ItemType.Build){
                all_builds.Add(new BuildBaseClass((int)current_item.build_type,current_item));
            }
        }
    }
    //Configure ItemID for all items in game
    void ItemId(Item item, int id){
        item.item_id = id;
    }
}
