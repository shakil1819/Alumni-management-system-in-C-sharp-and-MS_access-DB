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
    public partial class frmClass : Form {
        string conString;
        string aid;
        bool edit = false;
        bool add = false;

        private void resourceLoad() {
            conString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + Environment.CurrentDirectory + "/alumni.accdb";
            showAll(cmbAdviser, "Adviser");
            showAll(cmbCurriculum, "Curriculum");
            showAll(cmbLevel, "SLevel");
            showAll(cmbSchoolYear, "SchoolYear");
            showAll(cmbSection, "SSection");
        }

        public frmClass() {
            InitializeComponent();
            resourceLoad();
        }

        public frmClass(string id) {
            InitializeComponent();
            resourceLoad();
            aid = id;
            edit = true;
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void showAll(ComboBox cmb, string col) {
            cmb.Items.Clear();

            OleDbCommand cmd;
            OleDbDataReader reader;

            try {
                using (OleDbConnection cn = new OleDbConnection(conString)) {
                    cmd = new OleDbCommand("SELECT DISTINCT " + col + " FROM tblstudent ORDER BY " + col + ";", cn);
                    cn.Open();
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        while (reader.Read()) {
                            string item = reader[col].ToString();

                            cmb.Items.Add(item);
                        }
                    }
                    cn.Close();
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            string fname, mi, lname, ext, gender, cur, sy, lev, sec, adv;
            cur = cmbCurriculum.Text.ToUpper();
            sy = cmbSchoolYear.Text.ToUpper();
            lev = cmbLevel.Text.ToUpper();
            sec = cmbSection.Text.ToUpper();
            adv = cmbAdviser.Text.ToUpper();

            if ((cur.Length != 0) || (sy.Length != 0) || (lev.Length != 0) || (adv.Length != 0)) {
                frmStudent fs = new frmStudent();
                if (fs.ShowDialog() == DialogResult.OK) {
                    fname = fs.fname;
                    mi = fs.mi;
                    lname = fs.lname;
                    ext = fs.ext;

                    gender = fs.gender;

                    try {
                        OleDbConnection con = new OleDbConnection(conString);

                        OleDbCommand cmd = new OleDbCommand("INSERT INTO tblStudent(fname, mi, lname, ext, gender, curriculum, schoolyear, slevel, ssection, adviser) VALUES ('" + fname + "', '" + mi + "', '" + lname + "', '" + ext + "', '" + gender + "', '" + cur + "', '" + sy + "', '" + lev + "', '" + sec + "', '" + adv + "');", con);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        add = true;
                        loadStudents();
                    } catch (Exception ex) {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            } else {
                MessageBox.Show("All fields are required!", "Prompt", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            loadStudents();
        }

        private void loadStudents() {
            lvStudents.Items.Clear();

            OleDbCommand cmd;
            OleDbDataReader reader;

            string name, gender, cur, sy, lev, adv, sec;
            cur = cmbCurriculum.Text.ToUpper();
            sy = cmbSchoolYear.Text.ToUpper();
            lev = cmbLevel.Text.ToUpper();
            adv = cmbAdviser.Text.ToUpper();
            sec = cmbSection.Text.ToUpper();

            try {
                using (OleDbConnection cn = new OleDbConnection(conString)) {
                    string sort = "";
                    if (add) {
                        sort = " ORDER BY id DESC";
                    }
                    string sql = "SELECT * FROM tblStudent WHERE (curriculum='" + cur + "' AND schoolyear='" + sy + "' AND slevel='" + lev + "' AND ssection='" + sec + "' AND  adviser='" + adv + "')" + sort + ";";

                    cmd = new OleDbCommand(sql, cn);
                    cn.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            string id = reader["ID"].ToString();
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

                            string[] row = { id, name, gender };
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

        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                string eid = lvStudents.FocusedItem.SubItems[0].Text;

                frmStudent fs = new frmStudent(eid);
                if (fs.ShowDialog() == DialogResult.OK) {
                    loadStudents();
                }
            } catch {}
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e) {
            loadStudents();
            showAll(cmbAdviser, "Adviser");
            showAll(cmbCurriculum, "Curriculum");
            showAll(cmbLevel, "SLevel");
            showAll(cmbSchoolYear, "SchoolYear");
            showAll(cmbSection, "SSection");
        }

        private void frmClass_Load(object sender, EventArgs e) {
            if (edit) {
                OleDbConnection con = new OleDbConnection(conString);
                OleDbCommand cmd;
                OleDbDataAdapter da;
                DataTable dt;

                cmd = new OleDbCommand("SELECT * FROM tblStudent WHERE id=" + aid + ";", con);
                da = new OleDbDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0) {
                    cmbCurriculum.Text = dt.Rows[0][6].ToString();
                    cmbSchoolYear.Text = dt.Rows[0][7].ToString();
                    cmbLevel.Text = dt.Rows[0][8].ToString();
                    cmbSection.Text = dt.Rows[0][9].ToString();
                    cmbAdviser.Text = dt.Rows[0][10].ToString();
                    loadStudents();
                }
            }
        }
    }
}