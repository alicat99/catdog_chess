using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    White,
    Black,
}

public static class PieceTypeMethods
{
    public static PieceType Opponent(this PieceType pieceType) => pieceType == PieceType.White ? PieceType.Black : PieceType.White;
}

public class Piece
{
    public readonly Board board;
    public Vector2Int pos;
    public PieceType pieceType;
    public bool moved = false;

    public virtual string SpriteName => "pawn";

    public Piece(Board board, PieceType pieceType)
    {
        this.board = board;
        this.pieceType = pieceType;
    }

    public virtual IEnumerable<Vector2Int> MovablePositions()
    {
        var p = pos + new Vector2Int(0, pieceType == PieceType.White ? 1 : -1);
        if (board.IsInBoard(p))
            yield return p;
    }

    public virtual BoardAction Move(Vector2Int toPos)
    {
        return new SimpleMove(board, this, toPos);
    }

    protected BoardAction MoveOrCapture(Vector2Int toPos)
    {
        if (Get(toPos, out Piece target))
            return new CapturingMove(board, this, toPos, target);
        return new SimpleMove(board, this, toPos);
    }

    protected bool Get(Vector2Int pos, out Piece piece)
    {
        return board.placement.TryGetValue(pos, out piece);
    }

    protected Piece Get(Vector2Int pos)
    {
        return board.placement[pos];
    }
}

public class PiecePawn : Piece
{
    public override string SpriteName => "pawn";

    public PiecePawn(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        int d = pieceType == PieceType.White ? 1 : -1;
        var p = pos + new Vector2Int(0, d);
        if (board.IsInBoard(p) && !board.placement.ContainsKey(p))
            yield return p;

        Piece target;
        if (Get(p + Vector2Int.left, out target) && target.pieceType != pieceType)
        {
            yield return p + Vector2Int.left;
        }
        if (Get(p + Vector2Int.right, out target) && target.pieceType != pieceType)
        {
            yield return p + Vector2Int.right;
        }

        // Ã¹ ÀÌµ¿ 2Ä­
        if (!moved)
        {
            var p2 = pos + new Vector2Int(0, 2 * d);
            if (board.IsInBoard(p2) && !Get(p, out Piece _) && !Get(p2, out Piece _))
                yield return p2;
        }

        // ¾ÓÆÄ»ó
        if (Get(pos + Vector2Int.left, out target) && target.pieceType != pieceType && target.SpriteName == "pawn")
        {
            board.Undo();
            if (Get(pos + new Vector2Int(-1, 2 * d), out Piece target2) && target == target2)
                yield return p + Vector2Int.left;
            board.Redo();
        }
        if (Get(pos + Vector2Int.right, out target) && target.pieceType != pieceType && target.SpriteName == "pawn")
        {
            board.Undo();
            if (Get(pos + new Vector2Int(1, 2 * d), out Piece target2) && target == target2)
                yield return p + Vector2Int.right;
            board.Redo();
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        // ¾ÓÆÄ»ó
        if (toPos.x != pos.x && !board.placement.ContainsKey(toPos))
        {
            Piece target = Get(new Vector2Int(toPos.x, pos.y));
            return new CapturingMove(board, this, toPos, target);
        }

        return MoveOrCapture(toPos);
    }
} 

public class PieceKing : Piece
{
    public override string SpriteName => "king";

    public PieceKing(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                var p = pos + new Vector2Int(i, j);
                if (board.IsInBoard(p))
                {
                    if (Get(p, out Piece piece) && piece.pieceType == pieceType)
                        continue;
                    yield return p;
                }
            }
        }

        // Ä³½½¸µ
        if (!moved)
        {
            Vector2Int p;

            for (p = pos + Vector2Int.left; p.x > 0; p += Vector2Int.left)
            {
                if (Get(p, out Piece _)) break;
            }
            if (p.x == 0)
            {
                if (Get(p, out Piece rook) && !rook.moved)
                    yield return pos + new Vector2Int(-2, 0);
            }

            for (p = pos + Vector2Int.right; p.x < board.size.x - 1; p += Vector2Int.right)
            {
                if (Get(p, out Piece _)) break;
            }
            if (p.x == board.size.x - 1)
            {
                if (Get(p, out Piece rook) && !rook.moved)
                    yield return pos + new Vector2Int(2, 0);
            }
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        // Ä³½½¸µ
        if (Mathf.Abs(toPos.x - pos.x) == 2)
        {
            if (toPos.x < pos.x)
            {
                Piece rook = Get(new Vector2Int(0, toPos.y));
                return new CastlingMove(board, this, toPos, rook, new Vector2Int(3, toPos.y));
            }
            else
            {
                Piece rook = Get(new Vector2Int(board.size.x - 1, toPos.y));
                return new CastlingMove(board, this, toPos, rook, new Vector2Int(5, toPos.y));
            }
        }

        return MoveOrCapture(toPos);
    }
}

public class PieceRook : Piece
{
    public override string SpriteName => "rook";

    public PieceRook(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        foreach  (var d in dirs)
        {
            for (var p = pos + d; board.IsInBoard(p); p += d)
            {
                if (Get(p, out Piece piece))
                {
                    if (piece.pieceType != pieceType)
                        yield return p;
                    break;
                }
                yield return p;
            }
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        return MoveOrCapture(toPos);
    }
}

public class PieceBishop : Piece
{
    public override string SpriteName => "bishop";

    public PieceBishop(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    private static readonly Vector2Int[] dirs =
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        foreach (var d in dirs)
        {
            for (var p = pos + d; board.IsInBoard(p); p += d)
            {
                if (Get(p, out Piece piece))
                {
                    if (piece.pieceType != pieceType)
                        yield return p;
                    break;
                }
                yield return p;
            }
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        return MoveOrCapture(toPos);
    }
}

public class PieceKnight : Piece
{
    public override string SpriteName => "knight";

    public PieceKnight(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        foreach (var d in dirs)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                var p = pos + (2 * d) + new Vector2Int(i * d.y, i * d.x);

                if (!board.IsInBoard(p)) continue;

                if (Get(p, out Piece piece) && piece.pieceType == pieceType) continue;

                yield return p;
            }
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        return MoveOrCapture(toPos);
    }
}

public class PieceQueen : Piece
{
    public override string SpriteName => "queen";

    public PieceQueen(Board board, PieceType pieceType) : base(board, pieceType)
    {

    }

    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };

    public override IEnumerable<Vector2Int> MovablePositions()
    {
        foreach (var d in dirs)
        {
            for (var p = pos + d; board.IsInBoard(p); p += d)
            {
                if (Get(p, out Piece piece))
                {
                    if (piece.pieceType != pieceType)
                        yield return p;
                    break;
                }
                yield return p;
            }
        }
    }

    public override BoardAction Move(Vector2Int toPos)
    {
        return MoveOrCapture(toPos);
    }
}