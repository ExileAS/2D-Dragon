using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    [SerializeField] private AudioClip selectionAudio;
    [SerializeField] private AudioClip confirmAudio;
    private int currSelection;
    private float initialX;

    private void Awake() {
        initialX = transform.position.x;
        transform.position = new Vector2(initialX - (options[0].rect.width / 2 + 100), options[0].transform.position.y);
        currSelection = 0;
    }

    private void Update() {
        HandleSelectionInput();
        if(options[currSelection].CompareTag("Volume"))
            ChangeVolume();
        else
            InvokeCurrSelection();
    }

    private void InvokeCurrSelection() {
        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            options[currSelection].GetComponent<Button>().onClick.Invoke();
            SFXManager.Instance.PlaySound(confirmAudio);
        }
    }

    private void ChangeVolume() {
        bool increase = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
        bool decrease = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
        if(!decrease && !increase) return;

        RectTransform selectedOption = options[currSelection];
        TextMeshProUGUI currText = selectedOption.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if(increase) {
            float volumeValue = SFXManager.Instance.AdjustVolume(selectedOption.name, 1);
            currText.text = VolumeCorrection.GetVolumeToDisplay(volumeValue);
        }
        if(decrease) {
            float volumeValue = SFXManager.Instance.AdjustVolume(selectedOption.name, -1);
            currText.text = VolumeCorrection.GetVolumeToDisplay(volumeValue);
        }
    }

    private void HandleSelectionInput() {
        int change = 0;
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            change = currSelection -1 < 0 ? options.Length-1 : -1;
        }
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            change = currSelection + 1 >= options.Length ? -(options.Length-1) : 1;
        }
        if(change != 0) {
            currSelection += change;
            transform.position = new Vector2(initialX - (options[currSelection].rect.width / 2 + 100), options[currSelection].transform.position.y);
            SFXManager.Instance.PlaySound(selectionAudio);
        }
    }
}
