using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Fireball : Projectile
{
    MeshFilter meshFilter;
    Mesh mesh;
    Vector3 targetPosition;
    Vector3 pyramidTop = new Vector3(0, 0, 1);
    float distance;
    Renderer rend;
    bool firstTurn = true;
    public static Fireball InstantiateFireball(Vector3 position,Quaternion rotation,GameObject parent)
    {
        //https://stackoverflow.com/questions/49186166/unity-how-to-instantiate-a-prefab-by-string-name-to-certain-location
        var prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefab/Fireball.prefab", typeof(GameObject));
        GameObject fireball = Instantiate(prefab, position, rotation) as GameObject;
        fireball.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        fireball.transform.parent = parent.transform;
        return fireball.GetComponent<Fireball>();
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh();
        distance = Vector3.Distance(pyramidTop, new Vector3(1.5f, 0, -.5f));
        rend = GetComponent<Renderer>();
    }
    private void Start()
    {
        mapScript = FindObjectOfType<MapGeneration>();
    }
    private void Update()
    {
        if (isThrown && IsCharacterTurn)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 1.5f);
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {

                transform.position = targetPosition;
                GameObject collision = !firstTurn ? mapScript.MoveEntity(transform.position - direction, targetPosition) : mapScript.PutEntity(gameObject, targetPosition);//si il vient d'etre throw le met dans la map
                firstTurn = false;
                if (collision != null)
                {
                    Character chara = collision.GetComponent<Character>();
                    if(chara != null)
                    {
                        Attack(chara);
                    }
                    Destroy(gameObject);
                }
                IsCharacterTurn = false;
            }
        }
        else 
        {
            IsCharacterTurn = false;
            ClosePyramid();
        }
        //Debug.Log(meshFilter.mesh.vertices[4]);

        void ClosePyramid()
        {
            Vector3[] vertices = mesh.vertices;
            for (int i = 4; i < vertices.Length; i++)
            {
                vertices[i] += (pyramidTop - vertices[i]).normalized * Time.deltaTime;
            }

            mesh.vertices = vertices;
            float red = (distance - Vector3.Distance(pyramidTop, vertices[4])) / distance;
            rend.material.color = new Color(red, 0, 0, red);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
    public override void Move()
    {

        if(isThrown)
        {
            targetPosition += direction;
            IsCharacterTurn = true;
        }
    }
    override public void Throw(Vector3 position,int rotation)
    {
        //transform.parent = null;
        //switch (rotation)
        //{
        //    case 0:
        //        direction = new Vector3(0, 0, 1); break;
        //    case 90:
        //        direction = new Vector3(1,0, 0); break;
        //    case 180:
        //        direction = new Vector3(0, 0, -1); break;
        //    case 270:
        //        direction = new Vector3(-1, 0, 0); break;
        //}
        //isThrown= true;
        base.Throw(position,rotation);
        IsCharacterTurn = true;
        targetPosition = direction + position + new Vector3(0, 1, 0);
    }
    private int[] CreateTriangles()
    {
        return new int[] {
        0,1,2,
        3,2,1,
        4,3,1,
        0,2,5,
        6,1,0,
        2,3,7
        };
    }
    private Vector3[] CreateVerticies()
    {
        Vector3[] vertices =
        {
            new Vector3(-.5f,.5f,-.5f),//0
            new Vector3(.5f,.5f,-.5f),//1
            new Vector3(-.5f,-.5f, -.5f),//2
            new Vector3(.5f,-.5f,-.5f),//3
            new Vector3(1.5f,0,-.5f),//4
            new Vector3(-1.5f,0,-.5f),//5
            new Vector3(0,1.5f,-.5f),//6
            new Vector3(0,-1.5f,-.5f)//7
        };
        return vertices;
    }
    private Mesh CreateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = CreateVerticies();
        mesh.triangles = CreateTriangles();
        mesh.RecalculateNormals();
        return mesh;
    }
}
