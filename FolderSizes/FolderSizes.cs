using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FolderSizes
{
	public partial class FolderSizes : Form
	{
		private const int NAME_COL_INDEX = 0;
		private const int SIZE_COL_INDEX = 1;

		private string curPath;
		private ItemComparer itemSorter;
		private Dictionary<string, ListViewItem> subdirPathToItemMap;
		private DirSizesCalculator dirSizeCalculator;

		public FolderSizes()
		{
			curPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			itemSorter = new ItemComparer();
			subdirPathToItemMap = new Dictionary<string, ListViewItem>();
			dirSizeCalculator = new DirSizesCalculator();

			InitializeComponent();

			// dirListing setup
			dirListing.SmallImageList = new ImageList();
			dirListing.SmallImageList.Images.Add("folder", Properties.Resources.folder);
			dirListing.ListViewItemSorter = itemSorter;
		}

		private void UpdateDirListing()
		{
			dirSizeCalculator.AbortTasksAndWait();

			lock (dirListing)
			{
				dirListing.Items.Clear();
				subdirPathToItemMap.Clear();
			}

			IEnumerable<string> subdirPaths;
			IEnumerable<string> filePaths;

			try
			{
				subdirPaths = Directory.EnumerateDirectories(curPath);
				filePaths = Directory.EnumerateFiles(curPath);
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show("Unauthorized access!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			foreach (string subdirPath in subdirPaths)
			{
				string subdirName = Path.GetFileName(subdirPath);
				long subdirSize = 0L;
				string subdirSizeStr = "Calculating";

				ListViewItem item = new ListViewItem(new string[] { subdirName, subdirSizeStr });
				item.Tag = subdirSize;
				item.ImageKey = "folder";

				dirListing.Items.Add(item);
				subdirPathToItemMap.Add(subdirPath, item);

				dirSizeCalculator.StartTask(subdirPath, ProcessDirSizeResult);
			}

			foreach (string filePath in filePaths)
			{
				string fileName = Path.GetFileName(filePath);
				long fileSize = new FileInfo(filePath).Length;
				string fileSizeStr = FileSizeToReadableString(fileSize);

				ListViewItem item = new ListViewItem(new string[] { fileName, fileSizeStr });
				item.Tag = fileSize;

				dirListing.Items.Add(item);
			}

			dirListing.Sort();
		}

		private string FileSizeToReadableString(long size)
		{
			double value = size;
			string[] suffixes = { "B", "kB", "MB", "GB", "TB" };
			int suffixIndex = 0;

			while (value >= 1024 && suffixIndex < suffixes.Length - 1)
			{
				value /= 1024;
				suffixIndex++;
			}

			return string.Format("{0:0.##} {1}", value, suffixes[suffixIndex]);
		}

		private void ProcessDirSizeResult(long dirSize, string dirPath)
		{
			lock (dirListing)
			{
				if (!subdirPathToItemMap.ContainsKey(dirPath))
					return;

				try
				{
					this.BeginInvoke(new MethodInvoker(() =>
					{
						ListViewItem item = subdirPathToItemMap[dirPath];
						item.SubItems[SIZE_COL_INDEX].Text = FileSizeToReadableString(dirSize);
						item.Tag = dirSize;
					}));
				}
				catch (Exception)
				{
					// exceptions are thrown when form is closed, can be ignored
				}
			}
		}

		private void openMenuButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
			DialogResult result = folderBrowser.ShowDialog();

			if (result == DialogResult.OK)
			{
				curPath = folderBrowser.SelectedPath;
				UpdateDirListing();
			}
		}
		private void upMenuButton_Click(object sender, EventArgs e)
		{
			DirectoryInfo parentInfo = Directory.GetParent(curPath);
			if (parentInfo != null)
				curPath = parentInfo.FullName;
			UpdateDirListing();
		}

		private void refreshButton_Click(object sender, EventArgs e)
		{
			UpdateDirListing();
		}

		private void dirListing_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (e.Column == itemSorter.SortColumnIndex)
				itemSorter.SortingOrder =
					itemSorter.SortingOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			else
			{
				itemSorter.SortColumnIndex = e.Column;
				itemSorter.SortingOrder = SortOrder.Ascending;
			}

			dirListing.Sort();
		}

		private void dirListing_ItemActivate(object sender, EventArgs e)
		{
			string subdirName = dirListing.FocusedItem.Text;
			string subdirPath = Path.Join(curPath, subdirName);
			FileAttributes attributes = File.GetAttributes(subdirPath);

			if (attributes.HasFlag(FileAttributes.Directory))
			{
				curPath = subdirPath;
				UpdateDirListing();
			}
		}

		private void FolderSizes_FormClosing(object sender, FormClosingEventArgs e)
		{
			dirSizeCalculator.Terminate();
		}

		class ItemComparer : IComparer
		{
			public int SortColumnIndex { get; set; } = NAME_COL_INDEX;
			public SortOrder SortingOrder { get; set; } = SortOrder.Ascending;

			public int Compare(object o1, object o2)
			{
				ListViewItem i1 = o1 as ListViewItem;
				ListViewItem i2 = o2 as ListViewItem;

				// folders are always before files
				if (i1.ImageKey != i2.ImageKey)
					return i1.ImageKey == "folder" ? -1 : 1;
				else
				{
					// sort by name
					if (SortColumnIndex == NAME_COL_INDEX)
					{
						string s1 = i1.SubItems[NAME_COL_INDEX].Text;
						string s2 = i2.SubItems[NAME_COL_INDEX].Text;
						return SortingOrder == SortOrder.Ascending ? s1.CompareTo(s2) : s2.CompareTo(s1);
					}
					// sort by size
					else
					{
						long t1 = (long)i1.Tag;
						long t2 = (long)i2.Tag;

						if (t1 < t2) return SortingOrder == SortOrder.Ascending ? -1 : 1;
						if (t1 > t2) return SortingOrder == SortOrder.Ascending ? 1 : -1;
						else return 0;
					}
				}
			}
		}
	}
}
