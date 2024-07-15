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
       public AccordionUI AddCategory(string categoryPath, Card cardParent)
        {
            if (categoryPath == string.Empty) return null;
            string[] path = categoryPath.Split('/');

            HandleCategoryPath(path, cardParent);
            DisplayCurrentCard();
            return GetCategoryByName(path[^1]);
        }

        private AccordionUI HandleCategoryPath(string[] path, Card parent)
        {
            AccordionUI categoryToReturn=null;
            for(int i=0; i<path.Length; i++)
            {
                TreeNode<AccordionUI> categoryParent=null;
                if(i!=0)
                {
                    categoryParent=GetCategoryNode(path.Take(i).ToArray(),parent);
                }

                TreeNode<AccordionUI> categoryNode = GetCategoryNode(path.Take(i+1).ToArray(),parent);
                if (categoryNode != null) 
                {
                    categoryToReturn=categoryNode.item;
                    continue;
                }
                GameObject spawnedObject = Instantiate(_accordionPrefab, _panelParent);
                var createdCategory = spawnedObject.GetComponent<AccordionUI>();
                createdCategory.Initialize(path[i], _panelParent.GetComponent<RectTransform>(), parent);
                _accordionUis.Add(createdCategory);
                AssignCategoryToCard(createdCategory, parent);
                categoryToReturn = createdCategory;
                if(categoryParent!=null)
                {
                    categoryParent.AddChild(createdCategory);
                    categoryParent.item.AddChild(createdCategory.transform);
                }
                else//it means there's no parent so it will be root
                {
                    parent.categoriesTree.root=new TreeNode<AccordionUI>(createdCategory);
                    parent.AddChild(createdCategory.transform);
                }
            }

            return categoryToReturn;
            // for (int i = 0; i < path.Length - 1; i++)
            // {
            //     GetCategoryByName(path[i]).AddChild(GetCategoryByName(path[i + 1]).transform);
            // }
        }

        private AccordionUI GetCategoryByName(string categoryName)
        {
            if (categoryName == string.Empty) return null;
            AccordionUI toReturn = _accordionUis.FindLast(x => x.CategoryName == categoryName);

            return toReturn != null ? toReturn : null;
        }

        private TreeNode<AccordionUI> GetCategoryNode(string[] path, Card parent)
        {
            if(path.Length==0)
            {
                return null;
            }
            TreeNode<AccordionUI> currentCategoryNode = parent.categoriesTree.root;
            if(currentCategoryNode==null)
            {
                return null;
            }
            if(currentCategoryNode.item.CategoryName!=path[0])
            {
                return null;
            }

            for(int i=1; i<path.Length; i++)
            {
                currentCategoryNode = currentCategoryNode.GetChild(x => x.CategoryName==path[i]);
                if(currentCategoryNode==null)return null;
            }
            return currentCategoryNode;
        }


    }
}
