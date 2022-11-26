using Budget;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     
        //Variables
        private bool _canUpdateBudgetList;
        private bool _canUpdateCategoriesList;
        private bool _canFilterByCategory;
        private int selectedCategory;
        private bool? filterIsChecked;
        private DateTime? startDate;
        private DateTime? endDate;

        //Properties 
        public bool CanUpdateBudgetList {get {return this._canUpdateBudgetList; } }

        public int SelectedCategory { get { return this.selectedCategory; } }

        public bool? FilterIsChecked { get { return this.filterIsChecked; } }

        public DateTime? StartDate { get { return this.startDate; } }

        public DateTime? EndDate { get { return this.endDate; } }

        /// <summary>
        /// The list of objects from the itemSource
        /// </summary>
        public List<object> Items { get { return (List<object>)theDataGridView.ExpensesDataGrid.ItemsSource; } }

        /// <summary>
        /// The list of categories in the database.
        /// </summary>
        public List<Category> CategoryList { get; set; }
       
        /// <summary>
        /// The file that contains the home budget used.
        /// </summary>
        public HomeBudget HomeBudgetFile { get; set; }
        /// <summary>

        public DataPresenter presenter { get { return theDataGridView.presenter; } set { theDataGridView.presenter = value; } }

        /// <summary>
        /// Opens the Main Window
        /// </summary>
        public MainWindow()
        {  
            DataContext = this;
            InitializeComponent();

            presenter = new DataPresenter(this, theDataGridView);
            selectedCategory = CategoryDropdown.SelectedIndex + 1; 
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
        }


        /// <summary>
        /// Sets a one way binding with the passed in path.
        /// </summary>
        /// <param name="path">The path to be bound</param>
        /// <returns>The one way binding of the passed path</returns>
        private static Binding SetBinding(string path)
        {
            return new Binding(path) { Mode = BindingMode.OneWay };
        }

        /// <summary>
        /// Enables all features in the application
        /// </summary>
        /// <param name="file">The path to the homeBudgetFile</param>
        /// <param name="newPath">Checks if the database is new</param>
        public void EnableFields(string file, bool newPath)
        {
            //Hides file buttons and displays add buttons
            BtnAddCategory.IsEnabled = true;
            BtnAddExpense.IsEnabled = true;
            theDataGridView.SearchButton.IsEnabled = true;
            theDataGridView.SearchBox.IsEnabled = true;
            CategoryFilterCheckBox.IsEnabled = true;
            Category.IsEnabled = true;
            Month.IsEnabled = true;

            BtnFile.IsEnabled = false;
            BtnNewFile.IsEnabled = false;

            //creates home budget 
            HomeBudgetFile = new HomeBudget(file, newPath);

            _canUpdateBudgetList = true;
            _canUpdateCategoriesList = true;
            UpdateListOfCategories();
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Updates the list of categories.
        /// Sets the newly updates list of categories as the source item for the categories filter dropdown menu.
        /// </summary>
        private void UpdateListOfCategories() 
        {
            // checks if the budget item list cant be updated
            if (!_canUpdateCategoriesList)
            {
                return;
            }

            // populates the list of categories
            CategoryList = HomeBudgetFile.categories.List();

            // Used to refresh DataGrid
            CategoryDropdown.ItemsSource = null; 
            CategoryDropdown.ItemsSource = CategoryList;
        }

        #region Event Listeners

        /// <summary>
        /// Creates new Expense Window when the specific button has been clicked
        /// Opens the created window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow newExpenseWindow = new AddExpenseWindow(HomeBudgetFile);
            newExpenseWindow.ShowDialog();
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Creates new Category Window when the specific button has been clicked
        /// Opens the created window
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow newCategoryWindow = new AddCategoryWindow(HomeBudgetFile);
            newCategoryWindow.ShowDialog();
            UpdateListOfCategories();
        }

        /// <summary>
        /// Creates an open file dialog
        /// Opens the file dialog allowing the user to select their file
        /// Opens the existing home budget with that file using the filepath received 
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void BtnFile_Click(object sender, RoutedEventArgs e)
        {
            //default name to db file
            OpenFileDialog newFileDialog = new OpenFileDialog { FileName = "myDatabase.db", CheckFileExists = false };

            //if the file dialog was not canceled
            if (newFileDialog.ShowDialog() == true)
                EnableFields(newFileDialog.FileName, false);
        }

        /// <summary>
        /// Creates an open file dialog
        /// Opens the file dialog allowing the user to select where they store the file
        /// Creates new home budget with that file using the filepath received 
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void BtnNewFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog newFileDialog = new OpenFileDialog { FileName = "myDatabase.db", CheckFileExists = false };

            //if the file dialog was not canceled
            if (newFileDialog.ShowDialog() == true)
                EnableFields(newFileDialog.FileName, true);
        }

        /// <summary>
        /// Updates list of budget items when filter start date is changed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void StartDateInput_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Updates list of budget items when filter end date is changed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void EndDateInput_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Updates list of budget items when category filter checkbox is clicked
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void CategoryFilterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            _canFilterByCategory = !_canFilterByCategory;
            CategoryDropdown.IsEnabled = _canFilterByCategory;
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Updates list of budget items when filter category dropdown selection is changed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void CategoryDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Updates list of budget items when summary checkbox is changed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void Summary_Unchecked(object sender, RoutedEventArgs e)
        {
            //show list of all budget items
            selectedCategory = CategoryDropdown.SelectedIndex + 1;
            filterIsChecked = CategoryFilterCheckBox.IsChecked;
            startDate = StartDateInput.SelectedDate;
            endDate = EndDateInput.SelectedDate;
            presenter.UpdateListOfBudgetItems(HomeBudgetFile, _canUpdateBudgetList, selectedCategory, filterIsChecked, startDate, endDate);
        }

        /// <summary>
        /// Filters budget items in a summary summary by month, by category or both.
        /// Creates and binds columns of the DataGrid.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void Summary_Checked(object sender, RoutedEventArgs e)
        {           
            presenter.FilterItems(HomeBudgetFile, Month.IsChecked, Category.IsChecked, selectedCategory, filterIsChecked, startDate, endDate);
        }
        #endregion

    }
}
