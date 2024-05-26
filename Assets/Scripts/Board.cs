using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject bgTilePrefab;
    public Cat[] cats;
    public Cat[,] allCats;
    public float speedGame;
    public MatchFinder matchedfinder;
    public enum GameState { Playing,Pause,Win,Fail }
    public GameState gameState = GameState.Playing;
    public int moves;
    private void Awake()
    {
        matchedfinder = FindAnyObjectByType<MatchFinder>();
    }

    private void Start()
    {
        moves= Random.Range(20, 30);
        UIManager._instance.SetMoves(moves);
        UIManager._instance.SetLevels();

        allCats = new Cat[width, height];
        CreateBoard();
    }



    /// <summary>
    /// Tạo bảng khối block chứa các con mèo
    /// </summary>
    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "Block - " + x + ", " + y;
                int indexCat = Random.Range(0, cats.Length);

                int loopCount = 0;
                while (MatchesAt(new Vector2Int(x, y), cats[indexCat]) && loopCount < 100)
                {
                    indexCat = Random.Range(0, cats.Length);
                    loopCount++;
                }

                SpawnCat(new Vector3Int(x, y, 0), cats[indexCat]);
            }
        }
    }

    /// <summary>
    /// Khởi tạo kim cương
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="catToSPawn"></param>
    private void SpawnCat(Vector3Int pos, Cat catToSPawn)
    {
        Cat cat = Instantiate(catToSPawn, new Vector3(pos.x,pos.y+height/2,0f), Quaternion.identity);
        cat.transform.parent = transform;
        cat.name = "Cat - " + pos.x + ", " + pos.y;
        allCats[pos.x, pos.y] = cat;
        cat.SetUpCat(new Vector2Int(pos.x, pos.y), this);
    }

    /// <summary>
    /// Kiểm tra đầu vô có kết hợp 3 khối trở lên không
    /// </summary>
    /// <param name="postoCheck">vị trí check</param>
    /// <param name="catToCheck">Cat để check</param>
    /// <returns></returns>
    private bool MatchesAt(Vector2Int posToCheck, Cat catToCheck)
    {
        if (posToCheck.x > 1)
        {
            //Kiểm tra chiều ngang
            if (allCats[posToCheck.x - 1, posToCheck.y].cattype == catToCheck.cattype &&
                allCats[posToCheck.x - 2, posToCheck.y].cattype == catToCheck.cattype)
            {
                return true;
            }
        }
        if (posToCheck.y > 1)
        {
            //Kiểm tra chiều dọc
            if (allCats[posToCheck.x, posToCheck.y - 1].cattype == catToCheck.cattype &&
                allCats[posToCheck.x, posToCheck.y - 2].cattype == catToCheck.cattype)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Hủy đối tượng tại 1 vị trí
    /// </summary>
    /// <param name="pos"></param>
    private void DestroyMatchCatAt(Vector2Int pos)
    {
        if (allCats[pos.x, pos.y] != null)
        {
            if (allCats[pos.x, pos.y].isMatched)
            {
                UIManager._instance.listcurrentCat.Add(allCats[pos.x, pos.y]);
                Instantiate(allCats[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);
                Destroy(allCats[pos.x, pos.y].gameObject);
                allCats[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < matchedfinder.currentMatches.Count; i++)
        {
            if (matchedfinder.currentMatches[i] != null)
            {
               
                DestroyMatchCatAt(matchedfinder.currentMatches[i].posIndex);
            }
        }
        StartCoroutine(DeCreateCat());
    }

    /// <summary>
    /// Khởi tạo lại
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeCreateCat()
    {
        yield return new WaitForSeconds(0.2f);
        int nullCounter = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allCats[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allCats[x, y].posIndex.y -= nullCounter;
                    allCats[x, y - nullCounter] = allCats[x, y];
                    allCats[x, y] = null;
                }
            }
            nullCounter = 0;
        }
        StartCoroutine(FillBoard());
    }
    private IEnumerator FillBoard()
    {
        yield return new WaitForSeconds(.5f);
        ReFillBoard();
        yield return new WaitForSeconds(.5f);
        matchedfinder.FindAllMatches();
        if (matchedfinder.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }else
        {
            yield return new WaitForSeconds(0.5f);
            gameState = GameState.Playing;
        }

        //CheckWin
        StartCoroutine(CheckResult());


    }
    private IEnumerator  CheckResult()
    {
        yield return new WaitForSeconds(0.5f);
        bool isWin=UIManager._instance.CheckResult();
        yield return new WaitForSeconds(2.5f);
        if (isWin)
        {
            gameState = GameState.Win;
            UIManager._instance.ShowWinPanel();

        }
    }
    private void ReFillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allCats[x, y] == null)
                {
                    int indexCat = Random.Range(0, cats.Length);
                    SpawnCat(new Vector3Int(x, y, 0), cats[indexCat]);
                }
            }
        }
    }
}