using System;
using System.Collections.Generic;

namespace HomeBudgetApp
{
    public interface IDataView
    {
        DataPresenter presenter { get; set; }
        List<object> DataSource { get; set; }
        void ResetFocusAfterUpdate(int itemIndex);
        void DataClear();
        void InitializeStandardDisplay();
        void InitializeByMonthDisplay();
        void InitializeByCategoryDisplay();
        void InitializeByCategoryAndMonthDisplay(List<String> usedCategoryList);      
    }
}
