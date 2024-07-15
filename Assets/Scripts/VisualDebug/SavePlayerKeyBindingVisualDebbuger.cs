using Rewired;
using UnityEngine;

public class SavePlayerKeyBindingVisualDebbuger : VisualDebugger
{
    public override void CustomStart()
    {
        base.CustomStart();
        AddButton(this, (button) => Save(), "", "", "Save Key Bindings", Color.red);
    }

    private void Save()
    {
        if (ReInput.userDataStore != null)
            ReInput.userDataStore.Save();
    }
}
