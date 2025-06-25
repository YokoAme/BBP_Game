using UnityEngine;
using UnityEngine.UI;

public class FolderHighlighter : MonoBehaviour
{
    public Image emptyImage;
    public Image filledImage;
    public FolderType folderType;

    private bool alreadyUsed = false;

    public void OnPaperDropped()
    {
        if (alreadyUsed) return;

        alreadyUsed = true;
        if (emptyImage != null) emptyImage.enabled = false;
        if (filledImage != null) filledImage.enabled = true;

        Debug.Log("Папка активирована!");
    }
}
