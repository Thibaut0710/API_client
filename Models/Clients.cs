using System.ComponentModel.DataAnnotations;

namespace API_Client.Models
{
    public class Clients
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le numéro de téléphone est obligatoire.")]
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "La liste des commandes est obligatoire.")]
        [MinLength(1, ErrorMessage = "Au moins une commande doit être associée.")]
        public List<int> CommandeIds { get; set; }
    }
    public class ClientIdModel
    {
        [Required(ErrorMessage = "L'Id du client est obligatoire.")]
        [Range(1, int.MaxValue, ErrorMessage = "L'Id doit être supérieur à 0.")]
        public int Id { get; set; }

        public ClientIdModel(int id)
        {
            Id = id;
        }
    }
}
