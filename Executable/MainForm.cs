using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ResEx.Common;
using ResEx.Core;
using ResEx.StandardAdapters;
using ResEx.Win;

namespace ResEx
{
    public partial class MainForm : Form
    {    
        private readonly Context Context;
        private bool formLoading = true;
        private bool gridLoading;
        private readonly string HelpNamespace = Path.Combine(Application.StartupPath + Path.DirectorySeparatorChar, "ResEx.chm");
        
        #region " Menu and Toolbar handling "

        private bool checkedChangedWorking;

        private void CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkedChangedWorking)
            {
                return;
            }

            this.checkedChangedWorking = true;
            try 
            {                
                if (sender.Equals(HideTranslatedToolStripButton))
                {
                    HideTranslatedToolStripMenuItem.Checked = HideTranslatedToolStripButton.Checked;
                    this.ExecCommand("hidetranslated");
                }
                else if (sender.Equals(HideTranslatedToolStripMenuItem))
                {
                    HideTranslatedToolStripButton.Checked = HideTranslatedToolStripMenuItem.Checked;
                    this.ExecCommand("hidetranslated");
                }
                else if (sender.Equals(HideLockedToolStripMenuItem))
                {
                    this.ExecCommand("hidelocked");
                }
                else if (sender.Equals(HideVSFormItemsToolStripMenuItem))
                {
                    this.ExecCommand("hidevsformitems");
                }
                else if (sender.Equals(HideEmptyToolStripMenuItem))
                {
                    this.ExecCommand("hideemptyitems");
                }
            }
            finally
            {
                this.checkedChangedWorking = false;
            }
        }

        private void BaseCommentToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshButtonState();
        }

        private void LocalCommentToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshButtonState();
        }

        private void TreeViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshButtonState();
        }

        private void ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {        
            try
            {
                if (sender is ToolStripMenuItem)
                {
                    ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
                    toolStripMenuItem.DropDown.Close();
                }

                string key;

                if (e.ClickedItem.Name.EndsWith("ToolStripMenuItem"))
                {
                    key = e.ClickedItem.Name.Substring(0, e.ClickedItem.Name.Length - 17);
                }
                else if (e.ClickedItem.Name.EndsWith("ToolStripButton"))
                {
                    key = e.ClickedItem.Name.Substring(0, e.ClickedItem.Name.Length - 15);
                }
                else
                {
                    key = e.ClickedItem.Name;
                }

                if (key.ToLower() == "hidetranslated")
                {
                    // this is handled elsewhere (search for 'hidetranslated')
                    return;
                }

                if (key.ToLower() == "hidelocked")
                {
                    // this is handled elsewhere (search for 'hidelocked')
                    return;
                }

                this.ExecCommand(key);
            }
            catch (Exception ex)
            {
                Tools.ShowExceptionMessageBox(ex);
            }
        }

        #endregion

        #region " Form Load/Unload "

        public MainForm()
        {
            this.Context = new Context(this.AddButton, this);
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Settings_Restore();

            this.Context.Initialize();

            TreeView1.PathSeparator = Properties.Settings.Default.PathSeparator;
            Text = Tools.GetAssemblyProduct();
            this.RefreshButtonState();

            // handle command line
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                this.ResourceBundle_Open(args[1]);
            }

            this.formLoading = false;
        }

        private void Localize()
        {        
            MainToolStrip.Text = My.Resources.Local.Menu_MainToolbar;
            OpenToolStripButton.Text = My.Resources.Local.Menu_Open;
            OpenToolStripButton.ToolTipText = My.Resources.Local.Menu_OpenToolTip;
            SaveToolStripButton.Text = My.Resources.Local.Menu_Save;
            SaveToolStripButton.ToolTipText = My.Resources.Local.Menu_SaveToolTip;
            HideTranslatedToolStripButton.Text = My.Resources.Local.Menu_HideTranslated;
            SearchToolStripTextBox.ToolTipText = My.Resources.Local.Menu_SearchToolTip;
            ToolStripLabel1.Text = My.Resources.Local.Menu_Culture;
            CultureToolStripComboBox.ToolTipText = My.Resources.Local.Menu_CurrentCulture;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CustomDataGrid1.MainDataGridView.IsCurrentCellInEditMode || !CustomDataGrid1.MainDataGridView.EndEdit())
            {
                e.Cancel = !this.AskToSave();
                if (!e.Cancel)
                {
                    this.Settings_Save();
                }
            }

            try
            {
                if (this.Context != null)
                {
                    this.Context.Dispose();
                }
            }
            catch (Exception ex)
            {
                // TODO : LOG THE EXCEPTION
                Debug.WriteLine("Exception disposing context " + ex);
            }        
        }

        #endregion
        
        private void ExecCommand(string key)
        {
            key = key.ToLower();

            switch (key)
            {
                case "open":
                    this.ResourceBundle_Open();
                    break;
                case "save":
                    this.ResourceBundle_Save();
                    break;
                case "addculture":
                    this.ResourceSet_Add();
                    break;
                case "removeculture":
                    this.ResourceSet_Delete();
                    break;
                case "increasefont":
                    this.CustomDataGrid1.SetGridFont(CustomDataGrid1.MainDataGridView.Font.Size + 2);
                    break;
                case "decreasefont":
                    this.CustomDataGrid1.SetGridFont(CustomDataGrid1.MainDataGridView.Font.Size - 2);
                    break;
                case "showworkingfolder":
                    try 
                    {
                        FileInfo baseFileInfo = new FileInfo(this.Context.CurrentBaseFile);
                        using (Process p = new Process())
                        {
                            p.StartInfo = new ProcessStartInfo("explorer", baseFileInfo.DirectoryName);
                            p.Start();
                        }
                    }
                    catch
                    {
                        // there is some very strange case where Process.Start throws 'File not found' exception altough process does start.
                        // I hate catching exceptions without doing something with the exception, but in this case it seems there is nothing else to do!
                    }

                    break;
                case "hidetranslated":
                    this.ApplyCriteria();
                    break;
                case "hideemptyitems":
                    this.ApplyCriteria();
                    break;
                case "copybasevalue":
                    this.CopyBaseValue();
                    break;
                case "hidelocked":
                    this.ApplyCriteria();
                    break;
                case "hidevsformitems":
                    this.ApplyCriteria();
                    break;
                case "resexhome":
                    Main.OpenURL(Main.ResExHomePage);
                    break;
                case "discuss":
                    Main.OpenURL(Main.ForumsURL);
                    break;
                case "checkforupdates":
                    Main.CheckForUpdates(false);
                    break;
                case "helpcontents":
                    Help.ShowHelp(this, this.HelpNamespace);
                    break;
                case "about":
                    AboutBox about = new AboutBox();
                    about.ShowDialog();
                    break;
                case "exit":
                    Close();
                    break;
            }
        }

        private void CopyBaseValue()
        {
            // if a cell is selected
            if (CustomDataGrid1.CurrentCell != null)
            {
                DataGridViewRow row = CustomDataGrid1.MainDataGridView.Rows[CustomDataGrid1.CurrentCell.RowIndex];
                ResourceItem baseResourceItem = (ResourceItem)row.Tag;

                // if value is allowed to be localized
                if (!baseResourceItem.Locked)
                {
                    // if current column is not LocalValue, make it current
                    if (CustomDataGrid1.CurrentColumn.Name != ColumnNames.LocalValue)
                    {
                        CustomDataGrid1.CurrentCell = row.Cells[ColumnNames.LocalValue];
                    }
                    
                    // if not currently in edit mode, then begin edit mode
                    if (!CustomDataGrid1.MainDataGridView.IsCurrentCellInEditMode)
                    {
                        CustomDataGrid1.MainDataGridView.BeginEdit(false);
                    }
                    
                    // get instance of editor of cell and set base value
                    DataGridViewTextBoxEditingControl editControl = (DataGridViewTextBoxEditingControl)CustomDataGrid1.MainDataGridView.EditingControl;
                    editControl.Text = row.Cells[ColumnNames.BaseValue].Value.ToString();
                    editControl.SelectAll();
                }
            }
        }

        private void CustomDataGrid1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (CustomDataGrid1.MainDataGridView.Columns[e.ColumnIndex].Name == ColumnNames.LocalValue)
            {
                DataGridViewRow row = CustomDataGrid1.MainDataGridView.Rows[e.RowIndex];
                ResourceItem baseResourceItem = (ResourceItem)row.Tag;
                object localValue = row.Cells[ColumnNames.LocalValue].Value;
                object baseValue = baseResourceItem.Value;

                if (baseResourceItem is ResourceStringItem)
                {
                    ResourceStringItem baseResourceStringItem = baseResourceItem as ResourceStringItem;

                    // check for ampersands (&)
                    if (!StringTools.ValueNullOrEmpty(localValue) && !StringTools.ValueNullOrEmpty(baseValue))
                    {
                        if (StringTools.CountInstances(localValue.ToString(), "&", StringComparison.Ordinal) != StringTools.CountInstances(baseValue.ToString(), "&", StringComparison.Ordinal))
                        {
                            const string Message = "The ampersands (&) of base value do not much with the local value. Is this correct?";
                            if (MessageBox.Show(Message, Tools.GetAssemblyProduct(), MessageBoxButtons.YesNo) == DialogResult.No)
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                    }

                    // check for placeholders in strings
                    if (!StringTools.ValueNullOrEmpty(localValue) && !StringTools.ValueNullOrEmpty(baseValue))
                    {
                        int counter = 0;
                        string counterElement = "{0}";
                        while (true)
                        {
                            if (baseValue.ToString().IndexOf(counterElement) == -1)
                            {
                                break;
                            }

                            if (localValue.ToString().IndexOf(counterElement) == -1)
                            {
                                string message = String.Format(My.Resources.Local.PlaceholderExpectedError, counterElement);
                                MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                e.Cancel = true;
                                return;
                            }

                            counter += 1;
                            counterElement = "{" + counter + "}";
                        }
                    }

                    // check for max length
                    if (!StringTools.ValueNullOrEmpty(localValue))
                    {
                        if (baseResourceStringItem.MaxLength != ResourceStringItem.UnlimitedLength)
                        {
                            if (localValue.ToString().Length > baseResourceStringItem.MaxLength)
                            {
                                string message = String.Format(My.Resources.Local.MaximumLengthError, baseResourceStringItem.MaxLength, localValue.ToString().Length);
                                MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                e.Cancel = true;
                                return;
                            }
                        }
                    }

                    // check for exclusions
                    if (!StringTools.ValueNullOrEmpty(localValue) && !StringTools.ValueNullOrEmpty(baseValue))
                    {
                        var manager = new ExclusionManager();
                        manager.ExclusionPatterns = this.Context.CurrentBaseResourceSet.Exclusions;
                        var notExcludedExclusion = manager.Validate(localValue.ToString(), baseValue.ToString());
                        if (notExcludedExclusion != null && notExcludedExclusion.Length > 0)
                        {
                            var message = string.Format("The following terms of base item should not be translated. See Exclusions list for more information.\r\n\r\n{0}\r\n\r\nFix your translation so that these terms exist.", string.Join(", ", notExcludedExclusion));
                            MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
        }

        private void CustomDataGrid1_SelectionChanged(object sender, EventArgs e)
        {
            if (!this.formLoading && !this.gridLoading)
            {
                this.Context.InvokeCurrentResourceItemChanged();
                this.RefreshButtonState();
            }
        }

        #region " Progress Bar Handling "
        
        private void ProgressValueSet(int value, int pendingForReview)
        {
            // set progress bar
            if (value >= ToolStripProgressBar1.Minimum && (value <= ToolStripProgressBar1.Maximum))
            {
                ToolStripProgressBar1.Value = value;
            }

            // set text
            string text = My.Resources.Local.ProgressBarText;
            ProgressToolStripLabel.Text = String.Format(text, ToolStripProgressBar1.Value, ToolStripProgressBar1.Maximum);
            ProgressToolStripLabel.ForeColor = ToolStripProgressBar1.Maximum == ToolStripProgressBar1.Value ? Color.DarkGreen : SystemColors.ControlText;

            // set text for pending for review
            this.PendingForReviewToolStripLabel.Visible = pendingForReview > 0;
            this.PendingForReviewToolStripLabel.Text = string.Format("For Review {0} - ", pendingForReview);
        }

        private void ProgressSetMax(int value)
        {
            ToolStripProgressBar1.Maximum = value;
        }

        #endregion

        #region " Add/Remove/Select Culture "

        private void ResourceSet_Delete()
        {
            if (CultureToolStripComboBox.SelectedIndex != -1)
            {
                // prevent deletion of the last resource set
                if (this.Context.CurrentResourceBundle.Where(p => p.Status != ResourceSetStatus.Deleted).Count() == 1)
                {
                    MessageBox.Show("Deletion of the last resource set of the bundle is not possible!", Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                string message = String.Format(My.Resources.Local.DeletingResourceSetWarning, this.Context.CurrentLocalResourceSet.Culture);
                if (MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    // mark as deleted in collection
                    this.Context.CurrentResourceBundle.CultureRemove(CultureToolStripComboBox.Text);
                    
                    // remove from UI
                    CultureToolStripComboBox.Items.RemoveAt(CultureToolStripComboBox.SelectedIndex);
                    
                    // set current resource to nothing
                    this.Context.CurrentLocalResourceSet = null;
                    
                    // select another culture as current
                    if (CultureToolStripComboBox.Items.Count > 0)
                    {
                        CultureToolStripComboBox.SelectedIndex = 0;
                    }

                    this.RefreshButtonState();
                }
            }
        }

        private void ResourceSet_Add()
        {
            using (CultureSelectForm cultureSelectForm = new CultureSelectForm())
            {
                if (cultureSelectForm.ShowDialog() == DialogResult.OK)
                {
                    this.ResourceSet_Add(cultureSelectForm.SelectedCulture);
                }
            }
        }

        private void ResourceSet_Add(CultureInfo culture)
        {
            if (this.Context.CurrentResourceBundle.ContainsCulture(culture.Name))
            {
                MessageBox.Show(My.Resources.Local.CultureAlreadyExistsError, Tools.GetAssemblyProduct(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // add to collection
                this.Context.CurrentResourceBundle.Add(culture.Name);
                
                // add to UI
                CultureToolStripComboBox.Items.Add(culture);
                
                // select it
                CultureToolStripComboBox.SelectedIndex = CultureToolStripComboBox.Items.Count - 1;

                this.RefreshButtonState();
            }
        }

        private void CultureToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ResourceSet_SetCurrent(CultureToolStripComboBox.Text);
        }

        /// <summary>
        /// Sets the current resource file.
        /// </summary>
        private void ResourceSet_SetCurrent(string culture)
        {
            this.Context.CurrentLocalResourceSet = null;
            try
            {
                // unsubscribe from the events of the previous resource set
                if (this.Context.CurrentLocalResourceSet != null)
                {
                    this.Context.CurrentLocalResourceSet.CollectionChanged -= this.CurrentLocalResourceSet_CollectionChanged;
                    this.Context.CurrentLocalResourceSet.ResourceItemChanged -= this.CurrentLocalResourceSet_ResourceItemChanged;
                }

                ResourceSet localResourceSetTemp = this.Context.CurrentResourceBundle[culture];

                this.gridLoading = true;
                try
                {
                    foreach (DataGridViewRow row in CustomDataGrid1.MainDataGridView.Rows)
                    {
                        this.UpdateGridRowFromResourceItem(row, localResourceSetTemp);
                    }
                }
                finally
                {
                    this.gridLoading = false;
                }

                this.ProgressSetMax(this.Context.CurrentBaseResourceSet.CountStringItems(false) - this.Context.CurrentBaseResourceSet.CountLocked());
                this.ProgressValueSet(localResourceSetTemp.CountTranslatedItems(this.Context.CurrentBaseResourceSet), localResourceSetTemp.CountMarkedForReviewing());

                this.Context.CurrentLocalResourceSet = localResourceSetTemp;

                this.SelectFirstVisibleRow();

                // subscribe to events of the resource set
                this.Context.CurrentLocalResourceSet.CollectionChanged += this.CurrentLocalResourceSet_CollectionChanged;
                this.Context.CurrentLocalResourceSet.ResourceItemChanged += this.CurrentLocalResourceSet_ResourceItemChanged;

                this.RefreshButtonState();
            }
            catch
            {
                // If exception occurs (grid is left partially updated), the following makes sure that the end-user 
                // will not be able to save it and that buttons are refreshed as if no resource set was active.
                this.Context.CurrentProjectIsDirty = false;
                this.RefreshButtonState();

                throw;
            }
        }

        #endregion

        #region " Binding - We don't use real windows forms binding so we need to transfer changes from data to UI and back programmatically "

        // this is triggered when a new item is added to the base local resource set
        private void CurrentBaseResourceSet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var resourceSet = (ResourceSet)sender;
            this.Context.CurrentProjectIsDirty = true;
            
            if (resourceSet.Status == ResourceSetStatus.Unaffected)
            {
                resourceSet.Status = ResourceSetStatus.Updated;
            }

            this.RefreshButtonState();
        }

        // this is triggered when a property of an item that belongs to the base local resource set changes
        private void CurrentBaseResourceSet_ResourceItemChanged(object sender, ResourceItemChangedEventArgs e)
        {
            var resourceSet = (ResourceSet)sender;
            this.Context.CurrentProjectIsDirty = true;
            
            if (resourceSet.Status == ResourceSetStatus.Unaffected)
            {
                resourceSet.Status = ResourceSetStatus.Updated;
            }

            this.RefreshButtonState();
        }

        // this is triggered when a new item is added to the current local resource set
        private void CurrentLocalResourceSet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResourceSet resourceSet = (ResourceSet)sender;

            foreach (ResourceItem newItem in e.NewItems)
            {
                foreach (DataGridViewRow row in CustomDataGrid1.MainDataGridView.Rows)
                {
                    string rowKey = row.Cells[ColumnNames.Key].Value.ToString();

                    if (rowKey == newItem.Name)
                    {
                        this.UpdateGridRowFromResourceItem(row, resourceSet);
                    }
                }
            }

            this.ProgressValueSet(resourceSet.CountTranslatedItems(this.Context.CurrentBaseResourceSet), resourceSet.CountMarkedForReviewing());
        }

        // this is triggered when a property of an item that belongs to the current local resource set changes
        private void CurrentLocalResourceSet_ResourceItemChanged(object sender, ResourceItemChangedEventArgs e)
        {
            ResourceSet resourceSet = (ResourceSet)sender;

            foreach (DataGridViewRow row in CustomDataGrid1.MainDataGridView.Rows)
            {
                string rowKey = row.Cells[ColumnNames.Key].Value.ToString();

                if (rowKey == e.Item.Name)
                {
                    this.UpdateGridRowFromResourceItem(row, resourceSet);
                }
            }

            this.ProgressValueSet(resourceSet.CountTranslatedItems(this.Context.CurrentBaseResourceSet), resourceSet.CountMarkedForReviewing());
        }

        /// <summary>
        /// Updates the given grid row using the corresponding resource item of the given resource set
        /// </summary>
        private void UpdateGridRowFromResourceItem(DataGridViewRow row, ResourceSet resourceSet)
        {
            string rowKey = row.Cells[ColumnNames.Key].Value.ToString();

            // if given resource set does not contain resource item for the row
            // then put all values to null
            if (!resourceSet.ContainsKey(rowKey))
            {
                row.Cells[ColumnNames.LocalValue].Value = null;
                row.Cells[ColumnNames.LocalComment].Value = null;
                RowSetStatus(row, null);
            }
            else
            {
                ResourceItem resourceItem = resourceSet[rowKey];
                row.Cells[ColumnNames.LocalValue].Value = resourceItem.Value;
                row.Cells[ColumnNames.LocalComment].Value = resourceItem.Comment;
                RowSetStatus(row, resourceItem);
            }

            row.Height = row.GetPreferredHeight(row.Index, DataGridViewAutoSizeRowMode.AllCells, true);
        }

        /// <summary>
        /// Updates the given grid row using the corresponding resource item of the given resource set
        /// </summary>
        private void UpdateResourceItemFromGridRow(DataGridViewRow row)
        {
            ResourceSet resourceSet = this.Context.CurrentLocalResourceSet;
            string rowKey = row.Cells[ColumnNames.Key].Value.ToString();

            ResourceItem localResourceItem;

            // add resource item or get existing
            bool isNewItem;
            if (!resourceSet.ContainsKey(rowKey))
            {
                localResourceItem = new ResourceItem();
                localResourceItem.Name = rowKey;
                isNewItem = true;
            }
            else
            {
                localResourceItem = resourceSet[rowKey];
                isNewItem = false;
            }

            // save value of row
            localResourceItem.Value = row.Cells[ColumnNames.LocalValue].Value != null ? row.Cells[ColumnNames.LocalValue].Value.ToString() : null;

            // save comment of row
            localResourceItem.Comment = row.Cells[ColumnNames.LocalComment].Value != null ? row.Cells[ColumnNames.LocalComment].Value.ToString() : null;

            if (isNewItem)
            {
                resourceSet.Add(rowKey, localResourceItem);
            }

            if (resourceSet.Status == ResourceSetStatus.Unaffected)
            {
                resourceSet.Status = ResourceSetStatus.Updated;
            }

            this.RefreshButtonState();
        }

        #endregion

        #region " Open/Save Code "
        
        //delegate for asynchronous call
        private delegate void OpenFileDelegate(String s);

        private void ResourceBundle_Open()
        {
            this.ResourceBundle_Open(null);
        }

        private void ResourceBundle_Open(string fileName)
        {
            if (this.AskToSave())
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        fileName = OpenFileDialog1.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                // change current directory to match the files directory
                FileInfo fileInfo = new FileInfo(fileName);

                // unsubscribe to change events of previous base resource set
                if (this.Context.CurrentBaseResourceSet != null)
                {
                    this.Context.CurrentBaseResourceSet.CollectionChanged -= this.CurrentBaseResourceSet_CollectionChanged;
                    this.Context.CurrentBaseResourceSet.ResourceItemChanged -= this.CurrentBaseResourceSet_ResourceItemChanged;
                }

                ResXResourceBundleAdapter resXResourceBundle = new ResXResourceBundleAdapter(new StandardAdapters.Common.FileSystem());
                this.Context.CurrentResourceBundle = resXResourceBundle.Load(fileName);
                this.Context.CurrentBaseFile = fileName;
                this.Context.CurrentBaseResourceSet = this.Context.CurrentResourceBundle.NeutralOrFirst();

                // subscribe to change events of base resource set
                this.Context.CurrentBaseResourceSet.CollectionChanged += this.CurrentBaseResourceSet_CollectionChanged;
                this.Context.CurrentBaseResourceSet.ResourceItemChanged += this.CurrentBaseResourceSet_ResourceItemChanged;

                // load bundle on UI
                this.RefreshCurrentBundle();

                // insert into recent items list
                string fileNameStipped = StringTools.CompactText(fileName, 50);
                RecentItemsManager.PushItem(new RecentItem(fileNameStipped, "file", fileName));

                // show caption
                Text = fileInfo.Name + " - " + Tools.GetAssemblyProduct();

                this.Context.CurrentProjectIsDirty = false;
                this.Context.InvokeCurrentResourceBundleChanged();
                this.RefreshButtonState();

                // if there is only one resource set (the base one) the propose to create a new one
                // to start translation
                if (this.Context.CurrentResourceBundle.Count == 1)
                {
                    this.ResourceSet_Add();                    
                }
            }
        }

        internal void RefreshCurrentBundle()
        {
            // load base file into grid
            DataGridViewRow newRow;
            this.gridLoading = true;
            try
            {
                CustomDataGrid1.MainDataGridView.Rows.Clear();
                foreach (ResourceItem i in this.Context.CurrentBaseResourceSet.Values)
                {                        
                    // show when
                    //  - value is of type string
                    var item = i as ResourceStringItem;
                    if (item != null)
                    {
                        int newRowIndex = CustomDataGrid1.MainDataGridView.Rows.Add(null, item.Name, item.Comment, item.Value);
                        newRow = CustomDataGrid1.MainDataGridView.Rows[newRowIndex];
                        newRow.Tag = item;
                        RowSetStatus(newRow, null);
                        newRow.Cells[ColumnNames.LocalValue].ReadOnly = item.Locked;
                        newRow.Cells[ColumnNames.LocalComment].ReadOnly = item.Locked;
                        if (item.Locked)
                        {
                            newRow.Cells[ColumnNames.LocalValue].Style.BackColor = Color.AliceBlue;
                            newRow.Cells[ColumnNames.LocalComment].Style.BackColor = Color.AliceBlue;
                        }
                    }
                }
            }
            finally
            {
                this.gridLoading = false;
            }

            CustomDataGrid1.AutoSizeContent();

            // load base file into tree view
            string[] keySplit;
            TreeView1.Nodes.Clear();
            TreeView1.Nodes.Add("<root>", My.Resources.Local.TreeNavigatorRootText);
            TreeView1.Nodes["<root>"].ImageKey = "resx";
            TreeView1.Nodes["<root>"].SelectedImageKey = "resx";
            TreeView1.Nodes.Add("<all>", My.Resources.Local.TreeNavigatorAllText);
            TreeView1.Nodes["<all>"].ImageKey = "resx";
            TreeView1.Nodes["<all>"].SelectedImageKey = "resx";

            foreach (ResourceItem item in this.Context.CurrentBaseResourceSet.Values)
            {
                if (item is ResourceStringItem)
                {
                    // add value to tree
                    keySplit = item.Name.Split(Properties.Settings.Default.PathSeparator.ToCharArray());
                    TreeNode parentNode = TreeView1.Nodes["<root>"];
                    for (int i = 0; i <= keySplit.Length - 2; i++)
                    {                       
                        if (parentNode.Nodes.ContainsKey(keySplit[i]))
                        {
                            parentNode = parentNode.Nodes[keySplit[i]];
                        }
                        else
                        {
                            parentNode = parentNode.Nodes.Add(keySplit[i], keySplit[i]);
                            parentNode.ImageKey = "string";
                            parentNode.SelectedImageKey = "string";
                        }
                    }
                }
            }

            TreeView1.Nodes["<root>"].Expand();
            TreeView1.SelectedNode = TreeView1.Nodes["<root>"];

            // load combo with cultures
            CultureToolStripComboBox.Items.Clear();
            foreach (ResourceSet i in this.Context.CurrentResourceBundle)
            {
                CultureToolStripComboBox.Items.Add(i.Culture);     
            }

            // make sure first visible row is active and selected
            this.SelectFirstVisibleRow();

            this.Context.CurrentLocalResourceSet = null;
            if (CultureToolStripComboBox.Items.Count > 0)
            {
                CultureToolStripComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Makes sure first visible row is active and selected
        /// selected does not necesseraly means active
        /// (also, depending on the selected tree item above and current search criteria the first row may not be visible)
        /// </summary>
        private void SelectFirstVisibleRow()
        {
            if (CustomDataGrid1.MainDataGridView.Rows.Count > 0)
            {
                foreach (var row in this.CustomDataGrid1.MainDataGridView.Rows.OfType<DataGridViewRow>())
                {
                    if (row.Visible)
                    {
                        // set selection if not already set to avoid redundant event triggering
                        if (!row.Selected)
                        {
                            row.Selected = true;
                        }

                        // activate local value cell or base value cell if local one is not visible (no local resource set is active)
                        var currentCell = row.Cells[ColumnNames.LocalValue].Visible ? row.Cells[ColumnNames.LocalValue] : row.Cells[ColumnNames.BaseValue];
                        CustomDataGrid1.MainDataGridView.CurrentCell = currentCell;

                        break;
                    }
                }
            }
        }

        private bool AskToSave()
        {
            if (this.Context.CurrentProjectIsDirty)
            {
                string message = String.Format(My.Resources.Local.SaveChangesWarning, this.Context.CurrentLocalResourceSet.Culture);
                switch (MessageBox.Show(message, Tools.GetAssemblyProduct(), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        this.ResourceBundle_Save();
                        return true;
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void ResourceBundle_Save()
        {
            if (this.Context.CurrentProjectIsDirty)
            {
                ResXResourceBundleAdapter resXResourceBundle = new ResXResourceBundleAdapter(new StandardAdapters.Common.FileSystem());
                resXResourceBundle.Save(this.Context.CurrentBaseFile, this.Context.CurrentResourceBundle);
                this.Context.CurrentProjectIsDirty = false;
            }

            this.RefreshButtonState();
        }

        #endregion

        #region " Search and Filters "
        
        private void SearchToolStripTextBox_TextChanged(object sender, EventArgs e)
        {
            SearchDelayTimer.Stop();
            SearchDelayTimer.Start();
        }

        private void SearchDelayTimer_Tick(object sender, EventArgs e)
        {
            this.ApplyCriteria();
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ApplyCriteria();
        }

        /// <summary>
        /// Apply selected criteria to grid. Criteria are the selected item in treeview and/or the search text
        /// </summary>
        private void ApplyCriteria()
        {
            SearchDelayTimer.Stop();

            // get search text
            string searchText = SearchToolStripTextBox.Text.ToLower();

            // get the selected path from the selected item of tree (eg. String1)
            string treePath = "<all>";
            if (TreeView1.SelectedNode != null)
            {
                if (TreeView1.SelectedNode.Equals(TreeView1.Nodes["<all>"]))
                {
                    treePath = "<all>";
                }
                else if (TreeView1.SelectedNode.Equals(TreeView1.Nodes["<root>"]))
                {
                    treePath = "<root>";
                }
                else
                {
                    treePath = TreeView1.SelectedNode.FullPath.Substring(TreeView1.Nodes["<root>"].Text.Length + 1);
                }
            }

            try
            {
                CustomDataGrid1.SuspendLayout();

                string key;
                ResourceItem resourceItem;

                foreach (DataGridViewRow row in CustomDataGrid1.MainDataGridView.Rows)
                {
                    resourceItem = (ResourceItem)row.Tag;

                    // make row invisible. it will be decided later in this loop whether to show it
                    row.Visible = false;

                    // hide translated
                    if (HideTranslatedToolStripMenuItem.Checked)
                    {
                        if (row.Cells[ColumnNames.LocalValue].Value != null && !String.IsNullOrEmpty(row.Cells[ColumnNames.LocalValue].Value.ToString()))
                        {
                            // leave row hidden and continue to next row
                            continue;
                        }
                    }

                    // hide locked
                    if (HideLockedToolStripMenuItem.Checked)
                    {
                        // if current resource item is locked then consider in translated
                        if (resourceItem.Locked && resourceItem.LockedReason == LockedReason.DeveloperLock)
                        {
                            // leave row hidden and continue to next row
                            continue;
                        }
                    }

                    // hide VS form items
                    if (HideVSFormItemsToolStripMenuItem.Checked)
                    {
                        if (resourceItem.Locked && resourceItem.LockedReason == LockedReason.FrameworkLock)
                        {
                            continue;
                        }
                    }

                    //hide empty items
                    if (HideEmptyToolStripMenuItem.Checked)
                    {
                        if (resourceItem.ValueEmpty)
                        {
                            continue;
                        }
                    }

                    // hide items that are not translatable but just hold resex metadata
                    if (resourceItem.Locked && resourceItem.LockedReason == LockedReason.ResexMetadata)
                    {
                        continue;
                    }

                    // check if row contains search text. if not row should not be visible.
                    if (!String.IsNullOrEmpty(searchText))
                    {
                        bool searchTextOk = false;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.OwningColumn.Visible)
                            {
                                if (cell.Value != null && cell.Value.ToString().ToLower().IndexOf(searchText) != -1)
                                {
                                    searchTextOk = true;
                                    break;
                                }
                            }
                        }

                        // if search text not found in row, leave it hidden and continue to next row
                        if (!searchTextOk)
                        {
                            continue;
                        }
                    }

                    // if '<all>' node is selected, show the row
                    if (treePath == "<all>")
                    {
                        row.Visible = true;
                        continue;
                    }

                    // get key of row (eg. String1_SubString1 which should be visible, or String_Placeholder1 which should be hidden)
                    key = row.Cells[ColumnNames.Key].Value.ToString();

                    // if no path is selected in tree
                    if (treePath == "<root>")
                    {
                        // show row if key does not have separator (is a base string) and does not exist in tree as node (it will be shown when clicking on that node)
                        if (key.IndexOf(Properties.Settings.Default.PathSeparator) == -1 && TreeView1.Nodes.Find(key, true).Length == 0)
                        {
                            row.Visible = true;
                        }

                        continue;
                    }

                    // if key or row is the same as selected tree node, show the row
                    if (key == treePath)
                    {
                        row.Visible = true;
                        continue;
                    }

                    // continue if current row (in this loop) starts with the selected path
                    if (key.StartsWith(treePath))
                    {
                        // continue if current row (in this loop) is a direct child of current path and not the child of a child
                        if (key.Substring(treePath.Length + 1).IndexOf(Properties.Settings.Default.PathSeparator) == -1 && key.Substring(treePath.Length, 1) == Properties.Settings.Default.PathSeparator)
                        {
                            row.Visible = true;
                            continue;
                        }
                    }
                }
            }
            finally
            {
                CustomDataGrid1.ResumeLayout();
            }
        }

        #endregion
        
        #region " Form/Button State Refresh "
        
        private void CustomDataGrid1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!this.formLoading && !this.gridLoading)
            {
                this.Context.CurrentProjectIsDirty = true;
                this.RefreshButtonState();
            }

            // if there is no current resource set then do nothing - apparently the change was for other purpose
            if (this.Context.CurrentLocalResourceSet == null)
            {
                return;
            }

            this.UpdateResourceItemFromGridRow(CustomDataGrid1.GetRow(e.RowIndex));
        }
        
        private void RefreshButtonState()
        {
            SaveToolStripMenuItem.Enabled = this.Context.CurrentProjectIsDirty;
            AddCultureToolStripMenuItem.Enabled = this.Context.CurrentResourceBundle != null;
            CopyBaseValueToolStripMenuItem.Enabled = this.Context.CurrentResourceBundle != null;
            CopyBaseValueToolStripButton.Enabled = CopyBaseValueToolStripMenuItem.Enabled;
            RemoveCultureToolStripMenuItem.Enabled = this.Context.CurrentLocalResourceSet != null;
            CultureToolStripComboBox.Enabled = CultureToolStripComboBox.Items.Count > 0;
            SearchToolStripTextBox.Enabled = this.Context.CurrentResourceBundle != null;
            ShowWorkingFolderToolStripMenuItem.Enabled = this.Context.CurrentResourceBundle != null;
            CustomDataGrid1.MainDataGridView.Columns[ColumnNames.BaseComment].Visible = BaseCommentToolStripMenuItem.Checked;
            CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalComment].Visible = LocalCommentToolStripMenuItem.Checked && this.Context.CurrentLocalResourceSet != null;
            SplitContainer1.Panel1Collapsed = !TreeViewToolStripMenuItem.Checked;

            CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalValue].Visible = CultureToolStripComboBox.SelectedIndex != -1;

            if (this.Context.CurrentLocalResourceSet == null)
            {
                CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalValue].HeaderText = My.Resources.Local.EditGridValueColumnHeader;
                CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalComment].HeaderText = My.Resources.Local.EditGridCommentColumnHeader;
            }
            else
            {
                CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalValue].HeaderText = this.Context.CurrentLocalResourceSet.Culture + " " + My.Resources.Local.EditGridValueColumnHeader;
                CustomDataGrid1.MainDataGridView.Columns[ColumnNames.LocalComment].HeaderText = this.Context.CurrentLocalResourceSet.Culture + " " + My.Resources.Local.EditGridCommentColumnHeader;
            }

            SaveToolStripButton.Enabled = SaveToolStripMenuItem.Enabled;

            this.RefreshStatusBar();

            this.RefreshPlugInButtons();
        }

        private void RefreshStatusBar()
        {
            // status bar text
            string statusText = CustomDataGrid1.MainDataGridView.Rows.Count + " " + My.Resources.Local.Strings;
            ItemsToolStripStatusLabel.Text = statusText;

            // word count
            string wordCountString = string.Empty;
            if (this.Context.CurrentResourceBundle != null && this.Context.CurrentBaseResourceSet != null)
            {
                wordCountString = My.Resources.Local.WordCountLabel + " : " + this.Context.CurrentBaseResourceSet.CountWords();
            }

            WordCountToolStripStatusLabel.Text = wordCountString;
        }

        private static void RowSetStatus(DataGridViewRow row, ResourceItem localResourceItem)
        {
            ResourceItem baseResourceItem = (ResourceItem)row.Tag;

            // set color (mark translated with gray color)
            if (localResourceItem != null && !localResourceItem.ValueEmpty)
            {
                row.Cells[ColumnNames.Key].Style.ForeColor = Color.DarkGray;
            }
            else
            {
                row.Cells[ColumnNames.Key].Style.ForeColor = SystemColors.ControlText;
            }

            row.Cells[ColumnNames.BaseValue].Style.ForeColor = row.Cells[ColumnNames.Key].Style.ForeColor;
            row.Cells[ColumnNames.BaseComment].Style.ForeColor = row.Cells[ColumnNames.Key].Style.ForeColor;
            row.Cells[ColumnNames.LocalValue].Style.BackColor = Color.Empty;        // reset to empty - used when marking for reviewing
            row.Cells[ColumnNames.LocalComment].Style.BackColor = Color.Empty;      // reset to empty - used when marking for reviewing

            // set icon
            if (baseResourceItem is ResourceStringItem)
            {
                if (baseResourceItem.Locked)
                {
                    row.Cells[ColumnNames.Icon].Value = My.Resources.Local.resString_locked;
                }
                else
                {
                    if (localResourceItem != null && localResourceItem.ReviewPending)
                    {
                        row.Cells[ColumnNames.Icon].Value = My.Resources.Local.resString_review;
                        row.Cells[ColumnNames.LocalValue].Style.BackColor = Color.Yellow;
                        row.Cells[ColumnNames.LocalComment].Style.BackColor = Color.Yellow;
                    }
                    else if (localResourceItem != null && !localResourceItem.ValueEmpty)
                    {
                        row.Cells[ColumnNames.Icon].Value = My.Resources.Local.resString_translated;
                    }
                    else
                    {
                        row.Cells[ColumnNames.Icon].Value = My.Resources.Local.resString;
                    }
                }
            }
            else
            {
                row.Cells[ColumnNames.Icon].Value = null;
            }
        }

        #endregion  

        #region " Settings Save and Restore "

        private void Settings_Restore()
        {
            // recent items
            RecentItemsManager.Fill(Properties.Settings.Default.RecentFiles, Properties.Settings.Default.RecentFilesCount);

            BaseCommentToolStripMenuItem.Checked = Properties.Settings.Default.BaseCommentColumnVisible;
            LocalCommentToolStripMenuItem.Checked = Properties.Settings.Default.LocalCommentColumnVisible;
            TreeViewToolStripMenuItem.Checked = Properties.Settings.Default.TreeViewVisible;

            if (Properties.Settings.Default.FontSize > 0)
            {
                CustomDataGrid1.SetGridFont(Properties.Settings.Default.FontSize);
            }

            HideLockedToolStripMenuItem.Checked = Properties.Settings.Default.HideLocked;
            HideVSFormItemsToolStripMenuItem.Checked = Properties.Settings.Default.HideVSFormItems;
            HideEmptyToolStripMenuItem.Checked = Properties.Settings.Default.HideEmpty;
        }

        private void Settings_Save()
        {
            // recent items
            Properties.Settings.Default.RecentFiles = RecentItemsManager.ConvertToXml();
            
            Properties.Settings.Default.BaseCommentColumnVisible = BaseCommentToolStripMenuItem.Checked;
            Properties.Settings.Default.LocalCommentColumnVisible = LocalCommentToolStripMenuItem.Checked;
            Properties.Settings.Default.TreeViewVisible = TreeViewToolStripMenuItem.Checked;
            Properties.Settings.Default.TreeViewVisible = TreeViewToolStripMenuItem.Checked;
            Properties.Settings.Default.FontSize = CustomDataGrid1.MainDataGridView.Font.Size;
            Properties.Settings.Default.HideLocked = HideLockedToolStripMenuItem.Checked;
            Properties.Settings.Default.HideVSFormItems = HideVSFormItemsToolStripMenuItem.Checked;
            Properties.Settings.Default.HideEmpty = HideEmptyToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region " Recent Item List "
        
        private void RecentItemsManager_ItemClicked(object sender, RecentItemClickedEventArgs e)
        {        
            if (!File.Exists(e.RecentItem.Id))
            {
                if (MessageBox.Show(My.Resources.Local.FileRemoveFromRecent, Tools.GetAssemblyProduct(), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    RecentItemsManager.RemoveItem(e.RecentItem);
                }
            }
            else
            {
                this.ResourceBundle_Open(e.RecentItem.Id);
            }
        }

        #endregion

        #region " Plug ins button handling "

        private readonly List<ButtonInfo> plugInsButtons = new List<ButtonInfo>();

        /// <summary>
        /// This is called by <see cref="Context"></see> in order to create buttons when requested by other code
        /// </summary>
        /// <param name="buttonInfo">Information about the button to be created</param>
        public void AddButton(ButtonInfo buttonInfo)
        {
            if (buttonInfo == null)
            {
                throw new ArgumentNullException("buttonInfo");
            }

            // menu item
            ToolStripMenuItem menuItem = new ToolStripMenuItem(null, null, this.ButtonClick);
            menuItem.Tag = buttonInfo;
            menuItem.Name = "MenuButton" + buttonInfo.GetHashCode();
            menuItem.ShortcutKeys = buttonInfo.ShortcutKeys;
            ToolsToolStripMenuItem.DropDownItems.Add(menuItem);

            // toolbar item
            ToolStripButton toolbarItem = new ToolStripButton(null, buttonInfo.Image, this.ButtonClick);
            toolbarItem.Tag = buttonInfo;
            toolbarItem.Name = "ToolbarButton" + buttonInfo.GetHashCode();
            MainToolStrip.Items.Insert(3, toolbarItem);

            UpdateMenuItem(buttonInfo, menuItem, toolbarItem);

            this.plugInsButtons.Add(buttonInfo);
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            // get button information from tag
            var toolItem = (ToolStripItem)sender;
            var buttonInfo = toolItem.Tag as ButtonInfo;

            // if there is button information and click handler behind it, then invoke the handler
            if (buttonInfo != null && buttonInfo.OnClick != null)
            {
                buttonInfo.OnClick.Invoke(sender, e);

                // refresh state to update UI from changes that happened by the invokation
                this.RefreshPlugInButtons();
            }
        }

        private void RefreshPlugInButtons()
        {
            foreach (ButtonInfo buttonInfo in this.plugInsButtons)
            {
                // set button state depending on current context
                var pluginContextString = string.IsNullOrEmpty(buttonInfo.Context) ? PluginContext.Global.ToString() : buttonInfo.Context;
                var pluginContext = (PluginContext)Enum.Parse(typeof(PluginContext), pluginContextString, true);
                switch (pluginContext)
                {
                    case PluginContext.Global:
                        buttonInfo.Disabled = false;
                        break;
                    case PluginContext.ResourceBundle: 
                        buttonInfo.Disabled = this.Context.CurrentResourceBundle == null;
                        break;
                    case PluginContext.ResourceSet: 
                        buttonInfo.Disabled = this.Context.CurrentLocalResourceSet == null;
                        break;
                    case PluginContext.ResourceItem:
                        buttonInfo.Disabled = string.IsNullOrEmpty(this.Context.CurrentResourceItemKey);
                        break;
                    case PluginContext.LocalResourceItem:
                        buttonInfo.Disabled = this.Context.CurrentLocalResourceSet == null && string.IsNullOrEmpty(this.Context.CurrentResourceItemKey);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // invoke OnRefresh of the button and then apply its state to the menu and toolbar
                if (buttonInfo.OnRefresh != null)
                {
                    buttonInfo.OnRefresh.Invoke(this, EventArgs.Empty);
                }

                // find existing menu buttons in UI
                ToolStripItem menuItem = ToolsToolStripMenuItem.DropDownItems["MenuButton" + buttonInfo.GetHashCode()];
                ToolStripItem toolbarItem = MainToolStrip.Items["ToolbarButton" + buttonInfo.GetHashCode()];

                // update them
                UpdateMenuItem(buttonInfo, menuItem, toolbarItem);
            }
        }

        private static void UpdateMenuItem(ButtonInfo buttonInfo, ToolStripItem menuItem, ToolStripItem toolbarItem)
        {
            menuItem.Enabled = !buttonInfo.Disabled;
            menuItem.Text = buttonInfo.Caption;
            menuItem.Image = buttonInfo.Image;
            menuItem.Visible = buttonInfo.Visible;

            toolbarItem.Enabled = !buttonInfo.Disabled;
            toolbarItem.ToolTipText = buttonInfo.ToolTip;
            toolbarItem.Image = buttonInfo.Image;
            toolbarItem.Visible = buttonInfo.ToolBarVisible && buttonInfo.Visible;

            // text is not shown on toolbar button, except if no image is defined
            if (toolbarItem.Image == null)
            {
                toolbarItem.Text = buttonInfo.Caption;
            }
        }

        #endregion

        private void CheckForUpdateTimer_Tick(object sender, EventArgs e)
        {
            // automated check for updates is done once every N days
            if (Properties.Settings.Default.UpdateCheckLast.Subtract(DateTime.Now).TotalDays >= Properties.Settings.Default.UpdateCheckDaysInterval)
            {
                CheckForUpdateTimer.Stop();
                Main.CheckForUpdates(true);
                Properties.Settings.Default.UpdateCheckLast = DateTime.Now;
            }
        }

        #region " Drag & Drop "

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            //allow dropping files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                //get file data
                Array aFiles = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (aFiles != null)
                {
                    //just use the first file
                    String sFile = aFiles.GetValue(0).ToString();
                    // Use BeginInvoke for asynchronous call to prevent Explorer freezing while loading file,
                    // as Explorer will wait for this handler to return.
                    this.BeginInvoke(new OpenFileDelegate(this.ResourceBundle_Open), sFile);
                    // in the case Explorer overlaps this form
                    this.Activate();
                }
            }
            catch (Exception ex)
            {
                Tools.ShowExceptionMessageBox(ex);
            }
        }

        #endregion
    }
}
