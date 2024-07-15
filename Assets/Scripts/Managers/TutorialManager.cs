using UnityEngine;

public class TutorialManager : MonoBehaviour, IServiceLocatorComponent, IStartable, IAwake, ISaveable<SaveData>
{
    public ServiceLocator MyServiceLocator { get; set; }

    public bool IsTutorial;
    public bool ReputationBlocking;
    public bool FirstChocolateNougat;
    public bool BlockGenerateOrders;
    public bool CanOpenBook;

    [ServiceLocatorComponent] private PopUpManager _popUpManager;

    [SerializeField] private PopUp _tutorialPopup;

    public void CustomStart()
    {
        
    }

    public void CustomAwake()
    {
        
    }

    private void InitializeNewTutorial()
    {
        IsTutorial = true;
        CanOpenBook = false;
        FirstChocolateNougat = false;
        ReputationBlocking = false;
        BlockGenerateOrders= false;

        Invoke(nameof(SendPopup), 2f);
    }

    private void SendPopup()
    {
        _popUpManager.AddPopUpToBlockingQueue(_tutorialPopup);
    }

    public SaveData CollectData(SaveData data)
    {
        data.IsTutorial = IsTutorial;
        data.CanOpenBook = CanOpenBook;
        data.FirstChocolateNougat = FirstChocolateNougat;
        data.ReputationBlocking = ReputationBlocking;
        data.BlockGenerateOrders = BlockGenerateOrders;
        return data;
    }

    public void Initialize(SaveData save)
    {
        if (save == null)
        {
            InitializeNewTutorial();
            return;
        }

        CanOpenBook = save.CanOpenBook;
        IsTutorial = save.IsTutorial;
        FirstChocolateNougat = save.FirstChocolateNougat;
        ReputationBlocking = save.ReputationBlocking;
        BlockGenerateOrders = save.BlockGenerateOrders;

        if(IsTutorial && !CanOpenBook)
            Invoke(nameof(SendPopup), 2f);

    }

    public void EndTutorial(bool value)
    {
        IsTutorial = value;
    }

    public void BlockReputation(bool value)
    {
        ReputationBlocking = value;
    }

    public void FirstChocolateDone(bool value)
    {
        FirstChocolateNougat = value;
    }
    public void GenerateOrdersBlock(bool value)
    {
        BlockGenerateOrders = value;
    }
}
