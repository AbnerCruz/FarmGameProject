using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridClass
{
    //Management
    GameManager manager;
    MyUtils utils = new MyUtils();
    //Configuration
    int grid_width;
    int grid_height;
    int grid_cell_size;
    //Grids
    public GameObject[,] tile_grid;
    public GameObject[,] build_tile_grid;
    TextMesh[,] text_base_grid_value;
    public TextMesh[,] text_farm_accumulated_points;
    public int[,] base_grid;
    public int[,] crystal_grid;
    public int[,] ore_amount_grid;
    public int[,] ore_max_life_grid;
    public int[,] ore_life_grid;
    public int[,] player_tiles;
    public FarmClass[,] farm_grid;
    public CollectorClass[,] collector_grid;

    //Constructor
    public GridClass(int grid_width, int grid_height, int grid_cell_size, GameManager manager){
        //configuration
        this.grid_width = grid_width;
        this.grid_height = grid_height;
        this.grid_cell_size = grid_cell_size;
        this.manager = manager;
        //grid init
        text_base_grid_value = new TextMesh[grid_width, grid_height];
        text_farm_accumulated_points = new TextMesh[grid_width, grid_height];
        base_grid = new int[grid_width, grid_height];
        crystal_grid = new int[grid_width, grid_height];
        ore_amount_grid = new int[grid_width, grid_height];
        ore_max_life_grid = new int[grid_width, grid_height];
        ore_life_grid = new int[grid_width, grid_height];
        player_tiles = new int[grid_width, grid_height];
        farm_grid = new FarmClass[grid_width, grid_height];
        collector_grid = new CollectorClass[grid_width, grid_height];
        tile_grid = new GameObject[grid_width, grid_height];
        build_tile_grid = new GameObject[grid_width, grid_height];
        //functions
        GridDebug();
        GridTiles();
    }

    //Visual Debug
    void GridDebug(){
        for(int x = 0; x < base_grid.GetLength(0); x++){
            for(int y = 0; y < base_grid.GetLength(1); y++){
                //GRID LINES
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1,y), Color.white, 100f);
                //TEXT LINES
                text_base_grid_value[x,y] = utils.CreateText($"TextGrid[{x}{y}]", "TextGrid", base_grid[x,y].ToString(), GetWorldPosition(x,y) + new Vector2(grid_cell_size, grid_cell_size) * 0.5f, grid_cell_size*5, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 1);
                text_farm_accumulated_points[x,y] = utils.CreateText($"AccumulatedPoints[{x}{y}]", "AccumulatedFarmsGrid", "0", GetWorldPosition(x,y) + new Vector2(grid_cell_size, grid_cell_size) * 0.85f, grid_cell_size*2, Color.white, TextAnchor.MiddleRight, TextAlignment.Center, 1);
            }
        }
        //GRID LINES
        Debug.DrawLine(GetWorldPosition(0, grid_height), GetWorldPosition(grid_width, grid_height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(grid_width, 0), GetWorldPosition(grid_width, grid_height), Color.white, 100f);

    }

    //Create Grid Tiles
    void GridTiles(){
        for(int x = 0; x < tile_grid.GetLength(0); x++){
            for(int y = 0; y < tile_grid.GetLength(1); y++){
                //Base Tile Grid
                GameObject tile_parent = GameObject.Find("TileGrid");
                tile_grid[x,y] = new GameObject($"Tile[{x},{y}]");
                tile_grid[x,y].transform.position = GetWorldPosition(x,y) + new Vector2(grid_cell_size,grid_cell_size) * 0.5f;
                tile_grid[x,y].transform.SetParent(tile_parent.transform);
                tile_grid[x,y].transform.localScale = new Vector2(grid_cell_size,grid_cell_size); //CellSize
                //Sprite Config
                tile_grid[x,y].AddComponent<SpriteRenderer>();
                GridTilesUpdate(x,y,tile_parent);

                //Build Tile Grid
                GameObject build_parent = GameObject.Find("BuildGrid");
                build_tile_grid[x,y] = new GameObject($"Build[{x},{y}]");
                build_tile_grid[x,y].transform.position = GetWorldPosition(x,y) + new Vector2(grid_cell_size,grid_cell_size) * 0.5f;
                build_tile_grid[x,y].transform.SetParent(build_parent.transform);
                build_tile_grid[x,y].transform.localScale = new Vector2(grid_cell_size,grid_cell_size);
                //Sprite Config
                build_tile_grid[x,y].AddComponent<SpriteRenderer>();
                build_tile_grid[x,y].GetComponent<SpriteRenderer>().sortingOrder = 1;
                BuildTileGridUpdate(x,y,null);
            }
        }
    }
    public void GridTilesUpdate(int x, int y, GameObject parent){
        if(crystal_grid[x,y] == 0){
            tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = parent.GetComponent<TileGridScript>().rock_tile_sprite; //Initial Sprite
        }
        else if(crystal_grid[x,y] == 1){
            tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = parent.GetComponent<TileGridScript>().crystal_tile_sprite; //Initial Sprite
        }
    }
    public void BuildTileGridUpdate(int x, int y,Sprite sprite){
        if(sprite != null){
                build_tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = sprite;

        }
        else{
            build_tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = null;
        }
    }


    //Convert To Grid Position
    Vector2 GetWorldPosition(int x, int y){
        return new Vector2(x,y) * grid_cell_size;
    }
    //Convert Mouse Position To Grid Position
    public void GetXY(Vector2 worldMousePosition, out int x, out int y){
        x = Mathf.FloorToInt(worldMousePosition.x / grid_cell_size);
        y = Mathf.FloorToInt(worldMousePosition.y / grid_cell_size);
    }
    //Verify If Mouse Position Is In Grid
    public bool VerifyGridLimits(Vector2 cell_position){
        int x, y;
        GetXY(cell_position, out x, out y);
        if(x >= 0 && x < grid_width && y >= 0 && y < grid_height){
            return true;
        }
        else{
            return false;
        }
    }
    //Edit Grid Cell Value
    public void EditGridCellValue(Vector2 cell_position, BuildBaseClass build){
        int x,y;
        GetXY(cell_position, out x, out y);
        switch(build){
            case null: //REMOVE BUILDS
                farm_grid[x,y] = null;
                collector_grid[x,y] = null;
                base_grid[x,y] = 0;
                text_base_grid_value[x,y].text = base_grid[x,y].ToString();
                text_farm_accumulated_points[x,y].text = "0";
                break;
            case FarmClass: //BUILD IS A FARM
                if(farm_grid[x,y] == null){
                    farm_grid[x,y] = (FarmClass)build;
                    base_grid[x,y] = build.build_value;
                    text_base_grid_value[x,y].text = base_grid[x,y].ToString();
                    text_farm_accumulated_points[x,y].text = "0";
                }
                break;
            case CollectorClass: //BUILD IS A COLLECTOR
                if(collector_grid[x,y] == null){
                    collector_grid[x,y] = (CollectorClass)build;
                    base_grid[x,y] = build.build_value;
                    text_base_grid_value[x,y].text = base_grid[x,y].ToString();
                    text_farm_accumulated_points[x,y].text = "0";
                }
                break;
        }
    }

    //Verify If Is Null
    public BuildBaseClass ReturnCellValue(Vector2 cell_position){
        BuildBaseClass cell = null;
        int x,y;
        GetXY(cell_position, out x, out y);
        if(base_grid[x,y] == 0){
            cell = null;
        }
        else{
            switch(base_grid[x,y]){
                case 1:
                    cell = farm_grid[x,y];
                    break;
                case 2:
                    cell = farm_grid[x,y];  //Detail to remember Rocks Farms are FarmClass too and this is why farm_grid[x,y] here
                    break;
                case 3:
                    cell = collector_grid[x,y];
                    break;
            }
        }
        return cell;
    }
}
