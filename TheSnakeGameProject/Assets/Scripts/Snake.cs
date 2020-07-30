using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction 
    {
        Up,
        Down,
        Right,
        Left
    }

    public Animator animator; 

    private Direction gridMoveDirection;
    private Vector2Int gridPosition;

    private float gridMoveTimer;
    private float gridMoveTimerMax;

    private LevelGrid levelGrid;

    private int snakeBodySize = 0;
    private List<SnakeMovePosition> snakeMovePositionList;
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

        gridMoveDirection = Direction.Up;  //starting point

        snakeMovePositionList = new List<SnakeMovePosition>();
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
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Vertical", -1);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
                animator.SetFloat("Horizontal", 1);
                animator.SetFloat("Vertical", 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
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

            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Up: 
                    gridMoveDirectionVector = new Vector2Int(0, +1);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(+1, 0);
                    break;
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
            }

            gridPosition += gridMoveDirectionVector;

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
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    public Vector2Int GetGridPosition() 
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeGridPositionList() 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }


    private class SnakeBodyPart 
    {
        private Transform transform;
        private SnakeMovePosition snakeMovePosition;

        public SnakeBodyPart(int bodyIndex)
        {                                    
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) 
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector2(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle = 0f;
            switch (snakeMovePosition.GetDirection()) {
            default:
                case Direction.Up:
                    angle = 0;
                    break;
                case Direction.Down:
                    angle = 180;
                    break;
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90; break;
                        case Direction.Down:
                            angle = 45; break;
                    }
                    break;
                case Direction.Left:
                    angle = -90;
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    private class SnakeMovePosition 
    {
        private SnakeMovePosition previousSnakeMoveDirection;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMoveDirection, Vector2Int gridPosition, Direction direction) 
        {
            this.previousSnakeMoveDirection = previousSnakeMoveDirection;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() 
        {
            return gridPosition;
        }

        public Direction GetDirection() 
        {
            return direction;
        }

        public Direction GetPreviousDirection() 
        {
            if (previousSnakeMoveDirection == null)
            {
                return Direction.Right;
            }else { 
                return previousSnakeMoveDirection.direction;
            }
        }
    }
}


