using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyId)
        {
            StudentContextBD context = new StudentContextBD();
            return context.Major.Where(p=>p.FacultyID==facultyId).ToList();
        }
    }
}
