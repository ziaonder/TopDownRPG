using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public Transform playerTransform, forestBossTransform, desertBossTransform, arcticBossTransform;
    private string save;
    private Vector3 forestBossPosition, desertBossPosition, arcticBossPosition;
    private void Awake()
    {
        forestBossPosition = new Vector3(125f, -20f, 0f);
        desertBossPosition = new Vector3(-220f, -20f, 0f);
        arcticBossPosition = new Vector3(-20f, 155f, 0f);

        save = PlayerPrefs.GetInt("SaveSlot").ToString();
        if(PlayerPrefs.GetString(save) == "saved")
            LoadGame(save);
        else
            NewGame(save);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame(save);
            Debug.Log("saved");
        }
    }

    public static void DeleteSave(string saveSlot)
    {
        PlayerPrefs.SetString(saveSlot, " ");
    }
    
    private void NewGame(string saveSlot)
    {
        PlayerPrefs.SetString("isAxeBought" + saveSlot, "false");
        PlayerPrefs.SetString("isSpearBought" + saveSlot, "false");
        PlayerPrefs.SetString("isPistolBought" + saveSlot, "false"); ;

        Vector3 initalPlayerPosition = Vector3.zero;
        playerTransform.position = initalPlayerPosition;

        #region Loading Arrangements To Player
        playerTransform.GetComponent<PlayerMovement>().SetTargetToReached();
        playerTransform.GetComponent<AnimationHandler>().ChangeAnimation("isRunning", false);
        playerTransform.GetComponent<AnimationHandler>().ChangeAnimation("isIdle", true);
        PlayerHealth.health = 100;
        //PlayerHealth.GetDamage(0);
        #endregion

        Vector3 forestBossPos = forestBossPosition;
        forestBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        forestBossTransform.GetComponent<PatrolManager>().SetHealth(100);
        forestBossTransform.position = forestBossPos;

        Vector3 desertBossPos = desertBossPosition;
        desertBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        desertBossTransform.GetComponent<PatrolManager>().SetHealth(100);
        desertBossTransform.position = desertBossPos;

        Vector3 arcticBossPos = arcticBossPosition;
        arcticBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        arcticBossTransform.GetComponent<PatrolManager>().SetHealth(100);
        arcticBossTransform.position = arcticBossPos;
    }
    private void LoadGame(string saveSlot)
    {
        // Player Loading
        Vector3 position = new Vector3(PlayerPrefs.GetFloat("playerPosX" + saveSlot), 
            PlayerPrefs.GetFloat("playerPosY" + saveSlot), PlayerPrefs.GetFloat("playerPosZ" + saveSlot));
        playerTransform.position = position;

        #region Loading Arrangements To Player
        playerTransform.GetComponent<PlayerMovement>().SetTargetToReached();
        playerTransform.GetComponent<AnimationHandler>().ChangeAnimation("isRunning", false);
        playerTransform.GetComponent<AnimationHandler>().ChangeAnimation("isIdle", true);
        //Debug.Log(PlayerPrefs.GetInt("playerHP"));
        PlayerHealth.health = PlayerPrefs.GetInt("playerHP" + saveSlot);
        PlayerHealth.gold = PlayerPrefs.GetInt("playerGold" + saveSlot);
        //PlayerHealth.GetDamage(0);
        #endregion

        // ForestBoss Loading
        Vector3 forestBossPos =
            new Vector3(PlayerPrefs.GetFloat("forestBossPosX" + saveSlot), 
            PlayerPrefs.GetFloat("forestBossPosY" + saveSlot), PlayerPrefs.GetFloat("forestBossPosZ" + saveSlot));
        forestBossTransform.position = forestBossPos;
        forestBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        forestBossTransform.GetComponent<PatrolManager>().SetHealth(PlayerPrefs.GetInt("forestBossHP" + saveSlot));

        // DesertBoss Loading
        Vector3 desertBossPos =
            new Vector3(PlayerPrefs.GetFloat("desertBossPosX" + saveSlot), 
            PlayerPrefs.GetFloat("desertBossPosY" + saveSlot), PlayerPrefs.GetFloat("desertBossPosZ" + saveSlot));
        desertBossTransform.position = desertBossPos;
        desertBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        desertBossTransform.GetComponent<PatrolManager>().SetHealth(PlayerPrefs.GetInt("desertBossHP" + saveSlot));

        // ArcticBoss Loading
        Vector3 arcticBossPos =
            new Vector3(PlayerPrefs.GetFloat("arcticBossPosX" + saveSlot), 
            PlayerPrefs.GetFloat("arcticBossPosY" + saveSlot), PlayerPrefs.GetFloat("arcticBossPosZ" + saveSlot));
        arcticBossTransform.position = arcticBossPos;
        arcticBossTransform.GetComponent<PatrolManager>().SetStateToReached();
        arcticBossTransform.GetComponent<PatrolManager>().SetHealth(PlayerPrefs.GetInt("arcticBossHP" + saveSlot));
    }

    public void SaveGame(string saveSlot)
    {
        PlayerPrefs.SetString(saveSlot, "saved");
        // Player Position
        PlayerPrefs.SetFloat("playerPosX" + saveSlot, playerTransform.position.x);
        PlayerPrefs.SetFloat("playerPosY" + saveSlot, playerTransform.position.y);
        PlayerPrefs.SetFloat("playerPosZ" + saveSlot, playerTransform.position.z);
        PlayerPrefs.SetInt("playerHP" + saveSlot, PlayerHealth.health);
        PlayerPrefs.SetInt("playerGold" + saveSlot, PlayerHealth.gold);

        // ForestBoss Position
        PlayerPrefs.SetFloat("forestBossPosX" + saveSlot, forestBossTransform.position.x);
        PlayerPrefs.SetFloat("forestBossPosY" + saveSlot, forestBossTransform.position.y);
        PlayerPrefs.SetFloat("forestBossPosZ" + saveSlot, forestBossTransform.position.z);
        PlayerPrefs.SetInt("forestBossHP" + saveSlot, forestBossTransform.GetComponent<PatrolManager>().GetHealth());

        // DesertBoss Position
        PlayerPrefs.SetFloat("desertBossPosX" + saveSlot, desertBossTransform.position.x);
        PlayerPrefs.SetFloat("desertBossPosY" + saveSlot, desertBossTransform.position.y);
        PlayerPrefs.SetFloat("desertBossPosZ" + saveSlot, desertBossTransform.position.z);
        PlayerPrefs.SetInt("desertBossHP" + saveSlot, desertBossTransform.GetComponent<PatrolManager>().GetHealth());

        // ArcticBoss Position
        PlayerPrefs.SetFloat("arcticBossPosX" + saveSlot, arcticBossTransform.position.x);
        PlayerPrefs.SetFloat("arcticBossPosY" + saveSlot, arcticBossTransform.position.y);
        PlayerPrefs.SetFloat("arcticBossPosZ" + saveSlot, arcticBossTransform.position.z);
        PlayerPrefs.SetInt("arcticBossHP" + saveSlot, arcticBossTransform.GetComponent<PatrolManager>().GetHealth());
    }
    

}
