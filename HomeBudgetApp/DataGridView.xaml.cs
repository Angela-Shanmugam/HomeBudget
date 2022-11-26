using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl, IDataView 
    {
        private readonly Style _columnStyle = new Style();

        public DataPresenter presenter { get; set; }

        public List<object> DataSource { get { return this.DataSource; } set { ExpensesDataGrid.ItemsSource = value; } }

        
        public DataGridView()
        {
            DataContext = this;
            InitializeComponent();
         
            //align cells with numerical values to right-justified
            _columnStyle.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
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
        /// Clears data in DatagridView
        /// </summary>
        public void DataClear()
        {
            ExpensesDataGrid.Columns.Clear();
        }

        /// <summary>
        /// Sets the DataGridView to display budget items by month and category
        /// </summary>
        /// <param name="usedCategoryList">Contains the current list of categories</param>
        public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
        {
            //clear dataGridView
            DataClear();

            // adds and binds new columns to DataGrid
            DataGridTextColumn columnDate = new DataGridTextColumn { Binding = SetBinding("[Month]") };
            columnDate.Binding.StringFormat = "yyyy-MM";
            columnDate.Header = "Month";

            ExpensesDataGrid.Columns.Add(columnDate);

            ExpensesDataGrid.FrozenColumnCount = 1;


            // loops through all categories in category list
            foreach (string columnName in usedCategoryList.OrderBy(columnName => columnName))
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = columnName.ToString(),
                    Binding = SetBinding("[" + columnName + "]")
                };
                column.Binding.StringFormat = "F2";
                column.CellStyle = _columnStyle;
                ExpensesDataGrid.Columns.Add(column);
            }

            DataGridTextColumn columnTotal = new DataGridTextColumn { Binding = SetBinding("[Total]") };
            columnTotal.Binding.StringFormat = "F2";
            columnTotal.CellStyle = _columnStyle;
            columnTotal.Header = "Total";

            ExpensesDataGrid.Columns.Add(columnTotal);
        }

        /// <summary>
        /// Sets the DataGridView to display budget items by category
        /// </summary>
        public void InitializeByCategoryDisplay()
        {
            SearchButton.IsEnabled = false;
            SearchBox.IsEnabled = false;
           
            // disables edit and delete
            ExpenseMenuItemEdit.IsEnabled = false;
            ExpenseMenuItemDelete.IsEnabled = false;

            // clear all columns in DataGrid
            DataClear();

            //adds and binds new columns to DataGrid
            DataGridTextColumn columnCategory = new DataGridTextColumn { Binding = SetBinding("Category") };
            columnCategory.Header = "Category";
            ExpensesDataGrid.Columns.Add(columnCategory);

            DataGridTextColumn columnTotal = new DataGridTextColumn { Binding = SetBinding("Total") };
            columnTotal.Binding.StringFormat = "F2";
            columnTotal.CellStyle = _columnStyle;
            columnTotal.Header = "Total";

            ExpensesDataGrid.Columns.Add(columnTotal);          
        }

        /// <summary>
        /// Sets the DataGridView to display budget items by category
        /// </summary>
        public void InitializeByMonthDisplay()
        {
            //disable search
            SearchButton.IsEnabled = false;
            SearchBox.IsEnabled = false;

            // disables edit and delete
            ExpenseMenuItemEdit.IsEnabled = false;
            ExpenseMenuItemDelete.IsEnabled = false;

            // clear all columns in DataGrid
            DataClear();

            // adds and binds new columns to DataGrid
            DataGridTextColumn columnMonth = new DataGridTextColumn { Binding = SetBinding("Month") };
            columnMonth.Binding.StringFormat = "yyyy-MM-dd";
            columnMonth.Header = "Month";

            ExpensesDataGrid.Columns.Add(columnMonth);

            DataGridTextColumn columnTotal = new DataGridTextColumn { Binding = SetBinding("Total") };
            columnTotal.Binding.StringFormat = "F2";
            columnTotal.CellStyle = _columnStyle;
            columnTotal.Header = "Total";

            ExpensesDataGrid.Columns.Add(columnTotal);
        }

        /// <summary>
        /// Sets the DataGridView to display all budget items
        /// </summary>
        public void InitializeStandardDisplay()
        {
            // enables the delete and edit
            //ExpenseMenuItemDelete.IsEnabled = true;
            ExpenseMenuItemEdit.IsEnabled = true;
            SearchButton.IsEnabled = true;
            SearchBox.IsEnabled = true;

            // clear all columns in DataGrid
            DataClear();

            // adding and binding new columns to the DataGrid
            DataGridTextColumn columnDate = new DataGridTextColumn { Binding = SetBinding("Date") };
            columnDate.Binding.StringFormat = "yyyy-MM-dd";
            columnDate.Header = "Date";
            ExpensesDataGrid.Columns.Add(columnDate);

            DataGridTextColumn columnCategory = new DataGridTextColumn { Binding = SetBinding("Category"), Header = "Category" };
            ExpensesDataGrid.Columns.Add(columnCategory);

            DataGridTextColumn columnDescription = new DataGridTextColumn { Binding = SetBinding("ShortDescription"), Header = "Description" };
            ExpensesDataGrid.Columns.Add(columnDescription);

            DataGridTextColumn columnAmount = new DataGridTextColumn { Binding = SetBinding("Amount") };
            columnAmount.Binding.StringFormat = "F2";
            columnAmount.CellStyle = _columnStyle;
            columnAmount.Header = "Amount";
            ExpensesDataGrid.Columns.Add(columnAmount);

            DataGridTextColumn columnBalance = new DataGridTextColumn { Binding = SetBinding("Balance") };
            columnBalance.Binding.StringFormat = "F2";
            columnBalance.CellStyle = _columnStyle;
            columnBalance.Header = "Balance";
            ExpensesDataGrid.Columns.Add(columnBalance);
        }

        /// <summary>
        /// Resets the focus to remain on the updated item in the DataGridView
        /// </summary>
        /// <param name="itemIndex"></param>
        public void ResetFocusAfterUpdate(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex > ExpensesDataGrid.Items.Count)
                throw new ArgumentOutOfRangeException(nameof(itemIndex), @"Row index cannot be out of range.");

            var row = ExpensesDataGrid.Items[itemIndex];
            ExpensesDataGrid.SelectedItem = row;
        }

        /// <summary>
        /// Searches through the DataGriView for an amount or description that matches the user's input.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            presenter.Search();           
        }

        /// <summary>
        /// Creates new edit expense Window when the specific button has been clicked.
        /// Opens the created window.
        /// Updates the budget items list with any changes made.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void EditMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            presenter.Edit(ExpensesDataGrid.SelectedIndex);        
        }

        /// <summary>
        /// Deletes the selected expense off the database.
        /// Updates the budget items list with any changes made.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The routed event argument</param>
        private void DeleteMenuItem_OnClick(object sender, RoutedEventArgs e)   
        {
            presenter.Delete();
        }

    }
}
