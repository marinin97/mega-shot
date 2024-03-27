using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IKillable
{   
        public static PlayerController Instance;
    public float Speed;
    public CameraShake ShakeScript;

    private Rigidbody2D RigidBody;
    private GameObject LookPivot;
    private LineRenderer AimLaser;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        LookPivot = transform.GetChild(0).gameObject;
        AimLaser = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Move();
        Look();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot(LookPivot.transform.GetChild(0).transform.position, LookPivot.transform.right);
        }

        DrawLaser(LookPivot.transform.GetChild(0).transform.position, LookPivot.transform.right, 5, 1);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Kill();
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 netVelocity = new Vector2(horizontal, vertical);
        float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? 0.3f : 1f;
        RigidBody.velocity = netVelocity.normalized * Speed * speedMultiplier;
    }

    private void Look()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookPos = mousePos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        Transform lookTransform = LookPivot.GetComponent<Transform>();
        lookTransform.localScale = new Vector3(1, angle > 90 || angle < -90 ? -1 : 1);
        lookTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Shoot(Vector3 startPos, Vector3 rotation)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if (hit && Vector3.Distance(startPos, hit.point) < 0.1f)
        {
            return;
        }
        GetComponent<AudioSource>().Play();
        StartCoroutine(ShakeScript.Shake(.1f, .3f, .8f));
        List<Vector3> points = new List<Vector3> { startPos };
        Shoot(startPos, rotation, points, 0);
    }

    private void Shoot(Vector3 startPos, Vector3 rotation, List<Vector3> points, int reflections)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if (hit)
        {
            points.Add(hit.point);
            if (hit.collider.CompareTag("Reflector"))
            {
                IDamageable obj = hit.collider.GetComponent<WallController>();
                obj.Damage(1);
                Shoot(hit.point + hit.normal * 0.01f, Vector3.Reflect(rotation, hit.normal), points, reflections + 1);
            }
            else if (hit.collider.CompareTag("Enemy") && reflections > 0)
            {
                DrawShootEffect(points);
                IKillable obj = hit.collider.GetComponent<EnemyController>();
                obj.Kill();
                GameController.Instance.ScoreForKillEnemy++;
            }
            else
            {
                DrawShootEffect(points);
            }
        }
    }

    private void DrawLaser(Vector3 startPos, Vector3 rotation, int reflections, int index)
    {
        AimLaser.SetPosition(index - 1, startPos);
        RaycastHit2D hit = Physics2D.Raycast(startPos, rotation);
        if (hit)
        {
            if (AimLaser.positionCount <= index)
            {
                AimLaser.positionCount += 1;
            }

            if (AimLaser.positionCount - index > 1)
            {
                AimLaser.positionCount -= 1;
            }
            AimLaser.SetPosition(index - 1, startPos);
            AimLaser.SetPosition(index, hit.point);

            if (hit.collider.CompareTag("Reflector") && reflections > 0)
            {
                DrawLaser(hit.point + hit.normal * 0.01f, Vector3.Reflect(rotation, hit.normal), reflections - 1, index + 1);
            }
        }
    }

    private void DrawShootEffect(List<Vector3> points)
    {
        GameObject emitter = Instantiate(GameController.Instance.ShootParticles);
        if (emitter)
        {
            emitter.SetActive(true);
            emitter.GetComponent<BulletParticleController>().FollowPoints(points);
        }
    }

    public void Kill()
    {
        Instantiate(GameController.Instance.DeathParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
