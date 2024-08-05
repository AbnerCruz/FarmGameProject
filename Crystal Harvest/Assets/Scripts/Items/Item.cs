using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Item", menuName="Items/New Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public enum ItemType{
        none,
        Build,
        Material,
    }
    public enum BuildType{
        none,
        FarmObject = 1,
        ExtractingObject = 2,
        CollectorObject = 3,
    }
    public enum MaterialType{
        none,
        Rocks,
        Crystal,
        Energy,
        Tile
    }
    public enum PriceType{
        none,
        Rocks,
        Crystal,
        Money,
    }
    [Tooltip("Item sprite")]public Sprite item_sprite;
    [Tooltip("Item name")]public string item_name;
    [Tooltip("Item ID don't edit it")]public int item_id;
    [Tooltip("Defines what kind of material is")]public ItemType item_type;
    [Tooltip("Edit only if item is a build, default: none")]public BuildType build_type;
    [Tooltip("Edit only if item is a material, default: none")]public MaterialType material_type;
    [Tooltip("Define what PriceType (like materials) you need to buy this item")]public PriceType price_type;
    [Tooltip("Description of the item")]public string item_description;
    [Tooltip("Item price")]public int item_price;

}
