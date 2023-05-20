using System.Collections.Generic;
using Game;
using UnityEngine;

namespace AStar
{
    internal class Node
    {
        public int x;
        public int y;
        public Node parent;
        public bool isObs = false;
        public float G;
        public float H;
        public float F => G + H;

        public Node(int x, int y, Node parent = null)
        {
            this.x = x;
            this.y = y;
            this.parent = parent;
        }
    }
    
    public class AStarMgr
    {
        private static AStarMgr instance;

        public static AStarMgr Instance => instance ??= new AStarMgr();

        private Node _start;
        private Node _fin;

        private List<Node> _open;
        private List<Node> _close;
        private Node[,] _map;

        public Vector2Int[] GetPath(MapGrid[,] map, out Vector2Int[] openList, out Vector2Int[] closeList)
        {
            InitMap(map);
            var node = SearchPath();
            Stack<Vector2Int> path = new Stack<Vector2Int>();
            while (node != null)
            {
                path.Push(new Vector2Int(node.x, node.y));
                node = node.parent;
            }

            Vector2Int[] res = new Vector2Int[path.Count];
            int index = 0;
            while (path.Count > 0)
            {
                res[index++] = path.Pop();
            }

            openList = new Vector2Int[_open.Count];
            closeList = new Vector2Int[_close.Count];
            for (int i = 0; i < _open.Count; i++)
                openList[i] = new Vector2Int(_open[i].x, _open[i].y);
            for (int i = 0; i < _close.Count; i++)
                closeList[i] = new Vector2Int(_close[i].x, _close[i].y);

            return res;
        }

        //Init Grid Map方法，可添加重载
        private void InitMap(MapGrid[,] map)
        {
            //InitGrid
            _open = new List<Node>();
            _close = new List<Node>();
            _map = new Node[map.GetLength(0), map.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Node grid = new Node(i, j);
                    _map[i, j] = grid;
                    if (map[i, j] == NavMeshPanel.start)
                        _start = grid;
                    else if (map[i, j] == NavMeshPanel.fin)
                        _fin = grid;
                    else if (map[i, j].myType == E_GridType.Obs)
                        grid.isObs = true;
                }
            }
        }

        //AStar算法主体
        private Node SearchPath()
        {
            //Search
            _start.G = 0;
            _start.H = CalcDistance(_start, _fin);
            _open.Add(_start);

            while (_open.Count > 0)
            {
                //拿出最短路径点
                _open.Sort((x, y) => x.F.CompareTo(y.F));
                var node = _open[0];
                _open.RemoveAt(0);
                //添加进close
                _close.Add(node);
                
                //如果为终点，结束搜索
                if (node.Equals(_fin))
                {
                    return node;
                }

                //搜索相邻节点
                var nearNodes = GetNearNode(node);
                foreach (var nearNode in nearNodes)
                {
                    float G = node.G + CalcDistance(node, nearNode);
                    if (!_open.Contains(nearNode) || G < nearNode.G)
                    {
                        nearNode.G = G;
                        nearNode.H = CalcDistance(nearNode, _fin);
                        nearNode.parent = node;
                        if(!_open.Contains(nearNode))
                            _open.Add(nearNode);
                    }
                }
            }

            return null;
        }

        private float CalcDistance(Node start, Node fin)
        {
            return Mathf.Abs(start.x - fin.x) + Mathf.Abs(start.y - fin.y);
        }

        private List<Node> GetNearNode(Node node)
        {
            List<Node> res = new List<Node>();
            int x = node.x;
            int y = node.y;

            int lenX = _map.GetLength(0);
            int lenY = _map.GetLength(1);

            if (x > 0) //左
            {
                var left = _map[x - 1, y];
                if(!_close.Contains(left) && !left.isObs)
                    res.Add(left);
            }
            if (x < lenX - 1) //右
            {
                var right = _map[x + 1, y];
                if(!_close.Contains(right) && !right.isObs)
                    res.Add(right);
            }
            if (y > 0) //下
            {
                var down = _map[x, y - 1];
                if(!_close.Contains(down) && !down.isObs)
                    res.Add(down);
            }
            if (y < lenY - 1) //上
            {
                var up = _map[x, y + 1];
                if(!_close.Contains(up) && !up.isObs)
                    res.Add(up);
            }

            if (x > 0 && y > 0) //左下
            {
                var leftDown = _map[x - 1, y - 1];
                if (!_close.Contains(leftDown) && !leftDown.isObs &&
                    !(_map[x - 1, y].isObs && _map[x, y - 1].isObs)) //左和下不能两个都为障碍物
                    res.Add(leftDown);
            }
            if (x > 0 && y < lenY - 1) //左上
            {
                var leftUp = _map[x - 1, y + 1];
                if (!_close.Contains(leftUp) && !leftUp.isObs &&
                    !(_map[x - 1, y].isObs && _map[x, y + 1].isObs)) //左和上不能两个都为障碍物
                    res.Add(leftUp);
            }
            if (x < lenX - 1 && y > 0) //右下
            {
                var rightDown = _map[x + 1, y - 1];
                if (!_close.Contains(rightDown) && !rightDown.isObs &&
                    !(_map[x + 1, y].isObs && _map[x, y - 1].isObs)) //右和下不能两个都为障碍物
                    res.Add(rightDown);
            }
            if (x < lenX - 1 && y < lenY - 1) //右上
            {
                var rightUp = _map[x + 1, y + 1];
                if (!_close.Contains(rightUp) && !rightUp.isObs &&
                    !(_map[x + 1, y].isObs && _map[x, y + 1].isObs)) //右和上不能两个都为障碍物
                    res.Add(rightUp);
            }
            return res;
        }
    }
}