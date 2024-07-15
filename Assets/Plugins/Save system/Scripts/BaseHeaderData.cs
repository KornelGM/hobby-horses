using System;

[Serializable]
public class BaseHeaderData
{
	public string PrimeBackupsFileName;//Only if backup
	public int BackupID;
	public int AutoSaveID;
	public int QuickSaveID;
	public bool IsBackup => BackupID > 0;
	public bool IsQuickSave => QuickSaveID > 0;
	public bool IsAutoSave => AutoSaveID > 0;
}
