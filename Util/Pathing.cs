using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace EngineeringCorpsCS
{
    class PathNode
    {
        public PathNode parentNode;
        public float cost;
        public float heuristic;
        public float begin;
        public float pathCost;
        public int[] coords;
        public PathNode(PathNode parentNode, float frictionCost, float begin, float heuristic, int[] coords)
        {
            this.parentNode = parentNode;
            this.coords = coords;
            this.cost = heuristic + begin + frictionCost;
            this.heuristic = heuristic;
            this.begin = begin;
            this.pathCost = frictionCost;
        }
    }
    class Pathing
    {
        
        TileCollection tileCollection;
        float frictionFactor = 2.0f;

        public Pathing(TileCollection tileCollection)
        {
            this.tileCollection = tileCollection;
        }
        public PathNode GetPath(SurfaceContainer surface, Vector2 start, Vector2 target, int pathTimeout, Base.CollisionLayer unwalkableMask, Base.CollisionLayer collisionMask, float frictionFactor)
        {
            this.frictionFactor = frictionFactor;
            int frictionMask = (int)(collisionMask & Base.CollisionLayer.Terrain); //a nonzero value means the pathcost is higher for lower frictions
            frictionMask = frictionMask / (1 + frictionMask); //Normalize the frictionmask to 0 or 1 using +1 trick
            int[] startCoords = surface.WorldToAbsoluteTileCoords(start.x, start.y);
            int[] endCoords = surface.WorldToAbsoluteTileCoords(target.x, target.y);
            float heuristic = CalculateHeuristic(startCoords, endCoords);

            PathNode startNode = new PathNode(null, 0, 0, heuristic, startCoords);
            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();
            PathNode endNode = null;
            openList.Add(startNode);
            while(openList.Count > 0 && pathTimeout > 0)
            {
                pathTimeout--;
                PathNode curNode = null;
                int removeIndex = 0;
                for(int i = 0; i < openList.Count; i++)
                {
                    if(curNode == null || openList[i].cost < curNode.cost)
                    {
                        removeIndex = i;
                        curNode = openList[i];
                    }
                }
                openList.RemoveAt(removeIndex);
                closedList.Add(curNode);
                if (pathTimeout == 0 || (curNode.coords[0] == endCoords[0] && curNode.coords[1] == endCoords[1]))
                {
                    endNode = curNode;
                    break;
                }
                for(int i = curNode.coords[0] - 1; i <= curNode.coords[0] + 1; i++)
                {
                    for (int j = curNode.coords[1] - 1; j <= curNode.coords[1] + 1; j++)
                    {
                        bool ignore = false;
                        if (i < 0 || j < 0) //Check if coords are outside worldSize
                        {
                            ignore = true;
                        }
                        if (ignore == false) //else check that coords are not already in closedList
                        {
                            for (int k = 0; k < closedList.Count; k++)
                            {
                                if (closedList[k].coords[0] == i && closedList[k].coords[1] == j)
                                {
                                    ignore = true;
                                    break;
                                }
                            }
                        }
                        if(ignore == false) //else check that coords are not already in openList
                        {
                            for(int k = 0; k < openList.Count; k++)
                            {
                                if (openList[k].coords[0] == i && openList[k].coords[1] == j)
                                {
                                    ignore = true;
                                    break;
                                }
                            }
                        }
                        if (!ignore) //if neither of the preceding conditiosn set ignore to false, add tile to open list on path search
                        {
                            Tile tile = tileCollection.GetTerrainTile(surface.GetTileFromWorldInt(i, j));
                            int solidMask = (int)((unwalkableMask & collisionMask) & tile.collisionMask); // a zero value when the tile is walkable
                            if (solidMask == 0)
                            {
                                int[] coords = new int[] { i, j };
                                PathNode newNode = new PathNode(curNode, frictionMask * (frictionFactor * (1 / tile.frictionModifier)), curNode.begin + 1, CalculateHeuristic(coords, endCoords), coords);
                                openList.Add(newNode);
                            }
                        }
                    }
                }
            }
            return endNode;
        }

        public void DrawPath(RenderWindow window, PathNode node)
        {
            VertexArray pathArray = new VertexArray(PrimitiveType.Lines);
            byte startColor = 255;
            RectangleShape final = new RectangleShape(new Vector2f(32, 32));
            final.FillColor = Color.Red;
            final.Position = new Vector2f(node.coords[0] * Props.tileSize, node.coords[1] * Props.tileSize);
            while(node.parentNode != null)
            {
                pathArray.Append(new Vertex(new Vector2f((node.coords[0] + 0.5f) * Props.tileSize, (node.coords[1] + 0.5f) * Props.tileSize), new Color((byte)(255 - startColor), 0, --startColor)));
                node = node.parentNode;
                pathArray.Append(new Vertex(new Vector2f((node.coords[0] + 0.5f) * Props.tileSize, (node.coords[1] + 0.5f) * Props.tileSize), new Color((byte)(255 - startColor), 0, --startColor)));
            }
            RectangleShape start = new RectangleShape(new Vector2f(32, 32));
            start.FillColor = Color.Cyan;
            start.Position = new Vector2f(node.coords[0] * Props.tileSize, node.coords[1] * Props.tileSize);
            window.Draw(pathArray);
            window.Draw(start);
            window.Draw(final);
        }

        private float CalculateHeuristic(int[] start, int[] target)
        {
            return (float)(start[0] - target[0]) * (start[0] - target[0]) + (start[1] - target[1]) * (start[1] - target[1]);
        }
    }
}
