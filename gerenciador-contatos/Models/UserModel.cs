using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace gerenciador_contatos.Models
{
    public class UserModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(120)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(160), EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}