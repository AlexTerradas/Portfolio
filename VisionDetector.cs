using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionDetector : MonoBehaviour
{
    public LayerMask WhatIsPlayer;
    public LayerMask WhatIsVisible;

    public float DetectionRange;
    public float VisionAngle;
    public GameObject Player;

    public bool InRange;
    public bool InFOV;
    public bool NotBlocked;

    /// <summary>
    /// Gyzmos draw
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, DetectionRange);

        Gizmos.color = Color.red;
        var direction = Quaternion.AngleAxis(VisionAngle / 2, transform.forward)
            * transform.right;
        Gizmos.DrawRay(transform.position, direction * DetectionRange);
        var direction2 = Quaternion.AngleAxis(-VisionAngle / 2, transform.forward)
            * transform.right;
        Gizmos.DrawRay(transform.position, direction2 * DetectionRange);

        Gizmos.color = Color.white;
    }

    private void Update()
    {
        PlayersDetected();
    }

    /// <summary>
    /// Player detection
    /// </summary>
    private void PlayersDetected()
    {
        List<Transform> players = new List<Transform>();
        InRange = false;
        NotBlocked = false;
        InFOV = false;

        if (PlayerInRange())
        {
            InRange = true;
        }
        if (IsNotBlocked())
        {
            NotBlocked = true;
        }
        if (PlayerInAngle())
        {
            InFOV = true;
        }

    }

    /// <summary>
    /// Player Range detection
    /// </summary>
    /// <returns></returns>
    private bool PlayerInRange()
    {
        Collider2D[] playerColliders = Physics2D.OverlapCircleAll(
            transform.position, DetectionRange, WhatIsPlayer
            );
        if (playerColliders.Length == 0)
        {
            return false;
        }


        return true;
    }

    /// <summary>
    /// Player In angle detection
    /// </summary>
    /// <returns></returns>
    private bool PlayerInAngle()
    {

        var angle = GetAngle(Player.transform);

        if (angle > VisionAngle / 2)
        {
            return false;
        }
        return true;

    }

    /// <summary>
    /// Get the angle that the target is from the position of the object
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private float GetAngle(Transform target)
    {
        Vector2 targetDir = target.position - transform.position;
        float angle = Vector2.Angle(targetDir, transform.right);
        return angle;

    }

    /// <summary>
    /// Checks if the player is not blocked by any object
    /// </summary>
    /// <returns></returns>
    private bool IsNotBlocked()
    {
        var isVisible = IsVisible(Player.transform);
        if (!isVisible)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Function that checks if the target is visible
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsVisible(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(
           transform.position,
           dir,
           DetectionRange,
           WhatIsVisible
           );

        return hit.collider.transform == target;
    }
}
