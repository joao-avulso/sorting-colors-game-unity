using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Ball
{
    public bool IsActive { get; set; }
    public GameObject Obj { get; set; }
    public int ColorIndex { get; set; }
}

public class ContainerScript : MonoBehaviour
{
    public GameObject circle;
    public GameObject origin;
    public Stack<Ball> balls = new();
    public int amount = 0;
    public bool isComplete = false;

    private LogicScript gameLogic;
    private MovesScript moves;
    private readonly int max = 4;
    private int flipped = 1;

    void Start()
    {
        gameLogic = GameObject.Find("Logic").GetComponent<LogicScript>();
        moves = GameObject.Find("Logic").GetComponent<MovesScript>();
    }

    void OnMouseDown() 
    {
        if (Random.value > gameLogic.moveChance && SceneManager.GetActiveScene().buildIndex > 2)
        {
            moves.ChooseMove(this.GameObject());
        }
        else
        {
            if (!gameLogic.selected.IsActive && amount > 0)
                PopBall();
            else if (gameLogic.selected.IsActive && amount < max)
                PushBall();
        }
    }

    public void AddColor(Color color, int index, int count) 
    {
        // instantiate
        GameObject newCircle = Instantiate(circle, transform);
        newCircle.GetComponent<SpriteRenderer>().color = color;

        // transform
        Vector3 origPos = newCircle.transform.position;
        newCircle.transform.position += 7 * Vector3.up;
        if (amount != 0)
            newCircle.transform.LeanMove(origPos + amount * newCircle.transform.lossyScale.y * Vector3.up, 0.5f).setDelay(count * gameLogic.animSpeed/2).setEaseOutCirc();
        else
            newCircle.transform.LeanMove(origPos, 0.5f).setDelay(count * gameLogic.animSpeed/2).setEaseOutCirc();
        
        // add ball
        Ball newBall = new()
        {
            IsActive = true,
            Obj = newCircle,
            ColorIndex = index,
        };
        balls.Push(newBall);
        amount++;
    }

    void PopBall() 
    {
        if (balls.Count == 0 || !gameLogic.GetActive())
            return;

        gameLogic.selected = balls.Pop();
        gameLogic.selected.Obj.transform.parent = null;
        gameLogic.selected.Obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Debug.Log("Selected color: " + gameLogic.selected.ColorIndex);
        amount--;
    }

    void PushBall() 
    {
        if (!gameLogic.GetActive())
            return;

        Debug.Log(origin.transform.position.y);
        Ball newBall = gameLogic.selected;
        gameLogic.selected.Obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
        gameLogic.selected = new() { IsActive = false };
        newBall.Obj.transform.parent = transform;
        if (balls.Count != 0) 
            newBall.Obj.transform.position = balls.Peek().Obj.transform.position + flipped * newBall.Obj.transform.lossyScale.y * Vector3.up;
        else
            newBall.Obj.transform.position = origin.transform.position + flipped * newBall.Obj.transform.lossyScale.y/2 * Vector3.up;
        balls.Push(newBall);
        amount++;
        gameLogic.CheckEndGame();
    }

    public bool VerifyContainer(int[] colorCount)
    {
        int previous = -1;
        int count = 0;
        
        foreach (Ball item in balls)
        {
            if (previous == -1)
            {
                previous = item.ColorIndex;
                count++;
            }
            else
                if (item.ColorIndex != previous)
                    return false;
                else
                    count++;
        }

        if (count != 0 && colorCount[previous] != count)
            return false;
        else
            return true;
    } 

    public void SetFlipped()
    {
        flipped *= -1;
    }
}
