using UnityEngine;

public class PlayerContext
{
   public float xInput;
   public float yInput;

   public MoveMode moveMode;
   public float walkSpeed;
   public float runSpeed;
   public float jumpForce;
   public int jumpCount;
   public float dashForce;
   public float dashDuration;
   public float dashCooldownDuration;
   public float dashCoolDownTimer;
   public float slideSpeed;
   public float slideDuration;
   public float wallSlideSpeed;
   
   public float coyoteTimer;
   public float coyoteDuration;
   public float jumpBufferTimer;
   public float jumpBufferDuration;
   
   public bool isGrounded;
   public bool isOnWall;
   
   public bool IsMoving() => xInput != 0;
   
   public bool IsSprinting() => moveMode == MoveMode.Sprint;

   public bool IsCrouching() => Input.GetKey(KeyCode.C);

   public bool IsJumping()
   {
       bool spacePressed = false;

       if (Input.GetKeyDown(KeyCode.Space)) {jumpBufferTimer = jumpBufferDuration; spacePressed = true; }
       
      return (spacePressed && jumpCount > 0) /*|| coyoteTimer > 0*/
             || (jumpBufferTimer > 0 && isGrounded);
   }

   public bool IsDashing() => Input.GetKeyDown(KeyCode.LeftShift) && dashCoolDownTimer < 0f;

   public bool IsSliding() => Input.GetKeyDown(KeyCode.S)/* && isGrounded*/;

   public bool IsFalling() => isGrounded == false; // CHECK IF IT ON THE WALL OR LADDER 

   public bool IsWallSliding() => isOnWall && isGrounded == false;
}
