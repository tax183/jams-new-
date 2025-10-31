using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Round")]
    public int targetStraw = 14;  // ����� ���� 14 ���
    public int hearts = 3;  // 3 ����
    public float roundSeconds = 40f;  // ��� ������

    [Header("References")]
    public ItemSpawner spawner;  // ������� �������� ��� ����ѡ �����...

    int strawCount = 0;  // ��� ���� ���� �� ����
    float endAt;  // ��� ������ ������
    bool ended = false;  // ���� ������ ������

    void OnEnable()
    {
        // �������� �� �������
        GameEvents.OnStrawCollected += HandleStraw;
        GameEvents.OnHeartLost += HandleHeartLost;
        GameEvents.OnInstantFail += HandleInstantFail;
        GameEvents.OnItemCollected += HandleItemCollected;  // ������� �� �������
    }

    void OnDisable()
    {
        // ����� �������� �� �������
        GameEvents.OnStrawCollected -= HandleStraw;
        GameEvents.OnHeartLost -= HandleHeartLost;
        GameEvents.OnInstantFail -= HandleInstantFail;
        GameEvents.OnItemCollected -= HandleItemCollected;
    }

    void Start()
    {
        // ����� ��� ������
        endAt = Time.time + roundSeconds;
        ended = false;
    }

    void Update()
    {
        if (ended) return;

        // ���� ����� �������
        float remaining = Mathf.Max(0f, endAt - Time.time);
        GameEvents.RaiseTimerTick(remaining);  // ����� ������� �� �������

        // ��� ����� �����
        if (remaining <= 0f)
        {
            EndRound(false);
            GameEvents.RaiseTimeUp();  // ��� ����� ��� ������ �����
        }
    }

    // ������� �� ��� ����
    void HandleStraw()
    {
        if (ended) return;

        strawCount++;
        if (strawCount >= targetStraw)
        {
            EndRound(true);  // ����� ��� �� ��� ����� ������� �� ����
            GameEvents.RaiseGoalReached();
        }
    }

    // ������� �� ����� ������
    void HandleHeartLost(int amt)
    {
        if (ended) return;

        hearts -= Mathf.Abs(amt);
        if (hearts <= 0)
        {
            EndRound(false);  // ����� ��� ���� ������
        }
    }

    // ������� �� ����� ������
    void HandleInstantFail()
    {
        if (!ended) EndRound(false);  // ��� ������ �����
    }

    // ������� �� ��� ������� ��� ����� ������
    void HandleItemCollected(ItemType itemType)
    {
        if (ended) return;

        // ��������� ��� ��� �������:
        if (itemType == ItemType.Rock)  // ����� ���� ������
        {
            HandleHeartLost(1);  // ����� �����
        }
        else if (itemType == ItemType.Date)  // ����� ���� �����
        {
            endAt -= 5f;  // ��� 5 ����� �� �����
            GameEvents.RaiseTimerTick(endAt - Time.time);  // ����� �������
        }
        else if (itemType == ItemType.Coin)  // ����� ���� ������ �����
        {
            EndRound(false);  // ��� ������ �����
        }

        // ������ ������ ��� �������
        Destroy(itemType.gameObject);
    }

    // ����� ������ (��� ��� �� �����)
    void EndRound(bool win)
    {
        ended = true;
        if (spawner) spawner.enabled = false;  // ����� ��� spawner
        foreach (var mover in GameObject.FindObjectsOfType<ItemMover>()) mover.enabled = false;  // ����� ����� ������

        // ��� ����� ����� ��� �������
        if (win)
        {
            GameEvents.RaiseWin();
        }
        else
        {
            GameEvents.RaiseLose();
        }
    }

    // ������ ��� ��� ����
    public int GetStraw() => strawCount;

    // ������ ��� ��� ������ ��������
    public int GetHearts() => Mathf.Max(0, hearts);
}
