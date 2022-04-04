using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kruskal : MonoBehaviour
{
    public struct Wall
    {
        public Vector2 pos;
        public bool rotate;
        public int[] id1, id2;
    }

    public int height;
    public int length;
    public float scale;
    [SerializeField] private GameObject wall;
    private GameObject maze = null;

    private int[,] cellsId;
    public List<Wall> walls;

    public void Generate()
    {
        if(maze != null) Destroy(maze);
        maze = new GameObject("Maze");
        maze.transform.parent = transform;

        walls =  new List<Wall>();
        GenerateOuterWalls();
        GenerateSectors();
        CreateMaze();
    }

    private void GenerateOuterWalls()
    {
        GameObject left = Instantiate(wall, transform.position + new Vector3((scale/2)*length,0,0), Quaternion.Euler (0f, 0f, 0f), maze.transform);
        left.transform.localScale = new Vector3(left.transform.localScale.x, height*scale, left.transform.localScale.z);

        GameObject right = Instantiate(wall, transform.position + new Vector3(-(scale/2)*length,0,0), Quaternion.Euler (0f, 0f, 0f), maze.transform);
        right.transform.localScale = new Vector3(right.transform.localScale.x, height*scale, right.transform.localScale.z);

        GameObject top = Instantiate(wall, transform.position + new Vector3(0,(scale/2)*height,0), Quaternion.Euler (0f, 0f, 90f), maze.transform);
        top.transform.localScale = new Vector3(top.transform.localScale.x, length*scale, top.transform.localScale.z);

        GameObject bottom = Instantiate(wall, transform.position + new Vector3(0,-(scale/2)*height,0), Quaternion.Euler (0f, 0f, 90f), maze.transform);
        bottom.transform.localScale = new Vector3(bottom.transform.localScale.x, length*scale, bottom.transform.localScale.z);
    }

    private void GenerateSectors()
    {
        
        int cont = 0;
        cellsId = new int[height, length];
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < length; j ++)
            {
                cellsId[i,j] = cont;
                cont ++;
            }
        }

        
        int l = 0, h=0;

        for(float i = -(scale/2)*length + scale; i < (scale/2)*length; i += scale)
        {
            h = 0; 
            for(float j = (scale/2)*height - (scale/2); j > -(scale/2)*height; j -= scale)
            {
                Wall H = new Wall();
                H.pos = new Vector2(transform.position.x + i,transform.position.y + j);
                H.rotate = false;
                H.id1 = new int[2]{h,l};
                H.id2 = new int[2]{h,l+1};
                walls.Add(H);
                h++;
            }     
            l++;
        }

        l = 0;
        for(float i = -(scale/2)*length + (scale/2); i < (scale/2)*length; i += scale)
        {
            h = 0; 
            for(float j = (scale/2)*height - scale; j > -(scale/2)*height; j -= scale)
            {
                Wall L = new Wall();
                L.pos = new Vector2(transform.position.x + i,transform.position.y + j);
                L.rotate = true;
                L.id1 = new int[2]{h,l};
                L.id2 = new int[2]{h+1,l};
                walls.Add(L);
                h++;
            }     
            l++;
        }

        for (int i = 0; i < walls.Count; i++) //Shuffle
        {
            Wall temp = walls[i];
            int randomIndex = Random.Range(i, walls.Count);
            walls[i] = walls[randomIndex];
            walls[randomIndex] = temp;
        }

    }

    private void CreateMaze()
    {
        int i = 0;
        while(i < walls.Count)
        {
            Wall w = walls[i];
            if(cellsId[w.id1[0], w.id1[1]] != cellsId[w.id2[0], w.id2[1]])
            {
                walls.Remove(w);
                int id = cellsId[w.id2[0], w.id2[1]];
                for( int j = 0; j < cellsId.GetLength(0); j++)
                    for( int k = 0; k < cellsId.GetLength(1); k ++)
                        if(cellsId[j,k] == id) cellsId[j,k] = cellsId[w.id1[0], w.id1[1]]; 
            }
            else i++;
        }
        
        foreach(var w in walls)
        {   
            GameObject tmpWall;
            if(w.rotate) tmpWall = Instantiate(wall, (Vector3) w.pos, Quaternion.Euler (0f, 0f, 90f), maze.transform); 
            else tmpWall = Instantiate(wall, (Vector3) w.pos, Quaternion.Euler (0f, 0f, 0f), maze.transform);
            tmpWall.transform.localScale = new Vector3(tmpWall.transform.localScale.x, scale, tmpWall.transform.localScale.z);          
        }
    }

}