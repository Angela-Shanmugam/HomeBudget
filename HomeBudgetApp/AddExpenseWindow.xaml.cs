using System;
using System.Collections.Generic;
using System.IO;
using Budget;
using System.Windows;
using System.Windows.Controls;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for AddExpenseWindow.xaml
    /// TODO: Change path
    /// </summary>
    public partial class AddExpenseWindow
    {
        /// <summary>
        /// The list of categories in the database.
        /// Used for binding the <see cref="ComboBox"/> in the GUI.
        /// </summary>
        public List<Category> CategoryList { get; }

        private readonly HomeBudget _homeBudget;

        /// <summary>
        /// Initializes a new <see cref="AddExpenseWindow"/>.
        /// </summary>
        public AddExpenseWindow(HomeBudget homeBudget)
        {
            InitializeComponent();

            DataContext = this; // Used for binding the list to the combo box
            _homeBudget = homeBudget;
            CategoryList = _homeBudget.categories.List();
        }

        /// <summary>
        /// Event listener for the submit button.
        /// Gets all the values (and validates) entered by the user and
        /// adds a new <see cref="Expense"/> to the database with those values.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            var description = TbxDescription.Text;

            if (string.IsNullOrWhiteSpace(description) || CbxCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Please make sure all fields are not empty", "Invalid Entries", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var category = (Category) CbxCategory.SelectionBoxItem;

            if (!double.TryParse(TbxAmount.Text, out var amount))
            {
                MessageBox.Show("Please enter a valid amount. (Ex: 21.99)", "Invalid Amount", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParse(DpDate.Text, out var date))
            {
                MessageBox.Show("Please enter a valid date. (Ex: 2021-01-12)", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Add the item to the database
            _homeBudget.expenses.Add(date, category.Id, amount, description);

            //Show user that the expense was added
            SuccessfullyAdded();
            
            //Clear description and amount textboxes
            TbxDescription.Clear();
            TbxAmount.Clear();

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //close the window
            Close();
        }

        private void SuccessfullyAdded()
        {
            string messageBoxText = "Expense Added";
            string caption = "Successfully added";
            MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(messageBoxText, caption, button);
        }

    }
}
