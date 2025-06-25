using UnityEngine;

public class PaperCard : MonoBehaviour
{
    public void DestroyCard()
    {
        Debug.Log("🗑 DestroyCard() — карточка помечена на удаление");
        Destroy(gameObject);                 // исчезнет к концу кадра
    }

    void OnDestroy()
    {
        var mgr = FindObjectOfType<SortSceneManager>();
        if (mgr != null)
        {
            mgr.UnregisterCard(gameObject);  // убираем себя из списка
            mgr.CheckForCompletion();        // и проверяем победу
        }
    }
}
