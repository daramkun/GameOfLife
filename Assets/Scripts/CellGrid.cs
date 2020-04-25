using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellGrid : MonoBehaviour
{
	public GameObject CellPrefab;

	public int Width = 10, Height = 10;

	public bool Playing = false;

	public GameObject PlayButton, PauseButton, ConnectToggle;

	GameObject [,] Objects;
	public bool [,] Grid;
	bool[,] BackGrid;

	IEnumerator CurrentCoroutine;

	void Awake ()
	{
		Canvas.GetDefaultCanvasMaterial ().enableInstancing = true;

		Refresh ();
		Stop ();
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
		Stop ();

		foreach (Transform child in transform)
			Destroy (child.gameObject);

		Objects = new GameObject [Width, Height];
		Grid = new bool [Width, Height];
		BackGrid = new bool [Width, Height];

		var gridLayoutGroup = GetComponent<GridLayoutGroup> ();
		gridLayoutGroup.constraintCount = Width;
		var rectTransform = GetComponent<RectTransform> ();
		rectTransform.sizeDelta = new Vector2 (Width * gridLayoutGroup.cellSize.x, Height * gridLayoutGroup.cellSize.y);

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
		PlayButton.GetComponent<Button> ().enabled = false;
		PauseButton.GetComponent<Button> ().enabled = true;

		StartCoroutine (CurrentCoroutine = UpdateRoutine ());
	}

	private IEnumerator UpdateRoutine ()
	{
		Toggle connect = ConnectToggle.GetComponent<Toggle> ();

		while (true)
		{
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					bool isAlive = Grid [x, y];
					int count = 0;
					for (int yy = -1; yy <= 1; ++yy)
					{
						for (int xx = -1; xx <= 1; ++xx)
						{
							if (xx == 0 && yy == 0)
								continue;
							count += __GetValueFromGrid (Grid, x + xx, y + yy, Width, Height, connect.isOn);
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

			yield return new WaitForSeconds (0.125f);
		}
	}

	public void Stop ()
	{
		if (CurrentCoroutine != null)
			StopCoroutine (CurrentCoroutine);
		Playing = false;
		PlayButton.GetComponent<Button> ().enabled = true;
		PauseButton.GetComponent<Button> ().enabled = false;
	}

	int __GetValueFromGrid (bool [,] grid, int x, int y, int width, int height, bool connect)
	{
		if (connect)
		{
			if (x < 0) x += width;
			if (y < 0) y += height;
			if (x >= width) x -= width;
			if (y >= height) y -= height;
		}
		else
		{
			if (x < 0 || y < 0 || x >= width || y >= height)
				return 0;
		}
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
