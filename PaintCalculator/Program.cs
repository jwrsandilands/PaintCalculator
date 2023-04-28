using PaintCalculator;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        //Get input
        string apology = "That's not a valid input!\nPlease try again.";
        string input;
        int inputInt = 0;

        //paint table
        Paint[] paints = new Paint[2];
        paints[0] = new Paint("white", 1f, 5.00f);
        paints[1] = new Paint("off-white", 1f, 4.00f);

        //Do console work here.
        Console.WriteLine("Welcome to the Paint Calculator!");
        Console.WriteLine("Press any key to begin.");
        Console.ReadKey(true);

        //Once begun, keep running
        while (true)
        {
            Console.WriteLine("\nPlease select the function you would like to perform by inputting its number!\n" +
                "   1. Paint Calculator\n"+
                "   2. Update Paint Data");
            Console.WriteLine("If you would like to exit, type \"end\"");

            //get the input and try to convert it to an int
            input = Console.ReadLine();
            try
            {
                inputInt = int.Parse(input);
            }
            catch
            {

            }
            finally
            {
                inputInt = Math.Clamp(inputInt, 0, 3);
            }

            //Make a decision based on the input.
            if (inputInt != 0)
            {
                switch(inputInt)
                {
                    case 1:
                        inputInt = 0;
                        Console.WriteLine("Booting Calculator...");
                        PainterCalculator(ref paints);
                        break;
                    case 2:
                        inputInt = 0;
                        Console.WriteLine("Extracting info...");
                        paints = PaintData(ref paints);
                        break;
                }
            }
            else if(input == "esc" || input == "stop" || input == "end")
            {
                break;
            }
            else
            {
                Console.WriteLine(apology);
            }
        }
    }

    public static void PainterCalculator(ref Paint[] paints)
    {
        //variables
        //General strings
        string apology = "That's not a valid input!\nPlease try again.";
        string input;

        //For Wall
        int walls = 0;
        float height = 0.0f, width = 0.0f;
        float area;
        float[] wallAreas;
        int paintNum = 0;
        float compoundArea;
        float valueAmount = 0.00f;
        int paintAmount = 0;

        //For Holes
        bool validator = false;
        int holes = 0;
        float holeHeight = 0.0f, holeWidth = 0.0f;
        float holeArea;
        float[] holeAreas;

        if (paints.Length == 0)
        {
            Console.WriteLine("\n**There is no paint information! Please add paints first!**");
            return;
        }

        //Get number of walls
        while (walls <= 0)
        {
            Console.WriteLine("\nPlease input the number of walls to be painted.");
            input = Console.ReadLine();
            try
            {
                walls = int.Parse(input);
                if (walls <= 0)
                {
                    Console.WriteLine(apology);
                }
            }
            catch
            {
                Console.WriteLine(apology);
            }
        }

        //populate wallAreas array
        wallAreas = new float[walls];
        int wallCount = 0;
        while (wallCount < walls)
        {
            Console.WriteLine("\n**For wall #" + (1 + wallCount) + "...**");
            //Get width
            while (width <= 0.0f)
            {
                Console.WriteLine("Please input the WIDTH of the wall in meters.");
                input = Console.ReadLine();
                try
                {
                    width = float.Parse(input);
                    if (width <= 0.0f)
                    {
                        Console.WriteLine(apology);
                    }
                }
                catch
                {
                    Console.WriteLine(apology);
                }
            }

            //Get Height
            while (height <= 0.0f)
            {
                Console.WriteLine("\nPlease input the HEIGHT of the wall in meters.");
                input = Console.ReadLine();
                try
                {
                    height = float.Parse(input);
                    if (height <= 0.0f)
                    {
                        Console.WriteLine(apology);
                    }
                }
                catch
                {
                    Console.WriteLine(apology);
                }
            }

            //get holes in the wall
            while (validator == false)
            {
                Console.WriteLine("\nDoes this wall have gaps? (IE: For Windows or Doors)\nAnswer with Y or N");
                input = Console.ReadLine();
                if (input.ToUpper().StartsWith("Y"))
                {
                    while (holes <= 0)
                    {
                        Console.WriteLine("\nHow many gaps does this wall have?");
                        input = Console.ReadLine();
                        try
                        {
                            holes = int.Parse(input);
                            if (holes <= 0)
                            {
                                Console.WriteLine(apology);
                            }
                        }
                        catch
                        {
                            Console.WriteLine(apology);
                        }
                    }
                    validator = true;
                }
                else if (input.ToUpper().StartsWith("N"))
                {
                    validator = true;
                }
            }
            validator = false;

            //populate holeAreas array
            holeAreas = new float[holes];
            int holeCount = 0;
            while (holeCount < holes)
            {
                Console.WriteLine("\n**For gap #" + (1 + holeCount) + "...**");
                //Get width
                while (holeWidth == 0.0f)
                {
                    Console.WriteLine("Please input the WIDTH of the gap in meters.");
                    input = Console.ReadLine();
                    try
                    {
                        holeWidth = float.Parse(input);
                        if (holeWidth <= 0.0f || holeWidth >= width)
                        {
                            holeWidth = 0.0f;
                            Console.WriteLine(apology);
                        }
                    }
                    catch
                    {
                        Console.WriteLine(apology);
                    }
                }

                //Get Height
                while (holeHeight == 0.0f)
                {
                    Console.WriteLine("\nPlease input the HEIGHT of the gap in meters.");
                    input = Console.ReadLine();
                    try
                    {
                        holeHeight = float.Parse(input);
                        if (holeHeight <= 0.0f || holeHeight >= height)
                        {
                            holeHeight = 0.0f;
                            Console.WriteLine(apology);
                        }
                    }
                    catch
                    {
                        Console.WriteLine(apology);
                    }
                }

                //send instruction to calculate the area of the wall
                holeArea = CalculateArea(holeWidth, holeHeight);
                holeAreas[holeCount] = holeArea;

                holeWidth = 0;
                holeHeight = 0;
                holeCount++;
            }
            holes = 0;

            //send instruction to calculate the area of the wall
            area = CalculateArea(width, height);
            //we must exclude the holes!
            foreach (float hole in holeAreas)
            {
                area -= hole;
            }

            if( area <= 0.0f)
            {
                Console.WriteLine("\n**Your wall is more hole than wall!**");
                Console.WriteLine("This wall's total area will be defaulted to 0.");
                wallAreas[wallCount] = 0;
            }
            else
            {
                wallAreas[wallCount] = area;
            }

            width = 0;
            height = 0;
            wallCount++;
        }
        //we must add all of the areas together.
        compoundArea = 0;
        foreach (float wall in wallAreas)
        {
            compoundArea += wall;
        }

        //Console.WriteLine("\n**The total area of every wall is " + compoundArea + "!**");

        //get paint
        Console.WriteLine("Please choose a paint to use from the list.");
        int counter = 0;
        foreach (Paint paint in paints)
        {
            Console.WriteLine("   " + (1 + counter) + ". " + paints[counter].PaintName.ToUpper());
            counter++;
        }

        //get paint input
        while (validator == false)
        {
            input = Console.ReadLine();
            try
            {
                paintNum = int.Parse(input);
                //select the paint
                Console.WriteLine("\n" + paints[paintNum - 1].PaintName.ToUpper() + " Selected!");

                //Get the Amount per cost

                //calculate total area to cover
                paintAmount = Convert.ToInt32(Math.Ceiling((compoundArea / 2.5f) / paints[paintNum - 1].PaintAmount));

                //calculate cost of covering that area
                valueAmount = paintAmount * paints[paintNum - 1].PaintCost;
                validator = true;

                /*switch (paintNum)
                {
                    case 1:
                        Console.WriteLine("\nWhite Selected.");
                        paintAmount = Convert.ToInt32(Math.Ceiling(compoundArea / 2.5f));
                        valueAmount = paintAmount * 5;
                        validator = true;
                        break;
                    case 2:
                        Console.WriteLine("\nOff-White Selected.");
                        paintAmount = Convert.ToInt32(Math.Ceiling(compoundArea / 2.5f));
                        valueAmount = paintAmount * 4;
                        validator = true;
                        break;
                    default:
                        Console.WriteLine(apology);
                        break;
                }*/
            }
            catch
            {
                Console.WriteLine(apology);
            }
        }

        Console.WriteLine("\nWe have calculated the results!");
        Console.WriteLine("The totals are...");
        Console.WriteLine("Total Area:  " + compoundArea.ToString("0.00") + "m^2");
        Console.WriteLine("Total Paint: " + paintAmount + " tins");
        Console.WriteLine("Total Cost:  £" + valueAmount.ToString("0.00"));
    }

    public static float CalculateArea(float x,float y)
    {
        float ans;
        ans = x * y;

        return ans;
    }

    public static Paint[] PaintData(ref Paint[] paints)
    {
        //needed variables
        string apology = "That's not a valid input!\nPlease try again.";
        string input;
        int inputInt = 0;
        bool validator = false;
        Paint newPaint;
        string paintName;
        float paintAmount;
        float paintCost;

        Console.WriteLine("\nThe current stored paints are:");

        int counter = 0;
        foreach (Paint paint in paints)
        {
            Console.WriteLine(paints[counter].PaintName.ToUpper().PadRight(12) + ": " + paints[counter].PaintAmount + " litres a tin at £" + paints[counter].PaintCost.ToString("0.00"));
            counter++;
        }

        while (!validator)
        {
            //ask for input
            Console.WriteLine("\nWhat would you like to do? Please enter the action's number:\n"
                + "   1. Add new paint type\n"
                + "   2. Edit a paint type\n"
                + "   3. Delete a paint type");
            Console.WriteLine("If you would like to return to the main menu, type \"back\"");

            //get input and convert to a number if impossible say so.
            input = Console.ReadLine();
            try { 
                inputInt = int.Parse(input);
                switch (inputInt)
                {
                    case 1:
                        //Add paint
                        Console.WriteLine("\nWhat's the name of the paint you would like to add?");
                        paintName = Console.ReadLine();
                        Console.WriteLine("\nWhat's the amount per tin (litres) of the paint you would like to add?");
                        paintAmount = float.Parse(Console.ReadLine());
                        Console.WriteLine("\nWhat's the cost per tin (£) of the paint you would like to add?");
                        paintCost = float.Parse(Console.ReadLine());

                        //establish new paint
                        newPaint = new Paint(paintName, paintAmount, paintCost);
                        Console.WriteLine("\n**Here are your paint details...!**");
                        Console.WriteLine(newPaint.PaintName.ToUpper().PadRight(12) + ": " + newPaint.PaintAmount + " litres a tin at £" + newPaint.PaintCost.ToString("0.00"));
                        Console.WriteLine("\nWould you like to confirm these details? (Y/N)");

                        //get approval
                        input = Console.ReadLine();
                        if (input.ToUpper().StartsWith("Y"))
                        {
                            Array.Resize(ref paints, paints.Length + 1);
                            paints[paints.Length - 1] = newPaint;
                            validator = true;
                        }
                        else if (input.ToUpper().StartsWith("N"))
                        {
                            Console.WriteLine("\nCancelling...");
                        }
                        else
                        {
                            Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                        }
                            break;

                    case 2:
                        //Edit Paint
                        Console.WriteLine("\nPlease choose a paint to edit from the list. If you would like to cancel, type \"cancel\"");
                        counter = 0;
                        foreach (Paint paint in paints)
                        {
                            Console.WriteLine("   " + (1 + counter) + ". " + paints[counter].PaintName.ToUpper());
                            counter++;
                        }
                        //Get response
                        input = Console.ReadLine();

                        try
                        {
                            inputInt = int.Parse(input);
                        }
                        finally { }

                        if (inputInt > 1 || inputInt <= paints.Length)
                        {
                            Console.WriteLine("\nEditing " + paints[inputInt - 1].PaintName.ToUpper());

                            //Edit paint
                            Console.WriteLine("\nWhat's the this paint's new name? \nLeave blank to keep it the same.");
                            input = Console.ReadLine();
                            if (!input.Equals(""))
                            {
                                paintName = input;
                            }
                            else
                            {
                                paintName = paints[inputInt - 1].PaintName;
                            }

                            Console.WriteLine("\nWhat's the new amount per tin (litres) of the paint? \nLeave blank to keep it the same.");
                            input = Console.ReadLine();
                            if (!input.Equals(""))
                            {
                                try
                                {
                                    paintAmount = float.Parse(input);
                                }
                                catch
                                {
                                    Console.WriteLine(apology + "\nContinuing with original totals...");
                                    paintAmount = paints[inputInt - 1].PaintAmount;
                                }
                            }
                            else
                            {
                                paintAmount = paints[inputInt - 1].PaintAmount;
                            }

                            Console.WriteLine("\nWhat's the new cost per tin (£) of the paint? \nLeave blank to keep it the same.");
                            input = Console.ReadLine();
                            if (!input.Equals(""))
                            {
                                try
                                {
                                    paintCost = float.Parse(input);
                                }
                                catch
                                {
                                    Console.WriteLine(apology + "\nContinuing with original totals...");
                                    paintCost = paints[inputInt - 1].PaintCost;
                                }
                            }
                            else
                            {
                                paintCost = paints[inputInt - 1].PaintCost;
                            }

                            //establish new paint
                            newPaint = new Paint(paintName, paintAmount, paintCost);
                            Console.WriteLine("\n**Here are your new paint details...!**");
                            Console.WriteLine(newPaint.PaintName.ToUpper().PadRight(12) + ": " + newPaint.PaintAmount + " litres a tin at £" + newPaint.PaintCost.ToString("0.00"));
                            Console.WriteLine("\nWould you like to confirm these details? (Y/N)");

                            //get approval
                            input = Console.ReadLine();
                            if (input.ToUpper().StartsWith("Y"))
                            {
                                paints[inputInt - 1] = newPaint;
                                validator = true;
                            }
                            else if (input.ToUpper().StartsWith("N"))
                            {
                                Console.WriteLine("\nCancelling...");
                            }
                            else
                            {
                                Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n**That's out of bounds!**\nReturning you to the Paint Data menu...");
                        }

                        if (input == "esc" || input == "stop" || input == "end" || input == "back" || input == "cancel")
                        {
                            Console.WriteLine("\nReturning you to the Paint Data menu...");
                        }
                        else if (inputInt == 0)
                        {
                            Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                        }

                        break;

                    case 3:
                        //Delete Paint
                        if (paints.Length != 0)
                        {
                            Console.WriteLine("\nPlease choose a paint to delete from the list. If you would like to cancel, type \"cancel\"");
                            counter = 0;
                            foreach (Paint paint in paints)
                            {
                                Console.WriteLine("   " + (1 + counter) + ". " + paints[counter].PaintName.ToUpper());
                                counter++;
                            }
                            //Get response
                            input = Console.ReadLine();
                            try
                            {
                                inputInt = int.Parse(input);

                                //get approval
                                if (inputInt > paints.Length || inputInt < 1)
                                {
                                    Console.WriteLine("\n**That's out of bounds!**\nReturning you to the Paint Data menu...");
                                }
                                else
                                {
                                    Console.WriteLine("\nAre you sure? (Y/N)");
                                    input = Console.ReadLine();
                                    if (input.ToUpper().StartsWith("Y"))
                                    {
                                        Console.WriteLine("\nDeleting...");
                                        counter = 0;
                                        int skip = 0;
                                        while (counter < (paints.Length - skip))
                                        {
                                            if ((inputInt - 1) == (counter + skip))
                                            {
                                                skip++;
                                            }
                                            else
                                            {
                                                paints[counter] = paints[counter + skip];
                                                counter++;
                                            }
                                        }
                                        Array.Resize(ref paints, paints.Length - 1);
                                    }
                                    else if (input.ToUpper().StartsWith("N"))
                                    {
                                        Console.WriteLine("\nCancelling...");
                                    }
                                    else
                                    {
                                        Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                                    }
                                }
                            }
                            catch
                            {
                                if (input == "esc" || input == "stop" || input == "end" || input == "back" || input == "cancel")
                                {
                                    Console.WriteLine("\nReturning you to the Paint Data menu...");
                                }
                                else
                                {
                                    Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                                }
                            }
                        }
                        else
                        {
                            //There arent any paints to delete!
                            Console.WriteLine("\n**There are no paints to delete!**");
                        }


                        break;

                    default:
                        //Inform User of invalid input
                        Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                        break;
                }
            } 
            catch 
            {
                if (input == "esc" || input == "stop" || input == "end" || input == "back")
                {
                    validator = true;
                }
                else
                {
                    Console.WriteLine(apology + "\nReturning you to the Paint Data menu...");
                }
            }
        }

        return paints;
    }
}