using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ennemi_AI : Ennemi
{

    [SerializeField] EnnemiType Type;

    private Player PlayerChara;

    private int turn;
    private GameObject Player;
    private Vector3 PlayerPosition;
    private List<Vector3> path;

    Vector3 targetPosition;
    Vector3 lookingPosition;
    private bool moving = false;
    private bool acting = false;

    float finalDirection;
    float targetDirection;
    const float rotationSpeed = 270;
    float rotating = 0;
    bool rotate = false;
    float attacking;
    const float timeAttacking = 0.8f;
    Vector3 posBeforeAttack;
    float jump = 0;
    const float jumpHeight = 4f;

    // Start is called before the first frame update
    void Start()
    {
        turn = 0;
        mapScript = FindObjectOfType<MapGeneration>();
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerChara = Player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if(jump> 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + (jump * Time.deltaTime * (jumpHeight*jump/jumpHeight)), transform.position.z);
                jump -= jump * Time.deltaTime * (jumpHeight * jump / jumpHeight);
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * (2.5f + jump));
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                moving = false;
                transform.position = targetPosition;

                if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] != null && mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].CompareTag("attack"))
                {
                    mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].GetComponent<Projectile>().Attack(this);
                }

                if (Type == EnnemiType.Crab)
                {
                    switch (transform.eulerAngles.y)
                    {
                        case 0:
                            lookingPosition = transform.position + new Vector3(0, 0, 1); break;
                        case 90:
                            lookingPosition = transform.position + new Vector3(1, 0, 0); break;
                        case 180:
                            lookingPosition = transform.position + new Vector3(0, 0, -1); break;
                        case 270:
                            lookingPosition = transform.position + new Vector3(-1, 0, 0); break;
                        default: lookingPosition = transform.position; break;
                    }
                    if (mapScript.mapGameObject[(int)lookingPosition.x, (int)lookingPosition.z] != null && (mapScript.mapGameObject[(int)lookingPosition.x, (int)lookingPosition.z].CompareTag("wall") || mapScript.mapGameObject[(int)lookingPosition.x, (int)lookingPosition.z].CompareTag("key")))
                    {
                        Turn(90);
                    }
                }
                if (rotating == 0)
                {
                    IsCharacterTurn = false;
                    acting = false;
                }
                    
            }
        }
        else if (rotating > 0)
        {
            transform.eulerAngles = new Vector3(0,
            transform.eulerAngles.y + (targetDirection * rotationSpeed * Time.deltaTime), 0);
            rotating -= rotationSpeed * Time.deltaTime;
            if (rotating <= 0)
            {
                rotating = 0;
                transform.eulerAngles = new Vector3(0, Mathf.Round(finalDirection % 360), 0);
                if(attacking == 0)
                {
                    IsCharacterTurn = false;
                    acting = false;
                }
            }
        }
        else if (attacking > 0)
        {
            if(attacking == timeAttacking)
            {
                Attack(PlayerChara);
                posBeforeAttack = transform.position;
            }
            if(attacking > timeAttacking / 2)
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Time.deltaTime * 5f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, posBeforeAttack, Time.deltaTime * 5f);
            }
            attacking -= Time.deltaTime * 10f;
            if (attacking <= 0)
            {
                attacking = 0;
                IsCharacterTurn = false;
                acting = false;
                transform.position = posBeforeAttack;
            }
        }
    }

    public override void Move()
    {
        turn++;
        PlayerPosition = Player.transform.position;
        if (Type == EnnemiType.Mantis)
            ActionM();
        else if (Type == EnnemiType.Crab)
            ActionC();
        else if (turn % 2 == 0)
            ActionF();
        else
        {
            IsCharacterTurn = false;
            acting = false;
        }
    }

    private void ActionM()
    {
        path = mapScript.Getpath(transform.position, PlayerPosition);
        if (path == null || path.Count < 2)
        {
            IsCharacterTurn = false;
            acting = false;
        }
        else if(path.Count == 2)
        {
            lookingAt(new Vector3(path[1].x, targetPosition.y, path[1].z));
            attacking = timeAttacking;
        }
        else
        {
            if(mapScript.mapGameObject[(int)path[1].x, (int)path[1].z] == null || mapScript.mapGameObject[(int)path[1].x, (int)path[1].z].CompareTag("attack"))
            {
                targetPosition = new Vector3(path[1].x, targetPosition.y, path[1].z);
                mapScript.BruteMoveEntity(transform.position, targetPosition);
                moving = true;
                lookingAt(new Vector3(path[2].x, targetPosition.y, path[2].z));
            }
            else
            {
                lookingAt(new Vector3(path[1].x, targetPosition.y, path[1].z));
            }
        }

    }
    private void ActionF()
    {
        path = mapScript.Getpath(transform.position, PlayerPosition);
        if (path == null || path.Count < 2)
        {
            IsCharacterTurn = false;
            acting = false;
        }
        else if (path.Count == 2)
        {
            lookingAt(path[1]);
            attacking = timeAttacking;
        }
        else if(path.Count >= 4 && (mapScript.mapGameObject[(int)path[2].x, (int)path[2].z] == null || mapScript.mapGameObject[(int)path[2].x, (int)path[2].z].CompareTag("attack")))
        {
            jump = jumpHeight;
            targetPosition = new Vector3(path[2].x, targetPosition.y, path[2].z);
            mapScript.BruteMoveEntity(transform.position, targetPosition);
            moving = true;
            lookingAt(new Vector3(path[3].x, targetPosition.y, path[3].z));
        }
        else if(mapScript.mapGameObject[(int)path[1].x, (int)path[1].z] == null || mapScript.mapGameObject[(int)path[1].x, (int)path[1].z].CompareTag("attack"))
        {
            targetPosition = new Vector3(path[1].x, targetPosition.y, path[1].z);
            mapScript.BruteMoveEntity(transform.position, targetPosition);
            moving = true;
            lookingAt(new Vector3(path[2].x, targetPosition.y, path[2].z));
        }
        else
        {
            lookingAt(new Vector3(path[1].x, targetPosition.y, path[1].z));
        }
    }
    private void ActionC()
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
        if(targetPosition.x == PlayerPosition.x && targetPosition.z == PlayerPosition.z)
        {
            attacking = timeAttacking;
        }
        else if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z] != null)
        { 
            if (mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].CompareTag("ennemi"))
                IsCharacterTurn = false;
            else if(mapScript.mapGameObject[(int)targetPosition.x, (int)targetPosition.z].CompareTag("wall"))
                Turn(90);
            else
            {
                mapScript.BruteMoveEntity(transform.position, targetPosition);
                moving = true;
            }
        }
        else
        {
            mapScript.BruteMoveEntity(transform.position, targetPosition);
            moving = true;
        }
    }
    private void Turn(int rotation)
    {
        rotating = rotation;
        if (rotation >= 0)
        {
            finalDirection = transform.eulerAngles.y + rotation;
            targetDirection = 1;
        }
        else
        {
            finalDirection = transform.eulerAngles.y + rotation;
            targetDirection = -1;
            rotating *= -1;
        }
    }
    private void lookingAt(Vector3 lookingAt)
    {
        Vector3 direction = lookingAt - new Vector3(transform.position.x, 0, transform.position.z);
        //------------code par blastone------------------------------------------------------------------
        float dot = Vector3.Dot(direction, transform.forward);
        float rotation = Mathf.Acos(dot) * Mathf.Rad2Deg;

        Vector3 normalizedDirection = lookingAt - new Vector3(transform.position.x, 0, transform.position.z);
        float whichWay = Vector3.Cross(transform.forward, normalizedDirection).y;
        //source:https://forum.unity.com/threads/finding-the-angle-between-a-direction-and-a-point.30639/

        rotating = rotation;
        if((int)rotating == 0 && attacking == 0)
        {
            IsCharacterTurn = false;
        }
        else
        {
            if (whichWay >= 0)
            {
                finalDirection = transform.eulerAngles.y + rotation;
                targetDirection = 1;
            }
            else
            {
                finalDirection = transform.eulerAngles.y - rotation;
                targetDirection = -1;
            }
        }
    }
}
