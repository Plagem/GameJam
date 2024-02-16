using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorMonsterBase : MonsterBase
{
    private float hp;
    public bool isRare;
    private int removeClick = 2;

    private DoorData doorData;

    private Inventory inventory;

    protected override void Start()
    {
        base.Start();
        inventory = GameManager.Instance.IngameUIManager.inventory;
        if(inventory == null)
        {
            Debug.Log("Inventory null error");
        }
        int randomDoorNum = Random.Range(1, 13);
        Debug.Log($"{Time.time} {randomDoorNum}번 생성");
        if(isRare)
        {
            randomDoorNum = 12;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(8, 8);
        }
        else
        {
            randomDoorNum  = Random.Range(1, 12);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(4, 4);

        }
        Debug.Log($"{randomDoorNum}번 생성");
        doorData = GameManager.Instance.FloorManager.AllDoorDataList[randomDoorNum];

        hp = doorData.hp / 2;
        this.gameObject.name = doorData.name;
        objectIndex = doorData.index;

        Sprite doorSprite = Resources.Load<Sprite>($"image/Entity/{objectIndex}");
        if(doorSprite == null )
        {
            Debug.Log("DoorSprite Null 발생");
        }
        gameObject.GetComponent<SpriteRenderer>().sprite = doorSprite;

        slider.gameObject.SetActive(true);
        warningTab.gameObject.SetActive(true);
        warningTab.transform.Find("Warning").GetComponent<Image>().sprite = Resources.Load<Sprite>("image/UIs/ui_catch");
        goalHp = doorData.hp;
    }

    protected override void MonsterGetDamage()
    {
        removeClick--;
        if(removeClick == 0)
        {
            warningTab.SetActive(false);
        }

        hp += damage;
        if(hp >= goalHp) 
        {
            Destroy(this.gameObject);
            slider.gameObject.SetActive(false);
            GameManager.Instance.FloorManager.FloorCleared();
            inventory.AddItem(doorData);
        }
    }

    protected override void Update()
    {
        base.Update();

        UpdateSlideBar();

        hp -= Time.deltaTime;
        if(hp <= 0 )
        {
            Destroy(this.gameObject);
            GameManager.Instance.IngameUIManager.ShowGameOver();
        }
    }

    private void UpdateSlideBar()
    {
        slider.value = (hp / goalHp);
    }
}
