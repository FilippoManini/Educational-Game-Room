using System.Collections;
using Assets.DM.Script.Utilities;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    private GameObject screen;
    public int IndexToLoad
    {
        get => gameInfo.firstSceneIndex + gameInfo.sceneMaterialIndex;
    }
    
    [Header("Canvas")]
    public GameObject canvasImage;

    // Level move zoned enter, if collider is a player
    // Move game to another scene

    [Header("Texture")]
    private static MeshRenderer _meshRenderer;
    private Object[] renderTextures;    // List of all the arcade's render textures
    public string gameName;
    private GameInfos gameInfo;          // GameInfo script


    // 2D Trigger
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Could use other.GetComponent<Player>() to see if the game object has a player component
        // Tags work too, maybe some players have different script components?
        Debug.Log("restart level: collison tag: "+ collision.tag);
        if (collision.tag == "Player")
        {
            // take the game property
            gameInfo = GameDatabase.games[gameName];
            SceneManager.UnloadSceneAsync(IndexToLoad -1);
            Debug.Log("restart level: this tag: " + gameObject.tag);
            if (CompareTag("Restart"))
            {
                Debug.Log("restart level");
                LoadNextLevel(true);
            }
            else
            {
                LoadNextLevel(false);
                
                ChangeRenderTexture(gameInfo.sceneMaterialIndex);
                // Increse the counter for the next level
                UpdateCounter();
            }
        }
    }

    // 3D Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Could use other.GetComponent<Player>() to see if the game object has a player component
        // Tags work too, maybe some players have different script components?

        if (other.tag == "Player")
        {
            Executor.variabili.Clear();

            _meshRenderer = transform.Find("Screen").GetComponent<MeshRenderer>();
            // take the game property
            gameInfo = GameDatabase.games[gameName];
            LoadNextLevel(false);

            StartCoroutine(ActiveMessage());

            // Increse the counter for the next level
            UpdateCounter();

            screen = transform.Find("Screen").gameObject;
            screen.SetActive(true);
        }
    }

    private void LoadNextLevel(bool restart)
    {
        
        // Get the game's material
        renderTextures = Resources.LoadAll("RenderTextures/" + gameInfo.sceneMaterialPath, typeof(Material));

        // Player entered, so move level
        if (restart)
            SceneManager.LoadScene(IndexToLoad-1, LoadSceneMode.Additive);
        else
            SceneManager.LoadScene(IndexToLoad, LoadSceneMode.Additive);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.UnloadSceneAsync(IndexToLoad - 1);
            gameInfo.sceneMaterialIndex--;
            
            if (gameInfo.isCompleted)
                gameInfo.isCompleted = false;

            screen.SetActive(false);
        }
    }

    private IEnumerator ActiveMessage()
    {
        if(canvasImage != null)
        {
            Transform canvasText = canvasImage.transform.Find("Text");
            canvasText.GetComponent<TextMeshProUGUI>().text = "Press 'F2' to start/stop playing";
            canvasImage.SetActive(true);
            yield return new WaitForSeconds(3);
            canvasImage.SetActive(false);
        }
    }

    public void ChangeRenderTexture(int orderInFolder)
    {
        // Change the plane's Render Texture
        _meshRenderer.material = (Material)renderTextures[orderInFolder];
    }

    public void UpdateCounter()
    {
        if (gameInfo.sceneMaterialIndex >= gameInfo.numLevels)
        {
            gameInfo.isCompleted = true;
        }
        
        gameInfo.sceneMaterialIndex++;
    }
}
