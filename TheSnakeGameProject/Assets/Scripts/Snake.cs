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

    public void Setup(LevelGrid levelGrid) 
    {
        this.levelGrid = levelGrid;  
    }

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);

        gridMoveTimerMax = .5f;
        gridMoveTimer = gridMoveTimerMax;

        gridDirection = new Vector2Int(0, 1);
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
            gridPosition += gridDirection;
            gridMoveTimer -= gridMoveTimerMax;

            transform.position = new Vector2(gridPosition.x, gridPosition.y);  //moving

            levelGrid.SnakeMoved(gridPosition);     //food and snake at a same position
        }       
    }

    public Vector2Int GetGridPosition() 
    {
        return gridPosition;
    }
}
