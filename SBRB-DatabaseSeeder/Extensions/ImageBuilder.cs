using Jil;
using SBRB.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.Collections.Generic;
using System.IO;

namespace SBRB.Seeder.Extensions
{
    /// <summary>Class housing an extension for layering an image over a subject, following Starbounds formats/methods.</summary>
    // Or at least what I think they are...
    public static class ImageBuilder
    {
        static readonly Dictionary<ArmorTypes, int[]> ARMOR_FRAME_COORDS = new Dictionary<ArmorTypes, int[]>
        {
            {ArmorTypes.Head, new int[]{ 0, 0, 16, 16} },
            {ArmorTypes.Chest, new int[]{ 16, 0, 32, 16} },
            {ArmorTypes.Legs, new int[]{ 32, 0, 48, 16} },
            {ArmorTypes.Back, new int[]{ 48, 0, 64, 16} }
        };

        // These nested classes are used in .frames file deserialization.
        // Nested because they're not used anywhere else
        class FramesFile
        {
            /// <summary>Dictionary holding aliases for frames. [alias:frame]</summary>
            public Dictionary<string, string> aliases;

            public Framegrid frameGrid;

            // Some things use a frame list for whatever reason...
            public Dictionary<string, int[]> frameList;
        }

        class Framegrid
        {
            /// <summary>Frame size [x,y]</summary>
            public int[] size;

            /// <summary>Number of rows and columns [x,y]</summary>
            public int[] dimensions;

            public string[][] names;
        }

        // Because try/catch is heavier than a simple enum check.
        /// <summary>Enum responsible for indicating the result of attempting to add a layer through the 'AddLayer' method.</summary>
        public enum AddLayerResult { Done, ImageFileNotfound, FramesFileNotFound, FrameNotFound, ErronousFramesFile }

        /// <summary>
        /// Method used to add a layer to the subject 'Image'. Can use an absolute path by not passing an 'itemFilePath'
        /// </summary>
        /// <param name="fullImage">Subject Image</param>
        /// <param name="imagePath">Path to image</param>
        /// <param name="itemFilePath">Path to the file using the image</param>
        /// <returns>Returns true if the image was added successfully. False otherwise.</returns>
        public static AddLayerResult AddLayer(this Image<Rgba32> fullImage, string imagePath, string itemFilePath, ArmorTypes? armorType)
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

            // Image file not found
            if (!File.Exists(imagePath))
                return AddLayerResult.ImageFileNotfound;

            bool useFull = false;
            int xOffset = 0;
            int yOffset = 0;
            int width = 0;
            int height = 0;

            if (frame == null)
            {
                // If the path doesn't point to a frame
                if (armorType == null)
                {
                    // If its a non-armor item
                    useFull = true;
                }
                else
                {
                    // If its an armor item, use the default armor icon assignment method
                    int[] frameCoords = ARMOR_FRAME_COORDS[armorType.Value];
                    xOffset = frameCoords[0];
                    yOffset = frameCoords[1];
                    width = frameCoords[2] - frameCoords[0];
                    height = frameCoords[3] - frameCoords[1];
                }
            }
            else
            {
                // If the path points to a frame
                string framesPath = $"{Path.GetDirectoryName(imagePath)}\\{Path.GetFileNameWithoutExtension(imagePath)}.frames";

                // If the frames file was not found, look for the default.frames file
                if (!File.Exists(framesPath))
                    framesPath = $"{Path.GetDirectoryName(imagePath)}\\default.frames";

                if (File.Exists(framesPath))
                {
                    // If it was found...
                    // Deserialize the .frames files
                    FramesFile frames = JSON.Deserialize<FramesFile>(File.ReadAllText(framesPath));

                    // If frameGrid is null, use frameList
                    if (frames.frameGrid == null)
                    {
                        if (frames.frameList == null)
                            return AddLayerResult.ErronousFramesFile;

                        if (!frames.frameList.ContainsKey(frame))
                            return AddLayerResult.FrameNotFound;

                        int[] frameCoords = frames.frameList[frame];
                        xOffset = frameCoords[0];
                        yOffset = frameCoords[1];
                        width = frameCoords[2] - frameCoords[0];
                        height = frameCoords[3] - frameCoords[1];
                    }
                    else
                    {
                        // The frame we're looking for might be an alias. Check if it is, and replace it with the original frame name.
                        if (frames.aliases != null && frames.aliases.ContainsKey(frame))
                            frame = frames.aliases[frame];

                        // If names are not defined, set their names to consequent numbers
                        if (frames.frameGrid.names == null)
                        {
                            int frameCount = 0;

                            frames.frameGrid.names = new string[frames.frameGrid.dimensions[1]][];
                            for (int i = 0; i < frames.frameGrid.dimensions[1]; i++)
                            {
                                frames.frameGrid.names[i] = new string[frames.frameGrid.dimensions[0]];
                                for (int j = 0; j < frames.frameGrid.dimensions[0]; j++)
                                {
                                    frames.frameGrid.names[i][j] = frameCount.ToString();
                                    frameCount++;
                                }
                            }
                        }

                        // Find the frame we're looking for
                        bool found = false;
                        int xIndex = 0;
                        int yIndex = 0;
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
                        xOffset = xIndex * frames.frameGrid.size[0];
                        yOffset = yIndex * frames.frameGrid.size[1];
                        width = frames.frameGrid.size[0];
                        height = frames.frameGrid.size[1];
                    }
                }
                else
                {
                    // If the frames file doesnt exist
                    if (armorType == null)
                    {
                        // If its not an armor item, return a failure
                        return AddLayerResult.FramesFileNotFound;
                    }
                    else
                    {
                        // If its an armor item, use the default armor icon assignment method
                        int[] frameCoords = ARMOR_FRAME_COORDS[armorType.Value];
                        xOffset = frameCoords[0];
                        yOffset = frameCoords[1];
                        width = frameCoords[2] - frameCoords[0];
                        height = frameCoords[3] - frameCoords[1];
                    }
                }
            }

            // Do the image magic
            using (Image<Rgba32> img = Image.Load(imagePath))
            {
                // If its an armor item, check if the image has the coordinates.
                // If the image is smaller than the requested frame, use the whole image instead.
                if (armorType != null)
                {
                    if (img.Width < xOffset + width || img.Height < yOffset + height)
                        useFull = true;
                }

                // Cut out the frame we need if useFull is false
                if (!useFull)
                {
                    img.Mutate(i => i.Crop(new Rectangle(xOffset, yOffset, width, height)));
                }

                // Set the subject images dimensions to that of the frame if they're 1;1 (1;1 marks the image as uninitialized)
                if (fullImage.Height == 1 && fullImage.Width == 1)
                    fullImage.Mutate(mi => mi.Resize(img.Width, img.Height));

                // Layer the frame over the subject image
                fullImage.Mutate(mi => mi.DrawImage(img, 1));
            }

            return AddLayerResult.Done;
        }
    }
}
