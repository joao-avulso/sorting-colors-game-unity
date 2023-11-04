using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public List<Color> circleColors;
    public int amount = 0;
    public bool isComplete = false;

    private LogicScript gameLogic;
    private readonly int max = 4;

    void Start()
    {
        gameLogic = GameObject.Find("Logic").GetComponent<LogicScript>();
    }

    void OnMouseDown() 
    {
        if (!gameLogic.selected.IsActive && amount > 0)
            PopBall();
        else if (gameLogic.selected.IsActive && amount < max)
            PushBall();
    }

    public void InitContainer(int[] colorCount) 
    {
        if (amount > max) { amount = max; }
        if (amount < 1) { amount = 0; }

        for (int i = 0; i < amount; i++)
        {
            GameObject newCircle = Instantiate(circle, transform);
            newCircle.transform.position += i * newCircle.transform.lossyScale.y * Vector3.up;

            int index = Random.Range(0,circleColors.Count);
            while (colorCount[index] == max) { index = Random.Range(0,circleColors.Count); }
            newCircle.GetComponent<SpriteRenderer>().color = circleColors[index];

            colorCount[index]++;

            Ball newBall = new()
            {
                IsActive = true,
                Obj = newCircle,
                ColorIndex = index,
            };

            balls.Push(newBall);
        }
    }

    void PopBall() 
    {
        if (balls.Count == 0)
            return;

        gameLogic.selected = balls.Pop();
        gameLogic.selected.Obj.transform.parent = null;
        gameLogic.selected.Obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Debug.Log("Selected color: " + gameLogic.selected.ColorIndex);
        amount--;
    }

    void PushBall() 
    {
        Ball newBall = gameLogic.selected;
        gameLogic.selected.Obj.GetComponent<SpriteRenderer>().sortingOrder = 0;
        gameLogic.selected = new() { IsActive = false };
        newBall.Obj.transform.parent = transform;
        if (balls.Count != 0) 
            newBall.Obj.transform.position = balls.Peek().Obj.transform.position + Vector3.up * newBall.Obj.transform.lossyScale.y;
        else
            newBall.Obj.transform.position = origin.transform.position + Vector3.up * newBall.Obj.transform.lossyScale.y/2;
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
}
