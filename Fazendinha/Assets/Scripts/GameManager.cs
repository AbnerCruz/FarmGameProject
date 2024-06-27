using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //References
    MyUtils utils = new MyUtils();
    [Header("References")]
    public Player player;
    public CameraMovement mainCamera;

    //Grid Variables
    GridClass principal_grid;
    [Header("Grid Config")]
    [SerializeField] int grid_width;
    [SerializeField] int grid_height;
    [SerializeField] int grid_cell_size;

    //Options
    [Header("Options")]
        //Debug Visibility
        public bool show_debugs;
        public GameObject text_grid;
    public bool play_mode; //if is running or not the playable mode
    public bool create_build; //if can or not build
    public bool remove_build; //if can or not remove build
    public bool can_colect_farms; //if can click to colect farms
    [Header("Build Options")]
    public Item build_object; //selected item to build or destroy
    public GameObject create_build_UI_button; //Button to activate build system
    public GameObject remove_create_build_UI_button;//Button to activate remove system
    float lastClickTime; //rock collect click timer

    //Materials
        //Crystals
        public GameObject crystal_material_prefab; // prefab of rocks
        public int crystal_life = 5;
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


//***UNITY METHODS***//
    void Awake(){
        principal_grid = new GridClass(grid_width, grid_height, grid_cell_size,this);
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

        //rock reward cooldown
        rock_reward_cooldown_initial = 1f;
    }
    void Update(){
        //Farm Function
        FarmGrowth();
        //Colector Function
        Collector();
        //Camera Movement
        mainCamera.Move();
        //BuildObject
        BuildObjectDefinition();
        //Rock Reward Cooldown
        rock_reward_cooldown += Time.deltaTime;
        if(rock_reward_cooldown >= rock_reward_cooldown_initial){
            rock_reward_cooldown = rock_reward_cooldown_initial;
        }
        //Debugs
        ShowDebugs();

        //Pause Mode
        if(play_mode){
            BuildSystem();
            ShopStatus();
            DoubleClick();
            Inputs();
        }
        else{ //Opening some panel
            can_colect_farms = false;
            ShopStatus();
            ShopAmountBoolController();
            if(shopping){
                ShopPanelController();
            }
        }
    }

//***MY METHODS***//
    //Build System
    void BuildSystem(){
        if(create_build || remove_build){
            can_colect_farms = false;
        }
        else{
            can_colect_farms = true;
        }
        if(Input.GetMouseButtonDown(0)){
            if(create_build){
                if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                    if(build_object != null && build_object.item_type == Item.ItemType.Build && principal_grid.ReturnCellValue(utils.GetMousePos()) == null){
                        int x,y;
                        principal_grid.GetXY(utils.GetMousePos(), out x, out y);
                        if(principal_grid.rocks_grid[x,y] >= crystal_life){
                           switch(build_object.build_type){
                                case Item.BuildType.FarmObject:
                                    principal_grid.EditGridCellValue(utils.GetMousePos(), new FarmClass((FarmObject)build_object, this));
                                    player.inventory.CreateBuildDecrease(build_object);
                                    break;
                                case Item.BuildType.ExtractingObject:
                                    principal_grid.EditGridCellValue(utils.GetMousePos(), new FarmClass((FarmObject)build_object, this));
                                    player.inventory.CreateBuildDecrease(build_object);
                                    break;
                                case Item.BuildType.CollectorObject:
                                    principal_grid.EditGridCellValue(utils.GetMousePos(), new CollectorClass(this,(CollectorObject)build_object));
                                    player.inventory.CreateBuildDecrease(build_object);
                                    break;
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
                    if(principal_grid.ReturnCellValue(utils.GetMousePos()) != null){
                        player.inventory.RemoveBuildIncrease(principal_grid.ReturnCellValue(utils.GetMousePos()).build_base_item);
                        principal_grid.EditGridCellValue(utils.GetMousePos(), null);
                    }
                }
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
            if(timeSinceLastClick <= 0.2f){
                if(can_colect_farms){
                    if(principal_grid.VerifyGridLimits(utils.GetMousePos())){
                        int x,y;
                        principal_grid.GetXY(utils.GetMousePos(), out x, out y);
                        if(principal_grid.farm_grid[x,y] != null){
                            if(principal_grid.farm_grid[x,y].farm_reward == FarmObject.FarmReward.Rocks){
                                player.player_rocks += principal_grid.farm_grid[x,y].farm_stock;
                                principal_grid.farm_grid[x,y].farm_stock = 0;
                                UpdateTextAccumulatedPoints();
                            }
                            else if(principal_grid.farm_grid[x,y].farm_reward == FarmObject.FarmReward.Energy){
                                player.player_energy += principal_grid.farm_grid[x,y].farm_stock;
                                principal_grid.farm_grid[x,y].farm_stock = 0;
                                UpdateTextAccumulatedPoints();
                            }
                        }
                        //World Click Rocks Reward
                        else if(principal_grid.rocks_grid[x,y] < crystal_life && principal_grid.base_grid[x,y] == 0 && can_colect_farms){
                            //Visual Function here function();
                            if(rock_reward_cooldown >= rock_reward_cooldown_initial){
                                int chance = 50; // % of chance
                                int sorted = Random.Range(1,100);
                                if(sorted > 100-chance){
                                    int reward = Random.Range(2,50);
                                    for(int i = 0; i < Mathf.FloorToInt(reward/2); i++){
                                        GameObject newRock = Instantiate(crystal_material_prefab, utils.GetMousePos(), Quaternion.identity);
                                        newRock.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-3f,3f),Random.Range(1f,4f),0);
                                    }
                                    player.player_rocks += reward;
                                    principal_grid.rocks_grid[x,y] += 1;
                                    rock_reward_cooldown = 0f;
                                    Color current_color = principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().color;
                                    principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().color = new Color(current_color.r, current_color.g,current_color.b,current_color.a - 0.2f);
                                }
                            }
                        }
                        if(principal_grid.rocks_grid[x,y] >= crystal_life){
                            principal_grid.tile_grid[x,y].GetComponent<SpriteRenderer>().sprite = null;
                        }
                    }
                    else{
                        player.player_rocks++;
                    }
                }
            }
            lastClickTime = Time.time;
        }
    }

    //UI Build System Control
    public void CreateBuildBoolControl(){
        if(remove_build){
            remove_build = false;
        }
        create_build =! create_build;
        if(create_build){
            remove_create_build_UI_button.GetComponent<Image>().color = new Color(255,0,0,0.2f);
            create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,1f); 
        }
        else{
            create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,0.2f);
        }
    }
    public void RemoveBuildBoolControl(){
        if(create_build){
            create_build = false;
            create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,0.2f);  
        }
        remove_build =! remove_build;
        if(remove_build){
            remove_create_build_UI_button.GetComponent<Image>().color = new Color(255,0,0,1f);
            create_build_UI_button.GetComponent<Image>().color = new Color(0,255,0,0.2f); 
        }
        else{
            remove_create_build_UI_button.GetComponent<Image>().color = new Color(255,0,0,0.2f);
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
            else{
                //ESC FUNCTION HERE
            }
        }
    }
    //Add a new Item to Inventory
    void NewItemToInventory(Item item, int quantity){
        if(item != null){
            player.inventory.BuyFunction(item, quantity);

        }
    }
    void NewSell(Item item, int quantity){
        if(item != null){
            switch(item.item_type){
                case Item.ItemType.Build:
                    player.inventory.SellFunction(item, quantity);
                    break;
                case Item.ItemType.Material:
                    switch(item.material_type){
                        case Item.MaterialType.Crystal:
                            if(player.player_rocks >= quantity){
                                player.player_rocks -= quantity;
                                player.player_money += item.item_price * quantity;
                            }
                            break;
                        case Item.MaterialType.Energy:
                            if(player.player_energy >= quantity){
                                player.player_energy -= quantity;
                                player.player_money += item.item_price * quantity;
                            }
                            break;
                    }
                    break;

            }
        }
    }
    //Buying Function
    public void Buying(int quantity){
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
            case Item.PriceType.Crystal:
                if(player.player_rocks >= item_to_shop.item_price * quantity){
                    //BUYED
                    player.player_rocks -= item_to_shop.item_price * quantity;
                    NewItemToInventory(item_to_shop, quantity);
                }
                else{
                    //NO ROCKS TO BUY FUNCTIONS
                }
                break;
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
        if(hot_bar.GetComponent<HotBarInventoryScript>().player_inventory_slots[hotbar_num-1] != null){
            if(hot_bar.GetComponent<HotBarInventoryScript>().player_inventory_slots[hotbar_num-1].item_type == Item.ItemType.Build){
                if(build_object != hot_bar.GetComponent<HotBarInventoryScript>().player_inventory_slots[hotbar_num-1]){
                    build_object = hot_bar.GetComponent<HotBarInventoryScript>().player_inventory_slots[hotbar_num-1];
                }
            }
        }
        else{
            build_object = null;
        }
    }
    //This shit control if buy or sell amount selection is active or not
    void ShopAmountBoolController(){
        shop_amount_screen.SetActive(shop_amount_bool);
        shop_amount_screen.GetComponent<ShopAmountQuantityScript>().selected_object = item_to_shop;
    }
    //button function to open amount selection to buy
    public void SelectAmountBuyingFunction(){
        if(!(item_to_shop.item_type == Item.ItemType.Material)){
            switch(item_to_shop.build_type){
                case Item.BuildType.FarmObject:
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
    }
    public void SelectAmountSellingFunction(){
        if(item_to_shop.item_type != Item.ItemType.Material){
            foreach(Item i in player.inventory.item_slots){
                if(i == item_to_shop){
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
            switch(item_to_shop.price_type){
                case Item.PriceType.Crystal:
                    current_panel.selected_item_price_icon.GetComponent<Image>().sprite = crystal_material_prefab.GetComponent<SpriteRenderer>().sprite;//Crystal
                    break;
                case Item.PriceType.Money:
                    current_panel.selected_item_price_icon.GetComponent<Image>().sprite = money_material_prefab.GetComponent<SpriteRenderer>().sprite;//Money
                    break;
            }
        }
    }
} 
