using UnityEngine;

public class BoardAction
{
    protected readonly Board board;

    public BoardAction(Board board)
    {
        this.board = board;
    }

    public virtual void Do()
    {

    }

    public virtual void Undo()
    {

    }
}

public class SimpleMove : BoardAction
{
    public Piece piece;
    public Vector2Int fromPos;
    public Vector2Int toPos;
    public bool isFirstMove;

    public SimpleMove(Board board, Piece piece, Vector2Int pos) : base(board)
    {
        this.piece = piece;

        fromPos = piece.pos;
        toPos = pos;
        isFirstMove = !piece.moved;
    }

    public override void Do()
    {
        piece.moved = true;
        board.MoveRaw(piece, toPos);
    }

    public override void Undo()
    {
        piece.moved = !isFirstMove;
        board.MoveRaw(piece, fromPos);
    }
}

public class CapturingMove : SimpleMove
{
    public Piece targetPiece;

    public CapturingMove(Board board, Piece piece, Vector2Int pos, Piece targetPiece) : base(board, piece, pos)
    {
        this.targetPiece = targetPiece;
    }

    public override void Do()
    {
        board.DestroyRaw(targetPiece);

        base.Do();
    }

    public override void Undo()
    {
        base.Undo();

        board.CreateRaw(targetPiece, toPos);
    }
}

public class CastlingMove : SimpleMove
{
    public Piece rook;
    public Vector2Int rookFromPos;
    public Vector2Int rookToPos;

    public CastlingMove(Board board, Piece piece, Vector2Int pos, Piece rook, Vector2Int rookPos) : base(board, piece, pos)
    {
        this.rook = rook;
        rookFromPos = rook.pos;
        rookToPos = rookPos;
    }

    public override void Do()
    {
        base.Do();

        rook.moved = true;
        board.MoveRaw(rook, rookToPos);
    }

    public override void Undo()
    {
        base.Undo();

        rook.moved = false;
        board.MoveRaw(rook, rookFromPos);
    }
}