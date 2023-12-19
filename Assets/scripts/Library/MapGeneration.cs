using PathfindingLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] int map = 1;
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject[] entities;
    [SerializeField] Material[] colors;
    AdjacencyList adjacencyList;

    public GameObject[,] mapGameObject;

    void Awake()
    {
        string path = "map" + SceneChanger.GlobalLevelNumber.ToString();
        var dataset = Resources.Load<TextAsset>(path);
        int nb;

        var zRows = dataset.text.Replace("\r", "").Split(new char[] { '\n' });

        mapGameObject = new GameObject[zRows[0].Split(new char[] { ',' }).Count(), zRows.Count()];

        for (int z = 0; z < zRows.Length; z++)
        {
            string[] xColumn = zRows[z].Split(new char[] { ',' });
            for (int x = 0; x < xColumn.Length; x++)
            {
                if (Int32.TryParse(xColumn[x], out nb))
                    spawnEntity(nb, x, z);
                else
                    spawnBlock(xColumn[x], x, z);
            }
        }
        GenerateNodeArray();
    }

    public List<Vector3> Getpath(Vector3 position, Vector3 destination){
        var tmp = Algorithms.BFS(adjacencyList,
                Get1DPositionOnMap((int)position.x, (int)position.z),
                Get1DPositionOnMap((int)destination.x, (int)destination.z));
        List <Vector3> toReturn = new List<Vector3>();
        foreach (var pos in tmp)
        {
            toReturn.Add(Get3DPositionOnMap(pos));
        }
        return toReturn;
    }

    private void GenerateNodeArray()
    {
        const string WallTag = "wall";
        adjacencyList = new AdjacencyList(mapGameObject.GetLength(0) * mapGameObject.GetLength(1));
        for(int z = 1; z < mapGameObject.GetLength(1) - 1; z++)
        {
            for (int x = 1;x < mapGameObject.GetLength(0) - 1;x++)
            {
                if (mapGameObject[x, z] == null || !mapGameObject[x, z].CompareTag(WallTag))
                {
                    if (mapGameObject[x + 1, z] == null || !mapGameObject[x + 1, z].CompareTag(WallTag))
                    {
                        adjacencyList.AddEdge(Get1DPositionOnMap(x, z), Get1DPositionOnMap(x + 1, z));
                    }
                    if (mapGameObject[x - 1, z] == null || !mapGameObject[x - 1, z].CompareTag(WallTag))
                    {
                        adjacencyList.AddEdge(Get1DPositionOnMap(x, z), Get1DPositionOnMap(x - 1, z));
                    }
                    if (mapGameObject[x, z + 1] == null || !mapGameObject[x, z + 1].CompareTag(WallTag))
                    {
                        adjacencyList.AddEdge(Get1DPositionOnMap(x, z), Get1DPositionOnMap(x, z+1));
                    }
                    if (mapGameObject[x, z - 1] == null || !mapGameObject[x, z - 1].CompareTag(WallTag))
                    {
                        adjacencyList.AddEdge(Get1DPositionOnMap(x, z), Get1DPositionOnMap(x, z - 1));
                    }

                }
            }
        }
    }
    private int Get1DPositionOnMap(int x,int y)
    {
        return x + (y * mapGameObject.GetLength(0));
    }
    private Vector3 Get3DPositionOnMap(int x)
    {
        return new Vector3(x% mapGameObject.GetLength(0), 0, (x - (x % mapGameObject.GetLength(0))) / mapGameObject.GetLength(0));
    }
    private void spawnBlock(string block, int x, int z)
    {
        GameObject toLoad;
        switch(block){
            case "P":
                mapGameObject[x, z] = Instantiate(blocks[0], new Vector3(x, 0, z), Quaternion.identity);
                break;
            case "F":
                mapGameObject[x, z] = Instantiate(blocks[1], new Vector3(x, 0, z), Quaternion.identity);
                break;
            case "H":
                mapGameObject[x, z] = Instantiate(blocks[2], new Vector3(x, 0, z), Quaternion.identity);
                break;
            case "G":
                mapGameObject[x, z] = Instantiate(blocks[3], new Vector3(x, 0, z), Quaternion.identity);
                break;
            case "X":
                Instantiate(blocks[4], new Vector3(x, 0, z), Quaternion.identity);
                break;
            case "Z":
                Instantiate(blocks[5], new Vector3(x, 0, z), Quaternion.identity);
                break;
            default:
                return;
        }
    }
    private void spawnEntity(int entity, int x, int z)
    {
        if(entity < 10) // joueur
        {
            mapGameObject[x, z] = Instantiate(entities[0], new Vector3(x, -0.5f, z), Quaternion.identity);
        }
        else if(entity < 40) // ennemi
        {
            if (entity < 20) //mantis
            {
                mapGameObject[x, z] = Instantiate(entities[(entity%10 + 1)], new Vector3(x, 0, z), Quaternion.identity);
            }
            else if (entity < 30) //crab
            {
                mapGameObject[x, z] = Instantiate(entities[(entity % 10 + 4)], new Vector3(x, 0, z), Quaternion.identity);
            }
            else //frog
            {
                mapGameObject[x, z] = Instantiate(entities[(entity % 10 + 7)], new Vector3(x, 0, z), Quaternion.identity);
            }
        }
        else if (entity < 50)// attack
        {

        }
        else //clé
        {
            mapGameObject[x, z] = Instantiate(entities[10], new Vector3(x, 0, z), Quaternion.identity);
        }
    }

    public void BruteMoveEntity(Vector3 pos, Vector3 destination)
    {
        mapGameObject[(int)destination.x, (int)destination.z] = mapGameObject[(int)pos.x, (int)pos.z];
        mapGameObject[(int)pos.x, (int)pos.z] = null;
    }


    public GameObject PutEntity(GameObject entity, Vector3 position)
    {
        GameObject content = mapGameObject[(int)position.x, (int)position.z];
        if (content == null)
        {
            mapGameObject[(int)position.x, (int)position.z] = entity;
            return null;
        }
        return content;
    }
    public GameObject MoveEntity(Vector3 originalPosition, Vector3 newPosition)
    {
        GameObject entity = mapGameObject[(int)originalPosition.x, (int)originalPosition.z];
        GameObject content = PutEntity(entity, newPosition);
        if (content == null)
        {
            mapGameObject[(int)originalPosition.x, (int)originalPosition.z] = null;
        }
        return content;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
