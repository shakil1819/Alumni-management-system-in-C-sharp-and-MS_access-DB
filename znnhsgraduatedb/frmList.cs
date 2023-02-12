using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace znnhsalumnidb {
    public partial class frmList : Form {
        string conString;

        public frmList() {
            InitializeComponent();
            conString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + Environment.CurrentDirectory + "/alumni.accdb";
        }

        private void btnViewClass_Click(object sender, EventArgs e) {
            frmClass fn = new frmClass();
            fn.ShowDialog();
        }

        private void searchStudents() {
            string stu = txtStudent.Text.ToUpper();

            lvStudents.Items.Clear();

            if (stu == "") {
                txtResults.Text = "Search field required!";
                return;
            }

            OleDbCommand cmd;
            OleDbDataReader reader;

            string id, name, gender, cur, sy, lev, adv, sec;

            try {
                using (OleDbConnection cn = new OleDbConnection(conString)) {
                    string sql = "";
                    string order = " ORDER BY schoolyear, gender DESC, lname, fname, mi;";
                    if (stu.Length == 0) {
                        sql = "SELECT TOP 50 * FROM tblStudent" + order;
                    } else {
                        sql = "SELECT * FROM tblStudent WHERE (fname LIKE '%" + stu + "%' OR lname LIKE '%" + stu + "%' OR ext LIKE '%" + stu + "%')" + order;
                    }
                    
                    cmd = new OleDbCommand(sql, cn);
                    cn.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            id = reader["id"].ToString();
                            string lname = reader["lname"].ToString();
                            string ext = reader["ext"].ToString();
                            if (ext.Length > 0) {
                                ext = " " + ext;
                            } else {
                                ext = "";
                            }
                            string fname = ", " + reader["fname"].ToString();
                            string mname = reader["mi"].ToString();
                            if (mname.Length > 0) {
                                mname = " " + mname[0] + ".";
                            } else {
                                mname = " ";
                            }
                            name = lname + ext + fname + mname;
                            gender = reader["gender"].ToString();
                            cur = reader["curriculum"].ToString();
                            sy = reader["schoolyear"].ToString();
                            lev = reader["slevel"].ToString();
                            adv = reader["adviser"].ToString();
                            sec = reader["ssection"].ToString();
                            string[] row = { id, name, gender, sy, sec, adv, cur, lev };
                            ListViewItem item = new ListViewItem(row);
                            lvStudents.Items.Add(item);
                        }
                    }
                    cn.Close();
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtResults.Text = "Result(s): " + lvStudents.Items.Count;
        }

        private void btnSearch_Click(object sender, EventArgs e) {
            searchStudents();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e) {
            searchStudents();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                string eid = lvStudents.FocusedItem.SubItems[0].Text;

                frmStudent fs = new frmStudent(eid);
                if (fs.ShowDialog() == DialogResult.OK) {
                    searchStudents();
                }
            } catch { }
        }

        private void viewStudentClassToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                string eid = lvStudents.FocusedItem.SubItems[0].Text;

                frmClass fs = new frmClass(eid);
                if (fs.ShowDialog() == DialogResult.OK) {
                    searchStudents();
                }
            } catch { }
        }

        private void lvStudents_SelectedIndexChanged(object sender, EventArgs e) {

        }
    }
}