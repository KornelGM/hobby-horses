using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TreeNode<T>
{
    public T item;

    public List<TreeNode<T>> children;

    public TreeNode(T item)
    {
        this.item=item;
        children = new List<TreeNode<T>>();
    }

    public void AddChild(T item)
    {
        children.Add(new TreeNode<T>(item));
    }

    public TreeNode<T> GetChild(Predicate<T> predicate)
    {
        foreach(var child in children)
        {
            if(predicate(child.item))
            {
                return child;
            }
        }
        return null;
    }

}