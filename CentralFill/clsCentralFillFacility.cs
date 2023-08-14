using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralFill
{
    /* The CentralFillFacility object describes characteristics of the facility
     */
    public class clsCentralFillFacility
    {
        // ----- variables -----
        private int _ID;    // Facility ID
        // facility location
        private clsLocation _Location = new clsLocation();
        // Costs (in US Dollars) for the three medications at this facility
        private decimal _A_cost;
        private decimal _B_cost;
        private decimal _C_cost;
        /* Note:
         * I have hard-coded 3 medications here. 
         * In a real application, we would probably create a Medication class and create a list of the medications available at this facility.
         */

        // ----- Properties -----

        public int ID
        {
            get { return _ID; }
        }
        public clsLocation Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public decimal Medication_A_Cost
        {
            get { return _A_cost; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Medication cost must be greater than zero.");
                }
                /* else */
                _A_cost = value;
            }
        }

        public decimal Medication_B_Cost
        {
            get { return _B_cost; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Medication cost must be greater than zero.");
                }
                /* else */
                _B_cost = value;
            }
        }

        public decimal Medication_C_Cost
        {
            get { return _C_cost; }
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Medication cost must be greater than zero.");
                }
                /* else */
                _C_cost = value;
            }
        }


        // ----- Methods -----

        // Contructor
        public clsCentralFillFacility(int ID)
        {
            _ID = ID;
        }

    }


    // The Location class describes locations in the "world"

    public class clsLocation
    {
        // _X and _Y are integer coordinates in the X-Y world space
        private int _X;
        private int _Y;

        // ----- Constructors -----
        public clsLocation(int X, int Y)
        {
            _X = X;
            _Y = Y;
        }
        public clsLocation()
        {
            _X = 0;
            _Y = 0;
        }

        // ----- Properties -----
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }

        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
    }
}
