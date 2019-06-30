using Jil;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SBRB_DatabaseSeeder.Workers
{
    /// <summary>
    /// Class housing an extension for layering an image over a subject, following Starbounds formats/methods.
    /// </summary>
    // Or at least what I think they are...
    static class ImageBuilder
    {
        // These nested classes are used in .frames file deserialization.
        // Nested because they're not used anywhere else
        class FramesFile
        {
            /// <summary>Dictionary holding aliases for frames. [alias:frame]</summary>
            public Dictionary<string, string> aliases { get; set; }

            public Framegrid frameGrid { get; set; }
        }

        class Framegrid
        {
            /// <summary>Frame size [x,y]</summary>
            public int[] size { get; set; }

            /// <summary>Number of rows and columns [x,y]</summary>
            public int[] dimensions { get; set; }

            public string[][] names { get; set; }
        }

        // Because try/catch is heavier than a simple enum check.
        /// <summary>Enum responsible for indicating the result of attempting to add a layer through the 'AddLayer' method.</summary>
        public enum AddLayerResult { Done, ImageFileNotfound, FramesFileNotFound, FrameNotFound }

        /// <summary>
        /// Method used to add a layer to the subject 'Image'. Can use an absolute path by not passing an 'itemFilePath'
        /// </summary>
        /// <param name="fullImage">Subject Image</param>
        /// <param name="imagePath">Path to image</param>
        /// <param name="itemFilePath">Path to the file using the image</param>
        /// <returns>Returns true if the image was added successfully. False otherwise.</returns>
        public static AddLayerResult AddLayer(this Image<Rgba32> fullImage, string imagePath, string itemFilePath)
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
                imagePath = $"{Path.GetDirectoryName(itemFilePath)}\\{imagePath}";
            }

            // Check whether a specific frame is used
            // Starting with an offset to ignore the disks : (D:\\)
            int frameStartPoint = imagePath.IndexOf(':', 3);
            string frame = null;

            // Extract the frame and clean 'imagePath' if it has a frame pointer (i.e myImage.png:frame2 -> myImage.png | frame2)
            if (frameStartPoint > -1)
            {
                string[] splitPath = imagePath.Split(':', 3); // Account for the disk split
                imagePath = $"{splitPath[0]}:{splitPath[1]}";
                frame = splitPath[2];
            }

            // Image file not found - Return false, indicating the addition was unsuccessful.
            if (!File.Exists(imagePath))
                return AddLayerResult.ImageFileNotfound;

            // If a specific frame is used, find the frames file.
            // EDGE CASE : the frames file may be defined in a different mod
            if (frame != null)
            {
                // Look for a file with the same name as the given 'imagePath', but has '.frames' added to it after its original extension
                string framesPath = $"{Path.GetDirectoryName(imagePath)}\\{Path.GetFileNameWithoutExtension(imagePath)}.frames";

                // If that doesn't exist, look for a file named 'default.frames' in the same directory
                if (!File.Exists(framesPath))
                    framesPath = $"{Path.GetDirectoryName(imagePath)}\\default.frames";

                // Frames file not found - Return false, indicating the addition was unsuccessful.
                if (!File.Exists(framesPath))
                    return AddLayerResult.FramesFileNotFound;

                // Deserialize the .frames files
                FramesFile frames = JSON.Deserialize<FramesFile>(File.ReadAllText(framesPath));

                // The frame we're looking for might be an alias. Check if it is, and replace it with the original frame name.
                if (frames.aliases != null && frames.aliases.ContainsKey(frame))
                    frame = frames.aliases[frame];

                // Find the frame we're looking for
                int xIndex = 0;
                int yIndex = 0;
                bool found = false;
                for (; yIndex < frames.frameGrid.names.Length; yIndex++)
                {
                    for (; xIndex < frames.frameGrid.names[yIndex].Length; xIndex++)
                    {
                        if (frames.frameGrid.names[yIndex][xIndex] == frame)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                // Frame not found - Return false, indicating the addition was unsuccessful.
                if (!found)
                    return AddLayerResult.FrameNotFound;

                // Setup the correct frame coordinates
                int xOffset = xIndex * frames.frameGrid.size[0];
                int yOffset = yIndex * frames.frameGrid.size[1];

                // Load the entire image containing the frames
                using (Image<Rgba32> img = Image.Load(imagePath))
                {
                    // Cut out only the required frame
                    img.Mutate(i => i.Crop(new Rectangle(xOffset, yOffset, frames.frameGrid.size[0], frames.frameGrid.size[1])));

                    // Set the subject images dimensions to that of the frame if they're 1;1 (1;1 marks the image as uninitialized)
                    if (fullImage.Height == 1 && fullImage.Width == 1)
                        fullImage.Mutate(mi => mi.Resize(img.Width, img.Height));

                    // Layer the frame over the subject image
                    fullImage.Mutate(mi => mi.DrawImage(img, 1));
                }
            }
            // Just load the required image, and overlay it over the subject image if it doesn't point to a frame.
            else
            {
                // This segment is separate from the one with the frames as it doesn't crop a frame from the required image
                // And I'm duplicating the code because I don't want a container wrapping around the entire thing for just a small code difference.

                // Load the entire image containing the frames
                using (Image<Rgba32> img = Image.Load(imagePath))
                {
                    // Set the subject images dimensions to that of the frame if they're 1x1 (1x1 marks the image as uninitialized)
                    if (fullImage.Height == 1 && fullImage.Width == 1)
                        fullImage.Mutate(mi => mi.Resize(img.Width, img.Height));

                    // Layer the frame over the subject image
                    fullImage.Mutate(mi => mi.DrawImage(img, 1));
                }
            }

            return AddLayerResult.Done;
        }
    }
}
