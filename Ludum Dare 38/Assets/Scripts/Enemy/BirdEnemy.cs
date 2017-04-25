using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy: Enemy {

  private float flyingSpeed = 0.2f;
  private float flyingMaxSpeed = 3f;

  protected override void MoveTowardsPosition(Vector3 position) {
    Vector3 displacement = position - transform.position;

    Rigidbody2D rb = GetComponent<Rigidbody2D>();

    if (rb != null) {
      rb.gravityScale = 0;
      rb.freezeRotation = true;
      Vector3 velocity = rb.velocity;

      if (Mathf.Abs(displacement.x) < 0.2f || Mathf.Abs(displacement.y) < 0.2f) {
        velocity.x = 0;
        velocity.y = 0;
      }

      else {
        velocity.x += Mathf.Sign(displacement.x) * flyingSpeed;
        velocity.y += Mathf.Sign(displacement.y) * flyingSpeed;

        if (Mathf.Abs(velocity.x) > flyingMaxSpeed && Mathf.Sign(velocity.x) == Mathf.Sign(displacement.x)) {
          velocity.x = Mathf.Sign(displacement.x) * flyingMaxSpeed;
        }

        if (Mathf.Abs(velocity.y) > flyingMaxSpeed && Mathf.Sign(velocity.y) == Mathf.Sign(displacement.y)) {
          velocity.y = Mathf.Sign(displacement.y) * flyingMaxSpeed;
        }
      }

      rb.velocity = velocity;
    }
  }
}
