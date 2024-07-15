using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ServiceLocator : MonoBehaviour, IServiceLocatorComponent, IStartable, IUpdateable, IAwake
{
    public bool Enabled => true;
    public ServiceLocator MyServiceLocator { get; set; }
    public List<IServiceLocatorComponent> ServiceLocatorComponents => _serviceLocatorComponents;

    protected List<IServiceLocatorComponent> _serviceLocatorComponents = new();
    private List<IUpdateable> _updatable = new();
    private List<IEarlyUpdate> _earlyUpdatable = new();

    private bool _isStarted = false;
    private bool _awakePerformed = false;

    protected virtual void Awake() => CustomAwake();
    protected virtual void OnDestroy() => MyServiceLocator?.DisconnectServiceLocator(this);

    public virtual void CustomAwake()
    {
        if (_awakePerformed) return;
        _awakePerformed = true;

        TryFindMyServiceLocator();
        GetAllNestedServiceLocatorComponents();
        InjectBaseFascadeToComponents();
        InitializeComponentsAttributes();
        InitializeComponents();
    }

    public virtual void CustomStart()
    {
        if (_isStarted) return;
        _isStarted = true;

        StartComponents();
    }

    protected virtual void TryFindMyServiceLocator()
    {
        if (MyServiceLocator == null)
        {
            if (transform.parent != null && transform.parent != transform)
            {
                ServiceLocator foundServiceLocator= transform.parent.GetComponentInParent<ServiceLocator>();
                if (foundServiceLocator != this)
                {
                    MyServiceLocator = foundServiceLocator;
                    MyServiceLocator?.CustomAwake();
                }
            }
        }

        if (MyServiceLocator == null)
        {
            MyServiceLocator = SceneServiceLocator.Instance;
            MyServiceLocator.CustomAwake();
            MyServiceLocator.ConnectServiceLocator(this);
        }
    }

    protected virtual List<IServiceLocatorComponent> GetNonMonoBehaviourServiceLocators() => new List<IServiceLocatorComponent>();

    protected virtual void InitializeComponents()
    {
        List<IAwake> awakeComponents = new List<IAwake>();
        Utilities.CastListToOtherType(_serviceLocatorComponents, awakeComponents);
        awakeComponents.ForEach(start => start.CustomAwake());
    }

    protected virtual void InitializeComponentsAttributes()
    {
        foreach (IServiceLocatorComponent serviceLocatorComponent in _serviceLocatorComponents)
        {
            InitializeComponentAttributes(serviceLocatorComponent);
        }
    }

    protected virtual void InitializeComponentAttributes(IServiceLocatorComponent serviceLocatorComponent)
    {
        FieldInfo[] fieldInfos = serviceLocatorComponent.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        for (int i = 0; i < fieldInfos.Length; i++)
        {
            ServiceLocatorComponentAttribute componentref = Attribute.GetCustomAttribute(fieldInfos[i], typeof(ServiceLocatorComponentAttribute)) as ServiceLocatorComponentAttribute;
            if (componentref == null) continue;

            object component = TryGetServiceLocatorComponent(fieldInfos[i].FieldType, componentref.SearchParents);

            if (component == null)
            {
                if (!componentref.CanBeNull) Debug.LogError($"<color=#FFFF00>Service Locator</color> The looking component <color=#FF00FF>{fieldInfos[i].FieldType}</color> was not found in <color=#FF00FF>{serviceLocatorComponent.GetType()}</color> component on <color=#FF00FF>{gameObject.name}</color>", gameObject);
                continue;
            }

            fieldInfos[i].SetValue(serviceLocatorComponent, component);
        }
    }

    public virtual void CustomUpdate()
    {
        if (!_isStarted) return;

        for (int i = 0; i < _earlyUpdatable.Count; i++) 
            _earlyUpdatable[i].CustomEarlyUpdate();

        for (int i = 0; i < _updatable.Count; i++)
        {
            if (_updatable[i].Enabled)
                _updatable[i].CustomUpdate();
        }
    }

    protected virtual void StartComponents()
    {
        List<IStartable> startable = new List<IStartable>();
        Utilities.CastListToOtherType(_serviceLocatorComponents, startable);
        Utilities.CastListToOtherType(_serviceLocatorComponents, _updatable);
        Utilities.CastListToOtherType(_serviceLocatorComponents, _earlyUpdatable);

        startable.ForEach(start => start.CustomStart());
    }

    private void InjectBaseFascadeToComponents()
    {
        _serviceLocatorComponents.ForEach(component => component.SetupServiceLocator(this));
    }

    public virtual void InjectManagers(IServiceLocatorComponent serviceLocatorComponent)
    {
        if (!_awakePerformed) CustomAwake();
        if (MyServiceLocator != null) MyServiceLocator.InjectManagers(serviceLocatorComponent);
    }

    public bool TryGetServiceLocatorComponent<T>(out T component, bool allowToBeNull = false, bool searchParents = true)
    {
        foreach (IServiceLocatorComponent fascadeComponent in _serviceLocatorComponents)
        {
            if (fascadeComponent is T)
            {
                component = (T)fascadeComponent;
                return true;
            }
        }

        if (MyServiceLocator != null && searchParents)
        {
            return MyServiceLocator.TryGetServiceLocatorComponent(out component, allowToBeNull);
        }

        component = default;
        if (!allowToBeNull) Debug.LogError($"The looking component {typeof(T)} was not found in {gameObject.name}", gameObject);
        return false;
    }

    public object TryGetServiceLocatorComponent(Type type, bool searchParents = true)
    {
        foreach (IServiceLocatorComponent fascadeComponent in _serviceLocatorComponents)
        {
            Type myType = fascadeComponent.GetType();
            if (myType == type) return fascadeComponent;
            if (myType.IsSubclassOf(type)) return fascadeComponent;
            if (myType.GetInterfaces().Contains(type)) return fascadeComponent;
        }

        if (MyServiceLocator != null && searchParents)
        {
            return MyServiceLocator.TryGetServiceLocatorComponent(type);
        }

        return null;
    }

    public bool TryGetAllServiceLocatorComponentsOfType<T>(out List<T> components, bool canBeNull = false)
    {
        List<T> componentsOfType = new List<T>();

        foreach (IServiceLocatorComponent fascadeComponent in _serviceLocatorComponents)
        {
            if (Utilities.TryCast(fascadeComponent, out T result))
            {
                componentsOfType.Add(result);
            }
        }

        if (componentsOfType.Count == 0 && !canBeNull)
        {
            components = default;
            Debug.LogError($"<color=#FFFF00>Service Locator</color> The looking component <color=#FF00FF>{typeof(T)}</color> was not found");
            return false;
        }

        components = componentsOfType;
        return true;
    }

    public void ConnectObject(Transform obj)
    {
        if (obj == null) return;
        List<IServiceLocatorComponent> components = GetServiceLocatorComponentsInChildren(obj);
        foreach (IServiceLocatorComponent component in components)
        {
            ConnectServiceLocator(component);
        }

    }

    private void GetAllNestedServiceLocatorComponents()
    {
        _serviceLocatorComponents = new List<IServiceLocatorComponent>();
        List<IServiceLocatorComponent> nestedComponents = GetServiceLocatorComponentsInChildren(transform);

        _serviceLocatorComponents.AddRange(nestedComponents);
        _serviceLocatorComponents.AddRange(GetNonMonoBehaviourServiceLocators());
    }

    private List<IServiceLocatorComponent> GetServiceLocatorComponentsInChildren(Transform obj)
    {
        List<IServiceLocatorComponent> components = obj.GetComponents<IServiceLocatorComponent>().ToList();
        if(components.Contains(this))components.Remove(this);
        if (obj.childCount == 0) return components;

        components.Remove(this);

        for (int i = 0; i < obj.childCount; i++)
        {
            Transform child = obj.GetChild(i);
            if (child.TryGetComponent(out ServiceLocator subServiceLocator))
            {
                components.Add(subServiceLocator);
                continue;
            }

            List<IServiceLocatorComponent> childComponents = GetServiceLocatorComponentsInChildren(child);

            if (childComponents == null || childComponents.Count == 0) continue;
            components.AddRange(childComponents);
        }

        return components;
    }

    public virtual void ConnectServiceLocator(IServiceLocatorComponent serviceLocatorComponent)
    {
        if (_serviceLocatorComponents.Contains(serviceLocatorComponent)) return;

        serviceLocatorComponent.SetupServiceLocator(this);
        InitializeComponentAttributes(serviceLocatorComponent);
        _serviceLocatorComponents.Add(serviceLocatorComponent);
        if (_awakePerformed) 
        {
            if (Utilities.TryCast(serviceLocatorComponent, out IAwake awake)) awake.CustomAwake();
        }
        if (_isStarted) StartCoroutine(StartAfterFrame(serviceLocatorComponent));
    }

    public virtual void DisconnectServiceLocator(IServiceLocatorComponent serviceLocatorComponent)
    {
        if (Utilities.TryCast(serviceLocatorComponent, out IUpdateable updateable))
        {
            _updatable.Remove(updateable);
        }
    }

    public void HideServiceLocator(bool isHidden = true)
    {
        gameObject.SetActive(!isHidden);
    }

    private IEnumerator StartAfterFrame(IServiceLocatorComponent serviceLocator)
    {
        yield return new WaitForEndOfFrame();
        if (Utilities.TryCast(serviceLocator, out IStartable startable))
        {
            startable.CustomStart();
        }

        if (Utilities.TryCast(serviceLocator, out IUpdateable updateable))
        {
            _updatable.Add(updateable);
        }
    }
}
