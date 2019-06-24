using Jil;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;

namespace SBRB_DatabaseSeeder.Workers
{
    static class ImageBuilder
    {
        class FramesFile
        {
            public Framegrid frameGrid { get; set; }
            public Dictionary<string, string> aliases { get; set; }
        }

        class Framegrid
        {
            public int[] size { get; set; }
            public int[] dimensions { get; set; }
            public string[][] names { get; set; }
        }

        public static void AddLayer(this Image<Rgba32> fullImage, string imagePath, string itemFilePath)
        {
            // Check if the path is from the same directory, or from the mods root
            if (imagePath.StartsWith('/'))
            {
                // Path from mod root
                imagePath = $"{Program.modPath}\\{imagePath.Replace('/', '\\')}";
            }
            else
            {
                // Same directory
                imagePath = $"{itemFilePath}\\{imagePath}";
            }

            fullImage.AddLayer(imagePath);
        }

        public static void AddLayer(this Image<Rgba32> fullImage, string imagePath)
        {
            // Check whether a specific frame is used
            // Starting with an offset to ignore the disks : (D:\\)
            int frameStartPoint = imagePath.IndexOf(':', 3);
            string frame = null;

            if (frameStartPoint > -1)
            {
                string[] splitPath = imagePath.Split(':', 3); // Account for the disk split
                imagePath = $"{splitPath[0]}:{splitPath[1]}";
                frame = splitPath[2];
            }

            if (File.Exists(imagePath))
            {
                // If a specific frame is used, find the frames file. Return null if it wasn't found.
                // EDGE CASE: the frames file may be defined in a different mod
                if (frame != null)
                {
                    string imageFile = Path.GetDirectoryName(imagePath);
                    string framesPath = $"{imageFile}\\{Path.GetFileNameWithoutExtension(imageFile)}.frames";

                    if (!File.Exists(framesPath))
                        framesPath = $"{imageFile}\\default.frames";

                    if (!File.Exists(framesPath))
                        return;

                    FramesFile frames = JSON.Deserialize<FramesFile>(File.ReadAllText(framesPath));

                    int xIndex = 0;
                    int yIndex = 0;
                    bool found = false;
                    for (; xIndex < frames.frameGrid.names.Length; xIndex++)
                    {
                        for (; yIndex < frames.frameGrid.names[xIndex].Length; yIndex++)
                        {
                            if (frames.frameGrid.names[xIndex][yIndex] == frame)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }

                    if (!found)
                        return;

                    int xOffset = xIndex * frames.frameGrid.dimensions[0];
                    int yOffset = yIndex * frames.frameGrid.dimensions[1];
                    int width = frames.frameGrid.size[0];
                    int height = frames.frameGrid.size[1];

                    using (Image<Rgba32> img = Image.Load(imagePath))
                    {
                        img.Mutate(i => i.Crop(new Rectangle(xOffset, yOffset, width, height)));

                        if (fullImage.Height == 1 && fullImage.Width == 1)
                            fullImage.Mutate(mi => mi.Resize(img.Width, img.Height));
                        fullImage.Mutate(mi => mi.DrawImage(img, 1));
                    }
                }
            }
        }
    }
}
