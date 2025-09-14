using UnityEngine;

public static class Extensions
{
    public static void ResetVelocity(this Rigidbody2D rb) => rb.linearVelocity = Vector2.zero;
    public static void ResetVelocityY(this Rigidbody2D rb) => rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
    public static void ResetVelocityX(this Rigidbody2D rb) => rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
    public static void SetZeroGravity(this Rigidbody2D rb) => rb.gravityScale = 0f;
}
