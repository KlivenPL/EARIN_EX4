using Assets.Source.Game.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class Menu : MonoBehaviour {

    [SerializeField] private Slider depthSlider;
    [SerializeField] private Text depthValueTxt;
    [SerializeField] private Text winTxt;
    [SerializeField] private Button playBtn;
    [SerializeField] private Button fakePlayBtn;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform playMenu;
    [SerializeField] private RectTransform escMenu;

    bool IsInGame => !mainMenu.gameObject.activeSelf;

    public static Menu Instance { get; private set; }

    private int depth;
    public int Depth {
        get => depth;
        private set {
            depthValueTxt.text = value.ToString();
            depth = value;
        }
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Depth = 4;
        depthSlider.onValueChanged.AddListener((value) => OnDepthSliderSlide((int)value));
        playBtn.onClick.AddListener(() => OnPlayButtonClick());
        exitButton.onClick.AddListener(() => OnExitButtonClick());
        restartButton.onClick.AddListener(() => OnRestartButtonClick());
        quitButton.onClick.AddListener(() => OnQuitButtonClick());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!IsInGame) {
                playMenu.gameObject.SetActive(false);
                fakePlayBtn.gameObject.SetActive(true);
            } else {
                if (!winTxt.gameObject.activeSelf) {
                    escMenu.gameObject.SetActive(!escMenu.gameObject.activeSelf);
                }
            }
        }
    }

    public void OnDepthSliderSlide(int value) {
        Depth = value;
    }
    public void OnPlayButtonClick() {
        mainMenu.gameObject.SetActive(false);
        SceneManager.LoadScene("Game");
    }

    public void OnExitButtonClick() {
        escMenu.gameObject.SetActive(false);
        winTxt.gameObject.SetActive(false);
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }

    public void OnRestartButtonClick() {
        escMenu.gameObject.SetActive(false);
        winTxt.gameObject.SetActive(false);
        SceneManager.LoadScene("Game");
    }

    public void DisplayWin(GameColor color) {
        winTxt.text = $"{color} wins!";
        winTxt.gameObject.SetActive(true);
        escMenu.gameObject.SetActive(true);
    }

    public void OnQuitButtonClick() {
        Application.Quit();
    }
}
