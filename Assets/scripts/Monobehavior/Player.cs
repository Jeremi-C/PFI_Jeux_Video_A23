using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Player : Character
{
    [SerializeField] InputAction TextInputRotate;
    [SerializeField] InputAction TextInputFoward;
    [SerializeField] InputAction TextInputAttack;
    [SerializeField] InputAction TextInputColor;

    TextMeshProUGUI keyUi;
    TextMeshProUGUI hpUi;
    TextMeshProUGUI endGameTextUi;

    Hat[] hatComponents;
    private Animator animator;
    private Transform transform;
    private Fireball fireball;
    private ParticleSystem lightning;
    Vector3 targetPosition;
    private bool moving = false;
    private CharacterColor[] colors = (CharacterColor[])Enum.GetValues(typeof(CharacterColor));
    float finalDirection;
    float targetDirection;
    const float rotationSpeed = 180;
    private float rotating = 0;
    bool attacking = false;
    bool changingColor = false;

    const int lightningLength = 2;

    private int key;
    public override void Move()
    {
        IsCharacterTurn = true;
    }
    public override void TakeDamage(CharacterColor attackerColor)
    {
        int damage = Character.weaknessDmg[(attackerColor, charColor)];
        hp -= damage;
        hpUi.text = "vie: " + hp + "/" + maxHp;
        if (hp <= 0)
        {
            animator.SetTrigger("death");
            endGameTextUi.text = "Tu as perdu";
        }
        else if( damage == 0 ) 
        {
            animator.SetTrigger("block");
        }
        else
        {
            animator.SetTrigger("takeDamage");
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        transform = GetComponent<Transform>();
        lightning = GetComponentInChildren<ParticleSystem>();
        keyUi = GameObject.Find("key (TMP)").GetComponent<TextMeshProUGUI>();
        hpUi = GameObject.Find("hp (TMP)").GetComponent<TextMeshProUGUI>();
        endGameTextUi = GameObject.Find("EndGameText (TMP)").GetComponent<TextMeshProUGUI>();

        lightning.Stop();
        targetPosition = transform.position;

        maxHp= 2;
        hp = 2;

        hatComponents = GetComponentsInChildren<Hat>();
        TextInputFoward.Enable();
        TextInputFoward.started += (x) =>
        {
            if (Available())
            {
                switch (transform.eulerAngles.y)
                {
                    case 0:
                        targetPosition = transform.position + new Vector3(0, 0, 1); break;
                    case 90:
                        targetPosition = transform.position + new Vector3(1, 0, 0); break;
                    case 180:
                        targetPosition = transform.position + new Vector3(0, 0, -1); break;
                    case 270:
                        targetPosition = transform.position + new Vector3(-1, 0, 0); break;
                    default: targetPosition = transform.position; break;
                }
                if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] != null)
                {
                    if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].name == "Gate(Clone)" && key>0)
                    {
                        Destroy(mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z]);
                        mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] = null;
                        key--;
                        keyUi.text = "clé: " + key;
                    }
                    else if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].CompareTag("key"))
                    {
                        Destroy(mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z]);
                        mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] = null;
                        key++;
                        keyUi.text = "clé: " + key;
                        animator.SetBool("walking", true);
                        mapScript.BruteMoveEntity(transform.position, targetPosition);
                        moving = true;
                    }
                    
                }
                else if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] == null)
                {
                    animator.SetBool("walking", true);
                    mapScript.BruteMoveEntity(transform.position, targetPosition);
                    moving = true;
                }
            }
        };

        TextInputRotate.Enable();
        TextInputRotate.started += (x) =>
        {
            if (Available())
            {
                rotating = 90;
                if (x.ReadValue<float>() == 1)
                {
                    finalDirection = transform.eulerAngles.y + 90;
                    targetDirection = 1;
                }
                else
                {
                    finalDirection = transform.eulerAngles.y - 90;
                    targetDirection = -1;
                }
            }
        };
        TextInputColor.Enable();
        TextInputColor.started += (x) =>
        {

            if (Available())
            {
                changingColor = true;
                int index = Array.IndexOf(colors, charColor);
                if (x.ReadValue<float>() == 1)
                {
                    index++;
                }
                else
                {
                    index--;
                }

                index = index == colors.Length ? 0 : index;
                index = index == -1 ? 2 : index;
                charColor = colors[index % colors.Length];
                foreach (Hat hat in hatComponents)
                    hat.ChangeColor(charColor);
                Invoke("turnFinished", 1);
            }
        };
        TextInputAttack.Enable();
        TextInputAttack.started += (x) =>
        {
            if (Available())
            {
                attacking = true;
                switch (charColor)
                {
                    case CharacterColor.Red:
                        {

                            if (fireball == null)
                            {
                                Vector3 ballPosition = transform.position;
                                switch (transform.eulerAngles.y)
                                {
                                    case 0:
                                        ballPosition += new Vector3(0, 1, 0.5f); break;
                                    case 90:
                                        ballPosition += new Vector3(0.5f, 1, 0); break;
                                    case 180:
                                        ballPosition += new Vector3(0, 1, -0.5f); break;
                                    case 270:
                                        ballPosition += new Vector3(-0.5f, 1, 0); break;
                                }

                                fireball = Fireball.InstantiateFireball(ballPosition, transform.rotation, gameObject);
                            }
                            else
                            {
                                fireball.Throw(gameObject.transform.position, (int)transform.eulerAngles.y);
                                fireball = null;
                            }

                            break;
                        }
                    case CharacterColor.Blue:
                        {
                            Vector3[] targets = new Vector3[lightningLength];
                            Array.Fill(targets, transform.position);
                            Vector3 target2 = transform.position;
                            switch (transform.eulerAngles.y)
                            {
                                case 0:
                                    for(int i=0; i < targets.Length; i++)
                                    {
                                        targets[i] += new Vector3(0, 0, 1 + i);
                                    }break;
                                case 90:
                                    for (int i = 0; i < targets.Length; i++)
                                    {
                                        targets[i] += new Vector3(1 + i, 0, 0);
                                    }
                                    break;
                                case 180:
                                    for (int i = 0; i < targets.Length; i++)
                                    {
                                        targets[i] += new Vector3(0, 0, -1 - i);
                                    }
                                    break;
                                case 270:
                                    for (int i = 0; i < targets.Length; i++)
                                    {
                                        targets[i] += new Vector3(-1 - i, 0, 0);
                                    }
                                    break;
                                default: targetPosition = transform.position; break;
                            }
                            foreach(Vector3 target in targets)
                            {
                                Debug.Log(target);
                                GameObject gameObject = mapScript.mapGameObject[(int)target.x, (int)target.z];
                                if(gameObject == null || gameObject.CompareTag("wall")) {
                                    break;                                
                                }
                                if (gameObject.CompareTag("ennemi"))
                                {
                                    Attack(gameObject.GetComponent<Ennemi_AI>());
                                    break;
                                }
                            }
                            lightning.Play();
                            break;
                        }
                    case CharacterColor.Green:
                        {
                            Gaz.InstantiateGaz().Throw(gameObject.transform.position, (int)transform.eulerAngles.y);
                            break;
                        }

                }
                animator.SetTrigger("attack");
                Invoke("turnFinished", 1);
            }
        };
    }

    void Start()
    {
        mapScript = FindObjectOfType<MapGeneration>();
        key = 0;
        keyUi.text = "clé: " + key;
        hpUi.text = "vie: " + hp + "/" + maxHp;
        IsCharacterTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                moving = false;
                animator.SetBool("walking", false);
                transform.position = targetPosition;
                IsCharacterTurn = false;
            }
        }
        if (rotating > 0)
        {
            transform.eulerAngles = new Vector3(0,
            transform.eulerAngles.y + (targetDirection * rotationSpeed * Time.deltaTime), 0);
            rotating -= rotationSpeed * Time.deltaTime;
            if (rotating <= 0)
            {
                rotating = 0;
                transform.eulerAngles = new Vector3(0, Mathf.Round(finalDirection % 360), 0);
            }
        }
    }

    bool Available()
    {
        return (!moving && rotating == 0 && IsCharacterTurn && !attacking && hp > 0 && !changingColor);
    }

    void turnFinished()
    {
        lightning.Stop();
        IsCharacterTurn = false;
        attacking = false;
        changingColor = false;
    }
}