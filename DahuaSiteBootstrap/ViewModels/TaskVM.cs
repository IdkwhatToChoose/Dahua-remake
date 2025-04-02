using DahuaSiteBootstrap.Model;

namespace DahuaSiteBootstrap.ViewModels
{
    public class TaskVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Phone { get; set; } = null!;

        public int? AdminId { get; set; }

        public virtual Admin? Admin { get; set; }

        public virtual int tid_to_update { get; set; }


    }
}
