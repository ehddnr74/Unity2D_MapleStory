using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : MonoBehaviour
{
    private GameObject skillTooltip;

    private Image skillImage;

    private SkillManager skillManager;

    private Sprite toolTipIcon;

    private void Start()
    {
        skillTooltip = GameObject.Find("SkillToolTip");
        skillImage = skillTooltip.GetComponent<Image>();
        skillTooltip.SetActive(false);

        skillManager = GameObject.Find("Player").GetComponent<SkillManager>();
    }

    private void Update()
    {
        if (skillTooltip.activeSelf)
        {
            skillTooltip.transform.position = Input.mousePosition;
        }

        if (!skillManager.activeUI)
        {
            Deactivate();
        }

    }
    public void Activate(string toolTipPath)
    {
        toolTipIcon = Resources.Load<Sprite>("ToolTips/" + toolTipPath);
        skillImage.sprite = toolTipIcon;
        skillTooltip.SetActive(true);
    }

    public void Deactivate()
    {
        skillTooltip.SetActive(false);
    }
}
