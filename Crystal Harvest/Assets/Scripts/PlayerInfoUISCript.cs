using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUISCript : MonoBehaviour
{
    GameManager manager;
    public Text player_nickname;
    public Text player_money;
    public Text player_rocks;
    public Text player_crystals;
    public Text player_energy;
    public Text player_tiles;


    void Awake(){
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Start(){
        player_nickname.text = manager.player.nickname;
    }
    void Update(){
        player_nickname.text = manager.player.nickname;
        player_money.text = manager.player.player_money.ToString();
        player_rocks.text = manager.player.player_rocks.ToString();
        player_crystals.text = manager.player.player_crystals.ToString();
        player_energy.text = manager.player.player_energy.ToString();
        player_tiles.text = manager.player.player_tiles.ToString();
    }
}
