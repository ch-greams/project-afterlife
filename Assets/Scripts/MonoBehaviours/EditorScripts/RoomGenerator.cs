using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour
{
#if UNITY_EDITOR

	public Point gridSize;
	public float cellSize = 1;
	public GameObject cellPrefab;
    public List<Cell> cells = new List<Cell>();


    public void CreateGrid()
	{
		this.transform.position = new Vector3();
		DestroyGrid();

		Debug.Log(string.Format("Generating Grid with size {0}", gridSize));

		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				Point point = new Point(x, y);
				GameObject cell = Instantiate(cellPrefab, new Vector3(x * cellSize, 0, y * cellSize), Quaternion.identity);

				cell.transform.SetParent(this.transform);
				cell.name = point.ToString();

				cells.Add(new Cell(point, cell));
			}
		}
	}

	public void CleanUpGrid() {
		cells.RemoveAll(c => c.obj == null);
	}

	private void DestroyGrid() {
		foreach (Cell cell in cells)
		{
			Destroy(cell.obj);
		}
		cells = new List<Cell>();
	}

#endif
}

