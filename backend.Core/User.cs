namespace backend.Core;

public class User
{

    // primary key
    public string userID { get; set; } = string.Empty;

    // basic information
    public string firstname {get; set;} = string.Empty;
    public string lastname {get; set;} = string.Empty;
    public string email {get; set;} = string.Empty;
    public string telephone {get; set;} = string.Empty;
    public string username {get; set;} = string.Empty;

    // authentication
    public string passwordHash { get; set; } = string.Empty;
    public string? refreshToken { get; set; }
    public DateTime? refreshTokenExpiryTime { get; set; }
    public string? FcmToken { get; set; }
    public string? SocketId { get; set; }

    // status & logs
    public bool isBlocked { get; set; } = false;
    public DateTime lastlogin { get; set; }
    public DateTime createdOn { get; set; }
    public DateTime lastUpdated { get; set; }

    // foreign keys
    public string? divisionId { get; set; }

    // navigation properties
    public Division? Division { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}
