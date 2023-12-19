using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    private Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    public void ChangeColor(CharacterColor color)
    {
        switch (color)
        {
            case CharacterColor.Red:
                rend.material.color = Color.red;
                break;
            case CharacterColor.Blue:
                rend.material.color = Color.blue;
                break;
            case CharacterColor.Green:
                rend.material.color = Color.green;
                break;
        }
    }
}
