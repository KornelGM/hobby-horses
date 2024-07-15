using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Playables;

public class SaveFileInfo<T>
{
	public string FileName;
	public DateTime DateTime;
	public T HeaderData;

	public SaveFileInfo(string fileName, DateTime dateTime, T saveFileData)
	{
		FileName = fileName;
		DateTime = dateTime;
		HeaderData = saveFileData;
	}
}


public abstract class SaveLoadManager<T,N> : MonoBehaviour
	where T : class, new() 
	where N : new()
{
	public bool Loading{ get; private set; }

	protected abstract SaveLoadInfo<T> _saveLoadInfo { get; }
	
	private string _persistentPath => Application.persistentDataPath;
	private BinaryFormatter _converter = new BinaryFormatter();

	private const string _savesFileName = "Saves";
	private const string _backupsFileName = "Save Backups";
	private const string _suffix = ".save";

	//For advance we are taking more space to expand header struct
	private const int _headerMaxSize = 528;

	public void ChangeCurrentSave(T saveData)
	{
		_saveLoadInfo.CurrentData = saveData;
	}

    #region Getting Saves
    public List<SaveFileInfo<N>> GetBackupSaves() => GetSaves(Path.Combine(_persistentPath, _backupsFileName));
	
	public List<SaveFileInfo<N>> GetMainSaves() => GetSaves(Path.Combine(_persistentPath, _savesFileName));
	
	public List<SaveFileInfo<N>> GetAllSaves()
	{
		List<SaveFileInfo<N>> allSaves = GetMainSaves();
		allSaves.AddRange(GetBackupSaves());
		return allSaves;
	}

	public List<SaveFileInfo<N>> GetSaves(string path)
	{
		if (!Directory.Exists(path)) return new();
		string[] filePaths = Directory.GetFiles(path);

		List<SaveFileInfo<N>> allSaves = new();
		foreach (string filePath in filePaths)
		{
			if (Path.GetExtension(filePath) != _suffix) continue;
			if (string.IsNullOrEmpty(filePath)) continue;
			if (!TryGetSaveFileInfo(filePath, out SaveFileInfo<N> saveFileInfo)) continue;

			allSaves.Add(saveFileInfo);
		}

		return allSaves;
	}

	public (T, SaveFileInfo<N>) GetSaveDataFile(string fileName, bool isBackup)
	{
		string path = GetFilePath(fileName, isBackup);
		if (!TryGetSaveFileInfo(path, out SaveFileInfo<N> saveFileInfo)) return (null, null);
		return (GetSave(fileName, isBackup), saveFileInfo);
	}

	protected abstract ISaveable<N> GetHeaderData();
    protected abstract ISaveable<T> GetSaveSequence();

    private bool TryGetSaveFileInfo(string path, out SaveFileInfo<N> saveFileInfo)
	{
		if (!GetHeader(path, out N header))
		{
			saveFileInfo = null;
			return false;
		}

		DateTime lastWriteTime = File.GetLastWriteTime(path);
		saveFileInfo = new SaveFileInfo<N>(Path.GetFileNameWithoutExtension(path), lastWriteTime, header);
		return true;
	}
	
	public T GetSave(string fileName, bool isBackup)
	{
		T data = GetSave(GetFilePath(fileName, isBackup));
		if (data == default) return null;

		return data;
	}

	private T GetSave (string path)
	{
		if (File.Exists(path))
		{
			try
			{
				FileStream dataStream = new FileStream(path, FileMode.Open);

				dataStream.Seek(_headerMaxSize, SeekOrigin.Begin);

				T saveData = _converter.Deserialize(dataStream) as T;
				dataStream.Close();

				return saveData;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return null;
			}
		}
		else
		{
			Debug.LogError("Save file not found in " + path);
			return null;
		}
	}

	private bool GetHeader(string filePath, out N header) 
	{
		if (File.Exists(filePath))
		{
			try
			{
				FileStream dataStream = new FileStream(filePath, FileMode.Open);

				if (!GetHeader(dataStream, out header))
				{
					dataStream.Close();
					return false;
				};

				dataStream.Close();
				return true;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
                Debug.LogError("Error occured during reading header " + filePath);
                header = default(N);
				return false;
			}
		}
		else
		{
			Debug.LogError("Error occured during reading header " + filePath);
			header = default(N);
			return false;
		}

	}

	private bool GetHeader(FileStream fileStream, out N header)
	{
		header = new();
		byte[] buffer = new byte[_headerMaxSize];
		GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

		try
		{
			fileStream.Seek(0, SeekOrigin.Begin);
			fileStream.Read(buffer, 0, _headerMaxSize);

			if (!FromByteArray(buffer, out header)) return false;

			bool isSuccessful = IsValidHeader(header);

			return isSuccessful;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return false;
		}
		finally
		{
			if (gch.IsAllocated)
				gch.Free();
		}
	}
	#endregion

	#region Loading Save
	public T CollectInformationsToSaveData()
	{
		T data = new T();
		GetSaveSequence()?.CollectData(data);
		return data;
	}

	public bool TryLoadSave(string fileName)
	{
		T data = GetSave(fileName, false);

        if (data == null)
		{
			Debug.Log($"There is no saved game with name '{fileName}'");
			return false;
		}

		ChangeCurrentSave(data);
		return true;
	}

    #endregion

    #region Saving 
	public bool SaveDataToFile(string fileName, bool isBackup)
    {
		T gameData = new();
		N headerData = new();

		GetSaveSequence()?.CollectData(gameData);
		GetHeaderData()?.CollectData(headerData);

		return SaveDataToFile(fileName, gameData, headerData, isBackup);
    }

	public bool SaveDataToFile(string fileName, T saveData, N saveFileInfo, bool isBackup = false)
	{
		if (saveData == null) return false;

		bool failed = !SaveToFile(GetFilePath(fileName, isBackup), saveData, saveFileInfo);
		if (failed) return false;

		ChangeCurrentSave(saveData);
		return true;
	}

	private bool SaveToFile(string path, T source, N header)
	{
		if (string.IsNullOrEmpty(path)) return false;

		try
		{
			FileStream dataStream = new FileStream(path, FileMode.Create);

			byte[] buffer = ToByteArray(header);

			dataStream.Seek(0, SeekOrigin.Begin);
			dataStream.Write(buffer, 0, _headerMaxSize);

			_converter.Serialize(dataStream, source);

			dataStream.Close();
		}
		catch (Exception e)
		{
			Debug.LogError("An error occured during saving file: " + e.ToString());
			return false;
		}

		return true;
	}

    #endregion

    #region Utilities
    private bool IsValidHeader(N fileHeader)
	{
		return fileHeader != null;
	}

	public bool DeleteFile(string name, bool isBackup)
	{
		return DeleteFile(GetFilePath(name, isBackup));
	}

	private bool DeleteFile(string path)
	{
		try
		{
			File.Delete(path);
			return true;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return false;
		}
	}

	public static bool CheckIfFolderExist(string path, bool createWhenNotExist = true)
	{
		if (Directory.Exists(path)) return true;
		if (!createWhenNotExist) return false;

		Directory.CreateDirectory(path);
		return true;
	}

	private string GetFilePath(string fileName, bool isBackup)
	{
		string path = _persistentPath;
		if (isBackup) path = Path.Combine(path, _backupsFileName);
		else path = Path.Combine(path, _savesFileName);
		CheckIfFolderExist(path);

		path = Path.Combine(path, Path.ChangeExtension(fileName, _suffix));

		return path;
	}

	private byte[] ToByteArray(N obj)
	{
		byte[] buffer = new byte[_headerMaxSize];

		if (obj == null)
			return null;
		using (MemoryStream ms = new MemoryStream(buffer))
		{
			_converter.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	private bool FromByteArray(byte[] data, out N output)
	{
		output = default(N);
		try
		{
			if (data == null)
				return false;

			using (MemoryStream ms = new MemoryStream(data))
			{
				output = (N)_converter.Deserialize(ms);
				return true;
			}
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return false;
		}
	}
    #endregion

    void OnApplicationQuit()
	{
		_saveLoadInfo.CurrentData = null;
	}
}
