/**
 * Description: Simple logging library for Unity.
 * Authors: Kornel, Michał, Tomek
 * Copyright: © 2021-2022 Kornel. All rights reserved. For license see: 'LICENSE.txt'
 **/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DreamParableLogger
{
	public class Logger: MonoBehaviour, IServiceLocatorComponent
	{
		public ServiceLocator MyServiceLocator { get; set; }
		public const string Special = "SPECIAL_LOGGING";
		public static Logger Instance { get; private set; }

		[SerializeField] private int _logMaxChars = 400;
		[SerializeField] private int _stackTraceMaxChars = 400;

		private Stream _fileStream;
		private Encoding _encoding = Encoding.Default;
		private string _fileName;

		void Awake()
		{
			if(Instance != null && Instance != this)
			{
				Debug.LogError($"Duplicate singelton {nameof(Logger)} on {gameObject.name}");
				Destroy(this);
			}
			else
			{
				Startup();
			}
		}

		void OnDestroy() => Cleanup();

		/// <summary>
		/// Log a message to the log file.
		/// Compiles only if "SPECIAL_LOGGING"(constant - Logger.Special) preprocessor definition is defined.
		/// </summary>
		/// <param name="logLevel">Log severity.</param>
		/// <param name="logCategory">Log category. Can be any arbitrary string.</param>
		/// <param name="caller">Please use 'this'.</param>
		/// <param name="message">Main log message.</param>
		/// <param name="detailedMessage">Optional details.</param>
		/// <param name="method">Automatically filled method name.</param>
		/// <param name="lineNumber">Automatically filled line number.</param>
		[Conditional(Special)]
		public void LogSpecial(LogType logLevel, string logCategory, object caller, string message, string detailedMessage = "", 
			[CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
		{
			Log(logLevel, logCategory, caller, message, detailedMessage, method, lineNumber);
		}

		/// <summary>
		/// Log a message to the log file.
		/// </summary>
		/// <param name="logLevel">Log severity.</param>
		/// <param name="logCategory">Log category. Can be any arbitrary string.</param>
		/// <param name="caller">Please use 'this'.</param>
		/// <param name="message">Main log message.</param>
		/// <param name="detailedMessage">Optional details.</param>
		/// <param name="method">Automatically filled method name.</param>
		/// <param name="lineNumber">Automatically filled line number.</param>
		public void Log(LogType logLevel, string logCategory, object caller, string message, string detailedMessage = "",
			[CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
		{
			WriteToLogFile($"{GetCurrentDateTime()} | {logLevel} | {logCategory} | {GetGameObjectName(caller)} | {caller?.GetType()} | {method} | {lineNumber} | {message} | {detailedMessage}\r\n");
		}

		public string GetCurrentLogFileName() => _fileName;

		private void Startup()
		{
			Instance = this;

			Application.logMessageReceived += LogUnityDebugMessages;
			_fileName = $"Log {DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.dp";
			string dir = $"{Application.persistentDataPath}/Logs";

			try
			{
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				_fileStream = File.Open($"{dir}/{_fileName}", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
			}
			catch (Exception e)
			{
				Debug.LogException(new Exception($"Could not create log file: {e.Message}"));

				Application.logMessageReceived -= LogUnityDebugMessages;

				if (_fileStream != null)
				{
					_fileStream.Close();
					_fileStream = null;
				}
			}
		}

		private void Cleanup()
		{
			Application.logMessageReceived -= LogUnityDebugMessages;
			if (Instance == this) Instance = null;

			if (_fileStream == null) return;

			_fileStream.Close();
			_fileStream = null;
		}

		private void LogUnityDebugMessages(string logString, string stackTrace, LogType type)
		{
			logString = logString.Length > _logMaxChars ? logString.Substring(0, _logMaxChars) : logString;

			if (type == LogType.Error || type == LogType.Exception)
			{
				stackTrace = stackTrace.Length > _stackTraceMaxChars ? stackTrace.Substring(0, _stackTraceMaxChars) : stackTrace;
			}
			else
			{
				stackTrace = "";
			}

			Log(type, "Unity debug", null, logString, stackTrace);
		}

		private void WriteToLogFile(string message)
		{
			if(_fileStream == null)
			{
				Application.logMessageReceived -= LogUnityDebugMessages;
				Debug.LogWarning($"Trying to write to a log file that is already closed (or was never opened)." +
					$" If this is done when quiting the game it might be because you are trying to write to a log or use Debug.Log in OnDestroy() or in OnDisable()." +
					$" Original message: <i>{message}</i>");

				return;
			}

			byte[] result = _encoding.GetBytes(message);

			_fileStream.Seek(0, SeekOrigin.End);
			_fileStream.Write(result, 0, result.Length);
			// TODO: Perhaps someday `await _fileStream.WriteAsync(result, 0, result.Length);`
			// And even add Flush() or even FlushAsync()?
		}

		private string GetGameObjectName(object caller)
		{
			if (caller == null) return "N/A";

			MonoBehaviour monoBehaviour = caller as MonoBehaviour;
			if (monoBehaviour == null) return "N/A";

			return monoBehaviour.gameObject.name;
		}

		private string GetCurrentDateTime()
		{
			DateTime dt = DateTime.Now;
			return dt.ToString("yyyy-MM-dd HH:mm:ss.ffff");
		}

		[ContextMenu("Open Log's Folder")]
		private void OpenLogs()
		{
			Application.OpenURL("file://" + Application.persistentDataPath);
		}
	}
}