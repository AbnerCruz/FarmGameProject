using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildBaseClass
{
    MyUtils utils = new MyUtils();
    public int build_value; //Value storing in grid_base
    public Item build_base_item; //Item reference

    //Constructor
    public BuildBaseClass(int build_value, Item build_base_item){
        this.build_value = build_value;
        this.build_base_item = build_base_item;
    }

//***BUILD SYSTEM***//
    //Build system
    public void BuildSystem(GameManager manager, int x, int y, Item build){
        if(manager.create_build_tool){
            if(manager.principal_grid.VerifyGridLimits(utils.GetMousePos())){
                if(build != null && build.item_type == Item.ItemType.Build && manager.principal_grid.ReturnCellValue(utils.GetMousePos()) == null){
                    if(manager.principal_grid.ore_life_grid[x,y] <= 0){
                        if(manager.principal_grid.player_buyed_tiles[x,y] == 1){
                            manager.principal_grid.EditBaseGrid(x,y,(int)build.build_type);
                            if(manager.show_debugs){
                                manager.principal_grid.EditBaseTextGrid(x,y,(int)build.build_type);
                            }
                            manager.GridBuildUpdater(manager.principal_grid);
                            manager.player.InventoryDecreaseItem(manager.principal_grid.ReturnCellValue(utils.GetMousePos()).build_base_item);
                        }
                    }
                    else{
                        //can't build
                    }
                }
            }
        }
        else if(manager.remove_build_tool){
            if(manager.principal_grid.VerifyGridLimits(utils.GetMousePos())){
                if(manager.principal_grid.ReturnCellValue(utils.GetMousePos()) != null){
                    if(manager.principal_grid.ReturnCellValue(utils.GetMousePos()).build_value == 1 || manager.principal_grid.ReturnCellValue(utils.GetMousePos()).build_value == 2){
                        manager.principal_grid.farm_grid[x,y].FarmCollect(manager.principal_grid.farm_grid[x,y]);
                    }
                    manager.player.NewItemToInventory(manager.principal_grid.ReturnCellValue(utils.GetMousePos()).build_base_item, 1);
                    manager.principal_grid.EditBaseGrid(x,y,0);
                    if(manager.show_debugs){
                        manager.principal_grid.EditBaseTextGrid(x,y,0);
                    }
                    manager.GridBuildUpdater(manager.principal_grid);
                    if(manager.principal_grid.player_buyed_tiles[x,y] == 1){
                        manager.principal_grid.BuildTileGridUpdate(x,y,null);
                    }
                }
            }
        }
    }
}
