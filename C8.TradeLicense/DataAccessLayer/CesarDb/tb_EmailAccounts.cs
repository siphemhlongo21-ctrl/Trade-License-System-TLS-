namespace C8.TradeLicense.DataAccessLayer.CesarDb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tb_EmailAccounts
    {
        [Key]
        public int EmailAccountId { get; set; }

        public int ApplicationId { get; set; }

        [Required]
        [StringLength(256)]
        public string EmailAccountDescription { get; set; }

        [Required]
        [StringLength(256)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(256)]
        public string ServiceUrl { get; set; }

        public int PasswordExpiredDays { get; set; }

        public DateTime PasswordExpiredDate { get; set; }

        public bool PasswordNeverExpires { get; set; }

        public bool DeleteServerEmail { get; set; }

        public bool CanRecieve { get; set; }

        public bool CanSend { get; set; }

        public bool CheckUndeliverable { get; set; }

        public int AccountFailureCount { get; set; }

        public int MaxFailureCount { get; set; }

        public bool DisableAccount { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }
    }
}
