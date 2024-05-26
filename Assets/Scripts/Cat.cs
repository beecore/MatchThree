using System.Collections;
using UnityEngine;

public class Cat : MonoBehaviour
{
    public Vector2Int posIndex;

    [HideInInspector]
    public Board board;

    private Vector2 finalTouchPosition;
    private Vector2 firstTouchPosition;
    private Vector2Int previousPos;
    private bool mousePressed;
    private float swipeAngle;
    private Cat otherCat;
    public enum CatType { blue, black, red, yellow, purple }
    public CatType cattype;
    public bool isMatched;
    public GameObject destroyEffect;
    [HideInInspector]
    
    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.speedGame * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allCats[posIndex.x, posIndex.y] = this;
        }

        if (mousePressed && Input.GetMouseButtonUp(0))
        {
           
            if (board.gameState == Board.GameState.Playing)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
                mousePressed = false;
                UIManager._instance.SetMoves(board.moves--);
            }
          
           
        }
    }

    public void SetUpCat(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void OnMouseDown()
    {
        if (board.gameState == Board.GameState.Playing)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
        
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        if (Vector2.Distance(firstTouchPosition, finalTouchPosition) > 0.5f)
        {
            MoveCat();
        }
    }

    private void MoveCat()
    {
        previousPos = posIndex;
        if (swipeAngle <= 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
        {
            otherCat = board.allCats[posIndex.x + 1, posIndex.y];
            otherCat.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
        {
            otherCat = board.allCats[posIndex.x, posIndex.y + 1];
            otherCat.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            otherCat = board.allCats[posIndex.x, posIndex.y - 1];
            otherCat.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            otherCat = board.allCats[posIndex.x - 1, posIndex.y];
            otherCat.posIndex.x++;
            posIndex.x--;
        }
        board.allCats[posIndex.x, posIndex.y] = this;
        board.allCats[otherCat.posIndex.x, otherCat.posIndex.y] = otherCat;
        //kiểm tra các khối
        StartCoroutine(CheckMove());
    }

    public IEnumerator CheckMove()
    {
        board.gameState = Board.GameState.Pause;
        yield return new WaitForSeconds(0.5f);
        board.matchedfinder.FindAllMatches();
        if (otherCat != null)
        {
            if (!isMatched && !otherCat.isMatched)
            {
                otherCat.posIndex = posIndex;
                posIndex = previousPos;
                board.allCats[posIndex.x, posIndex.y] = this;
                board.allCats[otherCat.posIndex.x, otherCat.posIndex.y] = otherCat;
                yield return new WaitForSeconds(.5f);
                board.gameState = Board.GameState.Playing;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }
  
}