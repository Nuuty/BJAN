namespace WCFServiceWebRole1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Status
    {
        public int Id { get; set; }

        public DateTime InsertedDatetime { get; set; }

        [Column("Status")]
        public bool State { get; set; }

        public int ToiletId { get; set; }
    }
}
