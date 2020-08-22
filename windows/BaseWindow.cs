using Godot;

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

		private void BuildWindow()
		{
			TileMap tileMap = (TileMap)GetNode("TileMap");
			ClearTileMap(tileMap);
			BuildTileMap(tileMap);
			BuildCollisionShape(tileMap.CellSize);
		}

		private void ClearTileMap(TileMap tileMap)
		{
			foreach (Vector2 tilePos in tileMap.GetUsedCells())
			{
				tileMap.SetCellv(tilePos, (int)Tiles.Tileset.Empty);
			}
		}

		private void BuildTileMap(TileMap tileMap)
		{
			BuildBorder(tileMap);
			BuildBackground(tileMap);
		}

		private void BuildBorder(TileMap tileMap)
		{
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
		}

		private void BuildBackground(TileMap tileMap)
		{
			for (int x = 1; x < Width - 1; x++)
				for (int y = 1; y < Height - 1; y++)
				{
					tileMap.SetCell(x, y, (int)Tiles.Tileset.Sky);
				}
		}

		private void BuildCollisionShape(Vector2 sizeCoefficient)
		{
			CollisionShape2D collisionShape = new CollisionShape2D();
			RectangleShape2D rectangleShape = new RectangleShape2D();
			collisionShape.Shape = rectangleShape;

			Vector2 size = new Vector2(Width, Height) * sizeCoefficient;
			Vector2 halfSize = size / 2;
			rectangleShape.Extents = halfSize - new Vector2(1, 1) * sizeCoefficient;
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
	}
}
