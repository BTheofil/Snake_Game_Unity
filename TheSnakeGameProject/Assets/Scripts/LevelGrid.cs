using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid 
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;

    private Snake snake;

    public void Setup(Snake snake) 
    {
        this.snake = snake;

        SpawnFood();
    }

    public LevelGrid(int width, int height) 
    {
        this.width = width;
        this.height = height;       
    }

    private void SpawnFood()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.GetGridPosition() == foodGridPosition);

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));                            //create
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;      //visualize
        foodGameObject.transform.position = new Vector2(foodGridPosition.x, foodGridPosition.y);    //placing
    }

    public void SnakeMoved(Vector2Int snakeGridPosition) 
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
        }
    }
}
