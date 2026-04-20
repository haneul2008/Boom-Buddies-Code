using System.Collections.Generic;
using UnityEngine;

namespace Code.Levels
{
    public class AstarCalculator
    {
        public enum NodeType
        {
            Empty,
            Wall,
            Tower
        }

        public class Node
        {
            public Vector2Int Position;
            public float GCost;
            public float HCost;
            public float FCost => GCost + HCost;
            public Node Parent;
            public NodeType Type;
            public bool IsClosed;

            public Node(Vector2Int pos, NodeType type)
            {
                Position = pos;
                Type = type;
                Reset();
            }

            public void Reset()
            {
                GCost = float.MaxValue;
                HCost = 0;
                Parent = null;
                IsClosed = false;
            }
        }

        private readonly int _width;
        private readonly int _height;
        private readonly Node[,] _grid;

        private readonly Vector2Int[] _directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        private const float WallCost = 5f;
        private const float EmptyCost = 1f;
        private const int GridScale = 2;
        private int _placeableRange;

        public AstarCalculator(int width, int height, Dictionary<Vector2Int, NodeType> mapData)
        {
            _width = width;
            _height = height;
            _grid = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    mapData.TryGetValue(pos, out var type);
                    _grid[x, y] = new Node(pos, type);
                }
            }
        }

        public Vector2Int? GetOptimalWall(Vector2Int pos, Vector2Int target)
        {
            if (!InBounds(pos) || !InBounds(target)) return null;

            ResetAllNodes();

            Node startNode = _grid[pos.x, pos.y];
            Node goalNode = _grid[target.x, target.y];
            Debug.Log(goalNode.Type);

            startNode.GCost = 0;
            startNode.HCost = Heuristic(startNode.Position, target);

            List<Node> openList = new List<Node> { startNode };

            while (openList.Count > 0)
            {
                openList.Sort((a, b) => a.FCost.CompareTo(b.FCost));
                Node current = openList[0];
                openList.RemoveAt(0);
                current.IsClosed = true;

                if (current == goalNode)
                {
                    return ExtractFirstWallInPath(current);
                }

                foreach (Vector2Int dir in _directions)
                {
                    Vector2Int neighborPos = current.Position + dir;
                    if (!InBounds(neighborPos)) continue;

                    Node neighbor = _grid[neighborPos.x, neighborPos.y];
                    if (neighbor.IsClosed) continue;

                    float moveCost = neighbor.Type == NodeType.Wall ? WallCost : EmptyCost;
                    float newG = current.GCost + moveCost;

                    if (newG < neighbor.GCost)
                    {
                        neighbor.GCost = newG;
                        neighbor.HCost = Heuristic(neighbor.Position, target);
                        neighbor.Parent = current;

                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }
            }

            return null;
        }

        public void SetNodeType(int x, int y, NodeType nodeType)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (InBounds(pos))
            {
                _grid[x, y] = new Node(pos, nodeType);
            }
        }

        private Vector2Int? ExtractFirstWallInPath(Node endNode)
        {
            List<Node> wallNodes = new List<Node>();
            Node current = endNode;

            while (current != null)
            {
                if (current.Type == NodeType.Wall)
                {
                    wallNodes.Add(current);
                }

                current = current.Parent;
            }

            if (wallNodes.Count == 0)
                return null;

            Node outermostWall = wallNodes[0];
            foreach (var wall in wallNodes)
            {
                if (wall.GCost < outermostWall.GCost)
                {
                    outermostWall = wall;
                }
            }

            return outermostWall.Position * 2 + Vector2Int.one;
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private bool InBounds(Vector2Int pos)
        {
            return pos.x >= -_placeableRange && pos.x < _width + _placeableRange
                                             && pos.y >= -_placeableRange && pos.y < _height + _placeableRange;
        }

        private void ResetAllNodes()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y].Reset();
                }
            }
        }
    }
}