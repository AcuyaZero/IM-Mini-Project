using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_Mini_Project
{
    internal class MethodsForAll
    {
        public void DummyDVG(DataGridView dvg, string patientID, string name, string Action) //Loading DummyData
        {
            dvg.Rows.Add($"{patientID}", $"{name}", $"{Action}");
        }

        public void FixDVG(DataGridView dataGridView1) //Fixing DataGridView
        {
            // Clear selection
            dataGridView1.ClearSelection();

            // Header styling
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 48, 119);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // Grid styling
            dataGridView1.GridColor = Color.Gainsboro;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // Selection styling
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;

            // Auto-size columns
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Row styling
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 235, 255);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);
            dataGridView1.RowTemplate.Height = 35;

            // Alternating row colors
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // Allow user to add/delete rows
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;

            // Read-only
            dataGridView1.ReadOnly = true;
        }

        // NEW METHOD: Fix Billing DataGridView specifically
        public void FixBillingDVG(DataGridView dataGridView1)
        {
            // First, style the grid
            FixDVG(dataGridView1);

            // Then set column headers specifically for billing
            if (dataGridView1.Columns.Count >= 6)
            {
                dataGridView1.Columns[0].HeaderText = "Patient";
                dataGridView1.Columns[1].HeaderText = "Date";
                dataGridView1.Columns[2].HeaderText = "Total";
                dataGridView1.Columns[3].HeaderText = "Coverage";
                dataGridView1.Columns[4].HeaderText = "Balance";
                dataGridView1.Columns[5].HeaderText = "Status";

                // Set alignment for numeric columns
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
    }
}