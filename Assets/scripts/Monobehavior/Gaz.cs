using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Gaz : Projectile
{
    MeshFilter meshFilter;
    Mesh mesh;
    [SerializeField] float variation = 0.2f;
    [SerializeField] float MaxVariation = 1f;
    Vector3 decalage = new Vector3(-0.5f, 0, -0.5f);//le gaz se cree a partir du coin ce qui fait des problèmes
    const int duration = 3;
    int turnCount = 0;
    const int widht = 10;
    const int nbTriangles = widht - 1;
    System.Random random = new System.Random();
    public static Gaz InstantiateGaz()
    {
        //https://stackoverflow.com/questions/49186166/unity-how-to-instantiate-a-prefab-by-string-name-to-certain-location
        var prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefab/Gaz.prefab", typeof(GameObject));
        GameObject gaz = Instantiate(prefab) as GameObject;
        gaz.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        return gaz.GetComponent<Gaz>();
    }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh();
    }
    private void Start()
    {
        mapScript = FindObjectOfType<MapGeneration>();
    }
    private void Update()
    {

        MakeGazMove();

        void MakeGazMove()
        {
            Vector3[] vertices = mesh.vertices;
            int index = random.Next(9, 91);
            Vector3 deplacement;
            deplacement = vertices[index] + new Vector3(0, variation, 0);
            if (deplacement.y - transform.position.y < MaxVariation && index % widht != 0 && (index + 1) % widht != 0)
            {
                vertices[index] += new Vector3(0, variation, 0);
            }
            index = random.Next(10, 91);
            deplacement = vertices[index] + new Vector3(0, variation, 0);
            if (deplacement.y > 0)
            {
                vertices[index] -= new Vector3(0, variation, 0);
            }
            mesh.vertices = vertices;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
    public override void Move()
    {

        if (isThrown)
        {
            IsCharacterTurn = true;
            GameObject collision = mapScript.PutEntity(gameObject, gameObject.transform.position - decalage);
            Debug.Log(collision);
            if (collision != null && collision != gameObject)
            {
                Character chara = collision.GetComponent<Character>();
                if (chara != null)
                {
                    Attack(chara);
                }
                Destroy(gameObject);
            }
            IsCharacterTurn = false;
            turnCount++;
            if (turnCount >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
    override public void Throw(Vector3 position, int rotation)
    {
        base.Throw(position, rotation);
        gameObject.transform.position = position + direction + decalage;
    }
    private int[] CreateTriangles()
    {
        int[] triangles = new int[widht * widht * 6];
        for (int i = 0; i < nbTriangles; i++)
        {
            for (int j = 0; j < nbTriangles; j++)
            {
                int k = (i * nbTriangles * 6) + (j * 6);
                triangles[k] = (i * widht) + j;
                triangles[k + 1] = (i * widht) + j + 1;
                triangles[k + 2] = (i * widht) + j + widht;
                triangles[k + 5] = (i * widht) + j + 1;
                triangles[k + 4] = (i * widht) + j + widht;
                triangles[k + 3] = (i * widht) + j + widht + 1;
            }
        }
        return triangles;
    }
    private Vector3[] CreateVerticies()
    {
        Vector3[] vertices = new Vector3[widht * widht];
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < widht; j++)
            {
                vertices[(i * widht) + j] = new Vector3(i, 0, j);
            }
        }
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
