using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace InfimaGames.LowPolyShooterPack
{
    public class EnemyNavigation : MonoBehaviour {
    [Header("Navigation")]
    private NavMeshAgent agent;
    public Transform Target;
    public Transform EnemyEye;
    public Transform points;
    public float ViewAngle = 90f;
    public float ViewDistance = 15f;
    public float DetectionDistance = 3f;
    public float stoppingDistance;
    private int index = 0;
    public bool isInView;
    public float distanceToPlayer;
    public string status = "patrolling";
    public Transform[] points_list;

    [Header("Health")]
    public float start_health = 100;
    [SerializeField] private float current_health = 0;
    public Image HealthBar;
    public Transform HealthIndicator;
    public bool isDead = false;

    [Header("Animation")]
    public Animator animator;
    public Animator weaponAnimator;
    public Transform MainCamera;
    public GameObject ragdoll;
    public GameObject ragdollWeapon;
    private List<Rigidbody> rigidbodies;

    [Header("Attack")]
    public float damage;
    public float ammoAmount;
    public float startAmmoAmount;
    public float reloadTime;
    public float shotTime;
    public float timeBtwShots;
    public bool canShoot = true;
    public bool isReloading = false;
    [SerializeField] private bool isStopped;


    void Start() {
        agent = GetComponent<NavMeshAgent>();
        current_health = start_health;
        HealthBar.fillAmount = (current_health / start_health);
        ammoAmount = startAmmoAmount;
        points_list = points.GetComponentsInChildren<Transform>();
        
    }

    private void Update() {
        distanceToPlayer = Vector3.Distance(Target.transform.position, agent.transform.position);
        isInView = IsInView();
        if (isInView || distanceToPlayer <= DetectionDistance) {
            status = "attacking";
        } else {
            status = "patrolling";
        }


        if (status == "patrolling") {
            agent.stoppingDistance = 0;
            if (agent.remainingDistance <= 0.5f) {
                if (index != points_list.Length) {
                    index++;
                    if (agent != null) {
                        agent.SetDestination(points_list[index].position);
                    }

                } else {
                    index = 0;
                    if (agent != null) {
                        agent.SetDestination(points_list[index].position);
                    }
                }
            }
        } else if (status == "attacking") {
            if (!isInView) {
                agent.stoppingDistance = 0;
            } else {
                agent.stoppingDistance = stoppingDistance;
            }
            if (distanceToPlayer <= DetectionDistance) {
                RotateToTarget();
            }
            MoveToTarget();
        }

        SetAnimation();

        HealthIndicator.rotation = Quaternion.LookRotation(HealthIndicator.position - MainCamera.position);
        if (ammoAmount > 0) {
            if (isInView && canShoot) {
                Fire();
            }
        } else if (!isReloading) {
            isReloading = true;
            agent.isStopped = true;
            isStopped = true;
            animator.SetTrigger("reload");
            weaponAnimator.SetTrigger("reload");
            StartCoroutine(Reload());
        }

        DrawViewState();
    }

    public void RotateToTarget() {
        Quaternion endRotation = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * 30);
    }

    private bool IsInView() {
        float realAngle = Vector3.Angle(EnemyEye.forward, Target.position - EnemyEye.position);
        RaycastHit hit;
        Debug.DrawRay(EnemyEye.transform.position, Target.position - EnemyEye.position, Color.red);
        if (Physics.Raycast(EnemyEye.transform.position, Target.position - EnemyEye.position, out hit, ViewDistance)) {
            if (realAngle < ViewAngle / 2f && Vector3.Distance(EnemyEye.position, Target.position) <= ViewDistance && hit.transform.tag == Target.transform.tag) {
                return true;
            }
        }
        return false;
    }
    public void MoveToTarget() {
        if (agent != null) {
            agent.SetDestination(Target.position);
        }

    }


    private void DrawViewState() {
        Vector3 left = EnemyEye.position + Quaternion.Euler(new Vector3(0, ViewAngle / 2f, 0)) * (EnemyEye.forward * ViewDistance);
        Vector3 right = EnemyEye.position + Quaternion.Euler(-new Vector3(0, ViewAngle / 2f, 0)) * (EnemyEye.forward * ViewDistance);
        Debug.DrawLine(EnemyEye.position, left, Color.yellow);
        Debug.DrawLine(EnemyEye.position, right, Color.yellow);
    }
    private void SetAnimation() {
        if (agent.velocity != Vector3.zero) {
            animator.SetBool("is stopped", false);
        } else {
            animator.SetBool("is stopped", true);
        }
    }

    public void CheckHit(float damage, Vector3 force, Vector3 hitPosition) {
        current_health -= damage;
        HealthBar.fillAmount = (current_health / start_health);
        MoveToTarget();
        if (current_health <= 0 || isDead) {
            if (!isDead) {
                GameObject ragd = Instantiate(ragdoll, transform.position, transform.rotation);
                Instantiate(ragdollWeapon, transform.position, transform.rotation);
                rigidbodies = new List<Rigidbody>(ragd.GetComponentsInChildren<Rigidbody>());
                Rigidbody _injuredRigidbody = rigidbodies.OrderBy(rigidbody => Vector3.Distance(rigidbody.position, hitPosition)).First();
                _injuredRigidbody.AddForceAtPosition(force, hitPosition, ForceMode.Impulse);
            }

            Destroy(gameObject);
            isDead = true;
            
        }
        
    }

    private void Fire() {
        agent.isStopped = true;
        isStopped = true;
        int r = Random.Range(0, 100);
        if (r < 75) {
            Target.gameObject.GetComponent<PlayerHealth>().CheckHit(damage);
        }
        ammoAmount -= 1;
        canShoot = false;

        StartCoroutine(Firerate());
        animator.SetTrigger("shot");
        weaponAnimator.SetTrigger("shot");
        StartCoroutine(Timer(shotTime));

    }

    private IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        agent.isStopped = false;
        isStopped = false;
    }

    private IEnumerator Reload() {
        
        yield return new WaitForSeconds(reloadTime);
        ammoAmount = startAmmoAmount;
        agent.isStopped = false;
        isStopped = false;
        isReloading = false;
        
    }
    private IEnumerator Firerate() {
        yield return new WaitForSeconds(timeBtwShots);
        canShoot = true;
    }
    }
}
