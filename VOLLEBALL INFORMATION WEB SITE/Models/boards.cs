using System.ComponentModel.DataAnnotations;

namespace Mini_Project2.Models
{
    public class boards
    {
        [Key]
        public int No { get; set; }
        [Required(ErrorMessage = "이름을 작성해 주세요.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "제목을 작성해 주세요.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "내용을 작성해 주세요.")]
        public string Detail { get; set; }
        [Required(ErrorMessage = "비밀번호를 작성해 주세요.")]
        public int? Password { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Team { get; set; }

    }
}
