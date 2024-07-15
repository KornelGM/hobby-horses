using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PoerformanceTestServiceLocator : ServiceLocator
{
    [SerializeField] private int _testComponentsAmount = 1000;

    protected override List<IServiceLocatorComponent> GetNonMonoBehaviourServiceLocators()
    {
        UnityEngine.Debug.Log($"Test components: {_testComponentsAmount}");
        List<IServiceLocatorComponent> components = new List<IServiceLocatorComponent>();
        for (int i = 0; i < _testComponentsAmount; i++)
        {
            components.Add(new AutoInjectingServiceLocator());
        }
        return components;
    }

    protected override void InitializeComponents()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        base.InitializeComponents();

        stopWatch.Stop();
        WriteTime(stopWatch, "Try Get Component ");
    }

    protected override void InitializeComponentsAttributes()
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        base.InitializeComponentsAttributes();

        stopWatch.Stop();
        WriteTime(stopWatch, "Get Attributes ");
    }

    private void WriteTime(Stopwatch stopWatch, string prefix)
    {
        TimeSpan ts = stopWatch.Elapsed;
        UnityEngine.Debug.Log($"{prefix} runtime: {ts.Milliseconds}ms");
    }
}
