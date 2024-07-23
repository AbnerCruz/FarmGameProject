using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    float speed = 10f;
    public void Move(int grid_width, int grid_height,int cellsize){
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 newPosition = transform.position + new Vector3(move.x, move.y, 0) * speed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, 0, grid_width*cellsize);
        newPosition.y = Mathf.Clamp(newPosition.y, 0, grid_height*cellsize);

        transform.position = newPosition;
    }
}
