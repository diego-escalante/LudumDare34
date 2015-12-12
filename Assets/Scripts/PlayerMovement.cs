﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

  [SerializeField] private float gravity = -0.025f;
  [SerializeField] private float jumpStrength = 0.4f;
  [SerializeField] private float speed = 0.25f;
  [SerializeField] private float acceleration = 0.0075f;

  //Movement stuff.
  private bool grounded = false;
  private Vector2 velocity = Vector2.zero;
  private bool facingRight = true;

  //Collision Stuff.
  private Vector2 playerSize = Vector2.zero;
  private LayerMask solidMask = new LayerMask();

  //Properties.
  public Vector2 Velocity {get {return velocity;}}
  public bool FacingRight {get {return facingRight;}}


  //===================================================================================================================

  private void Start() { 
    //Get the height of the player.
    Collider2D coll = GetComponent<BoxCollider2D>();
    playerSize = new Vector2(coll.bounds.size.x, coll.bounds.size.y);

    //Get the solid layermask for raycasting.
    solidMask = LayerMask.GetMask("Solid");
  }

  //===================================================================================================================

  private void OnEnable() {
    InputManager.jumpPressed += jump;
    InputManager.horizontalAxis += move;
  }

  //===================================================================================================================

  private void OnDisable() {
    InputManager.jumpPressed -= jump;
    InputManager.horizontalAxis -= move;
  }

  //===================================================================================================================

  private void FixedUpdate() {
    //Vertical Movement and collision checking.
    if(!grounded) {
      velocity.y += gravity;
      checkVerticalCollisions();
    }

    //Horizontal collision checking.
    if(velocity.x != 0) checkHorizontalCollisions();

    //Face the correct way.
    if((velocity.x > 0 && !facingRight) || (velocity.x < 0) && facingRight) turnAround();
    
    //Move the player.
    transform.Translate(velocity);
  }

  //===================================================================================================================

  private void jump() {
    if(grounded) {
      velocity.y = jumpStrength;
      grounded = false;
    }
  }

  //===================================================================================================================

  private void move(float fraction) {

   if(velocity.x == fraction * speed) return;

    //If there is no horizontal axis input, or if we are shielding, stop.
    if(fraction == 0) {
      if(velocity.x > 0) velocity.x = Mathf.Max(0,velocity.x - acceleration);
      else               velocity.x = Mathf.Min(0,velocity.x + acceleration);
    }
    else velocity.x = Mathf.Clamp(velocity.x + acceleration * fraction, -speed, speed);
  }

  //===================================================================================================================

  private void checkVerticalCollisions() {

    float distance = Mathf.Abs(velocity.y) + playerSize.y/2;
    int direction = velocity.y > 0 ? 1 : -1;
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up * direction, distance, solidMask);
    
    if(hit) {
      //Move to contact, stop.
      float gap = hit.distance;
      transform.Translate(Vector2.up * direction * (gap - playerSize.y/2));
      velocity.y = 0;
      if(direction == -1) grounded = true;
    }
    else grounded = false;
    Debug.DrawRay(transform.position, -Vector2.up*distance, Color.red);
  }

  //===================================================================================================================

  private void checkHorizontalCollisions() {
    float distance = Mathf.Abs(velocity.x) + playerSize.x/2;
    int direction = velocity.x > 0 ? 1 : -1;
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, distance, solidMask);

    if(hit) {
      float gap = hit.distance;
      transform.Translate(Vector2.right * direction * (gap - playerSize.x/2));
      velocity.x = 0;
    }
  }

  //===================================================================================================================

  private void turnAround() {
    facingRight = !facingRight;
    Vector3 temp = transform.localScale;
    temp.x *= -1;
    transform.localScale = temp;
  }
}
