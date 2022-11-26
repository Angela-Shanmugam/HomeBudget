using System;
using System.Collections.Generic;
using System.IO;
using HomeBudgetApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeBudgetAppTests
{
    [TestClass]
    public class HomeBudgetAppTest 
    {
        public class TestUIInterface : IDataView
        {
            public bool calledInitializeByMonth = false;
            public bool calledInitializeByCategory = false;
            public bool called_InitializeStandard = false;

            public bool calledResetFocusAfterUpdate;
            public int passedItemIndex;

            public bool calledInitializeByCategoryAndMonthDisplay;
            public List<string> passedCategoryList;

            public bool calledDataClear = false;

            public DataPresenter presenter { get; set; }
            public List<object> DataSource { get; set; }

            public void DataClear()
            {
                calledDataClear = true;
            }

            public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
            {
                calledInitializeByCategoryAndMonthDisplay = true;
                passedCategoryList = usedCategoryList;
            }

            public void InitializeByCategoryDisplay()
            {
                calledInitializeByCategory = true;
            }

            public void InitializeByMonthDisplay()
            {
                calledInitializeByMonth = true;
            }

            public void InitializeStandardDisplay()
            {
                called_InitializeStandard = true;
            }

            public void ResetFocusAfterUpdate(int itemIndex)
            {
                calledResetFocusAfterUpdate = true;
                passedItemIndex = itemIndex;

            }
        }

      [TestMethod]
        public void TestInitializeStandard()
        {
            //Arrange
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            var homeBudgetFile = mainWindow.HomeBudgetFile;
            bool canUpdateBudgetList = true;
            DateTime? startDate = null;
            DateTime? endDate = null;
            var selectedCategory = -1;
            bool filterChecked = false;

            //Act
            theTestedUI.presenter.UpdateListOfBudgetItems(homeBudgetFile, canUpdateBudgetList, selectedCategory, filterChecked, startDate, endDate); ;

            //Assert        
            Assert.IsTrue(theTestedUI.called_InitializeStandard);
        }

        [TestMethod]
        public void TestInitializeByMonth()
        {
            //Arrange
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            var homeBudgetFile = mainWindow.HomeBudgetFile;
            bool monthIsChecked = true;
            bool categoryIsChecked = false;
            DateTime? startDate = null;
            DateTime? endDate = null;
            var selectedCategory = -1;
            bool filterChecked = false;

            //Act
            theTestedUI.presenter.FilterItems(homeBudgetFile, monthIsChecked, categoryIsChecked, selectedCategory, filterChecked, startDate, endDate);

            //Assert        
            Assert.IsTrue(theTestedUI.calledInitializeByMonth);
        }

        [TestMethod]
        public void TestInitializeByCategory()
        {
            //Arrange
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            var homeBudgetFile = mainWindow.HomeBudgetFile;
            bool monthIsChecked = false;
            bool categoryIsChecked = true;
            DateTime? startDate = null;
            DateTime? endDate = null;
            var selectedCategory = -1;
            bool filterChecked = false;

            //Act
            theTestedUI.presenter.FilterItems(homeBudgetFile, monthIsChecked, categoryIsChecked, selectedCategory, filterChecked, startDate, endDate);

            //Assert        
            Assert.IsTrue(theTestedUI.calledInitializeByCategory);
        }

        [TestMethod]
        public void TestResetFocusAfterUpdate()
        {
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            // Act
            theTestedUI.presenter.Edit(0);
            theTestedUI.presenter.editExpenseWindow.Close();

            // Assert
            Assert.IsTrue(theTestedUI.calledResetFocusAfterUpdate);
            Assert.AreEqual(0, theTestedUI.passedItemIndex);
        }

        [TestMethod]
        public void TestInitializeByCategoryAndMonth()
        {
            //Arrange
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            var homeBudgetFile = mainWindow.HomeBudgetFile;
            bool monthIsChecked = true;
            bool categoryIsChecked = true;
            DateTime? startDate = null;
            DateTime? endDate = null;
            var selectedCategory = -1;
            bool filterChecked = false;

            // Act
            theTestedUI.presenter.FilterItems(homeBudgetFile, monthIsChecked, categoryIsChecked, selectedCategory, filterChecked, startDate, endDate);

            // Assert
            Assert.IsTrue(theTestedUI.calledInitializeByCategoryAndMonthDisplay);
        }

        [TestMethod]
        public void TestDataClear()
        {
            //Arrange
            TestUIInterface theTestedUI = new TestUIInterface();
            MainWindow mainWindow = new MainWindow();
            theTestedUI.presenter = new DataPresenter(mainWindow, theTestedUI);

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
            string existingDB = $"{path}\\myDatabase.db";

            mainWindow.EnableFields(existingDB, false);

            //Act
            theTestedUI.presenter.IView.DataClear();

            //Assert        
            Assert.IsTrue(theTestedUI.calledDataClear);

        }
    }
}
