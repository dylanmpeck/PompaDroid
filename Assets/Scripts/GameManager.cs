using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public LifeBar enemylifeBar;
    public GameObject goIndicator;

    public Hero actor;
    public bool cameraFollows = true;
    public CameraBounds cameraBounds;

    public LevelData currentLevelData;
    private BattleEvent currentBattleEvent;
    private int nextEventIndex;
    public bool hasRemainingEvents;

    public List<GameObject> activeEnemies;
    public Transform[] spawnPositions;

    public GameObject currentLevelBackground;

    public GameObject robotPrefab;
    public GameObject bossPrefab;

    //references to prefabs of the level and gameover texts. 
    public GameObject levelNamePrefab;
    public GameObject gameOverPrefab;

    //parent transform of all UI elements
    public RectTransform uiTransform;

    public GameObject loadingScreen;

    //for hero entrance
    public Transform walkInStartTarget;
    public Transform walkInTarget;

    //for exit on level completion
    public Transform walkOutTarget;

    //for loading and keeping track of levels
    public LevelData[] levels;
    public static int CurrentLevel = 0;

    private GameObject SpawnEnemy(EnemyData data)
    {
        //By calling Instantiate, you create a new GameObject that has all the data from that prefab.
        GameObject enemyObj;
        if (data.type == EnemyType.Boss)
            enemyObj = Instantiate(bossPrefab);
        else
            enemyObj = Instantiate(robotPrefab);

        Vector3 position = spawnPositions[data.row].position;
        position.x = cameraBounds.activeCamera.transform.position.x +
            (data.offset * (cameraBounds.cameraHalfWidth + 1));
        enemyObj.transform.position = position;

        if (data.type == EnemyType.Robot)
            enemyObj.GetComponent<Robot>().SetColor(data.color);

        enemyObj.GetComponent<Enemy>().RegisterEnemy();

        return enemyObj;
    }

    private void PlayBattleEvent(BattleEvent battleEventData)
    {
        currentBattleEvent = battleEventData;
        nextEventIndex++;

        cameraFollows = false;
        cameraBounds.SetXPosition(battleEventData.column);

        //Destroys remnants of prior battle events
        //foreach (GameObject enemy in activeEnemies)
        //    Destroy(enemy);
        activeEnemies.Clear();
        Enemy.TotalEnemies = 0;

        foreach (EnemyData enemyData in currentBattleEvent.enemies)
            activeEnemies.Add(SpawnEnemy(enemyData));
    }

    private void CompleteCurrentEvent()
    {
        currentBattleEvent = null;

        cameraFollows = true;
        cameraBounds.CalculateOffset(actor.transform.position.x);
        hasRemainingEvents = currentLevelData.battleData.Count > nextEventIndex;

        enemylifeBar.EnableLifeBar(false);

        //with no more battle events. hero will walk off screen
        if (!hasRemainingEvents)
            StartCoroutine(HeroWalkout());
        else
            ShowGoIndicator();
    }

    void Start()
    {
        nextEventIndex = 0;
        StartCoroutine(LoadLevelData(levels[CurrentLevel]));
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);
    }

    private void Awake()
    {
        loadingScreen.SetActive(true);
    }

    void Update()
    {
        if (currentBattleEvent == null && hasRemainingEvents)
        {
            if (Mathf.Abs(currentLevelData.battleData[nextEventIndex].column - 
                          cameraBounds.activeCamera.transform.position.x) < 0.2f)
            {
                PlayBattleEvent(currentLevelData.battleData[nextEventIndex]);
            }
        }

        if (currentBattleEvent != null)
        {
            // has event, check if enemies are alive
            if (Robot.TotalEnemies == 0)
                CompleteCurrentEvent();
        }

        if (cameraFollows)
            cameraBounds.SetXPosition(actor.transform.position.x);
    }

    private IEnumerator LoadLevelData(LevelData data)
    {
        cameraFollows = false;
        currentLevelData = data;

        hasRemainingEvents = currentLevelData.battleData.Count > 0;
        activeEnemies = new List<GameObject>();

        //pauses the method for one frame
        yield return null;
        cameraBounds.SetXPosition(cameraBounds.minVisibleX);

        //destroys old level before loading new level
        if (currentLevelBackground != null)
            Destroy(currentLevelBackground);
        currentLevelBackground = Instantiate(currentLevelData.levelPrefab);

        cameraBounds.EnableBounds(false);
        actor.transform.position = walkInStartTarget.transform.position;

        yield return new WaitForSeconds(0.1f);

        actor.UseAutopilot(true);
        actor.AnimateTo(walkInTarget.transform.position, false, DidFinishIntro);

        cameraFollows = true;

        ShowTextBanner(currentLevelData.levelName);

        loadingScreen.SetActive(false);
    }

    private void DidFinishIntro()
    {
        actor.UseAutopilot(false);
        actor.controllable = true;
        cameraBounds.EnableBounds(true);
        ShowGoIndicator();
    }

    private IEnumerator HeroWalkout()
    {
        cameraBounds.EnableBounds(false);
        cameraFollows = false;
        actor.UseAutopilot(true);
        actor.controllable = false;
        actor.AnimateTo(walkOutTarget.transform.position, true, DidFinishWalkout);
        yield return null;
    }

    private void DidFinishWalkout()
    {
        CurrentLevel++;
        if (CurrentLevel >= levels.Length)
        {
            Victory();
        }
        else
            StartCoroutine(AnimateNextLevel());
        cameraBounds.EnableBounds(true);
        cameraFollows = false;
        actor.UseAutopilot(false);
        actor.controllable = false;
    }

    private IEnumerator AnimateNextLevel()
    {
        ShowTextBanner(currentLevelData.levelName + " COMPLETED");
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Game");
    }

    private void ShowGoIndicator()
    {
        StartCoroutine(FlickerGoIndicator(4));
    }

    private IEnumerator FlickerGoIndicator(int count = 4)
    {
        while (count > 0)
        {
            goIndicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            goIndicator.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            count--;
        }
    }

    //ShowBanner creates an instance of its prefab parameter and parents it to the class’
    //uiTransform field.It also sets the title of the prefab’s Text component to the value
    //of the bannerText parameter.
    private void ShowBanner(string bannerText, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.GetComponent<Text>().text = bannerText;
        RectTransform rectTransform = obj.transform as RectTransform;
        rectTransform.SetParent(uiTransform);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void GameOver()
    {
        ShowBanner("GAME OVER", gameOverPrefab);
    }

    public void Victory()
    {
        ShowBanner("YOU WON", gameOverPrefab);
    }

    public void ShowTextBanner(string levelName)
    {
        ShowBanner(levelName, levelNamePrefab);
    }
}
