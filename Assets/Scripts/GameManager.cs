using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System;

[Serializable]
class PlayerData{
    public string player_nickname;
    public float player_money;
    public float player_energy;
    public float player_rocks;
    public float player_crystals;
    public int player_tiles;
    public float player_played_time;
    public PlayerInventory player_inventory;
    public GridClass world_grid;
}
public class GameManager : MonoBehaviour
{
    float save_timer;
    float initial_save_timer;
    //References
    MyUtils utils = new MyUtils();
    [Header("References")]
    public Player player;
    public CameraMovement mainCamera;
    public Text active_tool;
    public List<BuildBaseClass> builds_list;
    public ItemListReferencesScript item_list;
    public GameObject PauseUI;
    public Text played_time_text;
    public AudioSource rock_drop_audio;
    public AudioSource rock_click_audio;
    SaveAndLoadScript save_load_manager = new SaveAndLoadScript();


    //Grid Variables
    public GridClass principal_grid;
    [Header("Grid Config")]
    public int grid_width;
    public int grid_height;
    public int grid_cell_size;

    //Options
    [Header("Options")]
        //Debug Visibility
        public bool show_debugs;
        public GameObject text_grid;
    public bool play_mode; //if is running or not the playable mode
    public bool pause;
    public bool create_build; //if can or not build
    public bool remove_build; //if can or not remove build
    public bool can_colect_farms; //if can click to colect farms
    public bool buying_tile; //if player is buying tile function
    [Header("Build Options")]
    public Item build_object; //selected item to build or destroy
    public GameObject create_build_UI_button; //Button to activate build system
    public GameObject remove_create_build_UI_button;//Button to activate remove system
    float lastClickTime; //rock collect click timer

    //Materials
        //Ores
        public int ore_life; // Range od Life of Ores
        public Vector2 ore_amount_range = new Vector2();
            //Rock
            public GameObject rock_material_prefab; // prefab of rocks
            //Crystal
            public GameObject crystal_material_prefab;
        //Money
        public GameObject money_material_prefab; // prefab of money
        
    


//***CANVAS***//
    //Shop Vars
        //Panels
        public GameObject shop_object; //shop manager object
        public GameObject shop_panel; // shop panel object
        public GameObject shop_amount_screen; // activate item quantity to buy panel
        public Item item_to_shop; // selected item to shop

        //Booleans
        public bool shopping; // if shop panel is active or not
        public bool shop_amount_bool; // if item quantity panel is active or not

    //HotBar
    public GameObject hot_bar; // player hot bar
    public int hotbar_num; // player selected hotbar

    //Rocks Vars
    float rock_reward_cooldown;
    public float rock_reward_cooldown_initial;

    //SAVE AND LOAD
    public string filePath;



//***UNITY METHODS***//
    void Awake(){
        //Save Path
        filePath = Application.persistentDataPath + "/"+ PlayerPrefs.GetString("SaveName");
        initial_save_timer = 10f;
        
        //Creating Base Grid
        principal_grid = new GridClass(grid_width, grid_height, grid_cell_size,this);
        foreach(BuildBaseClass i in builds_list){
            i.build_value = (int)i.build_base_item.build_type;
        }
        //Creating Crystal Grid
        CreatingCrystalGrid();
        
        
    }
    void Start(){
        
        play_mode = true;
        mainCamera.transform.position = new Vector3(grid_width/2, grid_height/2,-10);
        CreateBuildBoolControl();
        CreateBuildBoolControl();
        RemoveBuildBoolControl();
        RemoveBuildBoolControl();
        hotbar_num = 1;
        hot_bar.GetComponent<HotBarInventoryScript>().HotBarSelection(hotbar_num);
        //ItemsID
        for(int i = 0; i < item_list.items.Count; i++){
            item_list.items[i].item_id = i+1;
        }
        //rock reward cooldown
        rock_reward_cooldown_initial = 1f;
        //LoadScene 
        if(PlayerPrefs.GetInt("LoadOption") == 1){
            save_load_manager.Load(this,principal_grid,filePath);
        }
    }
    void Update(){
        player.played_time += Time.deltaTime;
        string memorytext = player.played_time.ToString("0.0");
        played_time_text.text = $"Time: {memorytext}s";
        //Farm Function
        FarmGrowth();
        //Colector Function
        Collector();
        //Camera Movement
        mainCamera.Move(grid_width,grid_height,grid_cell_size);
        //BuildObject
        BuildObjectDefinition();
        //Rock Reward Cooldown
        rock_reward_cooldown += Time.deltaTime;
        if(rock_reward_cooldown >= rock_reward_cooldown_initial){
            rock_reward_cooldown = rock_reward_cooldown_initial;
        }
        //Debugs
        ShowDebugs();
        ActiveToolController();
        AccumulatedFarmsVisibilityController();
        //SaveTimer
        save_timer = utils.MyTimer(initial_save_timer,save_timer);
        if(save_timer <= 0){
            save_load_manager.Save(this,principal_grid,filePath);
            save_timer = initial_save_timer;
        }

        //Pause Mode
        if(play_mode){
            ShopStatus();
            DoubleClick();
            Inputs();
            CanCollectFarmState();
        }
        else{ //Opening some panel
            if(pause == false){
                can_colect_farms = false;
                ShopStatus();
                ShopAmountBoolController();
                if(shopping){
                    ShopPanelController();
                }
            }
            else{
                if(Input.GetKeyDown(KeyCode.Escape)){
                    play_mode = true;
                    pause = false;
                    PauseUI.SetActive(false);
                }
            }
        }
    }

//***MY METHODS***//
    //Build System
    void BuildSystem(){
            if(create_build){
                if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                    if(build_object != null && build_object.item_type == Item.ItemType.Build && principal_grid.ReturnCellValue(utils.GetMousePos()) == null){
                        int x,y;
                        principal_grid.GetXY(utils.GetMousePos(), out x, out y);
                        if(principal_grid.ore_life_grid[x,y] <= 0){
                            if(principal_grid.player_tiles[x,y] == 1){
                                principal_grid.EditBaseGrid(x,y,(int)build_object.build_type);
                                if(show_debugs){
                                    principal_grid.EditBaseTextGrid(x,y,(int)build_object.build_type);
                                }
                                BuildGridUpdater(principal_grid);
                                player.CreateBuildDecrease(principal_grid.ReturnCellValue(utils.GetMousePos()).build_base_item);
                            }
                        }
                        else{
                            //can't build
                        }
                    }
                }
            }
            else if(remove_build){
                if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                    int x,y;
                    principal_grid.GetXY(utils.GetMousePos(), out x, out y);
                    if(principal_grid.ReturnCellValue(utils.GetMousePos()) != null){
                        if(principal_grid.ReturnCellValue(utils.GetMousePos()).build_value == 1 || principal_grid.ReturnCellValue(utils.GetMousePos()).build_value == 2){
                            FarmRewardFunction(x,y);
                        }
                        player.RemoveBuildIncrease(principal_grid.ReturnCellValue(utils.GetMousePos()).build_base_item);
                        principal_grid.EditBaseGrid(x,y,0);
                        if(show_debugs){
                            principal_grid.EditBaseTextGrid(x,y,0);
                        }
                        BuildGridUpdater(principal_grid);
                        if(principal_grid.player_tiles[x,y] == 1){
                            principal_grid.BuildTileGridUpdate(x,y,null);
                        }
                    }
                }
            }
        
    }
    int GridReader(GridClass grid,int x,int y){
        return grid.base_grid[x,y];
    }
    public void BuildGridUpdater(GridClass grid){
        for(int x = 0; x < grid.base_grid.GetLength(0); x++){
            for(int y = 0; y < grid.base_grid.GetLength(1); y++){
                switch(GridReader(grid,x,y)){
                    default:
                        grid.farm_grid[x,y] = null;
                        grid.collector_grid[x,y] = null;
                        grid.BuildTileGridUpdate(x,y,null);
                    break;
                    case 1: //Crystal Extractor
                        foreach(BuildBaseClass i in builds_list){
                            if((int)i.build_value == 1 && grid.farm_grid[x,y] == null){
                                grid.farm_grid[x,y] = new FarmClass((FarmObject)i.build_base_item,this);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                    case 2: //Energy Farm
                        foreach(BuildBaseClass i in builds_list){
                            if((int)i.build_value == 2 && grid.farm_grid[x,y] == null){
                                grid.farm_grid[x,y] = new FarmClass((FarmObject)i.build_base_item,this);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                    case 3: //Collector
                        foreach(BuildBaseClass i in builds_list){
                            if((int)i.build_value == 3 && grid.collector_grid[x,y] == null){
                                grid.collector_grid[x,y] = new CollectorClass(this,(CollectorObject)i.build_base_item);
                                grid.BuildTileGridUpdate(x,y,i.build_base_item.item_sprite);
                            }
                        }
                    break;
                }
                AccumulatedFarmsVisibilityController();
            }
        }
    }

    //Farm Growth
    void FarmGrowth(){
        for(int x = 0; x < principal_grid.farm_grid.GetLength(0); x++){
            for(int y = 0; y < principal_grid.farm_grid.GetLength(1); y++){
                if(principal_grid.farm_grid[x,y] != null){
                    principal_grid.farm_grid[x,y].Timer();
                }
            }
        }
    }
    //Collector
    void Collector(){
        for(int x = 0; x < principal_grid.collector_grid.GetLength(0); x++){
            for(int y = 0; y < principal_grid.collector_grid.GetLength(1); y++){
                if(principal_grid.collector_grid[x,y] != null){
                    principal_grid.collector_grid[x,y].CollectorTimer(x,y,principal_grid);
                }
            }
        }
    }

    //Shop Controller
    void ShopStatus(){
        ShopScript shop = shop_object.GetComponent<ShopScript>();
        if(item_to_shop == null){
            shop_amount_bool = false;
            shop_panel.SetActive(false);
        }
        else{
            shop_panel.SetActive(true);
        }
        
    }
    public void OpenOrCloseShop(){
        if(pause == false){
            play_mode =! play_mode;
            shop_object.SetActive(!play_mode);
            if(play_mode){
                shopping = false;
                item_to_shop = null;
            }
            else{
                shopping = true;
            }
        }
    }

    //DEBUG Accumulated Points
    public void UpdateTextAccumulatedPoints(){
        for(int x = 0; x < principal_grid.text_farm_accumulated_points.GetLength(0); x++){
            for(int y = 0; y < principal_grid.text_farm_accumulated_points.GetLength(1); y++){
                if(principal_grid.farm_grid[x,y] != null){
                    principal_grid.text_farm_accumulated_points[x,y].text = principal_grid.farm_grid[x,y].farm_stock.ToString();
                }
            }
        }
    }

    //Double Click Collect Function
    void DoubleClick(){
        if(Input.GetMouseButtonDown(0)){
            float timeSinceLastClick = Time.time - lastClickTime;
            if(timeSinceLastClick <= 0.5f){
                if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                    rock_click_audio.Play();
                    int x,y;
                    principal_grid.GetXY(utils.GetMousePos(), out x, out y);
                    //Farms Collect
                    if(can_colect_farms){ 
                        if(principal_grid.farm_grid[x,y] != null){
                            if(principal_grid.farm_grid[x,y].farm_reward == FarmObject.FarmReward.Crystal){
                                FarmRewardFunction(x,y);
                            }
                            else if(principal_grid.farm_grid[x,y].farm_reward == FarmObject.FarmReward.Energy){
                                if(principal_grid.farm_grid[x,y].farm_stock != 0){
                                    player.player_energy += principal_grid.farm_grid[x,y].farm_stock;
                                    principal_grid.farm_grid[x,y].farm_stock = 0;
                                    UpdateTextAccumulatedPoints();
                                    rock_drop_audio.Play();
                                }
                            }
                        }
                        //Rocks And Crystals Click Reward
                        CrystalReward(x,y); 
                    }
                    else if(buying_tile){//Buy new Tile
                        if(principal_grid.ore_life_grid[x,y] <= 0 && player.player_tiles > 0){
                            if(principal_grid.player_tiles[x,y] == 0){
                                principal_grid.player_tiles[x,y] = 1;
                                principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = GameObject.Find("TileGrid").GetComponent<TileGridScript>().flat_rock_tile;
                                player.player_tiles -= 1;
                            }
                            else{
                                Debug.Log("already have tile");
                            }
                        }
                    }
                }
            }
            lastClickTime = Time.time;
        }
    }
    void FarmRewardFunction(int x, int y){
        if(principal_grid.farm_grid[x,y].farm_stock != 0){
            rock_drop_audio.Play();
            player.player_rocks += UnityRandom.Range(1,principal_grid.farm_grid[x,y].farm_stock/2);
            player.player_crystals += principal_grid.farm_grid[x,y].farm_stock;
            principal_grid.farm_grid[x,y].farm_stock = 0;
            UpdateTextAccumulatedPoints();
        }
    }

    //UI Build System Control
    public void CreateBuildBoolControl(){
        create_build =! create_build;
        if(create_build){
            remove_build = false;
            buying_tile = false;
        }
    }
    public void RemoveBuildBoolControl(){
        remove_build =! remove_build;
        if(remove_build){
            create_build = false;
            buying_tile = false;
        }
    }

    //Receive Inputs
    void Inputs(){
        HotbarSelection();
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(create_build){
                CreateBuildBoolControl();
            }
            else if(remove_build){
                RemoveBuildBoolControl();
            }
            else if(buying_tile){
                BuyTileBoolController();
            }
            else{
                //Esc Function
                PauseUIController();
            }
        }
        if(Input.GetKeyDown(KeyCode.J)){
            BuyTileBoolController();
        }
        if(Input.GetKeyDown(KeyCode.B)){
            CreateBuildBoolControl();
        }
        if(Input.GetKeyDown(KeyCode.X)){
            RemoveBuildBoolControl();
        }
        //Build System Input
        if(Input.GetMouseButtonDown(0)){
            BuildSystem();
        }

    }
    //Add a new Item to Inventory
    void NewItemToInventory(Item item, int quantity){
        if(item != null){
            player.BuyFunction(item, quantity);

        }
    }
    void NewSell(Item item, int quantity){
        if(item != null){
            switch(item.item_type){
                case Item.ItemType.Build:
                    player.SellFunction(item, quantity);
                    break;
                case Item.ItemType.Material:
                    switch(item.price_type){
                        case Item.PriceType.Ore:
                            if(player.player_rocks >= quantity){
                                player.player_rocks -= quantity;
                                player.player_money += item.item_price * quantity;
                            }
                            break;
                        case Item.PriceType.Crystal:
                            if(player.player_crystals >= quantity){
                                player.player_crystals -= quantity;
                                player.player_money += item.item_price * quantity;
                            }
                            break;
                        case Item.PriceType.Money:
                            if(!(item.item_name == "Tile")){
                                if(player.player_energy >= quantity){
                                    player.player_energy -= quantity;
                                    player.player_money += item.item_price * quantity;
                                }
                            }
                            else{
                                if(player.player_tiles >= quantity){
                                    player.player_tiles -= quantity;
                                    player.player_money += item.item_price * quantity;
                                }
                            }
                            break;
                    }
                    break;

            }
        }
    }
    //Buying Function
    public void Buying(int quantity){
        if(!(item_to_shop.item_type == Item.ItemType.Material)){
            switch(item_to_shop.price_type){
                case Item.PriceType.Money:
                    if(player.player_money >= item_to_shop.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item_to_shop, quantity);
                        player.player_money -= item_to_shop.item_price * quantity;
                    }
                    else{
                        //NO MONEY TO BUY FUNCTIONS
                    }
                    break;
                case Item.PriceType.Ore:
                    if(player.player_rocks >= item_to_shop.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item_to_shop, quantity);
                        player.player_rocks -= item_to_shop.item_price * quantity;
                    }
                    else{
                        //NO ROCKS TO BUY FUNCTIONS
                    }
                    break;
                case Item.PriceType.Crystal:
                    if(player.player_crystals >= item_to_shop.item_price * quantity){
                        //BUYED
                        NewItemToInventory(item_to_shop, quantity);
                        player.player_crystals -= item_to_shop.item_price * quantity;
                    }
                    else{
                        //NO CRYSTALS TO BUY FUNCTIONS
                    }
                    break;
            }
        }
        else{
            if(item_to_shop.item_name == "Tile"){
                player.player_tiles += quantity;
                player.player_money -= item_to_shop.item_price * quantity;
            }
        }
        
    }
    public void Selling(int quantity){
        NewSell(item_to_shop, quantity);
    }

    //HotBarSelectionFunction
    void HotbarSelection(){
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
    //This shit control if buy or sell amount selection function is active or not
    void ShopAmountBoolController(){
        shop_amount_screen.SetActive(shop_amount_bool);
        shop_amount_screen.GetComponent<ShopAmountQuantityScript>().selected_object = item_to_shop;
    }
    //button function to open amount selection to buy
    public void SelectAmountBuyingFunction(){
        if(!(item_to_shop.item_type == Item.ItemType.Material)){
            switch(item_to_shop.price_type){
                case Item.PriceType.Crystal:
                    if(player.player_crystals >= item_to_shop.item_price){
                        shop_amount_bool = true;
                        shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
                    }
                    break;
                case Item.PriceType.Ore:
                    if(player.player_rocks >= item_to_shop.item_price){
                        shop_amount_bool = true;
                        shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
                    }
                    break;
                default:
                    if(player.player_money >= item_to_shop.item_price){
                        shop_amount_bool = true;
                        shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
                    }
                    break;
            }

        }
        else{
            if(item_to_shop.item_name == "Tile"){
                if(player.player_money >= item_to_shop.item_price){
                    shop_amount_bool = true;
                    shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = true;
                }
            }
            else{
                //can't buy this material
            }
        }
    }
    public void SelectAmountSellingFunction(){
        if(item_to_shop.item_type != Item.ItemType.Material){
            foreach(int i in player.inventory.item_slots){
                if(i == item_to_shop.item_id){
                    shop_amount_bool = true;
                    shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;
                    break;
                }
            }
        }
        else{
            shop_amount_bool = true;
            shop_amount_screen.GetComponent<ShopAmountQuantityScript>().buy_or_sell = false;
        }
    }

    public void ShowDebugs(){
        if(show_debugs){
            text_grid.SetActive(true);
        }
        else{
            text_grid.SetActive(false);
        }
    }

    public void ShopPanelController(){
        if(item_to_shop != null){
            ShopPanelScript current_panel = shop_panel.GetComponent<ShopPanelScript>();
            current_panel.selected_item_sprite.GetComponent<Image>().sprite = item_to_shop.item_sprite;
            current_panel.selected_item_name.GetComponent<Text>().text = item_to_shop.item_name;
            current_panel.selected_item_description.GetComponent<Text>().text = item_to_shop.item_description;
            current_panel.selected_item_price.GetComponent<Text>().text = ": " + item_to_shop.item_price.ToString();
            if((item_to_shop.material_type == Item.MaterialType.none)){
                switch(item_to_shop.price_type){
                    case Item.PriceType.Ore:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = rock_material_prefab.GetComponent<SpriteRenderer>().sprite;//Crystal
                        break;
                    case Item.PriceType.Money:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = money_material_prefab.GetComponent<SpriteRenderer>().sprite;//Money
                        break;
                    case Item.PriceType.Crystal:
                        current_panel.selected_item_price_icon.GetComponent<Image>().sprite = crystal_material_prefab.GetComponent<SpriteRenderer>().sprite;
                        break;
                }
            }
            else{
                current_panel.selected_item_price_icon.GetComponent<Image>().sprite = money_material_prefab.GetComponent<SpriteRenderer>().sprite;//Money
            }
        }
    }
    public void CrystalReward(int x, int y){
        if(principal_grid.base_grid[x,y] == 0 && can_colect_farms){
            if(rock_reward_cooldown >= rock_reward_cooldown_initial){
                if(principal_grid.ore_life_grid[x,y] > 0){
                    int reward = 0;
                    if(principal_grid.crystal_grid[x,y] == 1){
                        //Is a Crystal cell
                        if(principal_grid.ore_amount_grid[x,y] > 0){
                            //Have Crystals
                            reward = UnityRandom.Range(principal_grid.ore_amount_grid[x,y]/2,principal_grid.ore_amount_grid[x,y]);
                            player.player_crystals += reward;
                            principal_grid.ore_life_grid[x,y]--;
                            rock_drop_audio.Play();
                        }
                    }
                    else{
                        //Is not a Crystal cell
                        if(principal_grid.ore_amount_grid[x,y] > 0){
                            reward = UnityRandom.Range(principal_grid.ore_amount_grid[x,y]/2,principal_grid.ore_amount_grid[x,y]);
                            player.player_rocks += reward;
                            principal_grid.ore_life_grid[x,y]--;
                            rock_drop_audio.Play();
                        }
                    }
                    for(int i = 0; i < Mathf.FloorToInt(reward/2); i++){
                        GameObject newRock = Instantiate(rock_material_prefab, utils.GetMousePos(), Quaternion.identity);
                        newRock.GetComponent<Rigidbody2D>().velocity = new Vector3(UnityRandom.Range(-3f,3f),UnityRandom.Range(1f,4f),0);
                    }
                    rock_reward_cooldown = 0f;
                    Color current_color = principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().color;
                    if(principal_grid.ore_life_grid[x,y] <= 0){
                        principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = GameObject.Find("TileGrid").GetComponent<TileGridScript>().breaked_tile_sprite;
                    }
                }
            }
        }
    }
    void CreatingCrystalGrid(){
        for(int x = 0; x < principal_grid.crystal_grid.GetLength(0); x++){
            for(int y = 0; y < principal_grid.crystal_grid.GetLength(1); y++){
                int sorted = UnityRandom.Range(0,100);
                int chance = 20;
                if(sorted > 100 - chance){
                    principal_grid.crystal_grid[x,y] = 1;
                    principal_grid.GridTilesUpdate(x,y,GameObject.Find("TileGrid"));
                }
                principal_grid.ore_amount_grid[x,y] = UnityRandom.Range(Mathf.RoundToInt(ore_amount_range.x),Mathf.RoundToInt(ore_amount_range.y));
                principal_grid.ore_max_life_grid[x,y] = UnityRandom.Range(2,ore_life+1);
                principal_grid.ore_life_grid[x,y] = principal_grid.ore_max_life_grid[x,y];
            }
        }
    }
    void CanCollectFarmState(){
        if(create_build || remove_build || buying_tile){
            can_colect_farms = false;
        }
        else{
            can_colect_farms = true;
        }
    }
    void BuyTileBoolController(){
        buying_tile = !buying_tile;
        create_build = false;
        remove_build = false;
    }
    public void AccumulatedFarmsVisibilityController(){
        for(int x = 0; x < principal_grid.text_farm_accumulated_points.GetLength(0); x++){
            for(int y = 0; y < principal_grid.text_farm_accumulated_points.GetLength(1); y++){
                //farm stock memory
                if(principal_grid.farm_grid[x,y] != null){
                    principal_grid.farm_stock_grid[x,y] = principal_grid.farm_grid[x,y].farm_stock;
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
        }
        
    }
    public void ManagerDestroy(GameObject x){
        Destroy(x);
    }
    public void SaveFunction(){
        save_load_manager.Save(this,principal_grid,filePath);
    }
    public void MenuFunction(int indexScene){
        SaveFunction();
        SceneManager.LoadScene(indexScene);
    }
    public void PauseUIController(){
        pause = !pause;
        if(pause){
            PauseUI.SetActive(true);
            play_mode = false;
        }
        else{
            PauseUI.SetActive(false);
        }
    }
    void ActiveToolController(){
        if(create_build){
            active_tool.text = "Active Tool: Create Build";
        }
        else if(remove_build){
            active_tool.text = "Active Tool: Remove Build";
        }
        else if(buying_tile){
            active_tool.text = "Active Tool: Buying Tile";
        }
        else{
            active_tool.text = "Active Tool: None";
        }
    }
} 
