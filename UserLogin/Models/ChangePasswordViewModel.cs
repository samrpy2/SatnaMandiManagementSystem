using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "पुराना पासवर्ड डालना अनिवार्य है")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "नया पासवर्ड डालना अनिवार्य है")]
        [StringLength(100, ErrorMessage = "{0} कम से कम {2} अक्षर का होना चाहिए।", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "नया पासवर्ड और कन्फर्म पासवर्ड मैच नहीं हो रहे हैं")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
