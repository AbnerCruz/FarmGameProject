using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtils{
    public TextMesh CreateText(string _name, string parent,string text, Vector2 position, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder){
        GameObject newText = new GameObject(_name, typeof(TextMesh));
        newText.transform.SetParent(GameObject.Find(parent).transform,false);
        newText.transform.localPosition = position;

        TextMesh textMesh = newText.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        return textMesh;
    }
    public SpriteRenderer CreateSprite(string _name,string parent, Sprite sprite, Vector2 position, Vector2 size, int sortingOrder){
        GameObject spriteObject = new GameObject(_name, typeof(SpriteRenderer));
        spriteObject.transform.SetParent(GameObject.Find(parent).transform,false);
        spriteObject.transform.localPosition = position;
        spriteObject.transform.localScale = size;


        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        return spriteRenderer;
    }

    public Vector2 GetMousePos(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;
    }
    public float MyTimer(float initial_timer,float timer){
        timer -= Time.deltaTime;
        if(timer <= 0){
            timer = 0;
        }
        return timer;
    }
}
