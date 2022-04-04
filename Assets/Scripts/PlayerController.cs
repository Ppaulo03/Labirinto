using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour


{
    [Header("Health")]
    [SerializeField] protected int health_points_max = 10;
    public int current_health_points;


    [Header("Movement")]
    [SerializeField] protected float move_speed = 12f;

    [Header("Dash")]
    [SerializeField] protected float dash_speed = 50f;
    [SerializeField] protected float dash_time_total = 0.1f;
    [SerializeField] protected float dash_cooldown_total = 5f;
    public float dash_cooldown = 0f;
    public float dash_time = 0f;
    public bool dashing = false;


    [Header("Attack")]
    [SerializeField] protected float attack_damage = 40f;
    [SerializeField] protected float attack_knockback = 50f;
    [SerializeField] protected float attack_range = 0.5f;
    [SerializeField] protected float attack_center = 1f;
    [SerializeField] protected float attack_cooldown_total = 1f;
    public float attack_cooldown = 0f;


    [Header("Parry")]
    [SerializeField] protected float parry_time_total = 0.5f;
    public float  parry_time = 0f;
    public bool parry = false;
 
    protected Rigidbody2D myRigidBody;
    protected Vector2 direction = Vector2.right;


    protected void Start()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("Cinemachine");
        if(virtualCamera != null)
            virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Follow = transform;

        myRigidBody = GetComponent<Rigidbody2D>();
        current_health_points = health_points_max;
    }
    
    protected void Update()
    {

        if(parry_time <= 0) parry = false;
        else parry_time -= 1*Time.deltaTime;

        if(dashing) dash();
        else
        {
            movement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            if(dash_cooldown <= 0)
            {
                if(Input.GetKeyDown(KeyCode.LeftShift))
                {
                    dashing = true;
                    dash_time = dash_time_total;
                }
            } else dash_cooldown -= 1*Time.deltaTime;

            if(attack_cooldown <= 0) 
            {
                if(Input.GetButtonDown("Jump"))
                {
                    attack();
                    attack_cooldown = attack_cooldown_total;
                }
            }
            else attack_cooldown -= 1*Time.deltaTime;
        }
    }

    protected void dash()
    {
        if(dash_time <= 0) 
        {
            dashing = false;
            dash_cooldown = dash_cooldown_total;
            myRigidBody.velocity = Vector2.zero;
        }
        else
        {
            dash_time -= 1*Time.deltaTime;
            myRigidBody.velocity = direction*dash_speed; 
        }
        
    }

    protected void attack()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position + (Vector3)direction*attack_center, attack_range);
        foreach(Collider2D collider in hit)
        {
            if(collider.tag == "Enemy" && !collider.isTrigger)
            {
                //collider.GetComponent<Enemy>().Hit(attack_damage, attack_knockback, direction);
            }
        }
    }

     public void Hit(float knockback, Vector2 direction)
    {
        if(parry) attack();
        else
        {
            current_health_points -= 1;
            myRigidBody.AddForce((Vector3) direction*knockback, ForceMode2D.Impulse);
            if(current_health_points <= 0) Die();
        }
       
    }

    protected void Die()
    {
        return;
        //Debug.Log("Dead");
    }

    protected void movement(float movementX, float movementY){
        if( movementX != 0 || movementY != 0)
        {
            direction.x = movementX;
            direction.y = movementY;
            direction.Normalize();
        }

        Vector2 move = new Vector2(movementX, movementY);
        move.Normalize();
        myRigidBody.velocity = move*move_speed;
    }

    private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(transform.position + (Vector3)direction * attack_center, attack_range);
}
