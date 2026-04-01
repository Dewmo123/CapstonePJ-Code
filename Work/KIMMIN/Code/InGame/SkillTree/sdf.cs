using System;
using UnityEngine;
using UnityEngine.UI;

namespace Work.Code.SkillTree
{
    public class sdf : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => Debug.Log(gameObject.name));
        }
    }
}