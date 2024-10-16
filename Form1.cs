using BUS;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace bai6_winform_3lop
{
    public partial class Form1 : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           try
            {
                setGridViewStyle(dgvTableSV);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    
        private void FillFalcultyCombobox(List< Faculty > listFacultys)
                {
        listFacultys.Insert(0, new Faculty());
        this.cbbkhoa.DataSource = listFacultys;
        this.cbbkhoa.DisplayMember =  "FacultyName" ;
        this.cbbkhoa.ValueMember =  "FacultyID"  ;
                 }
            //Hàm binding gridView từ list sinh viên
        private void BindGrid(List< Student > listStudent)
            {
        dgvTableSV.Rows.Clear();
        foreach (var item in listStudent)
          {
            int index = dgvTableSV.Rows.Add();
            dgvTableSV.Rows[index].Cells[0].Value = item.StudentID;
            dgvTableSV.Rows[index].Cells[1].Value = item.FullName;
            if (item.Faculty != null)
                dgvTableSV.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
            dgvTableSV.Rows[index].Cells[3].Value = item.AverageScore + "" ;
            if (item.MajorID != null)
                dgvTableSV.Rows[index].Cells[4].Value = item.Major.Name + "" ;
            
             }
          }
            
        public void setGridViewStyle(DataGridView dgview)
            {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
            


            private void chkkiemtra_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List< Student > ();
            if (this.chkkiemtra.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnThemSua_Click(object sender, EventArgs e)
        {
           
                try
                {
                    // Kiểm tra xem dữ liệu đã được nhập đầy đủ chưa
                    if (string.IsNullOrEmpty(txtMSSV.Text) || string.IsNullOrEmpty(txtHoten.Text) || cbbkhoa.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!");
                        return;
                    }

                    // Tạo đối tượng sinh viên từ thông tin người dùng nhập vào
                    var student = new Student
                    {
                        StudentID = txtMSSV.Text, // MSSV là kiểu string
                        FullName = txtHoten.Text,
                        FacultyID = (int)cbbkhoa.SelectedValue,
                        AverageScore = float.Parse(txtDTB.Text)
                    };

                    // Nếu là thêm mới sinh viên (kiểm tra xem MSSV đã tồn tại chưa)
                    if (studentService.GetById(student.StudentID) == null) // Kiểm tra nếu chưa có MSSV trong hệ thống
                    {
                        studentService.InsertUpdate(student);
                        MessageBox.Show("Thêm sinh viên thành công!");
                    }
                    // Nếu là cập nhật thông tin sinh viên
                    else
                    {
                        studentService.InsertUpdate(student);
                        MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
                    }

                    // Sau khi thêm hoặc cập nhật, load lại danh sách sinh viên
                    var listStudents = studentService.GetAll();
                    BindGrid(listStudents);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

        private void btnXoa_Click(object sender, EventArgs e)
        {
                try
                {
                    if (dgvTableSV.SelectedRows.Count > 0)
                    {
                        // Lấy MSSV (StudentID) của sinh viên được chọn
                        string studentID = dgvTableSV.SelectedRows[0].Cells[0].Value.ToString();

                        // Xác nhận việc xóa
                        var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            studentService.Delete(studentID);
                            MessageBox.Show("Xóa sinh viên thành công!");

                            // Load lại danh sách sinh viên
                            var listStudents = studentService.GetAll();
                            BindGrid(listStudents);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn sinh viên để xóa.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }

        private void cc_nhanmanhinh(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại mà người dùng đã nhấn
                DataGridViewRow row = dgvTableSV.Rows[e.RowIndex];

                // Hiển thị dữ liệu lên các control tương ứng
                txtMSSV.Text = row.Cells[0].Value.ToString();   // MSSV
                txtHoten.Text = row.Cells[1].Value.ToString();    // Tên đầy đủ

                // Nếu khoa của sinh viên có dữ liệu
                if (row.Cells[2].Value != null)
                {
                    cbbkhoa.Text = row.Cells[2].Value.ToString();   // Tên khoa
                }
                else
                {
                    cbbkhoa.SelectedIndex = -1; // Nếu không có khoa, bỏ chọn combobox
                }

                // Điểm trung bình
                if (row.Cells[3].Value != null)
                {
                    txtDTB.Text = row.Cells[3].Value.ToString();   // Điểm trung bình
                }
            }
        }
    }
}

