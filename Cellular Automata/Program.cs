using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CA
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            CellularAutomata CA = new CellularAutomata(15, 15);
        }
    }
    //Basic Grid Tile
    public class Node
    {
        public string image;
        public string[] values = { " 0 ", " 1 " };
        public int yPos;
        public int xPos;

        public Node(int _input, int _x, int _y) //C# excuse for a constructor
        {
            image = values[_input];
            yPos = _y;
            xPos = _x;
        }
    }

    public class CellularAutomata 
    {
        public Node[,] grid; //The grid
        int gridSizeX, gridSizeY; //Grid dimensions

        public CellularAutomata(int _gridSizeX, int _gridSizeY)
        {
            gridSizeX = _gridSizeX;
            gridSizeY = _gridSizeY;
            int numGos = 0;//Setting the initial value for the while loop
            //Populating the grid with dead nodes
            grid = new Node[gridSizeX, gridSizeY];
            for(int i = 0; i < _gridSizeX; i++)
            {
                for(int j = 0; j < _gridSizeY; j++)
                {
                    grid[i, j] = new Node(0, i, j);
                }
            }
            PrintGrid();
            RandomSeeding();
            PrintGrid();
            //This is where the cellular automata actually happens
            do
            {
                DetermineNewCellState();
                PrintGrid();
                numGos++;
            } while (numGos < 5);
        }

        //Randomly seeds alive nodes in the grid;
        public void RandomSeeding()
        {
            Random random = new Random();
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    if(random.Next(2) == 1)
                    {
                        grid[i, j].image = " 1 ";
                    }
                }
            }
        }

        //Prints out the grid
        public void PrintGrid()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    sb.Append(grid[i, j].image);
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }

        //Actual CA bit
        public void DetermineNewCellState()
        {
            Node[,] newGrid = grid;

            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    //This is a LINQ way of determining the number of same values in a generic list
                    //Need to find other way of doing this in other languages.
                    //Puts the values in a dictionary, with the key being the duplicate value
                    //And the value the number of duplicates
                    var query = GetNeighbourValues(grid[i, j]).GroupBy(x => x)
                                .Where(g => g.Count() > 1)
                                .ToDictionary(x => x.Key, y => y.Count());

                    if (query.ContainsKey(1))//this makes sure the node has alive nodes surrounding it otherwise you'll get an error from the dictionary
                    {
                        //Implements the rules of conway's game of life found here: https://natureofcode.com/book/chapter-7-cellular-automata/
                        if (grid[i, j].image == " 0 " && query[1] == 3)
                            newGrid[i, j].image = " 1 ";
                        if (grid[i, j].image == " 1 ")
                        {
                            if (query[1] >= 4 || query[1] <= 1)
                                newGrid[i, j].image = " 0 ";
                        }
                    }
                    else
                        newGrid[i, j].image = " 0 ";
                }
            }
            grid = newGrid;
        }

        //Given a node, get the 8 values of the neighbours surrounding it.
        public List<int> GetNeighbourValues(Node node)
        {
            List<int> neighbours = new List<int>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.xPos + x;
                    int checkY = node.yPos + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(Int32.Parse(grid[checkX, checkY].image));
                    }
                }
            }
            return neighbours;
        }
    }
}