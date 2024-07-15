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
    public partial class VisualConsoleUI
    {
        public void AssignCategoryToCard(AccordionUI category, Card card)
        {
            if (category == null) { return; }
            // DisplayCard(_currentCard);
            card.AddChild(category.transform);
        }

        private void AddCard(string cardName, string[] parentCardPath)
        {
            var parent = GetCardNode(parentCardPath);

            if (parent == null)
            {
                parent = _cardsTree.root;
            }

            Card newCard = new Card() { CardName = cardName, Previous = parent.item };
            GameObject cardSpawned = Instantiate(_cardPrefab, _panelParent);

            parent.AddChild(newCard);
            cardSpawned.GetComponent<Button>().onClick.AddListener(() => { DisplayCard(newCard); });
            cardSpawned.GetComponentInChildren<TextMeshProUGUI>().text = newCard.CardName;
            parent.item.AddChild(cardSpawned.transform);
            DisplayCard(_currentCard);
        }

        private void DisplayCard(Card card)
        {
            _currentCard = card;

            foreach (Transform child in _panelParent.GetComponentInChildren<Transform>())
            {
                child.gameObject.SetActive(_currentCard.CardContent.Contains(child));
            }

            _goBackButton.SetActive(_currentCard.Previous != null);
            gameObject.name = _currentCard.CardName;
            OnCardChanged?.Invoke();
        }

        public void DisplayCurrentCard()
        {
            DisplayCard(_currentCard);
        }

        private Card HandleCardPath(string cardPath)
        {
            if (cardPath == string.Empty)
            {
                return _cardsTree.root.item;
            }
            string[] path = cardPath.Split('/');

            for (int i = 0; i < path.Length; i++)
            {
                if (GetCardNode(path.Take(i + 1).ToArray()) != null) continue;

                AddCard(path[i], path.Take(i).ToArray());
            }

            return GetCardNode(path).item;
        }

        public TreeNode<Card> GetCardNode(string[] path)
        {
            if (path.Length == 0)
            {
                return null;
            }
            if (path.Length == 1 && path[0] == string.Empty)
            {
                return _cardsTree.root;
            }
            TreeNode<Card> currentNode = _cardsTree.root;

            if (currentNode == null) return null;

            int i = 0;
            if (path[0] == "main")
            {
                i = 1;
            }

            for (; i < path.Length; i++)
            {
                currentNode = currentNode.GetChild(x => x.CardName == path[i]);

                if (currentNode == null)
                {
                    return null;
                }
            }
            return currentNode;
        }

    }
}
