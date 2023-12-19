
using System.Collections.Generic;

using UnityEngine;

public enum CharacterColor
{
    Red,
    Green,
    Blue
};
public abstract class Entity : MonoBehaviour
{
    public virtual void DiedTrigger()
    {
        Destroy(gameObject);
    }
}
public abstract class Character : Entity
{
    protected MapGeneration mapScript;
    public CharacterColor charColor;
    protected int maxHp = 2;
    protected int hp = 2;
    public bool IsDead {
        get { return hp < 0; } }
    public bool IsCharacterTurn { get; protected set; }
    public static Dictionary<(CharacterColor, CharacterColor), int> weaknessDmg = new Dictionary<(CharacterColor, CharacterColor), int>()
    {
        {(CharacterColor.Red, CharacterColor.Red), 1 },
        {(CharacterColor.Red, CharacterColor.Green), 2 },
        {(CharacterColor.Red, CharacterColor.Blue), 0 },

        {(CharacterColor.Green, CharacterColor.Red), 0 },
        {(CharacterColor.Green, CharacterColor.Green), 1 },
        {(CharacterColor.Green, CharacterColor.Blue), 2 },

        {(CharacterColor.Blue, CharacterColor.Red), 2 },
        {(CharacterColor.Blue, CharacterColor.Green), 0 },
        {(CharacterColor.Blue, CharacterColor.Blue), 1 }
    };

    abstract public void Move();

    public virtual void Attack<chara>(chara target) where chara : Character
    {
        target.TakeDamage(charColor);
    }
    public virtual void TakeDamage(CharacterColor attackerColor)
    {
        hp -= Character.weaknessDmg[(attackerColor, charColor)];
        if(hp <= 0)
        {
            DiedTrigger();
        }
    }
}
public abstract class Ennemi : Character
{
    public enum EnnemiType
    {
        Mantis,
        Frog,
        Crab
    };
}
public abstract class Projectile : Character
{
    protected Vector3 direction;
    protected bool isThrown = false;
    virtual public void Throw(Vector3 position, int rotation)
    {
        transform.parent = null;
        switch (rotation)
        {
            case 0:
                direction = new Vector3(0, 0, 1); break;
            case 90:
                direction = new Vector3(1, 0, 0); break;
            case 180:
                direction = new Vector3(0, 0, -1); break;
            case 270:
                direction = new Vector3(-1, 0, 0); break;
        }
        isThrown = true;
    }
}
