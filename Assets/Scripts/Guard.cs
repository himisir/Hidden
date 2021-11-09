using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{


    public static event System.Action OnGuardHasSpotedPlayer;

    public Transform pathHolder;
    public Light spotlight;
    public float viewDistance;
    public float timeToSpotPlayer = 0.5f;
    float playerVisibleTimer;
    float viewAngle;
    public float speed = 10f;
    public float turnSpeed = 10f;
    public float waitTime = 1f;
    Transform player;
    public LayerMask viewMask;
    Color originalSpotlightColor;


    void Start()
    {
        originalSpotlightColor = spotlight.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        //Store positions of each waypoint 
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(FollowPath(waypoints));

    }
    void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
            spotlight.color = Color.red;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
            spotlight.color = originalSpotlightColor;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpotedPlayer != null)
            {
                OnGuardHasSpotedPlayer();
            }

        }
    }
    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }

        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 prevPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(prevPosition, waypoint.position);
            prevPosition = waypoint.position;
        }
        Gizmos.DrawLine(prevPosition, startPosition);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);

    }
    // Start is called before the first frame update

    IEnumerator FollowPath(Vector3[] waypoints)
    {

        transform.position = waypoints[0];
        int targetIndex = 1;
        Vector3 targetPosition = waypoints[targetIndex];
        //transform.LookAt(targetPosition);
        while (true)
        {
            yield return StartCoroutine(TurnFace(targetPosition));

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                targetIndex = (targetIndex + 1) % waypoints.Length;
                targetPosition = waypoints[targetIndex];
                yield return new WaitForSeconds(waitTime);

            }
            yield return null;
        }



    }
    IEnumerator TurnFace(Vector3 direction)
    {
        Vector3 targetPosition = (direction - transform.position).normalized;

        float targetAngle = Mathf.Atan2(targetPosition.x, targetPosition.z) * Mathf.Rad2Deg;


        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            Debug.Log(angle);
            yield return null;
        }

    }



}
