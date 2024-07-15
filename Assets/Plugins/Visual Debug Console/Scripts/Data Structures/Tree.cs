using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree<T> : IEnumerable<TreeNode<T>>
{
    public TreeNode<T> root;

    public Tree()
    {
        root= null;
    }

    public Tree(T root)
    {
        this.root=new TreeNode<T>(root);
    }

    public IEnumerator<TreeNode<T>> GetEnumerator()
    {
        List<TreeNode<T>> nodesToSearch = new List<TreeNode<T>>();
        nodesToSearch.Add(root);
        while(nodesToSearch.Count!=0)
        {
            var currentNode = nodesToSearch[0];
            yield return currentNode;
            nodesToSearch.RemoveAt(0);
            foreach(var node in currentNode.children)
            {
                nodesToSearch.Add(node);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
