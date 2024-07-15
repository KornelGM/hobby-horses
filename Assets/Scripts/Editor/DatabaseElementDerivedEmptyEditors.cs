using UnityEditor;
using UnityEngine;

// === AUDIO ===

[CustomEditor(typeof(AudioEvent))]
[CanEditMultipleObjects]
public class AudioEventEditor : DatabaseElementEditor<AudioEventVariant> {}

// === Item Datas ===
[CustomEditor(typeof(ItemDataDatabase))]
[CanEditMultipleObjects]
public class ItemDataDatabaseEditor : DatabaseElementEditor<ItemData> { }

[CustomEditor(typeof(ItemData))]
[CanEditMultipleObjects]
public class ItemDataEditor : DatabaseElementEditor<ServiceLocator> { }

// === Item Datas ===
[CustomEditor(typeof(StatsDatabase))]
[CanEditMultipleObjects]
public class StatsDatabaseEditor : DatabaseElementEditor<ActionStat> { }

[CustomEditor(typeof(ActionStat))]
[CanEditMultipleObjects]
public class StatsEditor : DatabaseElementEditor<GameObject> { }

[CustomEditor(typeof(QuestsDatabase))]
[CanEditMultipleObjects]
public class QuestsDatabaseEditor : DatabaseElementEditor<Quest> { }

// ==== Padlocks =====
[CustomEditor(typeof(Padlock))]
[CanEditMultipleObjects]
public class PadlockEditor : DatabaseElementEditor<ActionStat> { }

[CustomEditor(typeof(PadlocksDatabase))]
[CanEditMultipleObjects]
public class PadlockDatabaseDrawer : DatabaseElementEditor<Padlock> { }


// ==== Sprites =====
[CustomEditor(typeof(SpriteDatabase))]
[CanEditMultipleObjects]
public class SpriteDatabaseEditor : DatabaseElementEditor<Sprite> { }
