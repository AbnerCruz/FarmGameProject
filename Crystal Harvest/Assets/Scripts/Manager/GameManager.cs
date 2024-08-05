//System
using System.Collections;
using System.Collections.Generic;
using System;

//Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityRandom = UnityEngine.Random;


//Principal Class
public class GameManager : MonoBehaviour
{
    //Editor Click Options
    [Header("Editor Options")]
    public bool show_debugs; //Activate to show debug info on the screen. It's helpful for understanding the grid and its cells, and other test debugs

//***REFERENCES***//
    [Header("Player References")]
    //Player References
    public CameraMovement mainCamera; //Camera Object with camera movement script, this is the player movement
    public Player player; //Player
    public Text active_tool; //Object with text to show active tool
    float lastClickTime; //Double click timer

    //World References
    MyUtils utils = new MyUtils(); //My utility scripts
    SaveAndLoadScript save_load_manager = new SaveAndLoadScript(); //Save and load script
    [Header("World References")]
    public ItemListReferencesScript manager_item_list; //Object with all items
    public AudioManager audioManager; //Object with audio manager Script
    public GameObject manager_text_grid; //Object where grid text objects will be
    public GameObject manager_tile_grid; //Object where grid tile objects will be
    public GameObject manager_build_tile_grid; //Object where grid build sprite objects will be
    public GameObject manager_accumulated_farms_grid; //Object where accumulated drops text objects will be
    
    [Header("Material References")]
    public GameObject rock_item_prefab; //Rocks prefab
    public GameObject crystal_item_prefab; //Crystal prefab
    public GameObject money_material_prefab; //Money prefab
    
    //Grid Configurarion
    public GridClass principal_grid; //World grid
    [Header("Grid Config")]
    public int grid_width; //Grid width
    public int grid_height; //Grid height
    public int grid_cell_size; //Grid cell size
    public int crystal_chance;
    //ores
    public int ore_life; // Range of Life of Ores to random generation
    public Vector2 ore_amount_range = new Vector2(); //Range of ores in a crystal cell to random generated
            

    //Bool Statement Verifications 
    [Header("Boolean Verifications")]
    public bool GUI_state; //Permission state for some update functions
    public bool pause; //Verify pause state for game
    public bool create_build_tool; //Verify if create build tool is active, case true can't do other actions instead of create build
    public bool remove_build_tool; //Verify if remove build tool is active, case true can't do other actions instead of remove build
    public bool can_collect_farms; //Verify if can collect farms
    public bool buy_tile_tool; //Verify if buy tile tool is active, case true can't do other actions instead of buy tile
    public bool shopping; //Verify if shop panel is active or not
    public bool shop_amount_bool; //Verify if item quantity panel is active or not to buy items

    //Build
    [Header("Build References")]
    public Item build_object; //Selected build to create or destroy

//***CANVAS***//
    [Header("Canvas Player References")]
    //Player Canvas
    public GameObject PauseUI;
    public Text played_time_text; //Debug how much time player played
    public GameObject hot_bar; //Player hot bar object
    public int hotbar_num; //Player selected hotbar number

    [Header("Shop References")]
    public Item item_to_shop; // selected item to shop
    public ShopScript shop_manager; //Shop manager object

    [Header("Game Save Options")]
    //SAVE AND LOAD
    public string filePath; //Current save path
    float save_timer; //Auto save current timer
    public float initial_save_timer; //Auto save timer

//***UNITY METHODS***//
    void Awake(){
        //Save path
        filePath = Application.persistentDataPath + "/"+ PlayerPrefs.GetString("SaveName");

        //Creating base grid
        principal_grid = new GridClass(grid_width, grid_height, grid_cell_size,this);

        //Creating crystal grid
        CreatingCrystalGrid();
    }

    void Start(){
        GUI_state = false;
        //Initialize hotbar selection
        hotbar_num = 1;
        hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);


        //Load scene 
        if(PlayerPrefs.GetInt("LoadOption") == 1){
            save_load_manager.Load(this,principal_grid,filePath);
            UpdateTextAccumulatedPoints();
        }
        mainCamera.CameraCentralize();
    }

    void FixedUpdate(){
        //Played time
        player.played_time += Time.deltaTime;
        string memorytext = player.played_time.ToString("0.0");
        played_time_text.text = $"Time: {memorytext}s";

        //Player
        mainCamera.Move(grid_width,grid_height,grid_cell_size); //Camera movement

    }
    void Update(){
        //Builds
        GameLoop(); //Farm mature function
        BuildObjectDefinition(); //Define build object based in selected hotBar

        //Debugs
        ShowDebugs();
        ActiveToolController();

        //Save timer
        save_timer = utils.MyTimer(initial_save_timer,save_timer);
        if(save_timer <= 0){
            save_load_manager.Save(this,principal_grid,filePath);
            save_timer = initial_save_timer;
        }

        //Pause mode
        if(!GUI_state){
            mainCamera.CameraZoom(); //Camera zoom
            Inputs();
            CanCollectFarmState();
        }
        //Opening some GUI
        else{
            //Is not pause GUI
            if(pause == false){
                can_collect_farms = false;
                shop_manager.ShopPanelStatus();
                ShopAmountBoolController();
                if(shopping){
                    shop_manager.ShopPanelController(item_to_shop);
                }
            }
            //Is pause GUI
            else{
                if(Input.GetKeyDown(KeyCode.Escape)){
                    GUI_state = false;
                    pause = false;
                    PauseUI.SetActive(false);
                }
            }
        }
    }
//***GAME LOOP***//
    void GameLoop(){
        for(int x = 0; x < principal_grid.farm_grid.GetLength(0); x++){
            for(int y = 0; y < principal_grid.farm_grid.GetLength(1); y++){
                FarmGrowth(x,y);
                Collector(x,y);
                //Debug
                AccumulatedFarmsController(x,y);
            }
        }
    }

//***GRID CONTROLLER***//
    //Read cell values
    public int GridReader(GridClass grid,int x,int y){
        return grid.base_grid[x,y];
    }

    //Update every cell type
    public void GridBuildUpdater(GridClass grid){
        for(int x = 0; x < grid.base_grid.GetLength(0); x++){
            for(int y = 0; y < grid.base_grid.GetLength(1); y++){
                switch(GridReader(grid,x,y)){
                    default:
                        grid.farm_grid[x,y] = null;
                        grid.collector_grid[x,y] = null;
                        grid.BuildTileGridUpdate(x,y,null);
                    break;
                    case 1: //Crystal extractor
                        foreach(BuildBaseClass i in manager_item_list.all_builds){
                            if((int)i.build_value == 1 && grid.farm_grid[x,y] == null){
                                grid.farm_grid[x,y] = new FarmClass((FarmObject)i.build_base_item,this);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                    case 2: //Energy farm
                        foreach(BuildBaseClass i in manager_item_list.all_builds){
                            if((int)i.build_value == 2 && grid.farm_grid[x,y] == null){
                                grid.farm_grid[x,y] = new FarmClass((FarmObject)i.build_base_item,this);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                    case 3: //Collector
                        foreach(BuildBaseClass i in manager_item_list.all_builds){
                            if((int)i.build_value == 3 && grid.collector_grid[x,y] == null){
                                grid.collector_grid[x,y] = new CollectorClass(this,(CollectorObject)i.build_base_item);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                }
            }
        }
    }

    //Defines where crystals ores will spawn
    void CreatingCrystalGrid(){
        for(int x = 0; x < principal_grid.crystal_grid.GetLength(0); x++){
            for(int y = 0; y < principal_grid.crystal_grid.GetLength(1); y++){
                int sorted = UnityRandom.Range(0,100);
                if(sorted > 100 - crystal_chance){
                    principal_grid.crystal_grid[x,y] = 1;
                    principal_grid.GridTilesUpdate(x,y,manager_tile_grid);
                }
                //Ore Generation
                int generated_ore_amount = UnityRandom.Range(Mathf.RoundToInt(ore_amount_range.x),Mathf.RoundToInt(ore_amount_range.y));
                principal_grid.ore_amount_grid[x,y] = generated_ore_amount; //Define cell ore amount
                if(ore_life > 1){
                    principal_grid.ore_max_life_grid[x,y] = ore_life;
                }
                else{
                    principal_grid.ore_max_life_grid[x,y] = 2;
                }
                principal_grid.ore_life_grid[x,y] = principal_grid.ore_max_life_grid[x,y];
            }
        }
    }

//***REWARD SYSTEMS***//
    //Farm growth timer
    void FarmGrowth(int x, int y){
        if(principal_grid.farm_grid[x,y] != null){
            principal_grid.farm_grid[x,y].Timer();
        }
    }

    //Collector timer
    void Collector(int x, int y){
        if(principal_grid.collector_grid[x,y] != null){
            principal_grid.collector_grid[x,y].CollectorTimer(x,y,principal_grid);
        }
    }
    
    //Farms reward 
    void FarmCollect(int x, int y){
        if(principal_grid.farm_grid[x,y].farm_current_storage != 0){
            audioManager.item_drop_audio.Play();
            player.player_rocks += UnityRandom.Range(1,principal_grid.farm_grid[x,y].farm_current_storage/2);
            player.player_crystals += principal_grid.farm_grid[x,y].farm_current_storage;
            principal_grid.farm_grid[x,y].farm_current_storage = 0;
            UpdateTextAccumulatedPoints();
        }
    }

    //Farms accumulated points
    public void UpdateTextAccumulatedPoints(){
        for(int x = 0; x < principal_grid.text_farm_accumulated_points.GetLength(0); x++){
            for(int y = 0; y < principal_grid.text_farm_accumulated_points.GetLength(1); y++){
                if(principal_grid.farm_grid[x,y] != null){
                    principal_grid.text_farm_accumulated_points[x,y].text = principal_grid.farm_grid[x,y].farm_current_storage.ToString();
                }
            }
        }
    }

    public void MiningReward(int x, int y){
        if(GridReader(principal_grid,x,y) == 0 && can_collect_farms){
            if(principal_grid.ore_life_grid[x,y] > 0){
                int reward = 0;
                if(principal_grid.crystal_grid[x,y] == 1){
                    //Is a Crystal cell
                    if(principal_grid.ore_amount_grid[x,y] > 0){
                        //Have Crystals
                        reward = UnityRandom.Range(principal_grid.ore_amount_grid[x,y]/2,principal_grid.ore_amount_grid[x,y]);
                        player.player_crystals += reward;
                        principal_grid.ore_life_grid[x,y]--;
                        audioManager.item_drop_audio.Play();
                    }
                }
                else{
                    //Is not a Crystal cell
                    if(principal_grid.ore_amount_grid[x,y] > 0){
                        reward = UnityRandom.Range(principal_grid.ore_amount_grid[x,y]/2,principal_grid.ore_amount_grid[x,y]);
                        player.player_rocks += reward;
                        principal_grid.ore_life_grid[x,y]--;
                        audioManager.item_drop_audio.Play();
                    }
                }
                //Drop effect
                for(int i = 0; i < Mathf.FloorToInt(reward/2); i++){
                    GameObject newRock = Instantiate(rock_item_prefab, utils.GetMousePos(), Quaternion.identity);
                    newRock.GetComponent<Rigidbody2D>().velocity = new Vector3(UnityRandom.Range(-3f,3f),UnityRandom.Range(1f,4f),0);
                }
                //Sprite update when ore breaks
                if(principal_grid.ore_life_grid[x,y] <= 0){
                    principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = manager_tile_grid.GetComponent<TileGridScript>().breaked_tile_sprite;
                }
            }
        }
    }

//***BOOL CONTROLLERS***//
    //Create build bool controller
    public void CreateBuildBoolControl(){
        create_build_tool =! create_build_tool;
        if(create_build_tool){
            remove_build_tool = false;
            buy_tile_tool = false;
        }
    }

    //Remove build bool controller
    public void RemoveBuildBoolControl(){
        remove_build_tool =! remove_build_tool;
        if(remove_build_tool){
            create_build_tool = false;
            buy_tile_tool = false;
        }
    }

    //Control if buy or sell amount selection function is active or not to control panel activation
    void ShopAmountBoolController(){
        shop_manager.shop_amount_screen.SetActive(shop_amount_bool);
        shop_manager.shop_amount_screen.GetComponent<ShopAmountQuantityScript>().selected_object = item_to_shop;
    }

    //Text Debug Controller
    public void ShowDebugs(){
        if(show_debugs){
            manager_text_grid.SetActive(true);
        }
        else{
            manager_text_grid.SetActive(false);
        }
    }

    //Buy tile bool controller
    void BuyTileBoolController(){
        buy_tile_tool = !buy_tile_tool;
        create_build_tool = false;
        remove_build_tool = false;
    }

    //Can collect farm bool controller
    void CanCollectFarmState(){
        if(create_build_tool || remove_build_tool || buy_tile_tool){
            can_collect_farms = false;
        }
        else{
            can_collect_farms = true;
        }
    }

    //Pause GUI bool controller
    public void PauseGUIController(){
        pause = !pause;
        if(pause){
            PauseUI.SetActive(true);
            GUI_state = true;
        }
        else{
            PauseUI.SetActive(false);
        }
    }

//***PLAYER INPUTS***//
    void Inputs(){
        //HotBar inputs
        HotBarSystem();

        //Disable current option with escape key
        if(Input.GetKeyDown(KeyCode.Escape)){
            //Disable create build tool
            if(create_build_tool){
                CreateBuildBoolControl();
            }
            //Disable remove build tool
            else if(remove_build_tool){
                RemoveBuildBoolControl();
            }
            //Disable buy tile tool
            else if(buy_tile_tool){
                BuyTileBoolController();
            }
            //No options Esc Function
            else{
                PauseGUIController();
            }
        }
        //Buy tile tool activation
        if(Input.GetKeyDown(KeyCode.J)){
            BuyTileBoolController();
        }
        //Create build tool activation
        if(Input.GetKeyDown(KeyCode.B)){
            CreateBuildBoolControl();
        }
        //Remove build tool activation
        if(Input.GetKeyDown(KeyCode.X)){
            RemoveBuildBoolControl();
        }

    //***CLICK INPUTS***//
        //***DOUBLE CLICK**//
        if(Input.GetMouseButtonDown(0)){
            int x,y;
            principal_grid.GetXY(utils.GetMousePos(), out x, out y);
            float timeSinceLastClick = Time.time - lastClickTime;
            if(timeSinceLastClick <= 0.3f){
                if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                    audioManager.click_audio.Play();
                //***FARMS COLLECT***//
                    //Farms rewards
                    if(can_collect_farms){
                        FarmClass farm = principal_grid.farm_grid[x,y];
                        if(farm != null){
                            //Crystal farm
                            if(farm.farm_reward == FarmObject.FarmRewardType.Crystal){
                                farm.FarmCollect(farm);
                            }
                            //Energy farm
                            else if(farm.farm_reward == FarmObject.FarmRewardType.Energy){
                                if(farm.farm_current_storage != 0){
                                    player.player_energy += farm.farm_current_storage;
                                    farm.farm_current_storage = 0;
                                    UpdateTextAccumulatedPoints();
                                    audioManager.item_drop_audio.Play();
                                }
                            }
                        }
                        //Mining reward
                        MiningReward(x,y); 
                    }
                //***BUYING TILES***//
                    //Buy new tile
                    else if(buy_tile_tool){
                        if(principal_grid.ore_life_grid[x,y] <= 0 && player.player_tiles > 0){
                            if(principal_grid.player_buyed_tiles[x,y] == 0){
                                principal_grid.player_buyed_tiles[x,y] = 1;
                                principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = GameObject.Find("TileGrid").GetComponent<TileGridScript>().flat_rock_tile;
                                player.player_tiles -= 1;
                            }
                            else{
                                //Already have tile
                            }
                        }
                    }
                }
            }
            lastClickTime = Time.time;
        //***BUILD SYSTEM***//
            //Build system
            if(create_build_tool || remove_build_tool){
                BuildBaseClass build = new BuildBaseClass(0,null);
                build.BuildSystem(this, x, y, build_object);
            }
        }
    }

//***HOTBAR SYSTEM***//
    //HotBar selection
    void HotBarSystem(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            hotbar_num = 1;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)){
            hotbar_num = 2;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3)){
            hotbar_num = 3;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4)){
            hotbar_num = 4;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5)){
            hotbar_num = 5;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6)){
            hotbar_num = 6;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7)){
            hotbar_num = 7;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha8)){
            hotbar_num = 8;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha9)){
            hotbar_num = 9;
            hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        }
    }

    //Build Object Based in Selected HotBar
    void BuildObjectDefinition(){
        if(hot_bar.GetComponent<HotBarInventoryScript>().hotbar_inventory[hotbar_num-1] != null){
            if(hot_bar.GetComponent<HotBarInventoryScript>().hotbar_inventory[hotbar_num-1].item_type == Item.ItemType.Build){
                if(build_object != hot_bar.GetComponent<HotBarInventoryScript>().hotbar_inventory[hotbar_num-1]){
                    build_object = hot_bar.GetComponent<HotBarInventoryScript>().hotbar_inventory[hotbar_num-1];
                }
            }
        }
        else{
            build_object = null;
        }
    }

//***DEBUGS SYSTEM***//
    //Accumulated farms visibility
    public void AccumulatedFarmsController(int x, int y){
        //farm stock memory
        if(principal_grid.farm_grid[x,y] != null){
            principal_grid.farm_storage_grid[x,y] = principal_grid.farm_grid[x,y].farm_current_storage;
        }

        //Visibility controller
        if(principal_grid.text_farm_accumulated_points[x,y] != null){
            if(principal_grid.base_grid[x,y] != 0){
                principal_grid.text_farm_accumulated_points[x,y].gameObject.SetActive(true);
            }
            else{
                principal_grid.text_farm_accumulated_points[x,y].gameObject.SetActive(false);
            }
        }
    }

    //Current tool debug
    void ActiveToolController(){
        if(create_build_tool){
            active_tool.text = "Active Tool: Create Build";
        }
        else if(remove_build_tool){
            active_tool.text = "Active Tool: Remove Build";
        }
        else if(buy_tile_tool){
            active_tool.text = "Active Tool: Buying Tile";
        }
        else{
            active_tool.text = "Active Tool: None";
        }
    }

    //Destroy Objects support function
    public void ManagerDestroy(GameObject x){
        Destroy(x);
    }

//***Save SYSTEM***//
    //Save
    public void SaveFunction(){
        save_load_manager.Save(this,principal_grid,filePath);
    }
    //Go to main menu
    public void MenuFunction(int indexScene){
        SaveFunction();
        SceneManager.LoadScene(indexScene);
    }
} 
