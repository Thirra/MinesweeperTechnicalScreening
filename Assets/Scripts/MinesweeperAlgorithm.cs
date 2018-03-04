using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Numbered for clarity
public enum Cell
{
    EMPTY = 0,
    M1 = 1,
    M2 = 2,
    M3 = 3,
    M4 = 4,
    M5 = 5,
    M6 = 6,
    M7 = 7,
    M8 = 8,
    M9 = 9,
    MINE = 10,
    CLOSED = 11
}

public class MinesweeperAlgorithm : MonoBehaviour
{
    public GameObject prefabButtons;
    public GameObject minesweeperPanel;

    //Length and height should equal the same value for a minesweeper grid
    public int gridWidth;
    public int gridHeight;
    private GameObject[,] grid;
    private List<GameObject> mines = new List<GameObject>();
    List<GameObject> tilesSurrounding = new List<GameObject>();

    //The number of mines in the 
    public int totalMines;

    // Use this for initialization
    void Start ()
    {
        GenerateMineField(gridWidth, gridHeight, totalMines);
	}
	
    //Generating the field - solution always visible
    public void GenerateMineField(int width, int height, int totalMines)
    {
        grid = new GameObject[width, height];
        //Generating the grid
        RectTransform rtPanel = (RectTransform)minesweeperPanel.transform;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // --BUTTON CREATION-- //
                GameObject newButton = Instantiate(prefabButtons, minesweeperPanel.transform) as GameObject;

                RectTransform rtButton = (RectTransform)newButton.transform;
                newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(rtPanel.rect.width / width, rtPanel.rect.width / width);
                float startingPos = minesweeperPanel.transform.localPosition.x - (rtPanel.rect.width / 2) + (rtButton.rect.width / 2);

                float buttonXPosition = startingPos + (rtPanel.rect.width / width * x);
                float buttonYPosition = startingPos + (rtPanel.rect.height / height * y);
                newButton.transform.localPosition = new Vector3(buttonXPosition, buttonYPosition, newButton.transform.position.z);
                //         --         //

                //Grid reference
                grid[x, y] = newButton;
                grid[x, y].GetComponent<CellData>().cellType = Cell.EMPTY;
                grid[x, y].GetComponent<CellData>().x = x;
                grid[x, y].GetComponent<CellData>().y = y;

                Button button = newButton.GetComponent<Button>();
                button.onClick.AddListener(delegate { TaskOnClick(newButton); });
            }
        }

        //Creating the mines
        for (int index = 0; index < totalMines; index++)
        {
            int x = Random.Range(0, width-1);
            int y = Random.Range(0, height-1);

            GameObject mine = grid[x, y];
            mine.GetComponent<CellData>().cellType = Cell.MINE;
            grid[x, y].GetComponentInChildren<Text>().text = "MINE";

            tilesSurrounding.Add(grid[x, y < height? y + 1 : y]);     //up
            tilesSurrounding.Add(grid[x, y > 0 ? y - 1 : y]);   //down
            tilesSurrounding.Add(grid[(x > 0 ? x - 1 : x), y]);     //left
            tilesSurrounding.Add(grid[(x < width ? x + 1 : x), y]);     //right
            tilesSurrounding.Add(grid[x > 0 ? x - 1 : x, y < height ? y + 1 : y]);  //topLeft
            tilesSurrounding.Add(grid[x < width ? x + 1 : x, y < height ? y + 1 : y]);  //topRight
            tilesSurrounding.Add(grid[x > 0 ? x - 1 : x, y > 0 ? y - 1 : y]);   //bottomLeft
            tilesSurrounding.Add(grid[x < width? x + 1 : x, y > 0 ? y - 1 : y]);     //bottomRight

            for (int index2 = 0; index2 < tilesSurrounding.Count; index2++)
            {
                if (tilesSurrounding[index2].GetComponent<CellData>().cellType != Cell.MINE)
                {
                    AddEnumNumber(tilesSurrounding[index2]);
                }
            }
            tilesSurrounding.Clear();
        }
    }
    
    //Checking for enum value on the button to make it a little bit neater
    public void AddEnumNumber(GameObject button)
    {
        int enumToAdd = (int)button.GetComponent<CellData>().cellType;
        enumToAdd += 1;
        button.GetComponent<CellData>().cellType = (Cell)enumToAdd;
        button.GetComponentInChildren<Text>().text = button.GetComponent<CellData>().cellType.ToString();
    }

    //Checking if the player has clicked a button, performing the appropriate functions
    void TaskOnClick(GameObject button)
    {
        Cell cellType = button.GetComponent<CellData>().cellType;

        if (cellType == Cell.MINE)
        {
            Debug.Log("GAME OVER");
        }
        else if (cellType == Cell.EMPTY)
        {
            GridUncover(button.GetComponent<CellData>().x, button.GetComponent<CellData>().y);
        }
        else
        {
            CloseSpecificCell(button);
        }
    }

    //Uncovering a specific cell - for M1 - M9
    public void CloseSpecificCell(GameObject button)
    {
        button.GetComponent<CellData>().cellType = Cell.CLOSED;
        button.GetComponent<Button>().interactable = false;
        button.GetComponent<Image>().color = Color.gray;
    }

    //Uncovering a whole chunch of the cells for empty cells
    public void GridUncover(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            Debug.Log("in bounds");
            if (grid[x, y].GetComponent<CellData>().cellType == Cell.CLOSED || grid[x, y].GetComponent<CellData>().cellType == Cell.MINE)
                return;

            if (grid[x, y].GetComponent<CellData>().cellType != Cell.EMPTY)
            {
                CloseSpecificCell(grid[x, y]);
                return;
            }

            CloseSpecificCell(grid[x, y]);

            GridUncover(x + 1, y);
            GridUncover(x - 1, y);
            GridUncover(x, y + 1);
            GridUncover(x, y - 1);
            GridUncover(x + 1, y + 1);
            GridUncover(x - 1, y + 1);
            GridUncover(x + 1, y - 1);
            GridUncover(x - 1, y + 1);
        }
    }
}

