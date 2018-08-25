using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController: MonoBehaviour
{
    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private TextMesh scorePlayer1;
    [SerializeField] private TextMesh scorePlayer2;

    public const int gridRows = 2;
    public const int gridCols = 5;
    public const float offsetX = 2f;
    public const float offsetY = 3f;

    private int cantPares = gridCols;

    private MemoryCard _firstRevealed;
    private MemoryCard _secondRevealed;
    private int _score1 = 0;
    private int _score2 = 0;
    private bool player = false;

    // Use this for initialization
    void Start() {
        scorePlayer1.color = Color.blue;
        Vector3 startPos = originalCard.transform.position;
        /////////////////////////////////////////
        int[] numbers = new int[(cantPares * 2)];
        for (int i = 0; i < numbers.Length; i++) {
            numbers[i] = images.Length;
        }
        int agregar;
        int cant = 0;
        while (cant < cantPares) {
            agregar = Random.Range(0, images.Length);
            bool toAdd = true;
            int pos;
            for (pos = 0; ((pos < numbers.Length) && (numbers[pos] != images.Length)); pos++) {
                if (agregar == numbers[pos])
                    toAdd = false;
            }
            if (toAdd) {
                numbers[pos] = agregar;
                numbers[pos + 1] = agregar;
                cant++;
            }
        }
        //////////////////////////////////////////////
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < gridCols; i++) {
            for (int j = 0; j < gridRows; j++) {
                MemoryCard card;
                if (i == 0 && j == 0) {
                    card = originalCard;
                }
                else {
                    card = Instantiate(originalCard) as MemoryCard;
                }

                int index = j * gridCols + i;
                int id = numbers[index];
                card.SetCard(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = -(offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }

    }

    public bool canReveal {
        get { return _secondRevealed == null; }
    }

    public void CardRevealed(MemoryCard card) {
        if (_firstRevealed == null) {
            _firstRevealed = card;
        }
        else {
            _secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch() {
        if (_firstRevealed.id == _secondRevealed.id) {
            if (!player) {
                _score1++;
                scorePlayer1.text = "Score: " + _score1;
            }
            else {
                _score2++;
                scorePlayer2.text = "Score: " + _score2;
            }
            player = !player;
        }
        else {
            yield return new WaitForSeconds(.5f);

            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
        }
        player = !player;
        if (!player) {
            scorePlayer1.color = Color.blue;
            scorePlayer2.color = Color.white;
        } else {
            scorePlayer2.color = Color.blue;
            scorePlayer1.color = Color.white;
        }
        _firstRevealed = null;
        _secondRevealed = null;
    }

    private int[] ShuffleArray(int[] numbers) {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++) {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public void Restart () {
        SceneManager.LoadScene("MainScene");
    }
}
