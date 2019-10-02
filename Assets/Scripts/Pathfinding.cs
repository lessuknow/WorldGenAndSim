using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Uses A*Star pathfinding.
static class Pathfinding
{
    private static Vector2Int GetSmallest(Dictionary<Vector2Int, float> list, Vector2Int endPos)
    {
        float smallest_f = float.MaxValue;
        Vector2Int ret = Vector2Int.zero;

        // Self explanitory; 
        foreach (Vector2Int x in list.Keys)
        {
            // TODO: Take difficulty into consideration.
            if (list[x] + Mathf.Abs(x.x - endPos.x) + Mathf.Abs(x.y - endPos.y) <= smallest_f)
            {
                smallest_f = list[x] + Mathf.Abs(x.x - endPos.x) + Mathf.Abs(x.y - endPos.y);
                ret = x;
            }
        }
        return ret;
    }
    
    public static List<Vector2Int> GetPath(Vector2Int initPos, Vector2Int endPos,
        float[,] heights, Vector2Int boundries)
    {
        // Parents are for backtracking, open_list is the current nodes we're looking at, closed_list is
        // the nodes we want.
        Dictionary<Vector2Int, float> open_list = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> closed_list = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();

        // We're looking at our initial position, obviously.
        open_list.Add(initPos, 0);

        // While there's still nodes to peek at,
        while (open_list.Count > 0)
        {
            // Get the node on our open list that's closest to the end position and remove it from the open_list.
            Vector2Int cur_pos = GetSmallest(open_list, endPos);
            open_list.Remove(cur_pos);

            // We're at the end; backtrace through.
            if (cur_pos == endPos)
            {
                List<Vector2Int> ret = new List<Vector2Int>();
                Vector2Int pos = initPos;

                foreach (Vector2Int x in closed_list.Keys)
                {
                    if(x != pos)
                        ret.Add(x);
                }
                ret.Add(cur_pos);
                return ret;
            }
            // Store the current position with it's distance.
            // TODO: Add 'difficulty' modifier here.
            closed_list.Add(cur_pos, Mathf.Abs(cur_pos.x - endPos.x) + Mathf.Abs(cur_pos.y - endPos.y));

            // Get all of the neighbors for that node.
            // TODO: Change this so it parses neighbors from a given map and stores their difficulties.
            List<Vector2Int> neighbors = new List<Vector2Int>();
            neighbors.Add(cur_pos + new Vector2Int(0, -1));
            neighbors.Add(cur_pos + new Vector2Int(0, 1));
            neighbors.Add(cur_pos + new Vector2Int(1, 0));
            neighbors.Add(cur_pos + new Vector2Int(-1, 0));

            foreach (Vector2Int v in neighbors)
            {
                // Check the distances here.
                // height 1 = unpassable.
                /*if (cannotPass.Contains(v))
                    continue;*/
                // We out of bounds

                if (closed_list.ContainsKey(v))
                    continue;

                if (v.x >= boundries.x || v.y >= boundries.y || v.x < 0 || v.y < 0)
                    continue;

                if (heights[v.x, v.y] < 0.5f)
                    continue;

                // Current 'difficulty' to get to the end position.
                // TODO: Add difficulty here as well.

                float g = closed_list[cur_pos] + Mathf.Abs(v.x - cur_pos.x) + Mathf.Abs(v.y - cur_pos.y) + heights[v.x, v.y];
                float h = Mathf.Abs(v.x - endPos.x) + Mathf.Abs(v.y - endPos.y);
                float f = g + h;
                
                if (open_list.ContainsKey(v))
                {
                    if (open_list[v] > g)
                    {
                        open_list[v] = g;
                        parents[cur_pos] = v;
                    }
                }
                // Else we'll take a peek at it later; add it to the list for consideration.
                else
                {
                    open_list.Add(v, g);
                }
            }
        }
        // Oof.
        Debug.Log("No path found. womp womp");
        return new List<Vector2Int>();

    }

}
