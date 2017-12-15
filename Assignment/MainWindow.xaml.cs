using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Declare for use throughout page
        DateTime StartDate, EndDate;
        CarRental_S00174105Entities2 db = new CarRental_S00174105Entities2();  // access to database


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //Add items to drop down list
            ddlOption.Items.Add("All");
            ddlOption.Items.Add("Small");
            ddlOption.Items.Add("Medium");
            ddlOption.Items.Add("Large");
            ddlOption.SelectedIndex = 0;  //set default to All

            //Add image to ImageBox
            image.Source = new BitmapImage(new Uri(@"/images/cars.jpg",UriKind.Relative));
           
            //set datepicker to prevent dates earlier than today
            dpStartDate.DisplayDateStart = DateTime.Today;
          
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            int selectedID = (int)lbxAvailibleCars.SelectedValue;

            //Create a new booking object
            //Assign details from selected Car in list
            Booking newBooking = new Booking();           
            newBooking.StartDate = dpStartDate.SelectedDate.Value;
            newBooking.EndDate = dpEndDate.SelectedDate.Value;
            newBooking.CarId = selectedID;
            db.Bookings.Add(newBooking);    //Add to Bookings Table
            db.SaveChanges();               //Save changes to Rental Database

            //Open Message box, display confirmation details
            string confirmDetails = "Booking Confirmation\n"+GetDetails();
            MessageBox.Show(confirmDetails, "Confirmation");

            //Reset pages controls
            btnBook.IsEnabled = false;
            ddlOption.SelectedIndex = 0;
            dpEndDate.IsEnabled = false;
            lbxAvailibleCars.ItemsSource = null;

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Set dates as it is easier to read
            StartDate = dpStartDate.SelectedDate.Value;
            EndDate = dpEndDate.SelectedDate.Value;
            
            //Get Car IDs from bookings that overlap with the selected user dates
            var query = from b in db.Bookings                       
                         where (StartDate>=b.StartDate && StartDate <=b.EndDate || EndDate <= b.EndDate & EndDate >=b.StartDate)
                         select b.CarId;


            //Get all cars except those that are in the query results

            if (ddlOption.SelectedValue.ToString()=="All")    // For all option
            {
                var searched = from c in db.Cars
                               where !query.Contains(c.Id)
                               select c;
                //Set listbox to display results
                lbxAvailibleCars.ItemsSource = searched.ToList();

            }
            //For all other options
            else
            {
                var searched = from c in db.Cars
                               where !query.Contains(c.Id) && c.Size == ddlOption.SelectedValue.ToString()
                               select c;
                //Set listbox to display results
                lbxAvailibleCars.ItemsSource = searched.ToList();
            }
        }

        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Prevent user from setting Start date after the EndDate
            if (dpEndDate.SelectedDate < dpStartDate.SelectedDate)
            {
                dpEndDate.SelectedDate = dpStartDate.SelectedDate;
            }

            //Prevent user from entering a date earlier than today in the text box
            //By resetting it to todays date
            if (dpStartDate.SelectedDate < DateTime.Today)
            {
                dpStartDate.SelectedDate = DateTime.Today;
            }
            

            else
            {
                dpEndDate.IsEnabled = true;    // Enable EndDate datepicker
                dpEndDate.DisplayDateStart = dpStartDate.SelectedDate; //Prevent picking earlier dates than startdate
            }
            //clear listbox if date changed 
            lbxAvailibleCars.ItemsSource = null;
            btnBook.IsEnabled = false; // prevent crash

        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //clear listbox if date changed
            lbxAvailibleCars.ItemsSource = null;
            btnBook.IsEnabled = false; // prevent crash
        }

        private void lbxAvailibleCars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbxAvailibleCars.SelectedItems != null)
            {
                //Enable Booking button
                btnBook.IsEnabled = true;
                //Cast selected Item to Car type to access details
                Car selected = (Car)lbxAvailibleCars.SelectedItem;
                //Set textblock to display details
                txbkDescription.Text = GetDetails();
            }
            //If none selected, empty textblock and disable Book button
            else
            {
                txbkDescription.Text = "";
                btnBook.IsEnabled = false;

            }

        }

        private string GetDetails()
        {
            if (lbxAvailibleCars.SelectedValue != null)
            {
                Car selected = (Car)lbxAvailibleCars.SelectedItem;
                //Set textblock to display details
                return string.Format("Car ID: {0}\nMake: {1}\nModel: {2}\nStart Date: {3}\nEnd Date: {4}", selected.Id, selected.Make, selected.Model, StartDate.ToShortDateString(), EndDate.ToShortDateString());
            }
            else
            {
                return "";
            }

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Clear listbox when changed
            lbxAvailibleCars.ItemsSource = null;
            btnBook.IsEnabled = false;
            dpStartDate.SelectedDate = DateTime.Today;
            

        }
    }
}
