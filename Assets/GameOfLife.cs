using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour {
    public Sprite Tile;
    public int Width;
    public int Height;    

    //RULES:
    //If cell is alive and neighbours >= 4 cell = dead
    //If cell is alive and neighbours <= 1 cell = dead
    //If cell is dead and neighbours == 3 cell = alive
    
    private void Start() {
        BuildGrid();
        InvokeRepeating(nameof(DrawGrid), 0, 0.2f);
        InvokeRepeating(nameof(UpdateStates), 0, 0.2f);
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(NewGrid());
        }
    }

    public enum State {
        Alive,
        Dead    
    }

    public class Cell {
        public State State;
        public readonly int X;
        public readonly int Y;
        public GameObject Gobject;

        public Cell(int x, int y, State state, GameObject gobject) {
            X = x;
            Y = y;
            State = state;
            Gobject = gobject;
        }
    }

    private Cell GetCell(int x, int y) {
        return Cells["X:" + x + "Y:" + y];
    }

    public Dictionary<String, Cell> Cells = new Dictionary<string, Cell>();

    public int GetNeighbors(Cell cell) {
        int x = cell.X;
        int y = cell.Y;
        int value = 0;

        for (var j = -1; j <= 1; j++) {
            if (y + j < 0 || y + j >= Height) {
                continue;
            }

            int k = (y + j + Height) % Height;

            for (var i = -1; i <= 1; i++) {
                if (x + i < 0 || x + i >= Width) {
                    continue;
                }

                int h = (x + i + Width) % Width;

                if (GetCell(h, k).State == State.Alive) {
                    value++;
                }
            }
        }

        if (cell.State == State.Alive) {
            value--;
        }

        return value;
    }

    public void BuildGrid() {
        System.Random random = new System.Random();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                GameObject gobject = new GameObject("X:" + x + "Y:" + y);
                gobject.AddComponent<SpriteRenderer>();
                int randomnumber = random.Next(-1, 3);
                string key = "X:" + x + "Y:" + y;
                if (randomnumber == 1) {
                    Cell cell = new Cell(x, y, State.Alive, gobject);
                    Cells.Add(key, cell);
                }
                else {
                    Cell cell = new Cell(x, y, State.Dead, gobject);
                    Cells.Add(key, cell);
                }
                Instantiate(gobject);
            }
        }
    }
    public IEnumerator NewGrid() {
        System.Random random = new System.Random();
        foreach (var cell in Cells.Values) {
            Destroy(cell.Gobject);
        }
        Cells.Clear();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                GameObject gobject = new GameObject("X:" + x + "Y:" + y);
                gobject.AddComponent<SpriteRenderer>();
                int randomnumber = random.Next(-1, 3);
                string key = "X:" + x + "Y:" + y;
                if (randomnumber == 1) {
                    Cell cell = new Cell(x, y, State.Alive, gobject);
                    Cells.Add(key, cell);
                }
                else {
                    Cell cell = new Cell(x, y, State.Dead, gobject);
                    Cells.Add(key, cell);
                }
                Instantiate(gobject);
            }
        }
        yield return new WaitForSeconds(2);
    }

    public void DrawGrid() {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                Cell cell = GetCell(x, y);
                var s = cell.Gobject.GetComponent<SpriteRenderer>();
                cell.Gobject.transform.position = new Vector2(x, y);
                if (GetCell(x, y).State == State.Alive) {
                    s.color = Color.white;
                }
                else {
                    s.color = Color.black;
                }

                s.sprite = Tile;
            }
        }
    }
    
    public void UpdateStates() {
        Dictionary<string, Cell> newCells = new Dictionary<string, Cell>();
        foreach (Cell cell in Cells.Values) {
            Cell newCell = new Cell(cell.X, cell.Y, cell.State, cell.Gobject);
            if (cell.State == State.Alive) {
                if (GetNeighbors(cell) < 2 || GetNeighbors(cell) > 3) {
                    newCell.State = State.Dead;
                }
            }
            else {
                if (GetNeighbors(cell) == 3) {
                    newCell.State = State.Alive;
                }
            }

            newCells.Add("X:" + cell.X + "Y:" + cell.Y ,newCell);
        }

        Cells = newCells;
    }
        
}