using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Round")]
    public int targetStraw = 14;  // ÇáåÏİ áÌãÚ 14 ŞÔÉ
    public int hearts = 3;  // 3 ŞáæÈ
    public float roundSeconds = 40f;  // ãÏÉ ÇááÚÈÉ

    [Header("References")]
    public ItemSpawner spawner;  // áßÇÆäÇÊ ÇáãÚæŞÇÊ ãËá ÇáÊãÑ¡ ÇáÍÌÑ...

    int strawCount = 0;  // ÚÏÏ ÇáŞÔ ÇáĞí Êã ÌãÚå
    float endAt;  // æŞÊ ÇäÊåÇÁ ÇáÌæáÉ
    bool ended = false;  // ÍÇáÉ ÇäÊåÇÁ ÇááÚÈÉ

    void OnEnable()
    {
        // ÇáÇÔÊÑÇß İí ÇáÃÍÏÇË
        GameEvents.OnStrawCollected += HandleStraw;
        GameEvents.OnHeartLost += HandleHeartLost;
        GameEvents.OnInstantFail += HandleInstantFail;
        GameEvents.OnItemCollected += HandleItemCollected;  // ÇáÊÚÇãá ãÚ ÇáÚäÇÕÑ
    }

    void OnDisable()
    {
        // ÅáÛÇÁ ÇáÇÔÊÑÇß İí ÇáÃÍÏÇË
        GameEvents.OnStrawCollected -= HandleStraw;
        GameEvents.OnHeartLost -= HandleHeartLost;
        GameEvents.OnInstantFail -= HandleInstantFail;
        GameEvents.OnItemCollected -= HandleItemCollected;
    }

    void Start()
    {
        // ÊÍÏíÏ æŞÊ ÇááÚÈÉ
        endAt = Time.time + roundSeconds;
        ended = false;
    }

    void Update()
    {
        if (ended) return;

        // ÍÓÇÈ ÇáæŞÊ ÇáãÊÈŞí
        float remaining = Mathf.Max(0f, endAt - Time.time);
        GameEvents.RaiseTimerTick(remaining);  // ÊÍÏíË ÇáÊÇíãÑ İí ÇáæÇÌåÉ

        // ÅĞÇ ÇäÊåì ÇáæŞÊ
        if (remaining <= 0f)
        {
            EndRound(false);
            GameEvents.RaiseTimeUp();  // ÑİÚ ÇáÍÏË ÚäÏ ÇäÊåÇÁ ÇáæŞÊ
        }
    }

    // ÇáÊÚÇãá ãÚ ÌãÚ ÇáŞÔ
    void HandleStraw()
    {
        if (ended) return;

        strawCount++;
        if (strawCount >= targetStraw)
        {
            EndRound(true);  // ÇáİæÒ ÅĞÇ Êã ÌãÚ ÇáÚÏÏ ÇáãØáæÈ ãä ÇáŞÔ
            GameEvents.RaiseGoalReached();
        }
    }

    // ÇáÊÚÇãá ãÚ İŞÏÇä ÇáŞáæÈ
    void HandleHeartLost(int amt)
    {
        if (ended) return;

        hearts -= Mathf.Abs(amt);
        if (hearts <= 0)
        {
            EndRound(false);  // ÎÓÇÑÉ ÅĞÇ äİÏÊ ÇáŞáæÈ
        }
    }

    // ÇáÊÚÇãá ãÚ ÇáİÔá ÇáİæÑí
    void HandleInstantFail()
    {
        if (!ended) EndRound(false);  // ãæÊ ÇááÇÚÈ İæÑğÇ
    }

    // ÇáÊÚÇãá ãÚ ÌãÚ ÇáÚäÇÕÑ ãËá ÇáÊãÑ æÇáÍÌÑ
    void HandleItemCollected(ItemType itemType)
    {
        if (ended) return;

        // ÇáÊÃËíÑÇÊ ÚäÏ ÌãÚ ÇáÚäÇÕÑ:
        if (itemType == ItemType.Rock)  // ÇáÍÌÑ íŞáá ÇáŞáæÈ
        {
            HandleHeartLost(1);  // ÊŞáíá ÇáŞáÈ
        }
        else if (itemType == ItemType.Date)  // ÇáÊãÑ íŞáá ÇáæŞÊ
        {
            endAt -= 5f;  // ÎÕã 5 ËæÇäò ãä ÇáæŞÊ
            GameEvents.RaiseTimerTick(endAt - Time.time);  // ÊÍÏíË ÇáÊÇíãÑ
        }
        else if (itemType == ItemType.Coin)  // ÇáãÇá íãæÊ ÇááÇÚÈ İæÑğÇ
        {
            EndRound(false);  // ãæÊ ÇááÇÚÈ İæÑğÇ
        }

        // ÇÎÊİÇÁ ÇáÚäÕÑ ÈÚÏ ÇáÊİÇÚá
        Destroy(itemType.gameObject);
    }

    // ÅäåÇÁ ÇáÌæáÉ (ÅãÇ İæÒ Ãæ ÎÓÇÑÉ)
    void EndRound(bool win)
    {
        ended = true;
        if (spawner) spawner.enabled = false;  // ÊÚØíá ÇáÜ spawner
        foreach (var mover in GameObject.FindObjectsOfType<ItemMover>()) mover.enabled = false;  // ÊÚØíá ÚäÇÕÑ ÇááÚÈÉ

        // ÑİÚ ÇáÍÏË ÈäÇÁğ Úáì ÇáäÊíÌÉ
        if (win)
        {
            GameEvents.RaiseWin();
        }
        else
        {
            GameEvents.RaiseLose();
        }
    }

    // ÇáÍÕæá Úáì ÚÏÏ ÇáŞÔ
    public int GetStraw() => strawCount;

    // ÇáÍÕæá Úáì ÚÏÏ ÇáŞáæÈ ÇáãÊÈŞíÉ
    public int GetHearts() => Mathf.Max(0, hearts);
}
