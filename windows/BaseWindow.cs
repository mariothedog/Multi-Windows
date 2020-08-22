using Godot;

namespace MultiWindows.windows
{
	[Tool]
	public class BaseWindow : KinematicBody2D
	{
		private int _width = 10;
		[Export(PropertyHint.Range, "1,200,1,or_greater")]
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
		[Export(PropertyHint.Range, "1,10,1,or_greater")]
		public int Height
		{
			get => _height;
			set
			{
				_height = value;
				BuildWindow();
			}
		}

		private int _borderWidth = 8;
		[Export(PropertyHint.Range, "0,50")]
		public int BorderWidth
		{
			get => _borderWidth;
			set
			{
				_borderWidth = value;
				BuildWindow(true);
			}
		}

		public TileMap tileMap;
		public TileMap borderTileMap;

		public override void _Ready()
		{
			tileMap = (TileMap)GetNode("TileMap");
			borderTileMap = (TileMap)GetNode("Border");
		}

		public void BuildWindow(bool justBorder = false)
		{
			if (!justBorder)
			{
				tileMap = (TileMap)GetNode("TileMap");
				ClearTileMap(tileMap);
				BuildBackground();
				BuildCollisionShape(tileMap.CellSize);
			}
			
			borderTileMap = (TileMap)GetNode("Border");
			ClearTileMap(borderTileMap);
			BuildBorder();
		}

		public void ClearTileMap(TileMap tileMap)
		{
			foreach (Vector2 tilePos in tileMap.GetUsedCells())
			{
				tileMap.SetCellv(tilePos, (int)Tiles.Tileset.Empty);
			}
		}

		public void BuildBorder()
		{
			Vector2 topLeft = new Vector2(-1, -1);
			Vector2 bottomRight = new Vector2(Width, Height) * tileMap.CellSize;

			for (int x = -BorderWidth + 1; x <= bottomRight.x - topLeft.x + BorderWidth - 1; x++)
			{
				for (int w = 0; w < BorderWidth; w++)
				{
					int tileID = (int)Tiles.BorderTileset.Border;
					if (w == 0)
					{
						tileID = (int)Tiles.BorderTileset.BorderCollision;
					}
					borderTileMap.SetCell((int)topLeft.x + x, (int)topLeft.y - w, tileID);
					borderTileMap.SetCell((int)topLeft.x + x, (int)bottomRight.y + w, tileID);
				}
			}
			for (int y = 0; y < bottomRight.y - topLeft.y; y++)
			{
				for (int w = 0; w < BorderWidth; w++)
				{
					int tileID = (int)Tiles.BorderTileset.Border;
					if (w == 0)
					{
						tileID = (int)Tiles.BorderTileset.BorderCollision;
					}
					borderTileMap.SetCell((int)topLeft.x - w, (int)topLeft.y + y, tileID);
					borderTileMap.SetCell((int)bottomRight.x + w, (int)topLeft.y + y, tileID);
				}
			}
		}

		public void BuildBackground()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					tileMap.SetCell(x, y, (int)Tiles.Tileset.Sky);
				}
			}
		}

		public void BuildCollisionShape(Vector2 sizeCoefficient)
		{
			CollisionShape2D collisionShape = new CollisionShape2D();
			RectangleShape2D rectangleShape = new RectangleShape2D();
			collisionShape.Shape = rectangleShape;

			Vector2 size = new Vector2(Width, Height) * sizeCoefficient;
			Vector2 halfSize = size / 2;
			rectangleShape.Extents = halfSize;
			collisionShape.Position = halfSize;

			if (HasNode("CollisionShape2D"))
			{
				CollisionShape2D oldCollisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
				oldCollisionShape.ReplaceBy(collisionShape);
				return;
			}

			AddChild(collisionShape);
			collisionShape.Owner = GetTree().EditedSceneRoot;
		}

		public void TeleportSafely(Vector2 pos)
		{
			for (int i = 0; i < 200; i++)
			{
				float targetDist = Position.DistanceTo(pos);
				if (targetDist < 1)
				{
					Position = Position;
					return;
				}
				Vector2 targetDir = Position.DirectionTo(pos);
				Vector2 velocity = targetDir * 50;
				MoveAndSlide(velocity);
			}
		}

		public void RemoveOverlappingBorder(BaseWindow overlappingWindow)
		{
			Vector2 overlappingWindowSize = new Vector2(overlappingWindow.Width, overlappingWindow.Height) * overlappingWindow.tileMap.CellSize;
			Rect2 overlappingWindowRect = new Rect2(overlappingWindow.Position, overlappingWindowSize);

			// Only iterates through the border tiles that have collision shapes
			// The border tiles that do not have any collision shapes have a z-index of -1 which means
			// that they will appear behind the window itself and so do not have to be removed
			foreach (Vector2 tilePos in borderTileMap.GetUsedCellsById((int)Tiles.BorderTileset.BorderCollision))
			{
				Vector2 pos = Position + tilePos;
				
				if (overlappingWindowRect.HasPoint(pos))
				{
					borderTileMap.SetCellv(tilePos, (int)Tiles.BorderTileset.Empty);
				}
			}
		}
	}
}
