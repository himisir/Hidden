using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnLevelComplete;
    public float moveSpeed = 5f;
    public float smoothInputTime = .1f;
    float smoothInputMagnitude;
    float smoothVelocity;
    float angle;
    Vector3 velocity;
    public float turnSpeed = 5f;
    Rigidbody playerRb;

    bool disabled;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpotedPlayer += Disabled;
    }

    void Update()
    {

        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }



        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothVelocity, smoothInputTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        //transform.eulerAngles = Vector3.up * angle;

        // transform.Translate(transform.forward * speed * smoothInputMagnitude * Time.deltaTime, Space.World);
        velocity = transform.forward * moveSpeed * inputMagnitude;



    }
    void FixedUpdate()
    {
        playerRb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        playerRb.MovePosition(playerRb.position + velocity * Time.deltaTime);
    }
    void Disabled()
    {
        disabled = true;
    }
    void OnDestroyed()
    {
        Guard.OnGuardHasSpotedPlayer -= Disabled;
    }
    void OnTriggerEnter(Collider hitCollider)
    {

        if (hitCollider.tag == "Finish")
        {
            Debug.Log("Player reached End of the level");
            Disabled();
            if (OnLevelComplete != null)
            {
                OnLevelComplete();
            }
        }
    }
}
