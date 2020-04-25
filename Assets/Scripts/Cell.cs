using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int X;
    public int Y;
    public bool Alive;

    CellGrid cellGrid;
    Image image;

    void Awake ()
    {
        cellGrid = transform.parent.GetComponent<CellGrid> ();
        image = GetComponent<Image> ();
    }

    void Update ()
    {
        image.color = Alive ? Color.white : new Color (0.2f, 0.2f, 0.2f);
    }

    public void Toggle()
    {
        if (cellGrid.Playing)
            return;
        Alive = !Alive;
        cellGrid.Grid [X, Y] = Alive;
    }
}
