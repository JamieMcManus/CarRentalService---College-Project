namespace Assignment
{
    using System;
    using System.Collections.Generic;

    public partial class Car
    {
        // Custom to string method
        public override string ToString()
        {
            return string.Format("{0} {1}", Make, Model);  
        }



    }
}
