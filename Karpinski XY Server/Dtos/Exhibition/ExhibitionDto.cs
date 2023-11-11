namespace Karpinski_XY_Server.Dtos.Exhibition
{
    public class ExhibitionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Organizer { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public List<ExhibitionImageDto> ExhibitionImages { get; set; } = new List<ExhibitionImageDto>();
    }
}
