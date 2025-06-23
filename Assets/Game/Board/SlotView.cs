using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotView : MonoBehaviour
{
    [SerializeField]
    private Color c1;
    [SerializeField]
    private Color c2;
    [SerializeField]
    AssetList sprites;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteRenderer pieceSpriteRenderer;
    [SerializeField]
    private SpriteRenderer overlaySpriteRenderer;

    private BoardView boardView;
    private Vector2Int pos;
    private Piece piece;

    public bool overlay
    { 
        get
        {
            return _overlay;
        }
        set
        {
            _overlay = value;
            overlaySpriteRenderer.enabled = _overlay;
        }
    }
    private bool _overlay = false;

    public void Initialize(BoardView boardView, Vector2Int pos)
    {
        this.boardView = boardView;
        this.pos = pos;

        spriteRenderer.color = (pos.x + pos.y) % 2 == 0 ? c1 : c2;
    }

    public void UpdateState()
    {
        if (!boardView.board.placement.TryGetValue(pos, out Piece piece))
        {
            this.piece = null;
            pieceSpriteRenderer.sprite = null;
            return;
        }

        this.piece = piece;
        var spriteName = (piece.pieceType == PieceType.White ? "w_" : "b_") + piece.SpriteName;
        foreach (var asset in sprites.data)
        {
            if (asset.name == spriteName)
            {
                Texture2D texture = asset as Texture2D;
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                pieceSpriteRenderer.sprite = sprite;
                return;
            }
        }
    }

    public void OnClick()
    {
        if (boardView.currentSelectedPos == null)
        {
            if (piece == null || piece.pieceType != boardView.currentPlayerType) return;

            boardView.currentSelectedPos = pos;

            bool movable = false;

            foreach (var pos in piece.MovablePositions())
            {
                boardView.placement[pos].overlay = true;
                movable = true;
            }

            if (!movable)
            {
                boardView.currentSelectedPos = null;
            }
        }
        else
        {
            if (overlay)
            {
                boardView.board.MovePiece((Vector2Int)boardView.currentSelectedPos, pos);
            }
            boardView.ClearOverlay();
            boardView.currentSelectedPos = null;
        }
    }
}
