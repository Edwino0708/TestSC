
using Domain;

namespace DataAccessPackage
{
    public interface IAssignmentService
    {
        int CreateAssignment(Assignment assignment);
        List<Assignment> ReadAllAssignments();
        Assignment ReadAssignment(int id);
        void UpdateAssignment(int id, Assignment assignment);
        void DeleteAssignment(int id);
    }
}
