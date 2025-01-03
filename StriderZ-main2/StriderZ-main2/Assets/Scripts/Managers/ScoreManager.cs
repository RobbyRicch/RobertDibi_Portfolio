using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance => _instance;

    [Header("Score Data")]
    [SerializeField] private float _timeForNextUser = 1;
    [SerializeField] private float _timeTillUserIcon = 1, _timeTillModelIcons = 1, _timeTillGainedScore = 1, _timeTillNewScore = 1;

    [SerializeField] private bool _isScoreboardDone = false;
    public bool IsScoreboardDone => _isScoreboardDone;

    [Header("Score Components")]
    [SerializeField] private Image _userIcon;
    [SerializeField] private Transform _iconsLayout, _scoreTextsLayout, _gainedScoreTextsLayout;
    [SerializeField] private List<PlayerInputHandler> _leaderboard;

    private List<GameObject> _modelIcons;
    private List<Image> _modelIconsImage;

    private List<GameObject> _scoreTMPro;
    private List<TextMeshProUGUI> _scoreTexts;

    private List<GameObject> _gainedScoreTMPro;
    private List<TextMeshProUGUI> _gainedScoreTexts;

    [Header("Score Data")]
    private List<int> _playersPreviousScore;
    public List<int> PlayersScore => _playersPreviousScore;

    private List<int> _playersNewScore;
    public List<int> PlayersNewScore => _playersNewScore;

    private List<int> _playersGainedScore;
    public List<int> PlayersGainedScore => _playersGainedScore;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _leaderboard = new();
        
        //_showScore = ShowScore();
    }
    private void OnEnable()
    {
        EventManager.OnGameModeLaunched += OnGameModeLaunched;
        EventManager.OnGameModeResults += OnGameModeResults;
    }
    private void OnDisable()
    {
        EventManager.OnGameModeLaunched -= OnGameModeLaunched;
        EventManager.OnGameModeResults -= OnGameModeResults;
    }
    #endregion

    #region Scoreboard Handling
    private List<PlayerInputHandler> SortPlayerSocores(List<PlayerInputHandler> playersByScore)
    {
        Dictionary<int, List<PlayerInputHandler>> playerScoreSets = new Dictionary<int, List<PlayerInputHandler>>();
        List<PlayerInputHandler> unmatchedPlayers = new List<PlayerInputHandler>();

        for (int i = 0; i < playersByScore.Count; i++)
        {
            PlayerInputHandler player = playersByScore[i];
            if (!playerScoreSets.ContainsKey(player.Data.Score))
                playerScoreSets[player.Data.Score] = new List<PlayerInputHandler>();

            playerScoreSets[player.Data.Score].Add(player);
        }

        List<PlayerInputHandler> allPlayerInputHandlersByScore = new List<PlayerInputHandler>();

        // Sort matching sets by score value
        List<int> sortedScores = new List<int>(playerScoreSets.Keys);
        sortedScores.Sort();
        foreach (var score in sortedScores)
        {
            List<PlayerInputHandler> set = playerScoreSets[score];
            if (set.Count > 1)
            {
                set.Sort((a, b) => a.Data.ID.CompareTo(b.Data.ID)); // Sort by player ID

                for (int i = 0; i < set.Count; i++)
                {
                    allPlayerInputHandlersByScore.Add(set[i]);
                }
            }
            else
            {
                // Insert unmatchedPlayer before the first PlayerInputHandler with a higher score
                bool inserted = false;
                for (int i = 0; i < allPlayerInputHandlersByScore.Count; i++)
                {
                    if (allPlayerInputHandlersByScore[i].Data.Score > score)
                    {
                        allPlayerInputHandlersByScore.Insert(i, set[0]);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    allPlayerInputHandlersByScore.Add(set[0]);
                }
            }
        }

        allPlayerInputHandlersByScore.Reverse();
        playersByScore = allPlayerInputHandlersByScore;
        return playersByScore;

        /*for (int i = 0; i < allPlayerInputHandlersByScore.Count; i++)
        {
            Debug.Log("Id: " + allPlayerInputHandlersByScore[i].Data.ID + "Score: " + allPlayerInputHandlersByScore[i].Data.Score);
        }*/
    }
    private void SetNewScores()
    {
        _modelIcons = new();
        _modelIconsImage = new();
        _scoreTMPro = new();
        _scoreTexts = new();
        _gainedScoreTMPro = new();
        _gainedScoreTexts = new();
        _playersPreviousScore = new();
        _playersNewScore = new();
        _playersGainedScore = new();
        _isScoreboardDone = false;

        /* setting up leaderboard */
        List<PlayerInputHandler> playersByScore = new();
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
            playersByScore.Add(allPlayers[i]);

        _leaderboard = SortPlayerSocores(playersByScore);
        /* ---------------------- */

        /* setting up scores */
        for (int i = 0; i < _leaderboard.Count; i++)
        {
            PlayerData playerData = _leaderboard[i].Data;
            _playersPreviousScore.Add(playerData.PreviousScore);
            _playersNewScore.Add(playerData.Score);
            _playersGainedScore.Add(playerData.Score - playerData.PreviousScore);
            playerData.PreviousScore = playerData.Score;
        }
        /* ---------------------- */

        /* setting up UI */
        for (int i = 0; i < _leaderboard.Count; i++)
        {
            PlayerData playerData = _leaderboard[i].Data;
            ModelData modelData = _leaderboard[i].Data.ModelData;
            _modelIcons.Add(modelData.Icon);
            _modelIconsImage.Add(modelData.IconImage);
            _scoreTMPro.Add(modelData.ScoreTMPro);
            _scoreTexts.Add(modelData.ScoreText);
            _scoreTexts[i].text = playerData.Score.ToString();
            _gainedScoreTMPro.Add(modelData.GainedScoreTMPro);
            _gainedScoreTexts.Add(modelData.GainedScoreText);
            _gainedScoreTexts[i].text = "+" + _playersGainedScore[i].ToString();
        }
        /* ---------------------- */
    }
    private IEnumerator ClearLeaderboard()
    {
        /* Clear Icons */
        if (_iconsLayout.childCount > 0)
            for (int i = 0; i < _iconsLayout.childCount; i++)
                Destroy(_iconsLayout.GetChild(i).gameObject);

        /* Clear Gained Scores */
        if (_gainedScoreTextsLayout.childCount > 0)
            for (int i = 0; i < _gainedScoreTextsLayout.childCount; i++)
                Destroy(_gainedScoreTextsLayout.GetChild(i).gameObject);

        if (_scoreTextsLayout.childCount > 0 )
            for (int i = 0; i < _scoreTextsLayout.childCount; i++)
                Destroy(_scoreTextsLayout.GetChild(i).gameObject);

        yield return new WaitForSeconds(_timeTillUserIcon);
    }
    private IEnumerator SetPlayerModelIcon(PlayerInputHandler player, int playerIndex)
    {
        GameObject newModelIcon = Instantiate(_modelIcons[playerIndex], _iconsLayout);
        newModelIcon.GetComponent<Image>().color = player.SetupData.ColorData.IconColor;
        yield return new WaitForSeconds(_timeTillGainedScore);
    }
    private IEnumerator SetPlayerGainedScore(int playerIndex)
    {
        GameObject newModelIcon = Instantiate(_gainedScoreTMPro[playerIndex], _gainedScoreTextsLayout);
        newModelIcon.SetActive(true);
        yield return new WaitForSeconds(_timeTillNewScore);
    }
    private IEnumerator SetPlayerNewScore(int playerIndex)
    {
        /* Clear Older Scores */
        if (_scoreTextsLayout.childCount > 0 && playerIndex < _scoreTextsLayout.childCount)
            Destroy(_scoreTextsLayout.GetChild(playerIndex).gameObject);

        GameObject newModelIcon = Instantiate(_scoreTMPro[playerIndex], _scoreTextsLayout);
        newModelIcon.SetActive(true);
        yield return new WaitForSeconds(_timeForNextUser);
    }
    private IEnumerator SetPlayerScoreboard()
    {
        yield return StartCoroutine(ClearLeaderboard());

        //_userIcon.sprite = _leaderboard[0].SetupData.UserPic;
        _userIcon.sprite = _leaderboard[0].Data.UserIcon;
        yield return new WaitForSeconds(_timeTillModelIcons);

        for (int i = 0; i < _leaderboard.Count; i++)
        {
            PlayerInputHandler player = _leaderboard[i];

            /*yield return StartCoroutine(SetPlayerModelIcon(player, i));
            yield return StartCoroutine(SetPlayerGainedScore(i));
            yield return StartCoroutine(SetPlayerNewScore(i));*/

            SetPlayerModelIconNoAnimation(player, i);
            //SetPlayerGainedScoreNoAnimation(i);
            SetPlayerNewScoreNoAnimation(i);
        }

        

        _isScoreboardDone = true;
    }

    private void SetPlayerModelIconNoAnimation(PlayerInputHandler player, int playerIndex)
    {
        GameObject newModelIcon = Instantiate(_modelIcons[playerIndex], _iconsLayout);
        newModelIcon.GetComponent<Image>().color = player.SetupData.ColorData.IconColor;
    }
    private void SetPlayerGainedScoreNoAnimation(int playerIndex)
    {
        GameObject newModelIcon = Instantiate(_gainedScoreTMPro[playerIndex], _gainedScoreTextsLayout);
        newModelIcon.SetActive(true);
    }
    private void SetPlayerNewScoreNoAnimation(int playerIndex)
    {
        /* Clear Older Scores */
        if (_scoreTextsLayout.childCount > 0 && playerIndex < _scoreTextsLayout.childCount)
            Destroy(_scoreTextsLayout.GetChild(playerIndex).gameObject);

        GameObject newModelIcon = Instantiate(_scoreTMPro[playerIndex], _scoreTextsLayout);
        newModelIcon.SetActive(true);
    }
    #endregion

    #region Events
    private void OnGameModeLaunched()
    {
        if (_playersPreviousScore == null)
            return;

        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerData playerData = allPlayers[i].Data;
            _playersPreviousScore.Add(playerData.PreviousScore);
        }
    }
    private void OnGameModeResults()
    {
        UIManager.Instance.ScoreboardPanel.SetActive(true);
        SetNewScores();
       
        StartCoroutine(SetPlayerScoreboard());
    }
    #endregion



    public void SetScore(PlayerInputHandler player)
    {
        _playersPreviousScore[player.SetupData.ID] = player.Data.Score;
    }
}
