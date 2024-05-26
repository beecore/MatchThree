using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using static Cat;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI txtMove;
    [SerializeField] TextMeshProUGUI txtLevels;
    public static UIManager _instance;
    public List<Image> listImageUiShow = new List<Image>();
    public List<CatResult> listResult = new List<CatResult>();
    public List<Cat> listcurrentCat = new List<Cat>();
    public List<TextMeshProUGUI> listTextCat = new List<TextMeshProUGUI>();
    //khoi tao mảng 5 prefab;
    public List<GameObject> listPrefabs = new List<GameObject>();

    public GameObject winPanel;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        listcurrentCat = new List<Cat>();
        listResult = new List<CatResult>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, listPrefabs.Count);
            GameObject obj = listPrefabs[randomIndex];
            Cat cat = obj.GetComponent<Cat>();

            if (listResult.Count > 0)
            {
               int countDup= listResult.Where(n => n.cattype == cat.cattype).ToList().Count;
                if (countDup > 0)
                {
                    randomIndex = UnityEngine.Random.Range(0, listPrefabs.Count);
                    obj = listPrefabs[randomIndex];
                    cat = obj.GetComponent<Cat>();
                }
            }

           
            CatResult item = new CatResult();
            item.index = i;
            item.cattype = cat.cattype;
            item.image = obj.GetComponent<SpriteRenderer>().sprite;
            item.count = UnityEngine.Random.Range(1, 3);//ScriptableObject
            listTextCat[i].text = item.count.ToString();
            listImageUiShow[i].sprite = item.image;
            listResult.Add(item);
        }
        

    }
    public void SetMoves(int moves)
    {
        txtMove.text = moves.ToString();
    }
    public void SetLevels()
    {
        int level = PlayerPrefs.GetInt("level", 1);
        txtLevels.text = level.ToString();
    }
    public bool  CheckResult()
    {
        bool isWin = false;
        int isDoneCheck =0;
        if (listResult.Count > 0 && listcurrentCat.Count>0)
        {
            foreach(CatResult cat in listResult)
            {
                int count = listcurrentCat.Where(n => n.cattype == cat.cattype).Count();

                UpdateResult(cat, count);
                if (count>= cat.count)
                {
                    isDoneCheck++;
                    cat.isdone = true;
                }
                
            }
           
        }
        if (listResult.Count > 0 && listcurrentCat.Count > 0 && isDoneCheck== listResult.Count)
        {
            isWin = true;

        }
        return isWin;
    }
    private void UpdateResult(CatResult cat, int count)
    {
        int coutResult = cat.count - count<0? 0: cat.count - count;
        listTextCat[cat.index].text = coutResult.ToString();

    }
    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }
    public void NextGame()
    {
        int level = PlayerPrefs.GetInt("level", 1)+1;
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene("Game");

    }
}
public class CatResult
{
    public int index;
    public CatType cattype;
    public int  count;
    public Sprite image;
    public bool isdone;
    public TextMeshProUGUI text;

}