using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Player Generation")]
    [SerializeField] private GameObject player = null;
    [SerializeField] private Transform player_spawn = null;
    [SerializeField] private Transform exit_spawn = null;
    [SerializeField] private GameObject spawn = null;
    [SerializeField] private int direction = 0;

    private BoxCollider bounds;
    private Kruskal Generator;
    private Vector2[] locations = 
    {
        new Vector2( 0, -1),
        new Vector2( 1, 0),
        new Vector2( 0, 1),
        new Vector2( -1, 0)
    };

    void Start()
    {
        bounds = GetComponent<BoxCollider>();
        GameObject c_VirtualCamera = GameObject.FindGameObjectWithTag("Cinemachine");
        if(c_VirtualCamera != null)
            c_VirtualCamera.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingVolume = bounds;
        Generator = GetComponent<Kruskal>();
        
        GerarMaze();
    }
    private void GerarSpawns()
    {
        foreach(Transform child in transform) if(child.tag == "Spawn") Destroy(child.gameObject);
        int length = Generator.length, heigth = Generator.height;
        float scale = Generator.scale;

        foreach(Kruskal.Wall w in Generator.walls)
        {
            int lados;
            float x = w.pos.x, y = w.pos.y;

            if(w.rotate)
            {
                //Embaixo
                lados = 0;
                if( Mathf.Abs(x) == (length*scale - scale)/2) 
                {
                    lados++;
                    if( y == -( heigth*scale/2 - scale) ) lados++;
                    else
                        foreach(Kruskal.Wall other_w in Generator.walls)
                        {
                            if(other_w.rotate && x == other_w.pos.x && y == other_w.pos.y + scale)
                            {
                                lados ++;
                                break;
                            }
                        }
                }

                foreach(Kruskal.Wall other_w in Generator.walls)
                {
                    if(!other_w.rotate)
                    {
                        if(y == other_w.pos.y + scale/2.0f)
                        {
                            if(x == other_w.pos.x + scale/2.0f) lados++;
                            else if( x == other_w.pos.x - scale/2.0f) lados++;
                        }
                    }

                    if(lados == 2)
                    {
                        Vector3 pos = new Vector3(x, y-scale/2.0f, 0);
                        if( pos != exit_spawn.position && pos != player_spawn.position)
                            Instantiate(spawn, pos, transform.rotation, transform);
                        break;
                    }
                }
                
                //Emcima
                lados = 0;
                if( Mathf.Abs(x) == (length*scale - scale)/2)
                {
                    lados++;
                    if(y == heigth*scale/2 - scale) lados++;   
                }
                
                foreach(Kruskal.Wall other_w in Generator.walls)
                {
                    if(!other_w.rotate)
                    {
                        if(y == other_w.pos.y - scale/2.0f)
                        {
                            if(x == other_w.pos.x + scale/2.0f) lados++;
                            else if(x == other_w.pos.x - scale/2.0f) lados++;
                        }
                            
                    }
                    if(lados == 2){
                        Vector3 pos = new Vector3(x, y+scale/2.0f, 0);
                        if( pos != exit_spawn.position && pos != player_spawn.position)
                            Instantiate(spawn, pos, transform.rotation, transform);
                        break;
                    }
                }
            }
            else
            {
                //Direita
                lados = 0;
                if(Mathf.Abs(y) == (heigth*scale - scale)/2)
                {
                    lados++;
                    if(x == -(length*scale/2 - scale)) lados++;
                    else
                        foreach(Kruskal.Wall other_w in Generator.walls)
                        {
                            if(!other_w.rotate && y == other_w.pos.y && x == other_w.pos.x + scale)
                            {
                                lados ++;
                                break;
                            }
                        }
                }
                
                foreach(Kruskal.Wall other_w in Generator.walls)
                {
                    if(other_w.rotate)
                    {
                        if(x == other_w.pos.x + scale/2.0f )
                        {
                            if(y == other_w.pos.y + scale/2.0f) lados++;
                            else if(y == other_w.pos.y - scale/2.0f) lados++;
                        }
                    }
                    if(lados == 2){
                        Vector3 pos = new Vector3(x - scale/2.0f, y, 0);
                        if( pos != exit_spawn.position && pos != player_spawn.position)
                            Instantiate(spawn, pos, transform.rotation, transform);
                        break;
                    }
                }
                
                //Esquerda
                lados = 0;
                if(Mathf.Abs(y) == (heigth*scale - scale)/2) 
                {
                    lados++;
                    if(x == length*scale/2 - scale) lados++;   
                }
                
                foreach(Kruskal.Wall other_w in Generator.walls)
                {
                    if(other_w.rotate)
                    {
                        if(x == other_w.pos.x - scale/2.0f)
                        {
                            if (y == other_w.pos.y + scale/2.0f) lados++;
                            else if (y == other_w.pos.y - scale/2.0f) lados++;
                        }             
                    }

                    if(lados == 2){
                        Vector3 pos = new Vector3 (x + scale/2.0f, y, 0);
                        if( pos != exit_spawn.position && pos != player_spawn.position) 
                            Instantiate(spawn, pos, transform.rotation, transform);
                        break;
                    }
                }
            }
        }
    }

    public void GerarMaze()
    {
        int length = Generator.length = Random.Range(5,16);
        int heigth = Generator.height = Random.Range(5,16);
        float scale = Generator.scale;
        bounds.size = new Vector3(length*scale, heigth*scale, bounds.size.z);

        int orientacao = Random.Range(0,2) == 0? 1:-1;
        Vector3 new_pos = (Vector3)locations[direction] * new Vector2(length*scale/2.0f - scale/2.0f, heigth*scale/2.0f - scale/2.0f);
        if( length % 2 == 0 && new_pos.x == 0 ) new_pos += new Vector3(orientacao*scale/2.0f,0,0);
        else if( heigth % 2 == 0 && new_pos.y == 0 ) new_pos += new Vector3(0,orientacao*scale/2.0f,0); 
        player_spawn.localPosition = new_pos;

        int exit = Random.Range(0,4);
        while(exit == direction) exit = Random.Range(0,4);
        direction = exit+2 >= 4? exit-2 : exit+2;
        
        new_pos = (Vector3)locations[exit] * new Vector2(length*scale/2.0f - scale/2.0f, heigth*scale/2.0f - scale/2.0f);
        if( length % 2 == 0 && new_pos.x == 0 ) new_pos -=  new Vector3(orientacao*scale/2.0f,0,0);
        else if( heigth % 2 == 0 && new_pos.y == 0 ) new_pos -= new Vector3(0,orientacao*scale/2.0f,0); 
        exit_spawn.localPosition = new_pos;

        Generator.Generate();
        GameObject old_player = GameObject.FindGameObjectWithTag("Player");
     
        if(old_player != null) old_player.transform.position = player_spawn.position;
        else Instantiate(player, player_spawn.position, player_spawn.rotation);
        GerarSpawns();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump")) GerarMaze();
    }
        
    
}
