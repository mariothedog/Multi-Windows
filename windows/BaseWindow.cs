using Godot;
using System.Collections.Generic;

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
		[Export(PropertyHint.Range, "0,10,1,or_greater")]
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
					borderTileMap.SetCell((int)topLeft.x + x, (int)topLeft.y - w, (int)Tiles.BorderTileset.Border);
					borderTileMap.SetCell((int)topLeft.x + x, (int)bottomRight.y + w, (int)Tiles.BorderTileset.Border);
				}
			}
			for (int y = 0; y < bottomRight.y - topLeft.y; y++)
			{
				for (int w = 0; w < BorderWidth; w++)
				{
					borderTileMap.SetCell((int)topLeft.x - w, (int)topLeft.y + y, (int)Tiles.BorderTileset.Border);
					borderTileMap.SetCell((int)bottomRight.x + w, (int)topLeft.y + y, (int)Tiles.BorderTileset.Border);
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

		Rect2 overlappingWindowRect = new Rect2();
		public void UpdateBorder(BaseWindow overlappingWindow)
		{
			Vector2 overlappingWindowSize = new Vector2(overlappingWindow.Width, overlappingWindow.Height) * overlappingWindow.tileMap.CellSize;
			overlappingWindowRect = new Rect2(overlappingWindow.Position, overlappingWindowSize);
			Update();

			foreach (Vector2 tilePos in borderTileMap.GetUsedCells())
			{
				Vector2 pos = Position + tilePos;
				
				if (overlappingWindowRect.HasPoint(pos))
				{
					borderTileMap.SetCellv(tilePos, (int)Tiles.BorderTileset.Empty);
				}
			}
		}

		//public override void _Draw()
		//{
		//	Rect2 rect = overlappingWindowRect;
		//	rect.Position -= Position;
		//	DrawRect(rect, new Color(1, 0, 0), true);
		//}

		//Rect2 overlappingWindowRect = new Rect2();
		//List<Rect2> rrr = new List<Rect2>();
		//public void UpdateBorder(BaseWindow overlappingWindow)
		//{
		//	Vector2 overlappingWindowSize = new Vector2(overlappingWindow.Width, overlappingWindow.Height) * overlappingWindow.tileMap.CellSize;
		//	Rect2 overlappingWindowRect = new Rect2(overlappingWindow.Position + new Vector2(1, 1) * overlappingWindow.tileMap.CellSize,
		//		overlappingWindowSize - new Vector2(2, 2) * overlappingWindow.tileMap.CellSize);
		//	//rrr = new List<Rect2>();
		//	foreach (Vector2 tilePos in borderTileMap.GetUsedCells())
		//	{
		//		Vector2 pos = Position + tilePos * tileMap.CellSize;
		//		Rect2 tileRect = new Rect2(pos + new Vector2(1, 1), tileMap.CellSize - new Vector2(2, 2));

		//		if (overlappingWindowRect.Encloses(tileRect))
		//		{
		//			//rrr.Add(tileRect);
		//			int tileID = (int)Tiles.Tileset.Empty;
		//			//if (!overlappingWindowRect.Encloses(tileRect)) 
		//			//{
		//			//	tileID = (int)Tiles.Tileset.Connecting;
		//			//}
		//			tileMap.SetCellv(tilePos, tileID);
		//		}
		//	}
		//	Update();
		//}

		//public override void _Draw()
		//{
		//	//Rect2 e = overlappingWindowRect;
		//	//e.Position -= Position;
		//	//DrawRect(e, Color.Color8(255, 0, 0), true, 1);

		//	//foreach (Rect2 p in rrr)
		//	//{
		//	//	Rect2 z = p;
		//	//	z.Position -= Position;
		//	//	DrawRect(z, Color.Color8(0, 255, 0), false, 1);
		//	//}
		//}

		//public bool EnclosesInAtLeastOneDimension(Rect2 rect, Rect2 rect2)
		//{
		//	return ((rect2.Position.x >= rect.Position.x) && ((rect2.Position.x + rect2.Size.x) <= (rect.Position.x + rect.Size.x))) ||
		//		((rect2.Position.y >= rect.Position.y) && ((rect2.Position.y + rect2.Size.y) <= (rect.Position.y + rect.Size.y)));
		//}
	}
}
