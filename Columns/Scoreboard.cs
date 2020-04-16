using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Columns.Services;

namespace Columns
{
    [Serializable]
    public class Scoreboard : List<ScoreboardEntry>
    {
        private string _scoreboardFilename;
        private static readonly  DataContractSerializer DataContractSerializer = new DataContractSerializer(typeof(Scoreboard));

        internal new void Add(ScoreboardEntry entry)
        {
            base.Add(entry);
            Sort();
            Reverse();
            if(Count > 10) RemoveAt(10);
        }

        internal static Scoreboard Load(string scoreBoardFile)
        {
            scoreBoardFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + scoreBoardFile;
            Scoreboard scoreboard;
            if (File.Exists(scoreBoardFile))
            {
                string encrypted;
                using (var sw = new StreamReader(scoreBoardFile))
                {
                    encrypted = sw.ReadToEnd();
                }
                using (var reader = new StringReader(Encryption.DecryptRj256(Encryption.Sky, Encryption.Siv, encrypted)))
                { 
                   var xmlReader = new XmlTextReader(reader);
                   scoreboard = (Scoreboard)DataContractSerializer.ReadObject(xmlReader, true);
                   xmlReader.Close(); 
                } 
                scoreboard._scoreboardFilename = scoreBoardFile;
            }
            else
            {
                scoreboard = new Scoreboard {new ScoreboardEntry("David", 25)};
                scoreboard._scoreboardFilename = scoreBoardFile;
                scoreboard.Save();
            }

            return scoreboard;
        }

        internal static Scoreboard LoadFromServer()
        {
            var scoreboard = new Scoreboard();
            
            var client = new scoreswsdlPortTypeClient();
            string scores = client.getscores("normal");
            string[] scoresArray = scores.Split(',');
            
            foreach (string entry in scoresArray)
            {
                string[] entryArray = entry.Split('#');
                scoreboard.Add(new ScoreboardEntry(entryArray[1], Convert.ToInt32(entryArray[0]), entryArray[2]));
            }
            
            return scoreboard;
        }

        internal void ReloadFromServer()
        {
            try
            {
                var scores = LoadFromServer();
                Clear();
                foreach (ScoreboardEntry entry in scores)
                {
                    Add(entry);
                }
            }
            catch(Exception)
            {
                Add(new ScoreboardEntry("Cannot connect", 100));
                Add(new ScoreboardEntry("to server", 90));
                Add(new ScoreboardEntry("to get scores.", 80));
                Add(new ScoreboardEntry("Please check your", 70));
                Add(new ScoreboardEntry("connection", 60));
            }

        }

        internal void SaveToServer()
        {
            try
            {
                var serverScores = LoadFromServer();
                serverScores.Sort();
                serverScores.Reverse();

                var builder = new StringBuilder();
                foreach (ScoreboardEntry entry in this)
                    if (serverScores.IndexOf(entry) < 11 && !serverScores.Contains(entry)) builder.Append(entry.Score + "#" + entry.Name + "#" + entry.Hash + ",");
                var client = new scoreswsdlPortTypeClient();
                client.register(Encryption.EncryptRj256(Encryption.Sky, Encryption.Siv, builder.ToString().TrimEnd(',')));
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch(Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {

            }
        }

        internal void Save()
        {

            using (Stream stream = new MemoryStream())
            {
                string objectstring;
                DataContractSerializer.WriteObject(stream, this);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    objectstring = reader.ReadToEnd();
                }

                using (var sw = new StreamWriter(_scoreboardFilename))
                {
                    sw.Write(Encryption.EncryptRj256(Encryption.Sky, Encryption.Siv, objectstring));
                }
            }
        }

        internal bool IsHighScore(int score)
        {
            return (score > this[Count - 1].Score || Count < 10);
        }
        
    }
}
