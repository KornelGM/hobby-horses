using System;
using UnityEngine;

public class ModalWindowManager :  MonoBehaviour, IManager, IServiceLocatorComponent
{
    public ServiceLocator MyServiceLocator { get; set; }
    
    [SerializeField] private ModalWindowController modalWindowPrefab;
    [SerializeField] private ColorHook yesButtonColor;
    [SerializeField] private ColorHook noButtonColor;
    [ServiceLocatorComponent] private WindowManager _windowManager;

    private BlockingWindowOpener<ModalWindowController> _modalWindowOpener = new();
    
        public ModalWindowController CreateModalWindowYesNo(Action yesAction, Action noAction,
        string title = "", string description = "", bool blockPlayer = false)
    {
        return CreateModalWindowEmpty(blockPlayer, title, description)
            .AddButton(TranslationKeys.ModalYesButton, yesAction)
            .AddButton(TranslationKeys.ModalNoButton, noAction);
    }
    public ModalWindowController CreateModalWindowConfirm(Action action, string title = "", string description = "", bool blockPlayer = false)
    {
        return CreateModalWindowEmpty(blockPlayer, title, description)
            .AddButton(TranslationKeys.ModalConfirmButton, action);
    }
    private ModalWindowController CreateModalWindowEmpty(bool blockPlayer, string title = "", string description = "")
    {
        ModalWindowController windowController;

        if(blockPlayer)
        {
            _modalWindowOpener.OnOpenWindow(modalWindowPrefab, out windowController);
            windowController.OnDeleteWindowAction += () => _modalWindowOpener.UnblockPlayer();
        }
        else
        {
            windowController = _windowManager.CreateWindow(modalWindowPrefab, WindowManager.ModalBasePriority)
            .GetComponent<ModalWindowController>();
        }

        windowController.Title = title;
        windowController.Description = description;
        return windowController;
    }

    public void CustomReset()
    {
        
    }

}