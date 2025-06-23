using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    private readonly int boardSize = 8;

    [SerializeField]
    private SlotView tilePrefab;
    [SerializeField]
    private Text text;

    public Vector2Int? currentSelectedPos = null;
    public Board board;
    public Dictionary<Vector2Int, SlotView> placement = new Dictionary<Vector2Int, SlotView>();

    public PieceType currentPlayerType = PieceType.White;

    private void Start()
    {
        board = new Board(new Vector2Int(boardSize, boardSize));

        board.onBoardChange.AddListener(EndTurn);

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                var obj = Instantiate(tilePrefab, transform);
                obj.transform.localPosition = new Vector3(i - (boardSize - 1) * 0.5f, j - (boardSize - 1) * 0.5f);
                obj.Initialize(this, new Vector2Int(i, j));

                placement.Add(new Vector2Int(i, j), obj);
            }
        }

        for (int i = 0; i < boardSize; i++)
        {
            board.CreateRaw(new PiecePawn(board, PieceType.White), new Vector2Int(i, 1));
            board.CreateRaw(new PiecePawn(board, PieceType.Black), new Vector2Int(i, boardSize - 2));
        }

        var w_king = new PieceKing(board, PieceType.White);
        var b_king = new PieceKing(board, PieceType.Black);
        board.CreateRaw(w_king, new Vector2Int(4, 0));
        board.CreateRaw(b_king, new Vector2Int(4, boardSize - 1));
        board.w_king = w_king;
        board.b_king = b_king;

        board.CreateRaw(new PieceRook(board, PieceType.White), new Vector2Int(0, 0));
        board.CreateRaw(new PieceRook(board, PieceType.White), new Vector2Int(boardSize - 1, 0));
        board.CreateRaw(new PieceRook(board, PieceType.Black), new Vector2Int(0, boardSize - 1));
        board.CreateRaw(new PieceRook(board, PieceType.Black), new Vector2Int(boardSize - 1, boardSize - 1));

        board.CreateRaw(new PieceKnight(board, PieceType.White), new Vector2Int(1, 0));
        board.CreateRaw(new PieceKnight(board, PieceType.White), new Vector2Int(6, 0));
        board.CreateRaw(new PieceKnight(board, PieceType.Black), new Vector2Int(1, boardSize - 1));
        board.CreateRaw(new PieceKnight(board, PieceType.Black), new Vector2Int(6, boardSize - 1));

        board.CreateRaw(new PieceBishop(board, PieceType.White), new Vector2Int(2, 0));
        board.CreateRaw(new PieceBishop(board, PieceType.White), new Vector2Int(5, 0));
        board.CreateRaw(new PieceBishop(board, PieceType.Black), new Vector2Int(2, boardSize - 1));
        board.CreateRaw(new PieceBishop(board, PieceType.Black), new Vector2Int(5, boardSize - 1));

        board.CreateRaw(new PieceQueen(board, PieceType.White), new Vector2Int(3, 0));
        board.CreateRaw(new PieceQueen(board, PieceType.Black), new Vector2Int(3, boardSize - 1));

        UpdateSlots();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float w = boardSize * 0.5f;
            pos += new Vector2(w, w);

            Vector2Int posInt = Vector2Int.FloorToInt(pos);
            if (placement.TryGetValue(posInt, out SlotView slot))
            {
                slot.OnClick();
            }
        }
    }

    private void EndTurn()
    {
        currentPlayerType = currentPlayerType.Opponent();

        bool isChack = board.IsCheck(currentPlayerType);
        bool isMate = board.IsMate(currentPlayerType);
        if (isChack && isMate)
        {
            text.enabled = true;
            text.text = "Checkmate";
        }
        else if (!isChack && isMate)
        {
            text.enabled = true;
            text.text = "Stalemate";
        }

        UpdateSlots();
    }

    private void UpdateSlots()
    {
        foreach (SlotView slot in placement.Values)
        {
            slot.UpdateState();
        }
    }

    public void ClearOverlay()
    {
        foreach (SlotView slot in placement.Values)
        {
            slot.overlay = false;
        }
    }
}
