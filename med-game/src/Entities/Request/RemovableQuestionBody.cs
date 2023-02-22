namespace med_game.src.Entities.Request
{
    public class RemovableQuestionBody
    {
        public string LecternName { get; set; }
        public string ModuleName { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public TypeQuestion TypeQuestion { get; set; }
    }
}
