using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VisualDebug
{
    public class LabelManager : MonoBehaviour //!uncomment for animal shelter
    //public class LabelManager : Singleton<LabelManager>
    {
        [SerializeField] private GameObject _labelPrefab = null;
        [SerializeField] private Transform _labelsParent = null;

        private Dictionary<string, LabelBehavior> _labelsDictionary = new Dictionary<string, LabelBehavior>();

        public LabelBehavior GetLabelByID(String id)
        {
            return _labelsDictionary[id];
        }

        public void CreateLabelAtPoint(Vector3 point, string labelId, string labelText, float size = 1f, Color? color = null, bool displayTextInFront = false)
        {
            if (_labelsDictionary.ContainsKey(labelId))
            {
                Debug.LogError($"Label with id {labelId} already on scene");
            }

            LabelBehavior label = Instantiate(_labelPrefab, point, Quaternion.identity).GetComponent<LabelBehavior>();
            label.ToggleInFront(displayTextInFront);
            _labelsDictionary.Add(labelId, label);
            UpdateLabelText(labelId, labelText, size, color);
        }

        public void CreateLabelOverTransform(Transform transformToFollow, string labelId, string labelText, float size = 1f, Color? color = null, bool followTransform = false, bool displayTextInFront = false)
        {
            if (_labelsDictionary.ContainsKey(labelId))
            {
                Debug.LogError($"Label with id {labelId} already on scene");
            }

            LabelBehavior label = Instantiate(_labelPrefab, transformToFollow.position + Vector3.up * transformToFollow.localScale.magnitude, Quaternion.identity,_labelsParent).GetComponent<LabelBehavior>();
            label.ToggleInFront(displayTextInFront);
            _labelsDictionary.Add(labelId, label);
            UpdateLabelText(labelId, labelText, size, color);

            if (!followTransform) return;

            LabelFollowTransform followScript = label.gameObject.AddComponent<LabelFollowTransform>();
            followScript.Init(transformToFollow, Vector3.up * transformToFollow.localScale.magnitude);
        }

        public void UpdateLabelPosition(string labelId, Vector3 position)
        {
            LabelBehavior behavior = GetLabelByID(labelId);

            if (behavior.GetComponent<LabelFollowTransform>())
            {
                Debug.LogWarning($"label with id {labelId} is being displayed over a transform. You cannot assign a position to it");
                return;
            }

            behavior.transform.position = position;
        }

        public void UpdateLabelText(string labelId, string labelText, float size = 1f, Color? color = null)
        {
            LabelBehavior label = GetLabelByID(labelId);
            SetLabelScale(label, size);
            label.SetText(labelText);
            label.SetTextColor(color ?? Color.black);
            label.name = labelId;
        }

        private void SetLabelScale(LabelBehavior label, float size)
        {
            label.transform.localScale *= size;
        }
    }
}
