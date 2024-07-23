using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class SaveAndLoadScript
{
    public void Save(GameManager manager, GridClass target_grid, string filePath){
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        //World Data Saves
        GridClass grid = target_grid;
        BinaryFormatter world_format = new BinaryFormatter();
        FileStream world_file = File.Create(filePath+"_world.data");

        PlayerData world_data = new PlayerData();
        world_data.world_grid = grid;
        world_format.Serialize(world_file,world_data);
        world_file.Close();

        //Player Data Saves
        Player player = manager.player;
        BinaryFormatter player_format = new BinaryFormatter();
        FileStream player_file = File.Create(filePath+"_player.data");

        PlayerData player_data = new PlayerData();
        player_data.player_nickname = player.nickname;
        player_data.player_money = player.player_money;
        player_data.player_energy = player.player_energy;
        player_data.player_rocks = player.player_rocks;
        player_data.player_crystals = player.player_crystals;
        player_data.player_tiles = player.player_tiles;
        player_data.player_played_time = player.played_time;
        player_data.player_inventory = player.inventory;
        player_format.Serialize(player_file,player_data);
        player_file.Close();
    }
    public void Load(GameManager manager, GridClass target_grid, string filePath){
        manager.principal_grid = GridLoad(manager, target_grid, filePath);
        Player player = manager.player;
        if(File.Exists(filePath+"_player.data")){
            BinaryFormatter player_format = new BinaryFormatter();
            FileStream player_file = File.Open(filePath+"_player.data",FileMode.Open);

            PlayerData player_data = (PlayerData)player_format.Deserialize(player_file);
            player_file.Close();

            player.nickname = player_data.player_nickname;
            player.player_money = player_data.player_money;
            player.player_energy = player_data.player_energy;
            player.player_rocks = player_data.player_rocks;
            player.player_crystals = player_data.player_crystals;
            player.player_tiles = player_data.player_tiles;
            player.played_time = player_data.player_played_time;
            player.inventory = player_data.player_inventory;
            player.UpdateHotBar();
        }


        
    }
    GridClass GridLoad(GameManager manager, GridClass target_grid, string filePath){
        GridClass grid = target_grid;
        if(File.Exists(filePath+"_world.data")){
            BinaryFormatter world_format = new BinaryFormatter();
            FileStream world_file = File.Open(filePath+"_world.data", FileMode.Open);

            PlayerData world_data = (PlayerData)world_format.Deserialize(world_file);
            world_file.Close();

            //World Loading
            foreach(Transform child in GameObject.Find("TileGrid").transform){
                manager.ManagerDestroy(child.gameObject);
            }
            if(manager.show_debugs){
                foreach(Transform child in GameObject.Find("TextGrid").transform){
                manager.ManagerDestroy(child.gameObject);
                }
            }
            foreach(Transform child in GameObject.Find("BuildGrid").transform){
                manager.ManagerDestroy(child.gameObject);
            }
            foreach(Transform child in GameObject.Find("AccumulatedFarmsGrid").transform){
                manager.ManagerDestroy(child.gameObject);
            }

            grid = world_data.world_grid;
            grid.utils = new MyUtils();
            grid.tile_grid = new GameObject[manager.grid_width, manager.grid_height];
            grid.build_tile_grid = new GameObject[manager.grid_width, manager.grid_height];
            grid.text_base_grid_value = new TextMesh[manager.grid_width, manager.grid_height];
            grid.text_farm_accumulated_points = new TextMesh[manager.grid_width, manager.grid_height];
            grid.farm_grid = new FarmClass[manager.grid_width, manager.grid_height];
            grid.collector_grid = new CollectorClass[manager.grid_width, manager.grid_height];
            if(manager.show_debugs){
                grid.GridDebug();
            }
            grid.AccumulatedPointsDebug();
            grid.GridTiles();
            for(int x = 0; x < grid.ore_life_grid.GetLength(0); x++){
                for(int y = 0; y < grid.ore_life_grid.GetLength(1); y++){
                    if(grid.ore_life_grid[x,y] <= 0){
                        if(grid.player_tiles[x,y] == 0){
                            grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = GameObject.Find("TileGrid").GetComponent<TileGridScript>().breaked_tile_sprite;
                        }
                        else{
                            grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = GameObject.Find("TileGrid").GetComponent<TileGridScript>().flat_rock_tile;
                        }
                    }
                }
            }
            manager.BuildGridUpdater(grid);
            for(int x = 0; x < grid.ore_life_grid.GetLength(0); x++){
                for(int y = 0; y < grid.ore_life_grid.GetLength(1); y++){
                    if(grid.farm_grid[x,y] != null){
                        grid.farm_grid[x,y].farm_stock = grid.farm_stock_grid[x,y];
                    }
                }
            }
        }
        return grid;
    }
}
