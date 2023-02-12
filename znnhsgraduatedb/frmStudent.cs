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
    public partial class frmStudent : Form {
        string conString;
        string aid;
        bool edit = false;

        public frmStudent() {
            InitializeComponent();
        }

        public frmStudent(string id) {
            conString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + Environment.CurrentDirectory + "/alumni.accdb";
            InitializeComponent();
            aid = id;
            edit = true;
        }

        public string fname, mi, lname, ext, gender;

        private void btnAdd_Click(object sender, EventArgs e) {

            fname = txtFirst.Text.ToUpper();
            mi = cmbMI.Text.ToUpper();
            lname = txtLast.Text.ToUpper();
            ext = txtExt.Text.ToUpper();

            if (rdbMale.Checked) {
                gender = rdbMale.Text.ToUpper();
            } else {
                gender = rdbFemale.Text.ToUpper();
            }

            if (edit) {
                OleDbConnection con = new OleDbConnection(conString);
                OleDbCommand cmd = new OleDbCommand("UPDATE tblStudent SET fname='" + fname + "', mi='" + mi + "', lname='" + lname + "', ext='" + ext + "', gender='" + gender + "' WHERE id=" + aid + ";", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void frmStudent_Load(object sender, EventArgs e) {
            if (edit) {
                OleDbConnection con = new OleDbConnection(conString);
                OleDbCommand cmd;
                OleDbDataAdapter da;
                DataTable dt;

                this.Text = "Edit";
                btnAdd.Text = "EDIT";
                                
                cmd = new OleDbCommand("SELECT * FROM tblStudent WHERE id=" + aid + ";", con);
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0) {
                    txtFirst.Text = dt.Rows[0][1].ToString();
                    cmbMI.Text = dt.Rows[0][2].ToString();
                    txtLast.Text = dt.Rows[0][3].ToString();
                    txtExt.Text = dt.Rows[0][4].ToString();
                    string gender = dt.Rows[0][5].ToString();

                    if (gender == "MALE") {
                        rdbMale.Checked = true;
                        rdbFemale.Checked = false;
                    } else {
                        rdbMale.Checked = false;
                        rdbFemale.Checked = true;
                    }
                }
            }
        }
    }
}