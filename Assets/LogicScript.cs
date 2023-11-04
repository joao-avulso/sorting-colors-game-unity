using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public GameObject container;
    public Ball selected;
    public int colorAmount = 4;

    private int[] colorCount;
    private List<GameObject> containers = new();

    public GameObject gameOverScreen;

    void Start()
    {
        colorCount = new int[colorAmount];
        for (int i = 0; i < colorAmount; i++)
        {
            colorCount[i] = 0;
            Debug.Log("ColorCount: " + colorCount[i]);
        }

        GenerateContainers();
        selected.IsActive = false;
    }

    void Update()
    {
        if (selected.IsActive) 
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selected.Obj.transform.position = (Vector3)mousePos;
        }
    }

    void GenerateContainers() 
    {
        int numContainers = colorAmount + 2;
        int maxAmount = colorAmount * 4;

        Vector3 screenWidthPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height/2, 0));
        Vector3 screenWidthNeg = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width, Screen.height/2, 0));

        for (int i = 0; i < numContainers; i++)
        {
            GameObject newContainer = Instantiate(container, transform.position, transform.rotation);
            newContainer.transform.localScale /= 1.2f;
            Vector3 desPos = Vector3.Lerp(screenWidthPos, screenWidthNeg, (i + 0.5f) / numContainers / 2.0f);
            newContainer.transform.position = new Vector3(desPos.x, desPos.y, 0);

            ContainerScript cScript = newContainer.GetComponent<ContainerScript>();
            if (maxAmount >= colorAmount)
            {
                int curAmount = Random.Range(0, colorAmount+1);
                if (maxAmount <= (i*colorAmount))
                    curAmount = colorAmount;
                cScript.amount = curAmount;
                cScript.InitContainer(colorCount);
                maxAmount -= curAmount;            
            }
            else
            {
                cScript.amount = maxAmount;
                cScript.InitContainer(colorCount);
                maxAmount = 0;
            }

            containers.Add(newContainer);
        }
    }

    public void CheckEndGame() {
        foreach (var item in containers)
        {
            if (!item.GetComponent<ContainerScript>().VerifyContainer(colorCount))
            {
                gameOverScreen.SetActive(false);
                return;
            }
        }
        gameOverScreen.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
