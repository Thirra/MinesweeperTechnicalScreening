using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData : MonoBehaviour
{
    //The specific cell type of the button
    public Cell cellType;
    //Needed for button pressing
    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;
}
