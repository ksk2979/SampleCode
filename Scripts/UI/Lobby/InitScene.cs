using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InitScene : MonoBehaviour
{
    [SerializeField] Slider _loadingBar;
    [SerializeField] TextMeshProUGUI _loadingText;
    [SerializeField] TextMeshProUGUI _versionTMP;

    const string _loadingTextFormat = "{0}%";
    const string _versionTextFormat = "Ver. {0}";
    private void Start()
    {
        _versionTMP.text = string.Format(_versionTextFormat, Application.version);
        StartCoroutine(Loading());
    }
    IEnumerator Loading()
    {
        float lastValue = 4.0f;
        while(_loadingBar.value < 100)
        {
            if (_loadingBar.value < 15)
            {
                _loadingBar.value += 2.0f;
            }
            else if(_loadingBar.value < 50)
            {
                _loadingBar.value += 3.0f;
            }
            else if (_loadingBar.value <= 55)
            {
                _loadingBar.value += 0.5f;
            }
            else
            {
                lastValue += 1.0f;
                _loadingBar.value += lastValue;
            }
            _loadingText.text = string.Format(_loadingTextFormat, Mathf.RoundToInt(_loadingBar.value));
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        cSceneManager.GetInstance.ChangeScene("LoginPage", () => { });
    }
}
