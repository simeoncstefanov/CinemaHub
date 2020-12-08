namespace CinemaHub.Web.Filters.Action.ModelStateTransfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ModelStateDto
    {
        public string Key { get; set; }

        public string AttemptedValue { get; set; }

        public object RawValue { get; set; }

        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
    }
}
