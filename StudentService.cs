using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace BUS
{
    public class StudentService
    {
        // Tạo một context chung để tránh việc khởi tạo nhiều lần
        private readonly StudentContextBD context;

        public StudentService()
        {
            context = new StudentContextBD();
        }

        public List<Student> GetAll()
        {
            return context.Student.ToList(); // Chỉnh sửa thành Students nếu tên DbSet là Students
        }

        public List<Student> GetAllHasNoMajor()
        {
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID) // Thay đổi int thành string
        {
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(string studentId)
        {
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }

        public void InsertUpdate(Student s)
        {
            context.Student.AddOrUpdate(s); // Chỉnh sửa thành Students nếu tên DbSet là Students
            context.SaveChanges();
        }

        public void Delete(string studentID)
        {
            // Tìm sinh viên theo StudentID
            var student = context.Student.Find(studentID); // Chỉnh sửa thành Students nếu tên DbSet là Students
            if (student != null)
            {
                context.Student.Remove(student); // Xóa sinh viên
                context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            else
            {
                throw new Exception("Sinh viên không tồn tại."); // Thông báo lỗi nếu không tìm thấy sinh viên
            }
        }

        public Student GetById(string studentID)
        {
            // Tìm sinh viên theo StudentID
            var student = context.Student.Include(s => s.Faculty) // Bao gồm thông tin khoa nếu cần
                                    .FirstOrDefault(s => s.StudentID.Equals(studentID));
            return student; // Trả về sinh viên tìm thấy (hoặc null nếu không tìm thấy)
        }

        // Giải phóng tài nguyên
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
