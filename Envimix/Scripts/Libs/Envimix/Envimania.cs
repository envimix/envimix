namespace Envimix.Scripts.Libs.Envimix;

public static class Envimania
{
    public struct SMapInfo
    {
        public string Name;
        public string Uid;
    }

    public struct SRating
    {
        public float Difficulty;
        public float Quality;
    }

    public struct SRatingFilter
    {
        public string Car;
        public int Gravity;
        public string Type;
    }

    public struct SFilteredRating
    {
        public SRatingFilter Filter;
        public SRating Rating;
    }

    public struct SUserInfo
    {
        public string Login;
        public string Nickname;
        public string Zone;
        public string AvatarUrl;
        public string Language;
        public string Description;
        public Vec3 Color;
        public string SteamUserId;
        public int FameStars;
        public float LadderPoints;
    }

    public struct SEnvimaniaRecord
    {
        public SUserInfo User;
        public int Time;
        public int Score;
        public int NbRespawns;
        public float Distance;
        public float Speed;
        public bool Verified;
        public bool Projected;
        public string GhostUrl;
    }

    public struct SRatingServerRequest
    {
        public SUserInfo User;
        public string Car;
        public int Gravity;
        public SRating Rating;
    }

    public struct SStar
    {
        public string Login;
        public string Nickname;
    }
}
