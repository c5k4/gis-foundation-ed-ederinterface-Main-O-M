﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace PGE.BatchApplication.ROBCApp
{

    public partial class UnassignedROBCs : Page
    {
        private DataGridColumn currentSortColumn;
        private ListSortDirection currentSortDirection;
        private UnassignedROBCViewModel viewModel;
        private const string PAGE_NAME = "UNASSIGNED_ROBC";

        public UnassignedROBCs()
        {
            InitializeComponent();
            viewModel = new UnassignedROBCViewModel();
            this.Loaded += (s, e) =>
            {
                this.DataContext = viewModel; 
                viewModel.DataGridVisibility = false;
                viewModel.LoadButtonVisibility = true;
                //viewModel.ProcessingImageVisibility = false;
                /*PopulateCircuitWithoutROBCList(); */
            };
        }

        private void RefreshROBCList()
        {
            viewModel.RefreshDataGrid(PAGE_NAME);
        }

        private void PopulateCircuitWithoutROBCList()
        {
            viewModel.PopulateCircuitWithoutROBCList();
        }

        private void UnassignedROBCDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            /*
            // The current sorted column must be specified in XAML.
            currentSortColumn = dataGrid.Columns.Where(c => c.SortDirection.HasValue).Single();
            currentSortDirection = currentSortColumn.SortDirection.Value;
             */
        }

        /// <summary>
        /// Sets the sort direction for the current sorted column since the sort direction
        /// is lost when the DataGrid's ItemsSource property is updated.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Ignored.</param>
        private void UnassignedROBCDataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            /*
            if (currentSortColumn != null)
            {
                currentSortColumn.SortDirection = currentSortDirection;
            }*/
        }

        /// <summary>
        /// Custom sort the datagrid since the actual records are stored in the
        /// server, not in the items collection of the datagrid.
        /// </summary>
        /// <param name="sender">The parts data grid.</param>
        /// <param name="e">Contains the column to be sorted.</param>
        private void UnassignedROBCDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            string sortField = String.Empty;

            // Use a switch statement to check the SortMemberPath
            // and set the sort column to the actual column name. In this case,
            // the SortMemberPath and column names match.
            switch (e.Column.SortMemberPath)
            {
                case ("Id"):
                    sortField = "Id";
                    break;
                case ("Name"):
                    sortField = "Name";
                    break;
            }

            ListSortDirection direction = (e.Column.SortDirection != ListSortDirection.Ascending) ?
              ListSortDirection.Ascending : ListSortDirection.Descending;
            bool sortAscending = direction == ListSortDirection.Ascending;
            Sort(sortField, sortAscending);
            currentSortColumn.SortDirection = null;
            e.Column.SortDirection = direction;
            currentSortColumn = e.Column;
            currentSortDirection = direction;
        }
        private string sortColumn = "Id";
        private bool ascending = true;
        /// <summary>
        /// Sorts the list of ROBCs.
        /// </summary>
        /// <param name="sortColumn">The column or member that is the basis for sorting.</param>
        /// <param name="ascending">Set to true if the sort</param>
        public void Sort(string sortColumn, bool ascending)
        {
            this.sortColumn = sortColumn;
            this.ascending = ascending;
            RefreshROBCList();
        }

        public void AssignROBC(object sender, RoutedEventArgs e)
        {
            ManageROBC ObjManageROBC = new ManageROBC(viewModel.SelectedCircuitWithoutROBC.CircuitId);
            this.NavigationService.Navigate(ObjManageROBC);
            //ManageROBC ObjManageROBC = new ManageROBC(viewModel);
            //this.NavigationService.Navigate(ObjManageROBC);
        }

        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    this.NavigationService.RemoveBackEntry();
        //    ROBCCircuitFinder robcCircuitFinder = new ROBCCircuitFinder();
        //    this.NavigationService.Navigate(robcCircuitFinder);
        //}

       

        private void LoadDataGrid_Click(object sender, RoutedEventArgs e)
        {
            //this.PrgBarLoadData.Visibility = System.Windows.Visibility.Visible;
        }

       
    }
    
}
