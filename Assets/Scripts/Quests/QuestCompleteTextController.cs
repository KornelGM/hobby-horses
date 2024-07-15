using System.Collections;
using UnityEngine;

public class QuestCompleteTextController : MonoBehaviour
{
    [SerializeField] float _showLength = 2f;

    private WaitForSeconds _showWait;

    public void ShowText()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(WaitAndDisable());
    }

    private IEnumerator WaitAndDisable()
    {
        if (_showWait == null)
            _showWait = new(_showLength);

        yield return _showWait;

        gameObject.SetActive(false);
    }
}
