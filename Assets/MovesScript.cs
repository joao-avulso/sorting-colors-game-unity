using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovesScript : MonoBehaviour
{
    private List<GameObject> containers;
    public GameObject gameCamera;

    private LogicScript gameLogic;

    private float animSpeed;
    public float delay = 1;

    private int damageCount = 0;
    public int damage = 10;

    void Start()
    {
        gameLogic = transform.GetComponent<LogicScript>();
        animSpeed = gameLogic.animSpeed;
    }

    public void ChooseMove(GameObject container)
    {
        containers ??= gameLogic.GetContainers();
        int level = 1;
        if (SceneManager.GetActiveScene().buildIndex > 4)
            level = 2;
        if (SceneManager.GetActiveScene().buildIndex > 6)
            level = 3;
        if (SceneManager.GetActiveScene().buildIndex > 8)
            level = 4;
        if (SceneManager.GetActiveScene().buildIndex > 10)
            level = 5;
        
        float rand = Random.Range(0, level);

        if (SceneManager.GetActiveScene().buildIndex == 8)
            rand = 2;

        if (SceneManager.GetActiveScene().buildIndex == 11)
            rand = 4;

        switch (rand)
        {
            case 0:
                Swap(container);
                break;
            case 1:
                Flip(container);
                break;
            case 2:
                Shuffle(containers.Count);
                break;
            case 3:
                FlipCam();
                break;
            case 4:
                Damage();
                break;
        }
    }

    public void MoveDown(GameObject container)
    {
        float previous = container.transform.position.y;
        container.LeanMoveY(previous - 8, animSpeed).setEaseInOutBounce();
        container.LeanMoveY(previous, animSpeed).setDelay(1.5f*delay).setEaseInOutBounce();
    }

    public void Swap(GameObject cont1)
    {
        // pick second container
        int index = Random.Range(0, containers.Count);
        while (index == containers.IndexOf(cont1)) { index = Random.Range(0, containers.Count); }
        GameObject cont2 = containers[index];

        // save positions
        float cont1_x = cont1.transform.position.x;
        float cont2_x = cont2.transform.position.x;
        float previous = cont1.transform.position.y;

        // move
        cont1.LeanMoveY(previous - 8, animSpeed).setEaseInOutCirc();
        cont2.LeanMoveY(previous - 8, animSpeed).setEaseInOutCirc();
        cont1.LeanMoveX(cont2_x, animSpeed).setDelay(animSpeed);
        cont2.LeanMoveX(cont1_x, animSpeed).setDelay(animSpeed);
        cont1.LeanMoveY(previous, animSpeed).setDelay(animSpeed*2*delay).setEaseInOutCirc();
        cont2.LeanMoveY(previous, animSpeed).setDelay(animSpeed*2*delay).setEaseInOutCirc();
    }

    public void Flip(GameObject container)
    {
        container.LeanRotateZ(180, animSpeed).setEaseInOutCirc();
        container.GetComponent<ContainerScript>().SetFlipped();
        gameLogic.SetActiveFalse();
        gameLogic.Invoke("SetActiveTrue", animSpeed);
        container.LeanRotateZ(0, animSpeed).setDelay(2.0f * delay).setEaseInOutCirc();
        container.GetComponent<ContainerScript>().Invoke("SetFlipped", (2.0f * delay) + animSpeed);
        gameLogic.Invoke("SetActiveFalse", 2.0f * delay);
        gameLogic.Invoke("SetActiveTrue", (2.0f * delay) + animSpeed);
    }

    public void FlipCam()
    {
        gameCamera.transform.LeanRotateZ(180, 0.3f).setEaseInOutCirc();
        gameCamera.transform.LeanRotateZ(0, 0.3f).setDelay(5f).setEaseInOutCirc();
    }

    [ContextMenu("Shuffle")]
    public void ShuffleMajor()
    {
        containers ??= gameLogic.GetContainers();
        Shuffle(containers.Count);
    }

    public void Shuffle(int n)
    {
        if (n == 0)
            return;

        int k = Random.Range(0, n);
        n--;

        float previous_x = containers[n].transform.position.x;
        float previous_y = containers[n].transform.position.y;

        containers[n].LeanMoveLocalX(containers[k].transform.position.x, animSpeed).setEaseInOutCirc();
        containers[k].LeanMoveLocalX(previous_x, animSpeed).setEaseInOutCirc().setOnComplete(() => Shuffle(n));
    }

    [ContextMenu("Damage")]
    public void Damage()
    {
        if (damageCount == 0)
        {
            gameLogic.healthText.transform.LeanMoveX(gameLogic.healthText.transform.position.x + 300, 0.5f).setDelay(0.3f).setEaseInOutCirc()
            .setOnComplete(() => {
                gameLogic.health -= damage;
                UpdateHealth();
                });
        }
        else
        {
            gameLogic.health -= damage;
            UpdateHealth();
        }
        damageCount++;
    }

    [ContextMenu("UpdateHealth")]
    void UpdateHealth()
    {
        // LeanTween.cancel(gameLogic.healthText.gameObject);

        gameCamera.transform.LeanRotateZ(10, 0.3f).setDelay(0.01f).setEasePunch();

        gameLogic.healthText.transform.LeanScale(Vector3.one * 10, 0.5f).setDelay(0.1f).setEasePunch()
                .setOnStart( () => { 
                    gameLogic.healthText.text = "Vida: " + gameLogic.health;
                    gameLogic.healthText.color = Color.red;
                });

        LeanTween.value(gameLogic.healthText.gameObject, 0.1f, 1f, 2f )
                .setEaseInOutCirc()
                .setOnUpdate( (value) =>
                    {
                        gameLogic.healthText.color = Color.Lerp(Color.red, Color.white, value);
                    });
    }
}
