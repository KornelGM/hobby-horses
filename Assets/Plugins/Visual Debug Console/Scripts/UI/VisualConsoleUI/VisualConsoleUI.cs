using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VisualDebug
{
    [Serializable]
    public class PanelConnectedWithValue
    {
        public GameObject Panel;
        public TextMeshProUGUI NameText;
        public TMP_InputField ValueField;
        public string PropertyName;
        public object ParentObject;
    }
    public class MethodButton
    {
        public List<TMP_InputField> inputFields;
        public List<ParameterInfo> parameterTypes;
        public object parentObject;
        public MethodInfo methodInfo;


        public void InvokeMethod()
        {
            List<object> values = new();
            bool error=false;
            for (int i = 0; i < inputFields.Count; i++)
            {
                try
                {
                    values.Add(Convert.ChangeType(inputFields[i].text, parameterTypes[i].ParameterType));
                }
                catch(Exception)
                {
                    error=true;
                    Debug.Log($"Error while trying to parse <color=red>{parameterTypes[i].Name}</color> parameter");
                }
            }
            if(error)
            {
                return;
            }
            methodInfo.Invoke(parentObject, values.ToArray());
        }
    }

    public class Card
    {
        public string CardName;
        public List<Transform> CardContent = new List<Transform>();
        public Tree<AccordionUI> categoriesTree = new Tree<AccordionUI>();
        public Card Previous = null;

        public void DisplayAll()
        {
            foreach (Transform element in CardContent)
            {
                element.gameObject.SetActive(true);
            }
        }

        public void AddChild(Transform childToAdd)
        {
            CardContent.Add(childToAdd);
        }

        public void HideAll()
        {
            foreach (Transform element in CardContent)
            {
                element.gameObject.SetActive(false);
            }
        }
    }

    public partial class VisualConsoleUI : MonoBehaviour
    {
        public static event Action OnCardChanged;


        [SerializeField] private GameObject _propertyPanel = null;
        [SerializeField] private Transform _panelParent = null;
        [SerializeField] private GameObject _accordionPrefab = null;
        [SerializeField] private GameObject _cardPrefab = null;
        [SerializeField] private GameObject _goBackButton = null;
        [SerializeField] private Button _remoteFuncitoButtonPrefab = null;
        [SerializeField] private GameObject _comboBoxPrefab = null;

        private List<PanelConnectedWithValue> _panels = new List<PanelConnectedWithValue>();
        private List<AccordionUI> _accordionUis = new List<AccordionUI>();
        private Card _currentCard;
        private Tree<Card> _cardsTree;

        public static readonly List<Type> AllowedTypes = new List<Type>()
            {typeof(int), typeof(float), typeof(string), typeof(double), typeof(char)};

        private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            Card mainCard = new Card() { CardName = "main" };
            _cardsTree = new Tree<Card>(mainCard);
            // _allCards.Add(mainCard);
            DisplayCard(mainCard);
        }

        public void GoCardBack()
        {
            if (_currentCard.Previous == null) return;
            DisplayCard(_currentCard.Previous);
        }
        
        public void CreateComboBox(Dictionary<string, Action> boxDictionary, string categoryPath, string card, string boxName = "combo box")
        {
            Card parent = HandleCardPath(card);

            AccordionUI category = null;
            if (categoryPath != string.Empty)
            {
                category = HandleCategoryPath(categoryPath.Split("/").ToArray(), parent);
            }

            ComboBox spawnedBox = Instantiate(_comboBoxPrefab, _panelParent).GetComponent<ComboBox>();
            parent.AddChild(spawnedBox.transform);

            List<string> options = boxDictionary.Keys.ToList();
            options.AddRange(new List<string>() { "" });
            spawnedBox.Dropdown.AddOptions(options);

            spawnedBox.Dropdown.onValueChanged.AddListener((index) =>
            {
                List<Action> actions = boxDictionary.Values.ToList();

                if (index < actions.Count)
                {
                    actions[index]?.Invoke();
                    spawnedBox.Dropdown.value = options.Count - 1;
                }

                spawnedBox.Label.text = boxName;
            });


            AssignButtonToCategory(spawnedBox.transform, category);


            LayoutRebuilder.ForceRebuildLayoutImmediate(_panelParent.GetComponent<RectTransform>());
            spawnedBox.Label.text = boxName;
            spawnedBox.Dropdown.onValueChanged.Invoke(options.Count - 1);
            DisplayCurrentCard();
        }
        public static bool CheckParent(object parentObject)
        {
            if(parentObject==null)
            {
                Debug.LogError($"Trying to add component to debug console, from parentobject that is null");
                return false;
            }
            return true;
        }
    }
}