using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNPCBarrier : MonoBehaviour
{
    public NPC_OLD_MAN npc;
    public Sprite barrier;
    void Start()
    {
        Invoke("CreateConstraint", 0.0f);
    }
    public void CreateConstraint()
    {
        int x = (int)npc.GetSpawnPoint().x - 5;
        int y = (int)npc.GetSpawnPoint().y;

        int newx = 0, newy = 0;
        for (int i = 0; i < 10; i++)
        {
            newx = x + i;
            newy = y + 5;
            CreateTile("barrier", barrier, newx, newy);
        }

        int newnewy = 0;
        for (int j = 0; j < 10; j++)
        {
            newnewy = newy - j;
            CreateTile("barrier", barrier, newx, newnewy);
        }

        int newnewx = 0;
        for (int k = 0; k < 10; k++)
        {
            newnewx = newx - k;
            CreateTile("barrier", barrier, newnewx, newnewy);
        }

        for (int l = 0; l < 10; l++)
        {
            CreateTile("barrier", barrier, newnewx, newnewy + l);
        }
    }
    private void CreateTile(string n, Sprite sprite, float x, float y)
    {
        GameObject newTile = new GameObject(name = n);
        newTile.transform.parent = this.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = sprite;
        newTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        newTile.AddComponent<ConstraintProp>();
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
    }
}
