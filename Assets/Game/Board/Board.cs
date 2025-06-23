using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board
{
    public readonly Vector2Int size;
    public Dictionary<Vector2Int, Piece> placement = new Dictionary<Vector2Int, Piece>();
    public PieceKing w_king, b_king;

    public UnityEvent onBoardChange = new UnityEvent();

    public List<BoardAction> actions = new List<BoardAction>();
    public List<BoardAction> undoActions = new List<BoardAction>();

    public Board(Vector2Int size)
    {
        this.size = size;
    }

    public void MoveRaw(Piece piece, Vector2Int pos)
    {
        placement.Remove(piece.pos);
        piece.pos = pos;
        placement.Add(pos, piece);
    }

    public void CreateRaw(Piece piece, Vector2Int pos)
    {
        piece.pos = pos;
        placement.Add(pos, piece);
    }

    public void DestroyRaw(Piece piece)
    {
        placement.Remove(piece.pos);
    }

    public bool IsInBoard(Vector2Int pos)
    {
        return 0 <= pos.x && pos.x < size.x && 0 <= pos.y && pos.y < size.y;
    }

    public bool MovePiece(Vector2Int fromPos, Vector2Int toPos)
    {
        if (!placement.ContainsKey(fromPos)) return false;

        undoActions.Clear();

        Piece piece = placement[fromPos];
        PieceType pieceType = piece.pieceType;

        MovePieceRaw(piece, toPos);

        if (IsCheck(pieceType))
        {
            Undo();
            return false;
        }

        onBoardChange.Invoke();
        return true;
    }

    private void MovePieceRaw(Piece piece, Vector2Int pos)
    {
        BoardAction boardAction = piece.Move(pos);
        boardAction.Do();
        actions.Add(boardAction);
    }

    public void Undo()
    {
        if (actions.Count == 0) return;

        BoardAction boardAction = actions[actions.Count - 1];
        boardAction.Undo();
        actions.RemoveAt(actions.Count - 1);
        undoActions.Add(boardAction);
    }

    public void Redo()
    {
        if (undoActions.Count == 0) return;

        BoardAction boardAction = undoActions[undoActions.Count - 1];
        boardAction.Do();
        undoActions.RemoveAt(undoActions.Count - 1);
        actions.Add(boardAction);
    }

    public bool IsCheck(PieceType pieceType)
    {
        var king = pieceType == PieceType.White ? w_king : b_king;
        var opponent = pieceType.Opponent();

        Piece[] pieces = new Piece[placement.Count];
        placement.Values.CopyTo(pieces, 0);

        foreach (var piece in pieces)
        {
            if (piece.pieceType != opponent) continue;

            foreach (var p in piece.MovablePositions())
            {
                if (p == king.pos) return true;
            }
        }

        return false;
    }

    public bool IsMate(PieceType pieceType)
    {
        Piece[] pieces = new Piece[placement.Count];
        placement.Values.CopyTo(pieces, 0);

        foreach (var piece in pieces)
        {
            if (piece.pieceType != pieceType) continue;

            foreach (var p in piece.MovablePositions())
            {
                MovePieceRaw(piece, p);
                if (!IsCheck(pieceType))
                {
                    Undo();
                    return false;
                }
                Undo();
            }
        }
        return true;
    }
}
