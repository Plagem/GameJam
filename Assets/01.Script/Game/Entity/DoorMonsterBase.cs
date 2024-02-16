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

        

        if (isRare)
        {
            randomDoorNum = 12;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(8, 8);
            idleSprite = Resources.Load<Sprite>($"image/Entity/12");
            attackedSprite = Resources.Load<Sprite>($"image/Entity/12_2");
        }
        else
        {
            randomDoorNum  = Random.Range(1, 12);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(4, 4);
            idleSprite = Resources.Load<Sprite>($"image/Entity/{randomDoorNum}");
            attackedSprite = Resources.Load<Sprite>($"image/Entity/{randomDoorNum}_2");

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
        gameObject.GetComponent<SpriteRenderer>().sprite = idleSprite;

        slider.gameObject.SetActive(true);
        warningTab.gameObject.SetActive(true);
        warningTab.transform.Find("Warning").Find("WarningMessage").GetComponent<TMP_Text>().text = "Door";
        goalHp = doorData.hp;
    }

    IEnumerator DoorAttack()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = attackedSprite;
        yield return new WaitForSecondsRealtime(.05f);
        gameObject.GetComponent<SpriteRenderer>().sprite = idleSprite;
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
            SoundManager.Instance.Play("9. door_catch_success");
            Destroy(this.gameObject);
            slider.gameObject.SetActive(false);
            GameManager.Instance.FloorManager.FloorCleared();
            inventory.AddItem(doorData);
        }

        else
        {
            SoundManager.Instance.Play("8. door_catch_touch");
        }
        StartCoroutine(DoorAttack());
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
            SoundManager.Instance.Play("10. door_catch_failure");
        }
    }

    private void UpdateSlideBar()
    {
        slider.value = (hp / goalHp);
    }
}
