using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildBaseClass
{
    public int build_value;
    public Item build_base_item;

    public BuildBaseClass(int build_value, Item build_base_item){
        this.build_value = build_value;
        this.build_base_item = build_base_item;
    }
}
