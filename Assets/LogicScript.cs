using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject nextLevelScreen;
    public GameObject container;
    private List<GameObject> containers = new();

    public Ball selected;
    public Color[] colors;

    public int health = 100;
    public int colorAmount = 4;
    private int[] colorCount;

    public float animSpeed = 0.1f;
    public float moveChance = 0.9f;

    private bool active = false;

    public Text levelText;
    public Text healthText;

    void Start()
    {
        levelText.text += " " + (SceneManager.GetActiveScene().buildIndex + 1);
        healthText.text += health;
        colorCount = new int[colorAmount];
        GenerateContainers();
        Invoke(nameof(GenerateColors), (colorAmount + 2) * animSpeed);
        selected.IsActive = false;
    }

    void Update()
    {
        if (selected.IsActive) 
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selected.Obj.transform.position = (Vector3)mousePos;
        }

        if (health < 1)
        {
            SetActiveFalse();
            gameOverScreen.SetActive(true);
        }
    }

    void GenerateContainers() 
    {
        int numContainers = colorAmount + 2;

        Vector3 screenWidthPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height/2, 0));
        Vector3 screenWidthNeg = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width, Screen.height/2, 0));

        for (int i = 0; i < numContainers; i++)
        {
            GameObject newContainer = Instantiate(container, transform.position, transform.rotation);
            if (numContainers > 6)
                newContainer.transform.localScale /= 2.0f;
            else
                newContainer.transform.localScale /= 1.2f;
            Vector3 desPos = Vector3.Lerp(screenWidthPos, screenWidthNeg, (i + 0.5f) / numContainers / 2.0f);
            newContainer.transform.position = new Vector3(desPos.x, -8);
            newContainer.transform.LeanMoveLocalY(desPos.y, animSpeed).setDelay(i * animSpeed).setEaseOutCirc();
            containers.Add(newContainer);
        }
    }

    void GenerateColors()
    {
        int maxAmount = colorAmount * 4;
        int count = 0;

        while(maxAmount > 0)
        {   
            // pick color
            int colorIndex = Random.Range(0, colorAmount);
            while (colorCount[colorIndex] == 4) { colorIndex = Random.Range(0, colorAmount); }
            colorCount[colorIndex]++;
            // pick container
            int containerIndex = Random.Range(0, containers.Count);
            while (containers[containerIndex].GetComponent<ContainerScript>().amount == 4) { containerIndex = Random.Range(0, containers.Count); }
            // generate
            containers[containerIndex].GetComponent<ContainerScript>().AddColor(colors[colorIndex], colorIndex, count);
            maxAmount--;
            count++;
        }
        Invoke(nameof(SetActiveTrue), count * animSpeed/2);
    }

    public void CheckEndGame() 
    {
        foreach (var item in containers)
        {
            if (!item.GetComponent<ContainerScript>().VerifyContainer(colorCount))
            {
                nextLevelScreen.SetActive(false);
                return;
            }
        }
        nextLevelScreen.SetActive(true);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetActiveTrue()
    {
        active = true;
    }

    public void SetActiveFalse()
    {
        active = false;
    }

    public bool GetActive()
    {
        return active;
    }

    public List<GameObject> GetContainers()
    {
        return containers;
    }
}
