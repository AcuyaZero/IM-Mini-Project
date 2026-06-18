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
            dataGridView1.ClearSelection();
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 48, 119);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.GridColor = Color.Gainsboro;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            //Header

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 48, 119);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 12, FontStyle.Bold);

            dataGridView1.ColumnHeadersHeight = 40;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dataGridView1.AutoSizeColumnsMode =
             DataGridViewAutoSizeColumnsMode.Fill; //Fill

            //Row
            dataGridView1.DefaultCellStyle.Font =
            new Font("Segoe UI", 12);

            dataGridView1.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(230, 235, 255);

            dataGridView1.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dataGridView1.RowTemplate.Height = 35;

        }
    }
}
