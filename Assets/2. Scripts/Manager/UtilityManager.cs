using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Image = UnityEngine.UI.Image;
using UnityEngine.Pool;

public class UtilityManager : MonoBehaviour
{
    private AudioSource audioSource;
    // 싱글톤 선언
    public static UtilityManager utility = null;
    void Awake()
    {
        if (utility == null)
        {
            utility = this;
        }
        else if (utility != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public Vector2 HorizontalDirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if (Mathf.Approximately(move.x, 0) == false)
        {
            moveDir.Set(move.x, 0);
            moveDir.Normalize();
        }
        return moveDir;
    }

    public Vector2 AllDirSet(Vector2 move)
    {
        Vector2 moveDir = new Vector2(0, 0);
        if (Mathf.Approximately(move.x, 0) == false || Mathf.Approximately(0, move.y) == false)
        {
            moveDir.Set(move.x, move.y);
            moveDir.Normalize();
        }
        return moveDir;
    }


    public Dictionary<GameObject, float> ItemNormalizer(Dictionary<GameObject, float> inputItem)
    {
        List<GameObject> keys = new List<GameObject>(inputItem.Keys);
        float sumValues = inputItem.Sum(item => item.Value);
        foreach (GameObject elem in keys)
        {
            inputItem[elem] = (inputItem[elem] / sumValues) * 100.0f;
        }
        return inputItem;
    }


    // 알파 변경자   
    public IEnumerator ChangeAlpha(Image changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float startAlpha = currentColor.a;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while (time < 1.0f)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }

    public IEnumerator ChangeAlpha(SpriteRenderer changeTarget, float targetAlpah, float changeTime)
    {
        Color currentColor = changeTarget.color;
        float startAlpha = currentColor.a;
        float time = 0.0f;

        // 현재 알파가 목표 알파보다 작은 동안 점진적으로 알파 값 변경
        while (time < 1.0f)
        {
            time += Time.deltaTime / changeTime;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpah, time);
            changeTarget.color = currentColor;
            yield return null;
        }
    }

    // 데미지 입으면 깜빡거리기 코루틴
    public IEnumerator BlinkOnDamage(SpriteRenderer spriteRenderer, float blinkTime)
    {
        bool isBlink = false;
        Color color = spriteRenderer.color;

        float maxBlinkTime = 1.0f; // 깜빡이는 총 시간
        float currentBlinkTIme = 0.0f;

        // 데미지를 입으면 깜빡임
        while(currentBlinkTIme < maxBlinkTime)
        {
            // 이전 상태 깜빡이면 되돌리기, 일반이면 깜빡임 반복시켜서 효과 적용
            if(isBlink == true)
            {
                color.a = 0.0f;
                spriteRenderer.color = color;
                isBlink = false;
            }
            else if(isBlink == false)
            {
                color.a = 1.0f;
                spriteRenderer.color = color;
                isBlink = true;
            }
            currentBlinkTIme += blinkTime;
            yield return new WaitForSeconds(blinkTime);
        }
        // 기본 상태로 초기화
        color.a = 1.0f;
        spriteRenderer.color = color;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    // 풀 관리 메서드들
    public void CreatePool(ref ObjectPool<GameObject> pool, GameObject prefab, int count, int max)
    {
        // pool 생성
        pool = new ObjectPool<GameObject>
        (
            createFunc: () => Instantiate(prefab),
            actionOnGet: (go) => go.SetActive(true),
            actionOnRelease: (go) => go.SetActive(false),
            actionOnDestroy: (go) => Destroy(go),
            collectionCheck: true,
            defaultCapacity: count,
            maxSize: max
        );
    }
    public void CreateDoNotDestroyPool(ref ObjectPool<GameObject> pool, GameObject prefab, int count, int max)
    {
        // pool 생성
        pool = new ObjectPool<GameObject>
        (
            createFunc: () =>
            {
                GameObject go = Instantiate(prefab);
                DontDestroyOnLoad(go);
                return go;
            },
            actionOnGet: (go) => go.SetActive(true),
            actionOnRelease: (go) => go.SetActive(false),
            actionOnDestroy: (go) => Destroy(go),
            collectionCheck: true,
            defaultCapacity: count,
            maxSize: max
        );
    }

    public ObjectPool<GameObject> CreatePlayerMagicPool(GameObject prefab, int count = 5, int max = 20)
    {

        if (GameManager.instance.magicPools.TryGetValue(prefab, out var pool)) return pool;

        // pool 생성
        pool = new ObjectPool<GameObject>
        (
            createFunc: () => Instantiate(prefab),
            actionOnGet: (go) => go.SetActive(true),
            actionOnRelease: (go) => go.SetActive(false),
            actionOnDestroy: (go) => Destroy(go),
            collectionCheck: true,
            defaultCapacity: count,
            maxSize: max
        );

        GameManager.instance.magicPools.Add(prefab, pool);
        return pool;
    }

    public T GetFromPool<T>(ObjectPool<T> pool, int maxActive) where T : UnityEngine.Object
    {
        if (pool.CountActive >= maxActive)
        {
             return null;
        }
        return pool.Get();
    }

    public void ReturnToPool<T>(ObjectPool<T> pool, T obj) where T : UnityEngine.Object
    {
        pool.Release(obj);
    }

    public void SetItemFromPool(Transform targetTransform, GameObject target)
    {
        ObjectPool<GameObject> pool = ItemManager.itemManager.SelectPool(target);
        target = GetFromPool(pool, 10);
        // target == null이란 것은 아이템 최대 생성치 초과했다는 뜻
        // 현재 겜 설계에서 한 씬에 한 아이템이 10 이상일 확률은 희박하니 그냥 드롭 안하게 처리
        if (target != null)
        {
            target.transform.position = targetTransform.position;
            target.transform.rotation = targetTransform.rotation;
        }
    }
    
    // Player 마법 관련
}
