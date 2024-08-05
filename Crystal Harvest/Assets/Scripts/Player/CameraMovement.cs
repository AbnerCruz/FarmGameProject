using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameManager manager;
    float speed = 15f;
    public float zoomSpeed = 20f; //Velocidade do zoom
    public float minZoom; //Zoom mínimo
    public float maxZoom; //Zoom máximo

    public void CameraCentralize(){
        float centerX = (manager.grid_width - 1) * manager.grid_cell_size / 2f;
        float centerY = (manager.grid_height - 1) * manager.grid_cell_size / 2f;
        transform.position = new Vector3(centerX + manager.grid_cell_size / 2f, centerY + manager.grid_cell_size / 2f, -10f);
    }
    public void Move(int grid_width, int grid_height,int cellsize){
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 newPosition = transform.position + new Vector3(move.x, move.y, 0) * speed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, grid_width*cellsize);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, grid_height*cellsize);

        transform.position = newPosition;
    }

    public void CameraZoom(){
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel"); //Pega o valor do scroll do mouse
        GetComponent<Camera>().orthographicSize -= scrollData * zoomSpeed; //Ajusta o tamanho do campo de visão da câmera baseado no scroll
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize, minZoom, maxZoom); //Limita o zoom dentro dos valores mínimos e máximos
    }



}
