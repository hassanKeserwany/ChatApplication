namespace API.Entities
{
    public class userLike
    {
        
        public int SourceUserId { get; set; }
        public AppUser SourceUser {  get; set; }
        
        public int LikedUserId { get; set; }
        public AppUser LikedUser { get; set; }



    }
}
