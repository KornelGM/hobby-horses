using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;

public static class AssertTool
{
	[Conditional("UNITY_EDITOR")]
	public static void IsNotNull<T1, T2>(this T1 value, T2 caller) where T1 : class
	{
		IsNotEqual(value, null, caller, "Not specified");
	}	
	
	[Conditional("UNITY_EDITOR")]
	public static void IsNotNull<T1, T2>(this T1 value, T2 caller, string nameOfField) where T1 : class
	{
		IsNotEqual(value, null, caller, nameOfField);
	}

	[Conditional("UNITY_EDITOR")]
	public static void IsNotEqual<T1, T2>(this T1 value, T1 comparingValue, T2 caller, string nameOfField) where T1 : class
	{
		if (Utilities.TryCast(caller, out MonoBehaviour mono))
		{
			Assert.AreNotEqual(value, comparingValue, $"Please check field <b>{nameOfField}</b> on <b>{typeof(T2)}</b> script on <b>{mono.name}</b> object");
			return;
		}

		Assert.AreNotEqual(value, comparingValue, $"Please check field <b>{nameOfField}</b> on <b>{typeof(T2)}</b> script {nameof(value)}");
	}

	[Conditional("UNITY_EDITOR")]
	public static void IsNotNull<T1, T2>(this List<T1> value, T2 caller, string nameOfField) where T1 : class
	{
		value.ForEach(e => IsNotNull(e, caller, nameOfField));
	}
}


