using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Animator animator; 

    private Vector2Int gridDirection;
    private Vector2Int gridPosition;

    private float gridMoveTimer;
    private float gridMoveTimerMax;

    private LevelGrid levelGrid;

    private int snakeBodySize = 0;
    private List<Vector2Int> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    public void Setup(LevelGrid levelGrid) 
    {
        this.levelGrid = levelGrid;  
    }

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);

        gridMoveTimerMax = .5f;
        gridMoveTimer = gridMoveTimerMax;

        gridDirection = new Vector2Int(0, 1);   //starting point

        snakeMovePositionList = new List<Vector2Int>();
        snakeBodyPartList = new List<SnakeBodyPart>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleGridMovement();
    }

    private void HandleInput() 
    {
        //getkeys to move
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridDirection.y != -1)
            {
                gridDirection = new Vector2Int(0, 1);
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridDirection.y != 1)
            {
                gridDirection = new Vector2Int(0, -1);
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", -1);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridDirection.x != -1)
            {
                gridDirection = new Vector2Int(1, 0);
                animator.SetFloat("Horizontal", 1);
                animator.SetFloat("Vertical", 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridDirection.x != 1)
            {
                gridDirection = new Vector2Int(-1, 0);
                animator.SetFloat("Horizontal", -1);
                animator.SetFloat("Vertical", 0);
            }
        }
    }

    private void HandleGridMovement() 
    {
        //timer to move
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            snakeMovePositionList.Insert(0, gridPosition);

            gridPosition += gridDirection;

            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);     //food and snake at a same position
            if (snakeAteFood)
            {
                snakeBodySize++;        //growing by food
                CreateSnakeBodyPart();
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            transform.position = new Vector2(gridPosition.x, gridPosition.y);  //moving 

            UpdateSnakeBodyPart();
        }       
    }

    private void CreateSnakeBodyPart() 
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyPart()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
        }
    }

    public Vector2Int GetGridPosition() 
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeGridPositionList() 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(snakeMovePositionList);
        return gridPositionList;
    }

    private class SnakeBodyPart 
    {
        private Transform transform;
        private Vector2Int gridPosition;

        public SnakeBodyPart(int bodyIndex)
        {                                    
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetGridPosition(Vector2Int gridPosition) 
        {
            this.gridPosition = gridPosition;
            transform.position = new Vector2(gridPosition.x, gridPosition.y);
        }
    }
}


