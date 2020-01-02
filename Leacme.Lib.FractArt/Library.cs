// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Utilities.Media;
using Utilities.Media.Procedural;

namespace Leacme.Lib.FractArt {

	public class Library {

		public Library() {

		}

		/// <summary>
		/// Generate Perlin Noise procedurally
		/// /// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="octavesPercent"></param>
		/// <param name="percentFrequency"></param>
		/// <returns>Bitmap stream in jpg format</returns>
		public async Task<Stream> GetPerlinNoise(int width, int height, int octavesPercent, int percentFrequency) {
			if ((octavesPercent < 1) || (octavesPercent > 100)) {
				throw new ArgumentException(nameof(octavesPercent) + " must between 1 and 100");
			}
			if ((percentFrequency < 1) || (percentFrequency > 100)) {
				throw new ArgumentException(nameof(percentFrequency) + " must between 1 and 100");
			}
			return await Task.Run(() => { return GetStreamFromBitmap(PerlinNoise.Generate(Width: width, Height: height, Octaves: (int)Math.Ceiling(octavesPercent / 33d), Frequency: (float)new Random().NextDouble() + new Random().Next((int)Math.Ceiling(percentFrequency / 25d)), Amplitude: 1f, Persistance: 1f, MinRGBValue: 0, MaxRGBValue: 255, Seed: Guid.NewGuid().GetHashCode())); });
		}

		/// <summary>
		/// Generate Midpoint Displacement procedurally
		/// /// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="crackedPercent"></param>
		/// <param name="iterationsPercent"></param>
		/// <param name="crackDirectionChangePercent"></param>
		/// <param name="crackLengthPercent"></param>
		/// <returns>Bitmap stream in jpg format</returns>
		public async Task<Stream> GetMidpointDisplacement(int width, int height, int crackedPercent, int iterationsPercent, int crackDirectionChangePercent, int crackLengthPercent) {
			if ((crackedPercent < 1) || (crackedPercent > 100)) {
				throw new ArgumentException(nameof(crackedPercent) + " must between 1 and 100");
			}
			if ((iterationsPercent < 1) || (iterationsPercent > 100)) {
				throw new ArgumentException(nameof(iterationsPercent) + " must between 1 and 100");
			}
			if ((crackDirectionChangePercent < 1) || (crackDirectionChangePercent > 100)) {
				throw new ArgumentException(nameof(crackDirectionChangePercent) + " must between 1 and 100");
			}
			if ((crackLengthPercent < 1) || (crackLengthPercent > 100)) {
				throw new ArgumentException(nameof(crackLengthPercent) + " must between 1 and 100");
			}
			return await Task.Run(() => { return GetStreamFromBitmap(MidpointDisplacement.Generate(Width: width, Height: height, NumberOfCracks: crackedPercent, Iterations: iterationsPercent, MaxChange: crackDirectionChangePercent / 10, MaxLength: crackLengthPercent, Seed: Guid.NewGuid().GetHashCode())); });
		}

		/// <summary>
		/// Generate Fault Formation procedurally
		/// /// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="faultsPercent"></param>
		/// <returns>Bitmap stream in jpg format</returns>
		public async Task<Stream> GetFaultFormation(int width, int height, int faultsPercent) {
			if ((faultsPercent < 1) || (faultsPercent > 100)) {
				throw new ArgumentException(nameof(faultsPercent) + " must between 1 and 100");
			}
			return await Task.Run(() => { return GetStreamFromBitmap(FaultFormation.Generate(Width: width, Height: height, NumberFaults: faultsPercent * 10, Seed: Guid.NewGuid().GetHashCode())); });
		}

		/// <summary>
		/// Generate Cellular Texture procedurally
		/// /// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="granularityPercent"></param>
		/// <returns>Bitmap stream in jpg format</returns>
		public async Task<Stream> GetCellularTexture(int width, int height, int granularityPercent) {
			if ((granularityPercent < 1) || (granularityPercent > 100)) {
				throw new ArgumentException(nameof(granularityPercent) + " must between 1 and 100");
			}
			return await Task.Run(() => { return GetStreamFromBitmap(CellularTexture.Generate(Width: width, Height: height, NumberOfPoints: granularityPercent * 10, Seed: Guid.NewGuid().GetHashCode())); });
		}

		/// <summary>
		/// Convert SwiftBitmap to Stream
		/// /// </summary>
		/// <param name="bitmapToStream"></param>
		/// <returns></returns>
		public async Task<Stream> GetStreamFromBitmap(SwiftBitmap bitmapToStream) {
			return await Task.Run(() => {
				var ts = new MemoryStream();
				using (var tb = bitmapToStream) {
					tb.InternalBitmap.Save(ts, ImageFormat.Jpeg);
					ts.Position = 0;
				}
				return ts;
			});
		}
	}

}