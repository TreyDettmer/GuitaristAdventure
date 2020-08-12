using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Image foregroundImage;
    [SerializeField] private float updateSpeedSeconds = 0.5f;
    [SerializeField] Transform camera;


    private void Awake()
    {
        GetComponentInParent<HealthManager>().OnHealthPercentChanged += HandleHealthChanged;
    }

    private void HandleHealthChanged(float changedPercent)
    {
        StartCoroutine("ChangeToPercent", changedPercent);
    }

    private IEnumerator ChangeToPercent(float percent)
    {
        float prechangePercent = foregroundImage.fillAmount;
        float elapsed = 0f;
        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(prechangePercent, percent, elapsed / updateSpeedSeconds);
            yield return null;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(camera);

    }
}
