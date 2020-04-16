using System;

namespace Columns
{
    [Serializable]
    public class ScoreboardEntry : IComparable, IComparable<ScoreboardEntry>
    {
        public string Name { get; private set; }
        public int Score { get; private set;}
        public string Hash {get; private set;}

        public ScoreboardEntry(string name, int score)
        {
            Name = name;
            Score = score;
            Hash = Guid.NewGuid().ToString();
        }

        public ScoreboardEntry(string name, int score, string hash)
        {
            Name = name;
            Score = score;
            Hash = hash;
        }

        public int CompareTo(object obj)
        {
            var entry = (ScoreboardEntry) obj;
            return Score.CompareTo(entry.Score);
        }

        public int CompareTo(ScoreboardEntry other)
        {
            return Score.CompareTo(other.Score);
        }


        public bool Equals(ScoreboardEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Score == Score && Equals(other.Hash, Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ScoreboardEntry)) return false;
            return Equals((ScoreboardEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Score;
                result = (result*397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                return result;
            }
        }
    }
}
