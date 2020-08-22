using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MultiWindows.windows
{
	[Tool]
	public class BaseWindow : KinematicBody2D
	{
		private int _width = 10;
		[Export(PropertyHint.Range, "3,200,1,or_greater")]
		public int Width
		{
			get => _width;
			set
			{
				_width = value;
				BuildWindow();
			}
		}

		private int _height = 10;
		[Export(PropertyHint.Range, "3,200,1,or_greater")]
		public int Height
		{
			get => _height;
			set
			{
				_height = value;
				BuildWindow();
			}
		}

		public TileMap tileMap;
		public TileMap collisionBorderTileMap;

		public override void _Ready()
		{
			tileMap = (TileMap)GetNode("TileMap");
			collisionBorderTileMap = (TileMap)GetNode("CollisionBorder");
		}

		public void BuildWindow()
		{
			tileMap = (TileMap)GetNode("TileMap");
			collisionBorderTileMap = (TileMap)GetNode("CollisionBorder");

			tileMap.Clear();
			collisionBorderTileMap.Clear();

			BuildTileMap();

			BuildCollisionShape();
		}

		public void BuildTileMap()
		{
			BuildBorder();
			BuildBackground();
		}

		public void BuildBorder()
		{
			Vector2 topLeft = Vector2.Zero;
			Vector2 bottomRight = new Vector2(Width, Height) * tileMap.CellSize;

			// Place visible border tiles
			for (int x = 0; x < Width; x++)
			{
				tileMap.SetCell(x, 0, (int)Tiles.Tileset.Border);
				tileMap.SetCell(x, Height - 1, (int)Tiles.Tileset.Border);
			}
			for (int y = 0; y < Height; y++)
			{
				tileMap.SetCell(0, y, (int)Tiles.Tileset.Border);
				tileMap.SetCell(Width - 1, y, (int)Tiles.Tileset.Border);
			}
			
			// Place collision border tiles
			for (int x = (int)tileMap.CellSize.x - 1; x < bottomRight.x - topLeft.x - (tileMap.CellSize.x - 1); x++)
			{
				collisionBorderTileMap.SetCell(x, (int)tileMap.CellSize.y - 1, (int)Tiles.CollisionBorderTileset.Border);
				collisionBorderTileMap.SetCell(x, (int)bottomRight.y - (int)tileMap.CellSize.y, (int)Tiles.CollisionBorderTileset.Border);
			}
			for (int y = 7; y < bottomRight.y - topLeft.y - 7; y++)
			{
				collisionBorderTileMap.SetCell(7, y, (int)Tiles.CollisionBorderTileset.Border);
				collisionBorderTileMap.SetCell((int)bottomRight.x - (int)tileMap.CellSize.y, y, (int)Tiles.CollisionBorderTileset.Border);
			}
		}

		public void BuildBackground()
		{
			for (int x = 1; x < Width - 1; x++)
			{
				for (int y = 1; y < Height - 1; y++)
				{
					tileMap.SetCell(x, y, (int)Tiles.Tileset.Sky);
				}
			}
		}

		public void BuildCollisionShape()
		{
			CollisionShape2D collisionShape = new CollisionShape2D();
			RectangleShape2D rectangleShape = new RectangleShape2D();
			collisionShape.Shape = rectangleShape;

			Vector2 size = new Vector2(Width, Height) * tileMap.CellSize;
			Vector2 halfSize = size / 2;
			rectangleShape.Extents = halfSize - new Vector2(1, 1) * tileMap.CellSize - new Vector2(0.1f, 0.1f);
			collisionShape.Position = halfSize;

			if (HasNode("CollisionShape2D"))
			{
				CollisionShape2D oldCollisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
				collisionShape.Name = "CollisionShape2D";
				oldCollisionShape.ReplaceBy(collisionShape);
				return;
			}

			AddChild(collisionShape, true);
			collisionShape.Owner = GetTree().EditedSceneRoot;
		}

		public void TeleportSafely(Vector2 pos)
		{
			for (int i = 0; i < 50; i++)
			{
				float targetDist = Position.DistanceTo(pos);
				if (targetDist < 1)
				{
					Position = Position;
					return;
				}
				Vector2 targetDir = Position.DirectionTo(pos);
				Vector2 velocity = targetDir * 200;
				MoveAndSlide(velocity);
			}
		}

		public void RemoveOverlappingBorderCollisions(BaseWindow overlappingWindow)
		{
			Vector2 overlappingWindowSize = new Vector2(overlappingWindow.Width - 2, overlappingWindow.Height - 2) * overlappingWindow.tileMap.CellSize;
			Rect2 overlappingWindowRect = new Rect2(overlappingWindow.Position + tileMap.CellSize, overlappingWindowSize);

			// Only iterates through the tiles of the TileMap that contains the collision shapes for the border
			// The border tiles that do not have any collision shapes (in the main TileMap) have a z-index of -1 which means
			// that they will appear behind the window itself and so do not have to be removed
			foreach (Vector2 tilePos in collisionBorderTileMap.GetUsedCells())
			{
				Vector2 pos = Position + tilePos;

				if (overlappingWindowRect.HasPoint(pos))
				{
					collisionBorderTileMap.SetCellv(tilePos, (int)Tiles.CollisionBorderTileset.Empty);
				}
			}
		}
	}
}
