using TMPro;
using UnityEngine;

public class PlayerStatPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _description;

    public void SetupTxt(string text)
    {
        _description.text = text;   
    }
}
