using System;

[AttributeUsage(AttributeTargets.Field)]
public class ServiceLocatorComponentAttribute : Attribute
{
    public bool SearchParents { get; private set; }
    public bool CanBeNull { get; private set; }

    public ServiceLocatorComponentAttribute(bool searchParents = true, bool canBeNull = false)
    {
        SearchParents = searchParents;
        CanBeNull = canBeNull;
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ManagerComponentAttribute : Attribute { }
