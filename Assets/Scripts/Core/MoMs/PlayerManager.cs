using UnityEngine;

public class PlayerManager : SingleBaseMono<PlayerManager>
{
    private GameObject playerRoot;
    private GameObject currentCharacter;

    public void SpawnCharacter(string characterName)
    {
        // 找到场景中已有的 Player 物体
        playerRoot = GameObject.Find("Player");
        if (playerRoot == null)
        {
            Debug.LogError("Player object not found in scene!");
            return;
        }

        string path = $"Prefabs/Player/{characterName}/{characterName}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab: {path}");
            return;
        }

        currentCharacter = Instantiate(prefab, playerRoot.transform);
        currentCharacter.transform.localPosition = Vector3.zero;

        GameManager.Instance.PlayerTransform = playerRoot.transform;

        var player = currentCharacter.GetComponent<Player>();
        if (player != null)
            player.Init(characterName);
    }

    public void SwitchCharacter(string characterName)
    {
        if (playerRoot == null)
        {
            playerRoot = GameObject.Find("Player");
            if (playerRoot == null)
            {
                return;
            }
        }

        if (currentCharacter != null)
            Destroy(currentCharacter);

        string path = $"Prefabs/Player/{characterName}/{characterName}";
        GameObject prefab = Resources.Load<GameObject>(path);
        currentCharacter = Instantiate(prefab, playerRoot.transform);

        currentCharacter.transform.localPosition = Vector3.zero;

        GameManager.Instance.PlayerTransform = playerRoot.transform;

        var player = currentCharacter.GetComponent<Player>();
        if (player != null)
            player.Init(characterName);
    }
}
