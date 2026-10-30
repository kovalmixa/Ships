using Cysharp.Threading.Tasks;
using UnityEngine;

public class CutsceneWindow : UIWindow
{
    [SerializeField] private UnityEngine.UI.Image _cutsceneImage;
    [SerializeField] private TMPro.TextMeshProUGUI _dialogueText;

    public async UniTask PlaySlideshow(Sprite[] slides, string[] texts)
    {
        for (int i = 0; i < slides.Length; i++)
        {
            _cutsceneImage.sprite = slides[i];
            _dialogueText.text = texts[i];
            await UniTask.Delay(3000);
        }
    }
}
