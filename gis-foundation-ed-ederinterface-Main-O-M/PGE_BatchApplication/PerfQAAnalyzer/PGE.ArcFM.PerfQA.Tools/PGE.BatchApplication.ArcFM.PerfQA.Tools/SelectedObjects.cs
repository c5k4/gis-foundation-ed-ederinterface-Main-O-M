using ESRI.ArcGIS.Geodatabase;
using System;

namespace PGE.BatchApplication.ArcFM_PerfQA_Tools
{
	internal class SelectedObjects
	{
		private string sClassName;

		private int iClassID;

		private ISelectionSet pSelSet;

		public int ClassID
		{
			get
			{
				return this.iClassID;
			}
			set
			{
				this.iClassID = value;
			}
		}

		public string ClassName
		{
			get
			{
				return this.sClassName;
			}
			set
			{
				this.sClassName = value;
			}
		}

		public ISelectionSet SelectionSet
		{
			get
			{
				return this.pSelSet;
			}
		}

		public SelectedObjects(IFeatureClass pFC)
		{
			IDataset dataset = (IDataset)pFC;
			this.iClassID = pFC.FeatureClassID;
			this.sClassName = dataset.Name;
			this.pSelSet = pFC.Select(null, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionEmpty, dataset.Workspace);
		}
	}
}