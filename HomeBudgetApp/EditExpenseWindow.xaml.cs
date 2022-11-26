using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;
using Budget;

namespace HomeBudgetApp
{
    /// <summary>
    /// Interaction logic for EditExpenseWindow.xaml
    /// </summary>
    public partial class EditExpenseWindow : Window
    {
        public List<Category> CategoryList { get; set; }
        public BudgetItem ExpenseToEdit { get; private set; }

        private HomeBudget _homeBudgetFile;

        public EditExpenseWindow(HomeBudget homeBudget, BudgetItem expenseToEdit)
        {
            InitializeComponent();

            DataContext = this;

            _homeBudgetFile = homeBudget;
            CategoryList = _homeBudgetFile.categories.List();
            ExpenseToEdit = expenseToEdit;

            UpdateWindowFields();
        }

        private void UpdateWindowFields()
        {
            TbxAmount.Text = ExpenseToEdit.Amount.ToString(CultureInfo.InvariantCulture);
            TbxDescription.Text = ExpenseToEdit.ShortDescription;
            CbxCategory.SelectedIndex = ExpenseToEdit.CategoryID - 1;
            DpDate.Text = ExpenseToEdit.Date.ToString(CultureInfo.InvariantCulture);
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            var description = TbxDescription.Text;

            if (string.IsNullOrWhiteSpace(description) || CbxCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Please make sure all fields are not empty", "Invalid Entries", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var category = (Category)CbxCategory.SelectionBoxItem;

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

            // Edit the item in the database
            _homeBudgetFile.expenses.UpdateProperties(ExpenseToEdit.ExpenseID, date, category.Id, amount, description);

            string messageBoxText = "Expense Edited";
            string caption = "Successfully edited";
            MessageBoxButton button = MessageBoxButton.OK;

            MessageBox.Show(messageBoxText, caption, button);

            // Close the window
            Close();
        }
    }
}
