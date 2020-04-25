using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellGrid : MonoBehaviour
{
	public GameObject CellPrefab;

	public int Width = 10, Height = 10;

	public bool Playing = false;

	public GameObject PlayButton, PauseButton;

	GameObject [,] Objects;
	public bool [,] Grid;
	bool[,] BackGrid;

	void Awake ()
	{
		Refresh ();
	}

	public void InputWidth (string width)
	{
		Width = int.Parse (width);
	}

	public void InputHeight (string height)
	{
		Height = int.Parse (height);
	}

	public void Refresh ()
	{
		Playing = false;

		foreach (Transform child in transform)
			Destroy (child.gameObject);

		Objects = new GameObject [Width, Height];
		Grid = new bool [Width, Height];
		BackGrid = new bool [Width, Height];
		GetComponent<GridLayoutGroup> ().constraintCount = Width;
		for (int y = 0; y < Height; ++y)
		{
			for (int x = 0; x < Width; ++x)
			{
				GameObject cell = Instantiate (CellPrefab, transform);
				Cell cellComp = cell.GetComponent<Cell> ();
				cellComp.X = x;
				cellComp.Y = y;
				cellComp.Alive = false;
				Objects [x, y] = cell;
			}
		}
	}

	public void Play ()
	{
		Playing = true;
	}

	public void Stop ()
	{
		Playing = false;
	}

	void FixedUpdate ()
	{
		PlayButton.GetComponent<Button> ().enabled = !Playing;
		PauseButton.GetComponent<Button> ().enabled = Playing;

		if (Playing)
		{
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					bool isAlive = Grid [x, y];
					int count = 0;
					for (int yy = -1; yy <= 1; ++yy)
					{
						for(int xx = -1; xx <= 1; ++xx)
						{
							if (xx == 0 && yy == 0)
								continue;
							count += __GetValueFromGrid (Grid, x + xx, y + yy, Width, Height);
						}
					}

					if (isAlive)
					{
						BackGrid [x, y] = (count == 2 || count == 3);
					}
					else
					{
						BackGrid [x, y] = count == 3;
					}
				}
			}

			SwapBuffer ();
		}
	}

	int __GetValueFromGrid (bool [,] grid, int x, int y, int width, int height)
	{
		if (x < 0 || y < 0) return 0;
		if (x >= width || y >= height) return 0;
		return grid [x, y] ? 1 : 0;
	}

	public void SwapBuffer()
	{
		for(int y = 0; y < Height; ++y)
		{
			for(int x = 0; x < Width; ++x)
			{
				Objects [x, y].GetComponent<Cell> ().Alive = BackGrid [x, y];
			}
		}

		bool [,] temp = Grid;
		Grid = BackGrid;
		BackGrid = temp;
	}
}
