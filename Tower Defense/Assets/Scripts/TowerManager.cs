using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TowerManager : Loader<TowerManager>
{

    public TowerBtn towerBtnPressed{ get; set; }

    SpriteRenderer spriteRenderer;

    private List<TowerControl> TowerList = new List<TowerControl>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;

   void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //нажатие пкм
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);//считывает положение курсора расположение башни
            RaycastHit2D hit = Physics2D.Raycast(mousePoint, Vector2.zero); //проверка можно ставить башню или нет

            if (hit.collider.tag == "TowerSide")
            {
                buildTile = hit.collider;
                buildTile.tag = "TowerSideFull";
                RegisrerBuildSite(buildTile);
                PlaceTower(hit);
            }   
            
        }
        if (spriteRenderer.enabled)
        {
            FollowMouse();
        }
    }

    public void  RegisrerBuildSite(Collider2D buildTag)
    {
        BuildList.Add(buildTag);
    }

    public void RegisterTower(TowerControl tower)
    {
        TowerList.Add(tower);
    }

    public void RenameTagBuildSite()
    {
        foreach(Collider2D buildTag in BuildList)
        {
            buildTag.tag = "TowerSide";
        }
        BuildList.Clear();
    }

    public void DestroyAllTowers()
    {
        foreach(TowerControl tower in TowerList)
        {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }




    public void PlaceTower(RaycastHit2D hit)        //метод указывающий о расположении башни /объект который появится при клике 
    {
        if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null) //если мы наведем курсор на кнопку то мы не сможем поставить башню на кнопке
        {
            TowerControl newTower = Instantiate(towerBtnPressed.TowerObject);
            newTower.transform.position = hit.transform.position;  //положение башни, где было кликнуто
            BuyTower(towerBtnPressed.TowerPrice);
            Manager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            DisableDrag();
        }
    }

    public void BuyTower(int price)
    {
        Manager.Instance.subtractMoney(price);
    }

    public void SelectedTower(TowerBtn towerSelected)
    {
        if (towerSelected.TowerPrice <= Manager.Instance.TotalMoney)
        {
            towerBtnPressed = towerSelected;
            EnableDrag(towerBtnPressed.DragSprite);
        }
    }
     
    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition); //привязка к курсору
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void EnableDrag(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableDrag()
    {
        spriteRenderer.enabled = false;
    }

}
