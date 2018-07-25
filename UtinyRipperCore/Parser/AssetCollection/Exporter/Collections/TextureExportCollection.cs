﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters.Classes;
using UtinyRipper.Classes;

namespace UtinyRipper.AssetExporters
{
	public class TextureExportCollection : AssetsExportCollection
	{
		public TextureExportCollection(IAssetExporter assetExporter, Object textureAsset) :
			this(assetExporter, (Texture2D)textureAsset)
		{
		}

		public TextureExportCollection(IAssetExporter assetExporter, Texture2D texture):
			base(assetExporter, texture, CreateImporter(texture))
		{
			List<Sprite> sprites = new List<Sprite>();
			foreach (Object asset in texture.File.FetchAssets())
			{
				if (asset.ClassID == ClassIDType.Sprite)
				{
					Sprite sprite = (Sprite)asset;
					if (sprite.RD.Texture.IsObject(texture))
					{
						sprites.Add(sprite);
						AddAsset(sprite);
					}
				}
			}
			TextureImporter importer = (TextureImporter)MetaImporter;
			importer.Sprites = sprites;
		}

		protected static IAssetImporter CreateImporter(Texture2D texture)
		{
			//if (Config.IsConvertTexturesToPNG)
			{
				return new TextureImporter(texture);
			}
			//else
			{
				return new IHVImageFormatImporter(texture);
			}
		}

		public override ExportPointer CreateExportPointer(Object asset, bool isLocal)
		{
			if(Config.IsConvertTexturesToPNG)
			{
				return base.CreateExportPointer(asset, isLocal);
			}
			else
			{
				if(Asset == asset)
				{
					return base.CreateExportPointer(asset, isLocal);
				}
				else
				{
					string exportID = GetExportID(asset);
					return new ExportPointer(exportID, UtinyGUID.MissingReference, AssetType.Meta);
				}
			}
		}

		protected override string ExportInner(ProjectAssetContainer container, string filePath)
		{
			AssetExporter.Export(container, Asset, filePath);
			return filePath;
		}

		protected override string GenerateExportID(Object asset)
		{
			string id = $"{(int)asset.ClassID}{m_nextExportID:D5}";
			m_nextExportID += 2;
			return id;
		}

		private int m_nextExportID = 0;
	}
}
