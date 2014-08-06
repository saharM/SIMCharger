using JulMar.Windows.Mvvm;
using SIMCharger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;


namespace SIMCharger.ViewModels
{
	internal class MainWindowViewModel : ViewModel
	{
		private ChargingStats chargingStats;        

		public ICommand SearchCommand { get; set; }
        public ICommand ChargeCommand { get; set; }		
        public ICommand ImportCommand { get; set; }		
        public ObservableCollection<SearchResult> searchResults { get; set; }
		public ObservableCollection<ChargeResult> chargeResults { get; set; }
		//public ChargeResult SelectedResult { get; set; }
		#region public string SearchMobileNumber {get; set;}
		private string searchMobileNumber;
		public string SearchMobileNumber
		{
			get { return searchMobileNumber; }
			set
			{
				searchMobileNumber = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "SearchMobileNumber" );
			}
		}
		#endregion
        #region public datetime SearchMobileActivateDate {get; set;}
		private DateTime searchMobileActivateDate;
		public DateTime SearchMobileActivateDate
		{
			get { return searchMobileActivateDate; }
			set
			{
				searchMobileActivateDate = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "SearchMobileActivateDate" );
			}
		}
		#endregion
		#region public datetime SearchMobileLastChargeDate {get; set;}
		private DateTime searchMobileLastChargeDate;
		public DateTime SearchMobileLastChargeDate
		{
			get { return searchMobileLastChargeDate; }
			set
			{
				searchMobileLastChargeDate = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "SearchMobileLastChargeDate" );
			}
		}
		#endregion
		#region public string StatsText {get; set;}

		private string statsText;
		public string StatsText
		{
			get { return statsText; }
			set
			{
				statsText = value;
				CommandManager.InvalidateRequerySuggested();
				this.OnPropertyChanged( "StatsText" );
			}
		}
		#endregion
        #region public bool IsSearching {get; set;}
		private bool isSearching;
		public bool IsSearching
		{
			get
			{
				return isSearching;
			}
			set
			{
				isSearching = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}
		#endregion
		#region public bool IsCharging {get; set;}
		private bool isCharging;
		public bool IsCharging
		{
			get
			{
				return isCharging;
			}
			set
			{
				isCharging = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}
		#endregion
   		#region public bool IsImporting {get; set;}
		private bool isImporting;
		public bool IsImporting
		{
			get
			{
				return isImporting;
			}
			set
			{
				isImporting = value;
				CommandManager.InvalidateRequerySuggested();
			}
		}
		#endregion
        
		public MainWindowViewModel()
		{
            SearchCommand = new DelegatingCommand( OnSearch, CanSearch );
			ChargeCommand = new DelegatingCommand( OnCharge, CanCharge );			
            ImportCommand = new DelegatingCommand( OnImport, CanImport );			
                        
			searchResults = new ObservableCollection<SearchResult>();
			chargeResults = new ObservableCollection<ChargeResult>();
			SearchMobileNumber = "";
            SearchMobileActivateDate = DateTime.Now;
            SearchMobileLastChargeDate = DateTime.Now;
			StatsText = "Idle";

			UpdateStats();
		}

        private void OnSearch()
        {
            try
            {
                IsSearching = true;
                searchResults.Clear();

                Task<List<SearchResult>> task = SearchTaskAsync(SearchMobileNumber, SearchMobileActivateDate, SearchMobileLastChargeDate);
                //task.ContinueWith((i) =>
                //    {
                //        IsSearching = false;                   
                //    });
                var completedTask = task.ContinueWith((i) =>
                    {
                        isSearching = false;
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);
                completedTask.Wait();

                foreach (SearchResult result in task.Result)
                {
                    searchResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

		private async void OnCharge()
		{
			IsCharging = true;
			chargeResults.Clear();

			chargingStats.ChargeCount = searchResults.Count;

			List<Task<ChargeResult>> tasks = new List<Task<ChargeResult>>();

			foreach ( SearchResult mobileNum in searchResults )
			{
				tasks.Add( ChargeMobileTaskAsync( mobileNum.MobileNumber ) );
			}

			while ( tasks.Count > 0 )
			{
				Task<ChargeResult> finishTask = await Task.WhenAny<ChargeResult>( tasks.ToArray() );
				tasks.Remove( finishTask );
				IsCharging = tasks.Count > 0;

				chargingStats.ChargeCount--;
				chargingStats.CurrentTarget = finishTask.Result.MobileNumber;
				chargingStats.CurrentAction = "Charging mobile";
								
				chargeResults.Add( finishTask.Result );
				
			}
		}

        private async void OnImport()
		{
			isImporting = true;
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "EXCEL Files (*.xls)|*.xlsx"; 
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {                
                string filename = dlg.FileName;
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook;
                Excel.Worksheet worksheet;
                Excel.Range range;
                workbook = excelApp.Workbooks.Open(filename);
                worksheet = (Excel.Worksheet)workbook.Sheets["Sheet1"];

                int column = 0;
                int row = 0;

                range = worksheet.UsedRange;                
                for (row = 2; row <= range.Rows.Count; row++)
                {
                    DataAccess.UpdateMobileDates(string.Concat("0", (range.Cells[row, 1] as Excel.Range).Value2.ToString()), (range.Cells[row, 3] as Excel.Range).Value2.ToString(), (range.Cells[row, 3] as Excel.Range).Value2.ToString());                                     
                }
                workbook.Close(true, Missing.Value, Missing.Value);
                excelApp.Quit();                
            }
		}

        private bool CanSearch()
		{
			return
				IsSearching == false &&
				((SearchMobileNumber != null && SearchMobileNumber.Trim().Length > 0) || 
                 (SearchMobileActivateDate != null && SearchMobileActivateDate.ToString().Trim().Length > 0) || 
                 (SearchMobileLastChargeDate != null &&	SearchMobileLastChargeDate.ToString().Trim().Length > 0));
		}

		private bool CanCharge()
		{
            return
                IsCharging == false &&
                searchResults.Count > 0;
		}

   		private bool CanImport()
		{
            return
                isImporting == false;
		}

        //private void OnVisit()
        //{
        //    Process.Start( SelectedResult.SourceUrl );
        //}

        //private bool CanVisit()
        //{
        //    return SelectedResult != null;
        //}

		private async void UpdateStats()
		{
			while ( true )
			{
				await Task.Delay( 100 );

				if ( chargingStats.ChargeCount > 0 )
				{
					StatsText = string.Format( "[{0} pending] {1} Number {2}.",
										chargingStats.ChargeCount,
										chargingStats.CurrentAction,
										chargingStats.CurrentTarget );
				}
				else
				{
					StatsText = "Current action: Idle";
				}
			}
		}

		private async Task<ChargeResult> ChargeMobileTaskAsync(string mobileNum)
		{
			ChargeResult result = new ChargeResult();
			result.MobileNumber = mobileNum;
			result.Data = await Charger.ChargeMobileAsyncTask( mobileNum );

			return result;
		}

   		private async Task<List<SearchResult>> SearchTaskAsync(string mobileNum, DateTime mobileActivateDate, DateTime mobileLastChargeDate)
		{
			List<SearchResult> result = new List<SearchResult>();			
            result = await DataAccess.SearchAsyncTask(mobileNum, mobileActivateDate, mobileLastChargeDate);
			return result;
		}

	}
}
