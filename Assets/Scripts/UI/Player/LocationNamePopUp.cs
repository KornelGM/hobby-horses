using TMPro;
using UnityEngine;

public class LocationNamePopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _locationNameText;
    [SerializeField] private Animator _animator;
    public void SetLocationName(string locationName)
    {
        _locationNameText.text = locationName;
        _animator.SetTrigger("Show");
    }

}
