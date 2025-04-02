using DahuaSiteBootstrap.Model;

namespace DahuaSiteBootstrap.ViewModels
{
    public class OwnerData
    {
        public ICollection<Dsfile> files { get; set; }

        public void SortFilesByCategory(string category)
        {
            files = files.Where(f => f.Category == category).ToList();
        }

        public void SortFilesBySearch(string searchString) 
        {
            files = files.Where(s => s.DisplayName!.ToUpper().Contains(searchString.ToUpper())).ToList();
        }
    }
}