using UnityEngine;
//using Yarn.Unity;
using Cysharp.Threading.Tasks;
using Assets.Handlers.SceneHandlers;

//public class YarnBridge : MonoBehaviour
//{
//    [SerializeField] private DialogueRunner _dialogueRunner;
//    [SerializeField] private CutsceneDatabase _cutsceneDb;

//    private void Awake()
//    {
//        // Регистрируем асинхронную команду для Yarn Spinner
//        // В самом файле .yarn это будет вызываться как: <<play_slideshow intro_cutscene>>
//        _dialogueRunner.AddCommandHandler<string>("play_slideshow", PlayCutsceneFromYarn);
//    }

//    private async UniTask PlayCutsceneFromYarn(string cutsceneId)
//    {
//        // 1. Получаем данные слайдов и текста из нашей базы данных по ID
//        var slidesData = _sceneDb.GetCutscene(cutsceneId);

//        if (slidesData == null) return;

//        // 2. Просим WindowManager открыть независимое окно катсцены
//        // (Оно должно быть предварительно загружено или лежать в базе окон)
//        var window = WindowManager.Instance.GetWindow("CutsceneWindow") as CutsceneWindow;

//        if (window != null)
//        {
//            // Открываем окно
//            await window.OpenAsync();

//            // Запускаем прокрутку слайдов и ЖДЕМ (Yarn Spinner будет на паузе, пока идет катсцена)
//            await window.PlayCutscene(slidesData);

//            // Закрываем окно после окончания
//            await window.CloseAsync();
//        }
//    }
//}
