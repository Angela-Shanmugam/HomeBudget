using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Budget;


namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        /// <summary>
        /// The list of CategoryType in the database.
        /// Used for binding the <see cref="ComboBox"/> in the GUI.
        /// </summary>
        public List<Category.CategoryType> CategoryTypeList { get; }

        private readonly HomeBudget _homeBudget;
        private const string Path = "Database.sqlite";

        public AddCategoryWindow(HomeBudget homeBudget)
        {
            InitializeComponent();

            DataContext = this; // Used for binding the list to the combo box
            _homeBudget = homeBudget;

            //Gets all the enum values,casts to Category.CategoryType and sets it to a list. 
            CategoryTypeList = Enum.GetValues(typeof(Category.CategoryType)).Cast<Category.CategoryType>().ToList();
        }

        /// <summary>
        /// Event listener for the submit button.
        /// Gets all the values (and validates) entered by the user and
        /// adds a new <see cref="Categories"/> to the database with those values.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            var description = TbxDescription.Text;

            //verify if fields are not empty
            if (string.IsNullOrWhiteSpace(description) || CbxCategoryType.SelectedIndex < 0)
            {
                MessageBox.Show("Please make sure all fields are not empty", "Invalid Entries", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var categoryType = (Category.CategoryType)CbxCategoryType.SelectionBoxItem;

            // Add the item to the database
            _homeBudget.categories.Add(description,categoryType);

            //Show user that the category was added
            SuccessfullyAdded();

            //Clear description textbox
            TbxDescription.Clear();

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //close the window
            Close();
        }

        private void SuccessfullyAdded()
        {
            string messageBoxText = "Category Added";
            string caption = "Successfully added";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBox.Show(messageBoxText, caption, button);
        }
    }
}
