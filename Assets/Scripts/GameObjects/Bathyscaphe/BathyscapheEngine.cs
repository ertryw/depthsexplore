using System.Collections;
using UnityEngine;

public class BathyscapheEngine : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private MyControl movement;

    [SerializeField]
    private Vector2 forceMultiply;

    [SerializeField]
    private float audioDeltaSpeed;

    [SerializeField]
    private AudioSource audioSource;

    private Device horizontalEngine;
    private Device verticalEngine;
    private Vector2 move;

    private void Start()
    {
        movement = new MyControl();
        movement.Enable();

        horizontalEngine = Device.Init();
        horizontalEngine.Setup(() => UserPreferences.Instance.playerData.statSteering.value, () => true, null);

        verticalEngine = Device.Init();
        verticalEngine.Setup(() => UserPreferences.Instance.playerData.statSteeringDown.value, () => true, null);

        StartCoroutine(EngineSound(audioDeltaSpeed));
    }

    private void OnEnable()
    {
        Bathyscaphe.WaterEntry += OnWater;
    }

    private void OnDisable()
    {
        Bathyscaphe.WaterEntry -= OnWater;
    }

    private void OnWater()
    {
        rb.gravityScale = 0.2f;
    }

    private void FixedUpdate()
    {
        float maxXVelocity = Bathyscaphe.Instance.State.MaxVelocity;
        float maxYVelocity = Bathyscaphe.Instance.State.MaxVelocity + (Mathf.Abs(move.y) * 2);

        // going to zero
        if (move.y != 0)
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxYVelocity);
        else
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxXVelocity);

        // stays in direction
        // if (Mathf.Abs(rb.velocity.x) > maxXVelocity)
        //     rb.velocity = new Vector2(rb.velocity.normalized.x * maxXVelocity, rb.velocity.y);

        // if (Mathf.Abs(rb.velocity.y) > maxYVelocity)
        //     rb.velocity = new Vector2(rb.velocity.x, rb.velocity.normalized.y * maxYVelocity);

        if (Bathyscaphe.Instance.State is BathyscapeOnSurface)
            return;

        move = movement.Bathyscaphe.Move.ReadValue<Vector2>();
        Vector2 power = new Vector2(forceMultiply.x * horizontalEngine.Level * move.x, forceMultiply.y * verticalEngine.Level * move.y);

        rb.AddForce(power);

        horizontalEngine.Active = move.x != 0;
        verticalEngine.Active = move.y != 0;
    }

    private IEnumerator EngineSound(float speed)
    {
        if (audioSource.isPlaying == false)
            audioSource.Play();

        while (true)
        {
            float endVolume = move.magnitude > 0 ? 1.0f : 0.0f;

            if (audioSource.volume != endVolume)
                audioSource.volume = Mathf.Lerp(audioSource.volume, endVolume, Time.deltaTime * speed);

            yield return new WaitForEndOfFrame();
        }
    }

}