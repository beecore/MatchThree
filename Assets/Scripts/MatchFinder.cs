using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    public Board board;

    //lưu mảng các cặp kết hợp
    public List<Cat> currentMatches = new List<Cat>();

    /// <summary>
    /// Tìm kiếm 3  khối kết hợp
    /// </summary>
    public void FindAllMatches()
    {
        currentMatches.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Cat currentCat = board.allCats[x, y];
                if (currentCat != null)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        Cat leftCat = board.allCats[x - 1, y];
                        Cat rightCat = board.allCats[x + 1, y];
                        if (leftCat != null && rightCat != null)
                        {
                            if (leftCat.cattype == currentCat.cattype && currentCat.cattype == rightCat.cattype)
                            {
                                currentCat.isMatched = true;
                                leftCat.isMatched = true;
                                rightCat.isMatched = true;
                                currentMatches.Add(currentCat);
                                currentMatches.Add(leftCat);
                                currentMatches.Add(rightCat);
                            }
                        }
                    }

                    if (y > 0 && y < board.width - 1)
                    {
                        Cat aboveCat = board.allCats[x, y + 1];
                        Cat bellowCat = board.allCats[x, y - 1];
                        if (aboveCat != null && bellowCat != null)
                        {
                            if (aboveCat.cattype == currentCat.cattype && currentCat.cattype == bellowCat.cattype)
                            {
                                currentCat.isMatched = true;
                                aboveCat.isMatched = true;
                                bellowCat.isMatched = true;

                                currentMatches.Add(currentCat);
                                currentMatches.Add(aboveCat);
                                currentMatches.Add(bellowCat);
                            }
                        }
                    }
                }
            }
        }
        if (currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }
    }
}