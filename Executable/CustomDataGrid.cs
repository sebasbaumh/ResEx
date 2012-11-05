using System;
using System.Drawing;
using System.Windows.Forms;
using ResEx.Common;
using System.Diagnostics;

namespace ResEx
{
    public partial class CustomDataGrid : UserControl
    {
        public delegate void RowValidatingHandler(object sender, DataGridViewCellCancelEventArgs e);
        
        public delegate void CellValueChangedHandler(object sender, DataGridViewCellEventArgs e);
        
        public delegate void ValueFilledHandler(object sender, DataGridViewCellEventArgs e);
        
        public delegate void ValueClearedHandler(object sender, DataGridViewCellEventArgs e);

        public event RowValidatingHandler RowValidating;
        
        public event CellValueChangedHandler CellValueChanged;
        
        public event ValueFilledHandler ValueFilled;
        
        public event ValueClearedHandler ValueCleared;

        public event EventHandler SelectionChanged;

        private bool valueBeforeNullOrEmpty;
        
        private string valueStringBefore;

        public DataGridViewCell CurrentCell 
        {
            get
            {
                return MainDataGridView.CurrentCell;
            }
            set
            {
                MainDataGridView.CurrentCell = value;
            }
        }

        public DataGridViewColumn CurrentColumn
        {
            get
            {
                if (MainDataGridView.CurrentCell == null)
                    return null;
                else
                    return MainDataGridView.Columns[MainDataGridView.CurrentCell.ColumnIndex];
            }
        }

        public DataGridViewRow CurrentRow
        {
            get
            {
                if (MainDataGridView.CurrentCell == null)
                    return null;
                else
                    return MainDataGridView.CurrentRow;
            }
        }

        public CustomDataGrid()
        {
            InitializeComponent();

            MainDataGridView.CellBeginEdit += MainDataGridView_CellBeginEdit;
            MainDataGridView.CellEndEdit += MainDataGridView_CellEndEdit;
            MainDataGridView.RowValidating += MainDataGridView_RowValidating;
            MainDataGridView.CellValueChanged += MainDataGridView_CellValueChanged;
            MainDataGridView.SelectionChanged += this.MainDataGridView_SelectionChanged;
        }

        void MainDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            object value = MainDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            valueBeforeNullOrEmpty = StringTools.ValueNullOrEmpty(value);
            valueStringBefore = string.Empty;
            if (value != null) valueStringBefore = value.ToString();
        }

        void MainDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object newValue = MainDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            bool newValueNullOrEmpty = StringTools.ValueNullOrEmpty(newValue);
            if (valueBeforeNullOrEmpty && !newValueNullOrEmpty && ValueFilled != null)
                ValueFilled(this, e);
            if (!valueBeforeNullOrEmpty && newValueNullOrEmpty && ValueCleared != null)
                ValueCleared(this, e);
        }

        void MainDataGridView_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            object ValueAfter = MainDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            string ValueStringAfter = string.Empty;
            if (ValueAfter != null) ValueStringAfter = ValueAfter.ToString();
            if (string.Compare(ValueStringAfter, valueStringBefore, false) != 0 && RowValidating != null)
                RowValidating(this, e);
        }

        void MainDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            const int gridHeaderRow = -1;
            if (e.RowIndex != gridHeaderRow && grid.Columns[e.ColumnIndex].Name != ColumnNames.Icon && this.CellValueChanged != null)
                this.CellValueChanged(this, e);
        }

        private void MainDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (MainDataGridView.CurrentCell != null && LocalValue.Visible && MainDataGridView.CurrentCell.ColumnIndex != LocalValue.Index)
                MainDataGridView.CurrentCell = MainDataGridView.Rows[MainDataGridView.CurrentCell.RowIndex].Cells[ColumnNames.LocalValue];

            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, e);
            }
        }

        [DebuggerStepThrough]
        private void CustomDataGrid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Gray, 0, 0, this.Width - 1, this.Height - 1);
        }

        private void CustomDataGrid_Resize(object sender, EventArgs e)
        {
            this.AutoSizeContent();
        }

        public DataGridViewRow GetRow(int rowIndex)
        {
            return MainDataGridView.Rows[rowIndex];
        }

        public void AutoSizeContent()
        {
            MainDataGridView.AutoResizeColumn(MainDataGridView.Columns[ColumnNames.Key].Index, DataGridViewAutoSizeColumnMode.AllCells);
            MainDataGridView.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }
        
        public void SetGridFont(Single fontSize)
        {
            if (fontSize > 14 || fontSize < 6) return;
            MainDataGridView.SuspendLayout();
            try
            {         
                Font originalFont = MainDataGridView.Font;
                Font f = new Font(originalFont.FontFamily, fontSize);
                MainDataGridView.Font = f;
                this.AutoSizeContent();
            }
            finally
            {
                MainDataGridView.ResumeLayout();
            }
        }
    }
}
