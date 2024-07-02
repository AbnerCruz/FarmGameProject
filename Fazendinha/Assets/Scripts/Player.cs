using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameManager manager;
    public string nickname;
    public float player_money;
    public float player_energy;
    public float player_rocks;
    public float player_crystals;
    public int player_tiles;
    public float reward_initial_timer;
    public float reward_timer;
    public PlayerInventory inventory;



    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        inventory = GetComponent<PlayerInventory>();
        manager.player = this;
        player_tiles = 1;
    }
}
