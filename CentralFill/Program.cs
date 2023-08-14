namespace CentralFill
{

    public class Program
    {
        /* This program builds a virtual world of Central Fill facilities and uses an operator entered location
         * to determine the three facilities closest to the user. The facility IDs, the 
         * lowest cost medication at the facility, and the distances to the user target location are displayed.
         */

        // define some operational constants
        // World size
        const int MIN_WORLD_X = -10;
        const int MAX_WORLD_X = 10;
        const int MIN_WORLD_Y = -10;
        const int MAX_WORLD_Y = 10;
        // number of central fill facilities
        const int MIN_FACILITIES = 5;
        const int MAX_FACILITIES = 180;
        // medication cost
        const double MAX_MED_COST = 2000;
        // Randomization seed; set to 0 to get different randomization each time it is run; set to an integer to get consistent randomization
        const int SEED = 0;

        public struct facilityInfo
        // This structure is used to save the facilities with the shortest distance from the target
        {
            public clsCentralFillFacility facility;
            public int distance;
            public string cheapestMed;   // this will be "A", "B", or "C"
        }

        // This is the virtual world; it is modeled as a two-dimensional array of clsCentralFillFacility objects.
        // Logically, the coordinates range from -10 to +10 in both the X and Y directions.
        static clsCentralFillFacility[,] world = new clsCentralFillFacility[Math.Abs(MAX_WORLD_X - MIN_WORLD_X) + 1, Math.Abs(MAX_WORLD_Y - MIN_WORLD_Y) + 1];

        static void Main(string[] args)
        {
            clsLocation targetLocation = new clsLocation(); // user entered target location
            bool Done = false;

            /*
             * This program accepts a user location and returns a list of the 3 closest Central Fill utilities, along with the lowest priced medication at each facility.
             */

            // Initialize facility data
            InitFacilityData(SEED);
            // Show a map of the world
            ShowWorld();

            while (!Done)
            {
                int targetDistance; // distance from facility to target

                // Get target location from user
                targetLocation = GetTargetLocation();   // This also checks to see if application should be terminated.

                // Find the 3 closest Central Fill facilities

                // initialize 3 nearest facilities and distance variables
                facilityInfo Near1 = new facilityInfo();
                facilityInfo Near2 = new facilityInfo();
                facilityInfo Near3 = new facilityInfo();
                facilityInfo tempFacilityInfo = new facilityInfo();
                clsCentralFillFacility nullFacility = new clsCentralFillFacility(0);
                tempFacilityInfo.facility = nullFacility;
                tempFacilityInfo.distance = int.MaxValue;
                Near1 = tempFacilityInfo;
                Near2 = tempFacilityInfo;
                Near3 = tempFacilityInfo;

                // traverse "world" for facilities 
                for (int x = 0; x < 21; x++)
                {
                    for (int y = 0; y < 21; y++)
                    {
                        if (world[x, y] != null)
                        {   // A facility is located at this location in the world.
                            // find distance from target to facility
                            clsCentralFillFacility fac = world[x, y];
                            // we compute the "Manhattan" distance, or right-angle distance between the facility and the target
                            targetDistance = Math.Abs(fac.Location.X - targetLocation.X) + Math.Abs(fac.Location.Y - targetLocation.Y);
                            tempFacilityInfo.facility = fac;
                            tempFacilityInfo.distance = targetDistance;

                            // see if this facility is closer than any of the 3 nearest ones
                            if (tempFacilityInfo.distance < Near1.distance)
                            {
                                swapFacilityInfo(ref tempFacilityInfo, ref Near1);  // put the new facility info into Near1
                                swapFacilityInfo(ref tempFacilityInfo, ref Near2); // move down the existing values
                                Near3 = tempFacilityInfo; // old Near3 not needed
                            }
                            else if (tempFacilityInfo.distance < Near2.distance)
                            {
                                swapFacilityInfo(ref tempFacilityInfo, ref Near2);  // put the new facility info into Near2
                                Near3 = tempFacilityInfo; // old Near3 not needed
                            }
                            else if (tempFacilityInfo.distance < Near3.distance)
                            {
                                Near3 = tempFacilityInfo; // old Near3 not needed
                            }
                            /* else, this facility is more distant than the current 3 nearest. */
                        }
                    }
                }

                // For each of the 3 closest facilities, find the cheapest medication at each one.
                findCheapestMed(ref Near1);
                findCheapestMed(ref Near2);
                findCheapestMed(ref Near3);

                // Display a list of the facilities, and the lowest price medication for each facility
                Console.WriteLine("The three closest Central Fill facilities to (" + targetLocation.X + "," + targetLocation.Y + ") are:");
                Console.WriteLine(BuildResultString(Near1));
                Console.WriteLine(BuildResultString(Near2));
                Console.WriteLine(BuildResultString(Near3));
            } // end While (and accept another input request from the user)
        }



        // ----- Helper Functions -----

        static void InitFacilityData(int seed)
        {
            /* This function creates a random set of Central Fill facilities and loads them into the world array.
             */
            Random rnd = new Random(seed);
            int numFacilities;
            int Xloc;
            int Yloc;

            // Determine how many facilities to add
            numFacilities = rnd.Next(MIN_FACILITIES, MAX_FACILITIES + 1);    // Although the "world" can support 441 locations, we limit the number of facilities to a reasonable range

            // Initialize each facility
            for (int i = 0; i < numFacilities; i++)
            {
                // Create a facility
                clsCentralFillFacility fac = new clsCentralFillFacility(i + 1);

                // For each facility, determine its location (X and Y Coordinates), and the medicine costs
                Xloc = rnd.Next(MIN_WORLD_X, MAX_WORLD_X + 1);
                Yloc = rnd.Next(MIN_WORLD_Y, MAX_WORLD_Y + 1);
                clsLocation loc = new clsLocation(Xloc, Yloc);
                fac.Location = loc;
                // for this example, we arbitrarily set an upper bound on the cost to 1000
                // we add 0.01 to ensure that the cost is greater than zero
                fac.Medication_A_Cost = (decimal)(rnd.NextDouble() * MAX_MED_COST + 0.01);
                fac.Medication_B_Cost = (decimal)(rnd.NextDouble() * MAX_MED_COST + 0.01);
                fac.Medication_C_Cost = (decimal)(rnd.NextDouble() * MAX_MED_COST + 0.01);

                // Place the facility into the "world" array
                // We need to offset the minimum X and Y to the zero location in the array.
                world[Xloc - MIN_WORLD_X, Yloc - MIN_WORLD_Y] = fac;
                // I am not checking to see if there is already a facility at this location; in a real application, this would need to be handled appropriately.
            }
        }

        static clsLocation GetTargetLocation()
        {
            /* This function prompts the user for a target location (X and Y) and returns the entered location.
             * It also checks if the user has requested that the application be terminated (CNTL-Z).
             */
            string? inputLine;
            int targetX = 0;
            int targetY = 0;
            bool ValidInput = false;
            clsLocation targetLocation = new clsLocation();

            while (!ValidInput)
            {
                Console.WriteLine();
                Console.WriteLine("Please input location coordinates (X,Y) as two integers separated by a comma:");
                inputLine = Console.ReadLine();
                if (inputLine != null)
                {
                    string[] locations = inputLine.Split(',');
                    // A production program would need more robust error handling here to make sure user input is valid.
                    try
                    {
                        targetX = int.Parse(locations[0]);
                        targetY = int.Parse(locations[1]);
                        targetLocation.X = targetX;
                        targetLocation.Y = targetY;
                        ValidInput = true;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid input.  (CNTL-Z and ENTER to quit)");
                        continue; // this will cause retry of input
                    }
                }
                else Environment.Exit(0);
            }
            return targetLocation;
        }

        static void swapFacilityInfo(ref facilityInfo f1, ref facilityInfo f2)
        {
            /* This function swaps the facilityInfo objects specified.
             */
            facilityInfo temp;
            temp = f1;
            f1 = f2;
            f2 = temp;
        }

        public static void findCheapestMed(ref facilityInfo facInfo)
        {
            /* This function checks the 3 medications at the facility in the facilityInfo record and determines the cheapest one.
             * It sets the cheapestMed field with the result.
             */
            string result;
            if (facInfo.facility.Medication_A_Cost <= facInfo.facility.Medication_B_Cost)
            {
                if (facInfo.facility.Medication_A_Cost <= facInfo.facility.Medication_C_Cost) { result = "A"; }
                else { result = "C"; }
            }
            else
            {
                if (facInfo.facility.Medication_B_Cost <= facInfo.facility.Medication_C_Cost) { result = "B"; }
                else { result = "C"; }
            }
            // result is returned in the facilityInfo structure (.cheapestMed)
            facInfo.cheapestMed = result;
        }

        static string BuildResultString(facilityInfo facInfo)
        {
            /* This function builds a string that shows the results contained in the facilityInfo record.
             */
            string str;
            str = "Central Fill " + string.Format("{0:00#}", facInfo.facility.ID);
            if (facInfo.cheapestMed == "A")
            {
                str += " - " + string.Format("{0:$0.00}, Medication A", facInfo.facility.Medication_A_Cost);
            }
            else if (facInfo.cheapestMed == "B")
            {
                str += " - " + string.Format("{0:$0.00}, Medication B", facInfo.facility.Medication_B_Cost);
            }
            else
            {
                str += " - " + string.Format("{0:$0.00}, Medication C", facInfo.facility.Medication_C_Cost);
            }
            str += ", Distance " + facInfo.distance;
            return str;
        }


        static void ShowWorld()
        {
            /* This function displays (on the console) a chart of the defined facilities and where they are in the
             * world array.
             */
            int ID;
            string str = "   |";

            Console.WriteLine("World Map:");

            // output data for each cell
            for (int y = 20; y >= 0; y--)
            {
                str = string.Format("{0,3}|", y - 10);  // Y Axis value
                for (int x = 0; x < 21; x++)
                {
                    if (world[x, y] == null)
                    {
                        str += " - |";
                    }
                    else
                    {
                        ID = world[x, y].ID;
                        str += string.Format("{0:00#}|", ID);
                    }
                }
                Console.WriteLine(str);
            }

            // create X axis
            str = "   |";
            for (int x = 0; x < 21; x++)
            {
                str += string.Format("{0,3}", x - 10) + "|";
            }
            Console.WriteLine(str);
        }
    }
}