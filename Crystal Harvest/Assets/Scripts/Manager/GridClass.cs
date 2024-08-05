using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GridClass
{
    //Management
    [NonSerialized] GameManager manager; //Game manager reference
    [NonSerialized] public MyUtils utils = new MyUtils(); //My utility scripts

    //Grid Configuration
    int grid_width; //Grid width
    int grid_height; //Grid height
    int grid_cell_size; //Grid cell size

    //Non Serialized layers
    [NonSerialized] public GameObject[,] tile_grid; //Layer of tiles[x,y] objects used to sprite rendering
    [NonSerialized] public GameObject[,] highlight_grid; //Layer of tiles[x,y] objects used to sprite rendering
    [NonSerialized] public GameObject[,] build_tile_grid; //Layer of tiles[x,y] where builds is created
    [NonSerialized] public TextMesh[,] text_base_grid_value; 
    [NonSerialized] public TextMesh[,] text_farm_accumulated_points;
    [NonSerialized] public FarmClass[,] farm_grid;
    [NonSerialized] public CollectorClass[,] collector_grid;

    //Serialized layers
    public int[,] base_grid; //Layer where the principal information is stored used to read what is positioned
    public int[,] crystal_grid; //Layer where crystal ores is positioned
    public int[,] ore_amount_grid; //Layer where ore amount is stored used to reward drops when player mines ore
    public int[,] ore_max_life_grid; //Layer where ore max life is stored used to define the maximum life of ore
    public int[,] ore_life_grid; //Layer where current life of ore is stored used to verify ore breaking
    public int[,] player_buyed_tiles; // Layer where player has bought tiles
    public int[,] farm_storage_grid; //Layer where farm stock is stored used to define the current stock of a farm drops

    //Constructor
    public GridClass(int grid_width, int grid_height, int grid_cell_size, GameManager manager){
        this.manager = manager;
        //Grid configuration
        this.grid_width = grid_width;
        this.grid_height = grid_height;
        this.grid_cell_size = grid_cell_size;

        //Non Serialized layers
        tile_grid = new GameObject[grid_width, grid_height];
        highlight_grid = new GameObject[grid_width, grid_height];
        build_tile_grid = new GameObject[grid_width, grid_height];
        text_base_grid_value = new TextMesh[grid_width, grid_height];
        text_farm_accumulated_points = new TextMesh[grid_width, grid_height];
        farm_grid = new FarmClass[grid_width, grid_height];
        collector_grid = new CollectorClass[grid_width, grid_height];

        //Serialized layers
        base_grid = new int[grid_width, grid_height];
        crystal_grid = new int[grid_width, grid_height];
        ore_amount_grid = new int[grid_width, grid_height];
        ore_max_life_grid = new int[grid_width, grid_height];
        ore_life_grid = new int[grid_width, grid_height];
        player_buyed_tiles = new int[grid_width, grid_height];
        farm_storage_grid = new int[grid_width, grid_height];

        //functions
        GridDebug();
        AccumulatedDropsTextDebug(manager);
        GridTiles(manager);
    }

//***DEBUG FUNCTIONS***//
    //Visual debug to see grid cells
    public void GridDebug(){
        for(int x = 0; x < base_grid.GetLength(0); x++){
            for(int y = 0; y < base_grid.GetLength(1); y++){
                //GRID LINES
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1,y), Color.white, 100f);
                //GRID TEXTS
                text_base_grid_value[x,y] = utils.CreateText($"TextGrid[{x}{y}]", manager.manager_text_grid, base_grid[x,y].ToString(), GetWorldPosition(x,y) + new Vector2(grid_cell_size, grid_cell_size) * 0.5f, grid_cell_size*5, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 2);
            }
        }
        //GRID LINES
        Debug.DrawLine(GetWorldPosition(0, grid_height), GetWorldPosition(grid_width, grid_height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(grid_width, 0), GetWorldPosition(grid_width, grid_height), Color.white, 100f);
    }

    //Accumulated drops text debug
    public void AccumulatedDropsTextDebug(GameManager manager){
        for(int x = 0; x < base_grid.GetLength(0); x++){
            for(int y = 0; y < base_grid.GetLength(1); y++){
                text_farm_accumulated_points[x,y] = utils.CreateText($"AccumulatedPoints[{x}{y}]", manager.manager_accumulated_farms_grid, "0", GetWorldPosition(x,y) + new Vector2(grid_cell_size, grid_cell_size) * 0.85f, grid_cell_size*2, Color.white, TextAnchor.MiddleRight, TextAlignment.Center, 3);
            }
        }
    }

//***TILES CONFIGURATIONS***//
    //Create Grid Tiles
    public void GridTiles(GameManager manager){
        for(int x = 0; x < tile_grid.GetLength(0); x++){
            for(int y = 0; y < tile_grid.GetLength(1); y++){
                GameObject tile_parent = manager.manager_tile_grid;
                tile_grid[x,y] = new GameObject($"Tile[{x},{y}]");
                tile_grid[x,y].transform.position = GetWorldPosition(x,y) + new Vector2(grid_cell_size,grid_cell_size) * 0.5f;
                tile_grid[x,y].transform.SetParent(tile_parent.transform);
                tile_grid[x,y].transform.localScale = new Vector2(grid_cell_size,grid_cell_size); //Tile size
                
                //Tile sprite config
                tile_grid[x,y].AddComponent<SpriteRenderer>();
                tile_grid[x,y].GetComponent<SpriteRenderer>().sortingOrder = 1;

                //HighLight tiles
                highlight_grid[x,y] = new GameObject($"HighLight[{x},{y}]");
                highlight_grid[x,y].transform.position = GetWorldPosition(x,y) + new Vector2(grid_cell_size,grid_cell_size) * 0.5f;
                highlight_grid[x,y].transform.SetParent(tile_parent.transform);
                highlight_grid[x,y].transform.localScale = new Vector2(grid_cell_size,grid_cell_size); //Tile size

                //HighLight Config
                highlight_grid[x,y].AddComponent<SpriteRenderer>();
                highlight_grid[x,y].GetComponent<SpriteRenderer>().sortingOrder = 3;
                highlight_grid[x,y].GetComponent<SpriteRenderer>().color = new Color(0,0,0,0f);
                highlight_grid[x,y].AddComponent<BoxCollider2D>();
                highlight_grid[x,y].GetComponent<BoxCollider2D>().size = new Vector2(1,1);
                highlight_grid[x,y].GetComponent<BoxCollider2D>().isTrigger = true;
                highlight_grid[x,y].AddComponent<HighLightScript>();
                GridTilesUpdate(x,y,tile_parent);

                //Build tile grid
                GameObject build_parent = GameObject.Find("GameManager/GridManager/BuildGrid");
                build_tile_grid[x,y] = new GameObject($"Build[{x},{y}]");
                build_tile_grid[x,y].transform.position = GetWorldPosition(x,y) + new Vector2(grid_cell_size,grid_cell_size) * 0.5f;
                build_tile_grid[x,y].transform.SetParent(build_parent.transform);
                build_tile_grid[x,y].transform.localScale = new Vector2(grid_cell_size,grid_cell_size);

                //Build sprite config
                build_tile_grid[x,y].AddComponent<SpriteRenderer>();
                build_tile_grid[x,y].GetComponent<SpriteRenderer>().sortingOrder = 2;
                BuildTileGridUpdate(x,y,null);
            }
        }
    }

    //Update tile grid sprite based in crystal grid 
    public void GridTilesUpdate(int x, int y, GameObject parent){
        if(crystal_grid[x,y] == 0){
            tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = parent.GetComponent<TileGridScript>().rock_tile_sprite; //Initial Sprite
        }
        else if(crystal_grid[x,y] == 1){
            tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = parent.GetComponent<TileGridScript>().crystal_tile_sprite; //Initial Sprite
        }
        //HighLight
        //Debug.Log(highlight_grid[x,y]);
        highlight_grid[x,y].GetComponent<SpriteRenderer>().sprite = parent.GetComponent<TileGridScript>().highlight_tile_sprite; //Initial Sprite
    }

    //Update build tile grid sprite based in any sprite referenced in call action
    public void BuildTileGridUpdate(int x, int y,Sprite sprite){
        if(sprite != null){
            build_tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = sprite;

        }
        else{
            build_tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = null;
        }
    }


//***GRID INFORMATION MANIPULATION***//
    //Convert grid position to world position
    Vector2 GetWorldPosition(int x, int y){
        return new Vector2(x,y) * grid_cell_size;
    }

    //Convert mouse world position to grid position
    public void GetXY(Vector2 worldMousePosition, out int x, out int y){
        x = Mathf.FloorToInt(worldMousePosition.x / grid_cell_size);
        y = Mathf.FloorToInt(worldMousePosition.y / grid_cell_size);
    }

    //Verify if a vector2 (position context) is in grid limits
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

    //Edit base grid cell value
    public void EditBaseGrid(int x, int y, int value){
        base_grid[x,y] = value;
    }

    //Edit base text grid cell value
    public void EditBaseTextGrid(int x, int y, int value){
        text_base_grid_value[x,y].text = base_grid[x,y].ToString();
    }

    //Verify base grid value to recognize cell type
    public BuildBaseClass ReturnCellValue(Vector2 cell_position){
        BuildBaseClass cell = null;
        int x,y;
        GetXY(cell_position, out x, out y);
        if(base_grid[x,y] == 0){
            cell = null;
        }
        else{
            switch(base_grid[x,y]){
                case 1: //Is farm
                    cell = farm_grid[x,y];
                    break;
                case 2: //Is farm also
                    cell = farm_grid[x,y];  //Detail to remember: Rocks Farms are FarmClass too and this is why farm_grid[x,y] here
                    break;
                case 3: //Is collector
                    cell = collector_grid[x,y];
                    break;
                //!!!Maybe future implemetations for other cell types!!!
                case 5: //Is defense build??
                    break;
                case 4: //Is decoration??
                    break;
                
            }
        }
        return cell;
    }
}
