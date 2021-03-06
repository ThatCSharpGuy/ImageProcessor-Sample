﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace ImageProcessorSample
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var balonBytes = File.ReadAllBytes("photo/balon.jpg");

			using (var inStream = new MemoryStream(balonBytes))
			using (var imageFactory = new ImageFactory(false))
			{
				Console.WriteLine("Working on balon.jpg");

				// Resize
				var img = imageFactory.Load(inStream);
				img.Resize(new Size(300, 0));
				img.Save("photo/bolaResized.jpg");

				//imageFactory.Load(inStream)
				//            .Resize(new Size(300, 0))
				//            .Save("photo/bolaResized.jpg");

				// Lower quality
				imageFactory.Load(inStream)
							.Quality(5) // Only works with jpg
							.Save("photo/bolaLow.jpg");

				// Change format
				imageFactory.Load(inStream)
							.Format(new PngFormat { Quality = 10 })
							.Save("photo/balon.png");

			}


			var michaBytes = File.ReadAllBytes("photo/micha.jpg");

			using (var inStream = new MemoryStream(michaBytes))
			using (var imageFactory = new ImageFactory(false))
			{
				Console.WriteLine("Working on micha.jpg");

				// Make a Micha cool portrait
				imageFactory.Load(inStream)
							.Filter(MatrixFilters.HiSatch)
							.Save("photo/michaArt.jpg");

				// Invert-a Micha
				imageFactory.Load(inStream)
							.Filter(MatrixFilters.Invert)
							.Save("photo/michaInverse.jpg");

				// Michstagram
				imageFactory.Load(inStream)
							.Filter(MatrixFilters.Sepia)
							.Tint(Color.LightSalmon)
							.Saturation(50)
							.Save("photo/michaInstagram.jpg");

			}


			var motherBoardBytes = File.ReadAllBytes("photo/motherboard.jpg");

			using (var inStream = new MemoryStream(motherBoardBytes))
			using (var imageFactory = new ImageFactory(false))
			{
				Console.WriteLine("Working on motherboard.jpg");

				// Cropping
				var m = imageFactory.Load(inStream)
	                    .Crop(new Rectangle(100, 100, 250, 250))
	                    .Save("photo/motherboardCropped.jpg");

				m.Rotate(10f)
				 .Save("photo/motherboardRotated.jpg");

				m.Flip(true, true)
				 .Save("photo/motherboardFlipped.jpg");

			}



			// Cognitive services:
			if (!String.IsNullOrEmpty(Keys.CognitiveServices))
			{
				FaceServiceClient faceServiceClient = new FaceServiceClient(Keys.CognitiveServices);


				// Cropping faces:
				var robbieBytes = File.ReadAllBytes("photo/robbie3.jpg");

				Console.WriteLine("Working on robbie3.jpg");

				Face face = null;

				using (var inStream = new MemoryStream(robbieBytes))
				{
					var detectionTask = faceServiceClient.DetectAsync(inStream);
					detectionTask.Wait();
					face = detectionTask.Result.FirstOrDefault();
				}

				if (face != null)
				{
					var faceContainer = new Rectangle(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height);
					using (var inStream = new MemoryStream(robbieBytes))
					using (var imageFactory = new ImageFactory(false))
					{
						imageFactory.Load(inStream)
								.Crop(faceContainer)
								.Save("photo/robbieFace.jpg");
					}
				}


				// Pixelate faces:
				var friendsBytes = File.ReadAllBytes("photo/friends2.jpg");

				Console.WriteLine("Working on friends2.jpg");
				Face[] faces;

				using (var inStream = new MemoryStream(friendsBytes))
				{
					var detectionTask = faceServiceClient.DetectAsync(inStream);
					detectionTask.Wait();
					faces = detectionTask.Result;
				}

				using (var inStream = new MemoryStream(friendsBytes))
				using (var imageFactory = new ImageFactory(false))
				{
					var friendsImage = imageFactory.Load(inStream);
					foreach (var f in faces)
					{
						var faceContainer = new Rectangle(f.FaceRectangle.Left, f.FaceRectangle.Top, f.FaceRectangle.Width, f.FaceRectangle.Height);
						friendsImage.Pixelate(20, faceContainer);
					}
					friendsImage.Save("photo/friendsAnonymous.jpg");
				}
			}

			Console.WriteLine("Done, press any key.");
			Console.Read();
		}
	}
}

