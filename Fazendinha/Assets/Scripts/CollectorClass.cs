using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorClass : BuildBaseClass
{
    GameManager manager;
    public CollectorObject collector_object;
    float collector_timer;
    float collector_initial_timer;


    int collector_range;
    public CollectorClass(GameManager manager, CollectorObject collector_object) : base((int)collector_object.build_type, collector_object){
        this.manager = manager;
        this.collector_object = collector_object;
        this.collector_range = collector_object.collector_level;
        this.collector_timer = collector_object.collector_initial_timer;
        this.collector_initial_timer = collector_object.collector_initial_timer;
    }

    public void CollectorTimer(int x, int y, GridClass principal_grid){
        collector_timer -= Time.deltaTime;
        if(collector_timer < 0f){
            CollectorFunction(x,y,principal_grid);
            collector_timer = collector_initial_timer;
        }
    }
    void CollectorFunction(int x, int y, GridClass principal_grid){
        for(int a = -collector_range; a <= collector_range; a++){
            for(int b = -collector_range; b <= collector_range; b++){
                int coordsX = (x+a);
                int coordsY = (y+b);
                if(coordsX >= 0 && coordsX < principal_grid.base_grid.GetLength(0) && coordsY >= 0 && coordsY < principal_grid.base_grid.GetLength(1)){
                    int current_cell = principal_grid.base_grid[coordsX,coordsY];
                    switch(current_cell){
                    case 0: //Null cell
                        break;
                    case 1: //Crystal Farm Cell
                        manager.player.player_energy += principal_grid.farm_grid[coordsX,coordsY].farm_stock;
                        principal_grid.farm_grid[coordsX,coordsY].farm_stock = 0;
                        break;
                    case 2: //Extractor Cell
                        manager.player.player_rocks += Random.Range(1,10);
                        manager.player.player_crystals += principal_grid.farm_grid[coordsX,coordsY].farm_stock;
                        principal_grid.farm_grid[coordsX,coordsY].farm_stock = 0;
                        break;
                    }
                    manager.UpdateTextAccumulatedPoints();
                    //Debug.Log($"[{a},{b}]" + current_cell);
                }
            }
        }
        
    }
}
