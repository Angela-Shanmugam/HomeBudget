using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Budget;

namespace HomeBudgetApp
{
    public class DataPresenter
    {
        public IDataView IView;
        public MainWindow MainWindow;
        public EditExpenseWindow editExpenseWindow;

         /// <summary>
         /// The list of budget items in the database.
         /// </summary>
        public List<BudgetItem> BudgetItemList { get; set; }

        /// <summary>
        /// The list of dictionaries for the budget items by month and category
        /// </summary>
        public List<Dictionary<string, object>> BudgetDictionary { get; set; }

        /// <summary>
        /// The list of budget items by month in the database.
        /// </summary>
        public List<BudgetItemsByMonth> BudgetMonthList { get; set; }
       
        /// <summary>
        /// The list of budget items by category in the database.
        /// </summary>
        public List<BudgetItemsByCategory> BudgetCategoryList { get; set; }
 
        /// <summary>
        /// Generates a DataPresenter object
        /// </summary>
        /// <param name="mainWindow">Contains access to the MainWindow</param>
        /// <param name="view">COntains the IDataView interface</param>
        public DataPresenter(MainWindow mainWindow, IDataView view)
        {
            IView = view;
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Checks if the DataGridView can be filtered by month, by category or by both.
        /// </summary>
        /// <param name="homeBudgetFile">Contains the HomeBudget file</param>
        /// <param name="monthIsChecked">Checks if user wants to display by month</param>
        /// <param name="categoryIsChecked">Checks if user wants to display by category</param>
        /// <param name="selectedCategory">Contains the selected category</param>
        /// <param name="filterIsChecked">Checks if user wants to filter by a specific category</param>
        /// <param name="startDate">Contains the start date</param>
        /// <param name="endDate">Contains the end date</param>
        public void FilterItems(HomeBudget homeBudgetFile, bool? monthIsChecked, bool? categoryIsChecked, int selectedCategory, bool? filterIsChecked, DateTime? startDate, DateTime? endDate)
        {
            if (monthIsChecked == true && categoryIsChecked == false)
            {
                BudgetMonthList = homeBudgetFile.GetBudgetItemsByMonth(startDate, endDate, filterIsChecked == true, selectedCategory);
                IView.DataSource = BudgetMonthList.Cast<object>().ToList();
                IView.InitializeByMonthDisplay();              
            }

            if (monthIsChecked == false && categoryIsChecked == true)
            {
                BudgetCategoryList = homeBudgetFile.GetBudgetItemsByCategory(startDate, endDate, filterIsChecked == true, selectedCategory);
                IView.DataSource = BudgetCategoryList.Cast<object>().ToList();
                IView.InitializeByCategoryDisplay();
            }

            if (monthIsChecked == true && categoryIsChecked == true)
            {
                List<Category> categoryList;
                List<string> usedCategoryList = new List<string>();
                BudgetDictionary = homeBudgetFile.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterIsChecked == true, selectedCategory);
                IView.DataSource = BudgetDictionary.Cast<object>().ToList();

                categoryList = homeBudgetFile.categories.List();

                foreach(Category category in categoryList)
                {
                    usedCategoryList.Add(category.Description);
                }
               
                IView.InitializeByCategoryAndMonthDisplay(usedCategoryList);
            }
        }

        /// <summary>
        /// Updates the list of budget items depending on what action the user does.
        /// Sets the newly updates list of budget items as the source item for the DataGrid.
        /// Creates and binds columns of the DataGrid.
        /// </summary>
        public void UpdateListOfBudgetItems(HomeBudget homeBudgetFile, bool canUpdateBudgetList, int selectedCategory, bool? filterIsChecked, DateTime? startDate, DateTime? endDate)
        {
            bool filterByCategory = false;
            int categoryId = 0;

            // checks if the budget item list cant be updated
            if (!canUpdateBudgetList)
                return;

            // checks if the user wants to filter by category
            if (filterIsChecked == true)
            {
                // checks if an option from the categories has been selected
                if (selectedCategory != -1)
                {
                    // sets the index of dropdown menu to categoryId
                    categoryId = selectedCategory;
                    filterByCategory = true;
                }
            }
            BudgetItemList = homeBudgetFile.GetBudgetItems(startDate, endDate, filterByCategory, categoryId);
            IView.DataSource = BudgetItemList.Cast<object>().ToList();
            IView.InitializeStandardDisplay();
        }

        /// <summary>
        /// Generates a dialog window allowing the user to modify the selected budget item.
        /// </summary>
        /// <param name="index">Index of the selected row</param>
        public void Edit(int index)
        {
            // if summary filter is active
            if (MainWindow.Month.IsChecked == true || MainWindow.Category.IsChecked == true)
            {
                // display error
                MessageBox.Show("Cannot edit when summary filter is activated.", "Error Editing!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {    
                // gets the selected expense
                MainWindow.theDataGridView.ExpensesDataGrid.SelectedIndex = index;
                MainWindow.theDataGridView.ExpensesDataGrid.SelectedItem = MainWindow.theDataGridView.ExpensesDataGrid.SelectedIndex;
              
                var selectedRow = (BudgetItem)MainWindow.theDataGridView.ExpensesDataGrid.SelectedItem;
                var rowIndex = MainWindow.theDataGridView.ExpensesDataGrid.SelectedIndex;

                // checks if the selected item was valid
                if (selectedRow == null)
                    return;

                //create instance for the EditExpenseWindow
                editExpenseWindow = new EditExpenseWindow(MainWindow.HomeBudgetFile, selectedRow);

                //Show user the edit form for the selected expense
                editExpenseWindow.Show();
        
                //update budget item list
                UpdateListOfBudgetItems(MainWindow.HomeBudgetFile, MainWindow.CanUpdateBudgetList, MainWindow.SelectedCategory, MainWindow.FilterIsChecked, MainWindow.StartDate, MainWindow.EndDate);
                IView.ResetFocusAfterUpdate(rowIndex); 
            }
        }

        /// <summary>
        /// Deletes the selected expense inside the DataGridView.
        /// </summary>
        public void Delete()
        {
            // gets the selected expense
            var selectedRow = (BudgetItem)MainWindow.theDataGridView.ExpensesDataGrid.SelectedItem;

            // checks if the selected item was valid
            if (selectedRow == null)
                return;

            // display message box to confirm delete
            var result = MessageBox.Show("Are you sure you want to delete?", "Would you like to delete?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // if yes button clicked
            if (result == MessageBoxResult.Yes)
            {
                // deletes expense
                MainWindow.HomeBudgetFile.expenses.Delete(selectedRow.ExpenseID);

                //update budget item list
                UpdateListOfBudgetItems(MainWindow.HomeBudgetFile, MainWindow.CanUpdateBudgetList, MainWindow.SelectedCategory, MainWindow.FilterIsChecked, MainWindow.StartDate, MainWindow.EndDate);
            }
        }

        /// <summary>
        /// Searches for expense that contains the user's input from the search bar.
        /// </summary>
        public void Search()
        {
            int rowIndex;
            var canBreak = false;
            var iteration = 0;
            var searchBoxText = MainWindow.theDataGridView.SearchBox.Text.ToLower();

            //getting the current item source
            var itemSource = MainWindow.Items.Cast<BudgetItem>().ToList();

            if (itemSource == null)
                return;

            rowIndex = MainWindow.theDataGridView.ExpensesDataGrid.SelectedItem == null ? 0 : MainWindow.theDataGridView.ExpensesDataGrid.Items.IndexOf(MainWindow.theDataGridView.ExpensesDataGrid.SelectedItem) + 1;

            if (rowIndex >= itemSource.Count)
                rowIndex = 0;

            // Loop until a row is found or it exceeds the number of iterations
            while (!canBreak)
            {
                // Loop through each row in the DataGrid
                for (var index = rowIndex; index < itemSource.Count; index++)
                {
                    var shortDescription = itemSource[index].ShortDescription.ToLower();
                    var amount = itemSource[index].Amount.ToString(CultureInfo.InvariantCulture).ToLower();

                    // Check if the row contains the search query
                    if (shortDescription.Contains(searchBoxText) || amount.Contains(searchBoxText))
                    {
                        MainWindow.theDataGridView.ExpensesDataGrid.SelectedIndex = index;

                        var row = (DataGridRow)MainWindow.theDataGridView.ExpensesDataGrid.ItemContainerGenerator.ContainerFromIndex(index);
                        row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                        rowIndex = index;
                        canBreak = true;
                        iteration = 0;

                        break;
                    }

                    // If the number of iterations is exceeded, play an exclamation sound and then return
                    if (iteration >= itemSource.Count)
                    {
                        SystemSounds.Exclamation.Play();
                        return;
                    }

                    iteration++;
                    rowIndex = 0;
                }
            }
        }
    }
}