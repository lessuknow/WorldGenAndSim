using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    Vector3Int position;

    public int end_x, end_z;

    private bool moving;
    private Vector3 startPos, endPos;
    private float curTime = 0;

    // This 'y' is actually our z. Fix in future somehow.
    private List<Vector2Int> path;

    private void Start()
    {
        path = new List<Vector2Int>();
        position = new Vector3Int(0,0,5);
        transform.position = position + new Vector3(0.5f, 1.375f, 0.5f);
        moving = false;
        startActions();
    }

    private void startActions()
    {
        InvokeRepeating("DoAction", 0.35f, 0.35f);
    }

    private void DoAction()
    {
        if (path.Count < 1)
        {
            Look();
        }
        Move();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (path.Count < 1)
            {
                Look();
            }
            Move();
        }
        if (moving)
        {
            curTime += Time.deltaTime * 5f;
            if (curTime > 0.95) { 
                curTime = 1;
                moving = false;
            }
            transform.position = Vector3.Lerp(transform.position, endPos, curTime);
            if (!moving)
                curTime = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("F");
    }

    


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("E");
        other.gameObject.SetActive(false);
    }

    private void Look()
    {
        int radius = 99;
        Collider[] results = new Collider[30];
        Physics.OverlapSphereNonAlloc(position, radius, results,~(1 << 8));
        GameObject wantedObj = null;
        Vector2Int boundaries = new Vector2Int(CreateMap.x_size, CreateMap.z_size);

        for (int i = 0;i < results.Length; i++)
        {
            if(results[i] == null)
            {
                break;
            }

            //TODO: REFACTOR:
            // chekcs: first if none found yet, then if distance is smallest, then if it can get a path.
            if (results[i].gameObject != gameObject) {
                if(wantedObj == null)
                    wantedObj = results[i].gameObject;
                if((Vector3.Distance(results[i].transform.position, gameObject.transform.position) < 
                    Vector3.Distance(wantedObj.transform.position, gameObject.transform.position)))
                    if(Pathfinding.GetPath(new Vector2Int(position.x, position.z), new Vector2Int(
                        (int)results[i].transform.position.x, (int)results[i].transform.position.z
                        ), CreateMap.heightMap, boundaries).Count > 0)
                    {
                        wantedObj = results[i].gameObject;
                    }
                        
            }
        }
        if (wantedObj != null)
        {
            // TODO: Switch from results[0].trans.pos.x to prob a reference toa  script that stores its posiiton as a VECTOR2INT.
   
            path = Pathfinding.GetPath(new Vector2Int(position.x, position.z), new Vector2Int(
                (int)wantedObj.transform.position.x, (int)wantedObj.transform.position.z
                ), CreateMap.heightMap, boundaries);
        }
        else
        {
            int newMove = Random.Range(0, 4);
            int x = 0, y = 0;
            // TODO: Limit this.
            switch(newMove)
            {
                case 0:
                    if (position.x != 0)
                        x = -1;
                    else
                        x = 1;
                    break;
                case 1:
                    x = 1;
                    break;
                case 2:
                    if (position.z != 0)
                        y = -1;
                    else
                        y = -1;
                    break;
                case 3:
                    y = 1;
                    break;
            }
            path = new List<Vector2Int>()
            {
                new Vector2Int(position.x + x, position.z + y)
            };
        }
    }

    private void Move()
    {
        if(path.Count > 0) {
            startPos = transform.position;
            endPos = new Vector3(path[0].x + 0.5f, transform.position.y, path[0].y + 0.5f);
            position = new Vector3Int(path[0].x, 0, path[0].y);
            path.RemoveAt(0); 
            moving = true;
        }
    }
}
