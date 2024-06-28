using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="New Item", menuName="Items/New Item")]
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
        Ore,
        Energy,
    }
    public enum PriceType{
        none,
        Ore,
        Crystal,
        Money,
    }
    public Sprite item_sprite;
    public string item_name;
    public ItemType item_type;
    public BuildType build_type;
    public MaterialType material_type;
    public PriceType price_type;
    public string item_description;
    public int item_price;
    public GameObject item_prefab;

}
