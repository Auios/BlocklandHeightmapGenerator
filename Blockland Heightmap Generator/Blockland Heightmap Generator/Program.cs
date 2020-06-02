using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

// In order to use the 'Bitmap' class you need to include the 'System.Drawing.Common' class from the NuGet manager.
// This solution should already have it included.

namespace Blockland_Heightmap_Generator
{
    class Program
    {
        // Constants
        const string inputDirectory = "input";
        const string outputDirectory = "output";
        const int defaultModifier = 10;

        static void Main(string[] args)
        {
            // Main variables
            string[] inputFiles = null;
            float heightMultiplier = 1.0f/ defaultModifier;
            
            // Make sure the user isn't a big ol' dummy...
            if(Directory.Exists(inputDirectory) && Directory.Exists(outputDirectory))
            {
                inputFiles = Directory.GetFiles(inputDirectory);
                if(inputFiles.Length <= 0)
                {
                    // pls...
                    Console.WriteLine("There are no files in the input directory!");
                    Console.WriteLine("Nothing to process. Quitting...");
                    Quit();
                }
            }
            else
            {
                Console.WriteLine("Failed to find either the input or output directory.\nCreating them...");
                try
                {
                    // Let me help you with that
                    Directory.CreateDirectory(inputDirectory);
                    Directory.CreateDirectory(outputDirectory);
                }
                catch(Exception e)
                {
                    // Let's all calm down...
                    Console.WriteLine($"What the fuck happened: '{e.Message}'");
                    Console.ReadLine();
                }
                Quit();
            }
            // You passed the first dummy test!

            // Clear out anything in the output directory.
            foreach(string filename in Directory.GetFiles(outputDirectory))
            {
                File.Delete(filename);
            }

            // Collect user input(s)
            bool inputValid = false;
            while(!inputValid)
            {
                Console.WriteLine($"Enter height multiplier (default = {1.0f / defaultModifier})");
                Console.WriteLine("Input:");
                string input = Console.ReadLine();
                if (input == string.Empty)
                {
                    inputValid = true;
                }
                else
                {
                    if (float.TryParse(input, out heightMultiplier))
                        inputValid = true;
                }
                Console.WriteLine($"Set multiplier to: {heightMultiplier}");
            }
            // You passed the second dummy test!

            if(inputFiles != null) // Better not be null... Else I'll be upset...
            {
                foreach (string filename in inputFiles) // For all the .bmp files in the input directory...
                {
                    if(Path.GetExtension(filename) == ".bmp") // FOR ALL THE .bmp FILES I SAID!
                    {
                        try
                        {
                            string outputFilename = $"{Path.GetFileNameWithoutExtension(filename)}.bls"; // Output filename
                            Bitmap bmp = new Bitmap($"{filename}");
                            int size = bmp.Width * bmp.Height; // Size is the brick count

                            // Let me tell you a bit about myself. I'm...
                            Console.WriteLine($"\nName: '{outputFilename}'");
                            Console.WriteLine($"Dimensions: {bmp.Width}x{bmp.Height}");
                            Console.WriteLine($"Size: {size}");

                            // 250,000 bricks takes my AMD Ryzen 9 3950X 16-Core Processor (3.5Ghz) apprx 40 seconds to load all the bricks.
                            if (size >= 250000)
                                Console.WriteLine($"That's {size - 250000} bricks over 250,000!");
                            else
                                Console.WriteLine($"That's {250000 - size} bricks under 250,000");

                            // Now for the real shit we are here for
                            Console.WriteLine("Generating...");
                            List<string> blsFileData = GenerateBlsHeader(filename, size);
                            for (int y = 0; y < bmp.Height; y++)
                            {
                                for (int x = 0; x < bmp.Width; x++)
                                {
                                    float height = bmp.GetPixel(x, y).R;
                                    height *= heightMultiplier;
                                    blsFileData.Add(GenerateBrick(x, y, height));
                                }
                            }

                            // Write the data to a file
                            Console.WriteLine("Writing...");
                            TextWriter writer = new StreamWriter($"{outputDirectory}\\{outputFilename}");
                            foreach (string line in blsFileData)
                            {
                                writer.WriteLine(line);
                            }
                            writer.Close();
                            Console.WriteLine("Finished!\n");
                        }
                        catch(Exception e)
                        {
                            // Either some bad input file or something went wrong while writing the file
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Press enter to continue.");
                            Console.ReadLine();
                        }
                    }
                    else // Uncool file detected. smh...
                    {
                        Console.WriteLine($"File '{filename}' is not a .bmp! Skipping...");
                        continue;
                    }
                }
                Console.WriteLine("\nNo more input files found...");
            }
            else
            {
                // How did you even get here?
                Console.WriteLine("Variable 'inputFiles' is null!");
                Console.WriteLine("(You shouldn't be here... What did you do?)");
            }

            Quit(); // Bye, thanks for playing!
        }

        static void Quit()
        {
            // Only quitters come here
            Console.WriteLine("Press any key to close the program.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static string GenerateBrick(int x, int y, float height)
        {
            return $"4x4x64\" {x * 2} {y * 2} {19.2 + height} 2 1 {15}  0 0 1 1 1"; // The {15} is the colorID. 15 is White in the default colorset.
        }

        static List<string> GenerateBlsHeader(string filename, int size)
        {
            List<string> fileData = new List<string>();
            fileData.Add("This is a Blockland save file.  You probably shouldn't modify it cause you'll screw it up."); // >Proceeds to modify it
            fileData.Add("1");
            fileData.Add("Generated with Auios' Heightmap Generator"); // You're not seriously going to tell people you built the map all by yourself right?
            fileData.Add("0.898039 0.000000 0.000000 1.000000");
            fileData.Add("0.898039 0.898039 0.000000 1.000000");
            fileData.Add("0.000000 0.498039 0.247059 1.000000");
            fileData.Add("0.200000 0.000000 0.800000 1.000000");
            fileData.Add("0.898039 0.898039 0.898039 1.000000");
            fileData.Add("0.749020 0.749020 0.749020 1.000000");
            fileData.Add("0.498039 0.498039 0.498039 1.000000");
            fileData.Add("0.200000 0.200000 0.200000 1.000000");
            fileData.Add("0.392157 0.192157 0.000000 1.000000");
            fileData.Add("0.901961 0.337255 0.078431 1.000000");
            fileData.Add("0.749020 0.176471 0.482353 1.000000");
            fileData.Add("0.384314 0.000000 0.113725 1.000000");
            fileData.Add("0.129412 0.266667 0.266667 1.000000");
            fileData.Add("0.000000 0.137255 0.329412 1.000000");
            fileData.Add("0.101961 0.458824 0.764706 1.000000");
            fileData.Add("1.000000 1.000000 1.000000 1.000000");
            fileData.Add("0.078431 0.078431 0.078431 1.000000");
            fileData.Add("1.000000 1.000000 1.000000 0.247059");
            fileData.Add("0.921569 0.513726 0.674510 1.000000");
            fileData.Add("1.000000 0.603922 0.419608 1.000000");
            fileData.Add("1.000000 0.874510 0.611765 1.000000");
            fileData.Add("0.956863 0.874510 0.784314 1.000000");
            fileData.Add("0.784314 0.921569 0.486275 1.000000");
            fileData.Add("0.537255 0.694118 0.549020 1.000000");
            fileData.Add("0.556863 0.929412 0.956863 1.000000");
            fileData.Add("0.694118 0.658824 0.901961 1.000000");
            fileData.Add("0.874510 0.556863 0.956863 1.000000");
            fileData.Add("0.666667 0.000000 0.000000 0.698039");
            fileData.Add("1.000000 0.498039 0.000000 0.698039");
            fileData.Add("0.988235 0.956863 0.000000 0.698039");
            fileData.Add("0.000000 0.470588 0.192157 0.698039");
            fileData.Add("0.000000 0.200000 0.639216 0.698039");
            fileData.Add("0.592157 0.156863 0.392157 0.694118");
            fileData.Add("0.549020 0.698039 1.000000 0.698039");
            fileData.Add("0.847059 0.847059 0.847059 0.698039");
            fileData.Add("0.098039 0.098039 0.098039 0.698039");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add("1.000000 0.000000 1.000000 0.000000");
            fileData.Add($"Linecount {size}");
            return fileData;
        }
    }

}
