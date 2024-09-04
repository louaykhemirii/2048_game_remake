using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    [SerializeField] GameObject YouWin;
    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    private Vector2 touchStartPosition;
    private bool isSwipe;
    private float minimumSwipeDistance = 3f; // Adjust this value to control the minimum swipe distance
    private float swipeThreshold = 50f;
    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }






    private void Update()
    {
        if (!waiting)
        {
            // Check for swipe input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPosition = touch.position;
                 
                    isSwipe = true;
                }
                else if (touch.phase == TouchPhase.Moved && isSwipe)
                {
                    Vector2 swipeDelta = touch.position - touchStartPosition;

                    // Check if the magnitude of swipeDelta is greater than the minimumSwipeDistance
                    if (swipeDelta.magnitude > minimumSwipeDistance)
                    {
                        Debug.Log("supérieur au min");
                        // Check for horizontal swipe
                        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                        {
                            if (swipeDelta.x > 0)
                            {
                                Move(Vector2Int.right, grid.width - 2, -1, 0, 1); // Swipe right
                                Debug.Log("swipe right");
                            }
                            else
                            {
                                Move(Vector2Int.left, 1, 1, 0, 1); // Swipe left
                                Debug.Log("swipe left");
                            }
                        }
                        // Check for vertical swipe
                        else if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x) )
                        {
                            Debug.Log("vertical swipe");
                            if (swipeDelta.y > 0)
                            {
                                Move(Vector2Int.up, 0, 1, 1, 1); // Swipe up
                                Debug.Log("swipe up");
                            }
                            else
                            {
                                Move(Vector2Int.down, 0, 1, grid.height - 2, -1); // Swipe down
                                Debug.Log("swipe down");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("inférieur au min");
                    }

                    isSwipe = false;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (isSwipe)
                    {
                       
                        isSwipe = false;
                    }
                }
            }
        }
    }




    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
       

        gameManager.IncreaseScore(newState.number);
        if (newState.number >= 2048)
        {
            YouWin.SetActive(true);
        }
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.size) {
            CreateTile();
        }

        if (CheckForGameOver()) {
            gameManager.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

}
