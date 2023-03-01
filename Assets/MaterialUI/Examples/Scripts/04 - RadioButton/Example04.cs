using UnityEngine;
using UnityEngine.UI;

public class Example04 : MonoBehaviour
{
    #region group1

    [SerializeField] private Text m_selectedValue1Text;

    public void onRadioButton1ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue1Text.text = "Selected Value: Apple";
    }

    public void onRadioButton2ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue1Text.text = "Selected Value: Banana";
    }

    public void onRadioButton3ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue1Text.text = "Selected Value: Mango";
    }

    #endregion

    #region group2

    [SerializeField] private Text m_selectedValue2Text;
    [SerializeField] private GameObject m_radioButtonsParent;
    [SerializeField] private GameObject m_radioButtonDraftPrefab;

    private void Start()
    {
        foreach (var toggle in m_radioButtonsParent.GetComponentsInChildren<Toggle>()) addToggleListener(toggle);
    }

    public void onAddButtonClicked()
    {
        if (m_radioButtonsParent.transform.childCount >= 8) return;

        var instance = Instantiate(m_radioButtonDraftPrefab);
        instance.transform.SetParent(m_radioButtonsParent.transform);
        instance.SetActive(true);
        instance.GetComponentInChildren<Text>().text = Random.Range(0, 1000).ToString();

        addToggleListener(instance.GetComponentInChildren<Toggle>());
    }

    public void onRemoveButtonClicked()
    {
        if (m_radioButtonsParent.transform.childCount <= 1) return;

        var lastChild = m_radioButtonsParent.transform.GetChild(m_radioButtonsParent.transform.childCount - 1);
        Destroy(lastChild.gameObject);
    }

    private void addToggleListener(Toggle toggle)
    {
        toggle.onValueChanged.AddListener(delegate(bool isOn)
        {
            var textToggle = toggle.transform.parent.GetComponentInChildren<Text>();
            onRadioButtonExample2ToggleChanged(textToggle, isOn);
        });
    }

    private void onRadioButtonExample2ToggleChanged(Text textToggle, bool isOn)
    {
        m_selectedValue2Text.text = "Selected Value: " + textToggle.text;
    }

    #endregion

    #region group3

    [SerializeField] private Text m_selectedValue3Text;

    public void onRadioButton4ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue3Text.text = "Selected Value: Red";
    }

    public void onRadioButton5ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue3Text.text = "Selected Value: Green";
    }

    public void onRadioButton6ToggleChanged(bool isOn)
    {
        if (isOn) m_selectedValue3Text.text = "Selected Value: Blue";
    }

    #endregion
}