using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Server
{
    public class MySQL
    {
        private MySqlConnection sqlConn;
        private string connStr;
        private bool isConnected;
        private int player_mySQL_size = 48;

        public MySQL(string connStr)
        {
            this.connStr = connStr;

            try
            {
                sqlConn = new MySqlConnection(this.connStr);
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Error connecting you to " +
                    "the my sql server. Internal error message: " + excp.Message, excp);
                throw myExcp;
            }

            this.isConnected = false;
        }

        public void Connect()
        {
            bool success = true;

            if (this.isConnected == false)
            {
                try
                {
                    this.sqlConn.Open();
                }
                catch (Exception excp)
                {
                    this.isConnected = false;
                    success = false;
                    Exception myException = new Exception("Error opening connection" +
                        " to the sql server. Error: " + excp.Message, excp);

                    throw myException;
                }

                if (success)
                {
                    this.isConnected = true;
                }
            }
        }

        public void Disconnect()
        {
            if (this.isConnected)
            {
                this.sqlConn.Close();
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }
        public bool CreateAccount(string acc_name, string acc_pass)
        {
            string Query = "INSERT INTO `accounts`(`name`, `password`) VALUES ('" + acc_name + "','" + acc_pass + "')";
            MySqlCommand addUser = new MySqlCommand(Query, this.sqlConn);

            try
            {
                addUser.ExecuteNonQuery();
                return true;
            }
            catch (Exception excp)
            {
                return false;
            }

        }

        public bool CreateCharacter(int acc_id, string name, int gender)
        {
            string Query = "INSERT INTO `players`(`name`, `account_id`, `sex`) VALUES ('" + name + "','" + acc_id + "', '" + gender + "')";
            MySqlCommand addCharacter = new MySqlCommand(Query, this.sqlConn);

            try
            {
                addCharacter.ExecuteNonQuery();
                return true;
            }
            catch (Exception excp)
            {
                return false;
            }

        }

        public bool UserExists(string username)
        {
            int returnValue = 0;

            string Query = "SELECT COUNT(*) FROM accounts where (name=" +
                "'" + username + "') LIMIT 1";

            MySqlCommand verifyUser = new MySqlCommand(Query, this.sqlConn);

            try
            {
                verifyUser.ExecuteNonQuery();

                MySqlDataReader myReader = verifyUser.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            if (returnValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool PlayerExist(string charname)
        {
            int returnValue = 0;

            string Query = "SELECT COUNT(*) FROM players where (name=" +
                "'" + charname + "') LIMIT 1";

            MySqlCommand verifyPlayer = new MySqlCommand(Query, this.sqlConn);

            try
            {
                verifyPlayer.ExecuteNonQuery();

                MySqlDataReader myReader = verifyPlayer.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            if (returnValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool VerifyUser(string username, string password)
        {
            int returnValue = 0;

            string Query = "SELECT COUNT(*) FROM accounts where (name=" +
                "'" + username + "' and password='" + password + "') LIMIT 1";

            MySqlCommand verifyUser = new MySqlCommand(Query, this.sqlConn);

            try
            {
                verifyUser.ExecuteNonQuery();

                MySqlDataReader myReader = verifyUser.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            if (returnValue == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int FindIdOfAccountName(string username)
        {
            int returnValue = 0;

            string Query = "SELECT `id` FROM accounts where (name=" +
                "'" + username + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public int FindTotalCharacterOfAccountId(int id)
        {
            int total_characters = 0;

            string Query = "SELECT COUNT(*) FROM players where (account_id=" +
            "'" + id + "')";

            MySqlCommand findCharactersCount = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findCharactersCount.ExecuteNonQuery();

                MySqlDataReader myReader = findCharactersCount.ExecuteReader();

                while (myReader.Read() != false)
                {
                    total_characters = myReader.GetInt32(0);
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            return total_characters;
        }

        public string[] FindCharacterList(int id, int total_characters)
        {
            string Query = "SELECT `name` FROM players where (account_id=" +
            "'" + id + "')";

            string[] character_names = new string[total_characters];

            MySqlCommand findCharacters = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findCharacters.ExecuteNonQuery();

                MySqlDataReader myReader = findCharacters.ExecuteReader();

                int i = 0;
                while (myReader.Read() != false)
                {
                    character_names[i] = myReader.GetString(0);
                    i++;
                }

                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            return character_names;
        }

        public int FindIdOfCharacter(string charname)
        {
            int returnValue = 0;

            string Query = "SELECT `id` FROM players where (name=" +
                "'" + charname + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public int FindAccountIdOfCharacter(string charname)
        {
            int returnValue = 0;

            string Query = "SELECT `account_id` FROM players where (name=" +
                "'" + charname + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public int FindAccountIdOfCharacterId(int char_id)
        {
            int returnValue = 0;

            string Query = "SELECT `account_id` FROM players where (id=" +
                "'" + char_id + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetInt32(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public string FindAccountNameOfId(int id)
        {
            string returnValue = "";

            string Query = "SELECT `name` FROM accounts where (id=" +
                "'" + id + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetString(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public string FindAccountPasswordOfId(int id)
        {
            string returnValue = "";

            string Query = "SELECT `password` FROM accounts where (id=" +
                "'" + id + "') LIMIT 1";

            MySqlCommand findId = new MySqlCommand(Query, this.sqlConn);

            try
            {
                findId.ExecuteNonQuery();

                MySqlDataReader myReader = findId.ExecuteReader();

                while (myReader.Read() != false)
                {
                    returnValue = myReader.GetString(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public string[] LoadCharacter(int id)
        {
            string[] returnValue = new string[player_mySQL_size];

            string Query = "SELECT * FROM players WHERE id = '"+id+"'";

            MySqlCommand loadCharacter = new MySqlCommand(Query, this.sqlConn);

            try
            {
                loadCharacter.ExecuteNonQuery();

                MySqlDataReader myReader = loadCharacter.ExecuteReader();

                while (myReader.Read() != false)
                {
                    for (int i = 0; i < returnValue.Length; i++)
                    {
                        returnValue[i] = myReader.GetString(i);
                    }
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
            return returnValue;
        }

        public Player[] LoadPlayers()
        {
            string[] playerValues;
            int player_count = 0;
            string Query = "SELECT * FROM `players`";

            MySqlCommand loadPlayers = new MySqlCommand(Query, this.sqlConn);

            try
            {
                loadPlayers.ExecuteNonQuery();

                MySqlDataReader myReader = loadPlayers.ExecuteReader();

                while (myReader.Read() != false)
                {
                    player_count = myReader.GetInt32(0);
                }
                myReader.Close();
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not verify user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }

            Player[] players = new Player[player_count];

            for (int i = 0; i < players.Length; i++)
            {
                playerValues = LoadCharacter(i+1);
                string acc_name = FindAccountNameOfId(Int32.Parse(playerValues[3]));
                string acc_pass = FindAccountPasswordOfId(Int32.Parse(playerValues[3]));
                bool online;
                if (playerValues[29] == "True")
                    online = true;
                else
                    online = false;
                players[i] = new Player(acc_name, acc_pass, Int32.Parse(playerValues[0]), playerValues[1], Int32.Parse(playerValues[2]), Int32.Parse(playerValues[3]), Int32.Parse(playerValues[4]), Int32.Parse(playerValues[5]), Int32.Parse(playerValues[6]), Int32.Parse(playerValues[7]), Int64.Parse(playerValues[8]), Int32.Parse(playerValues[9]), Int32.Parse(playerValues[10]),
                    Int32.Parse(playerValues[11]), Int32.Parse(playerValues[12]), Int32.Parse(playerValues[13]), Int32.Parse(playerValues[14]), Int32.Parse(playerValues[15]), Int32.Parse(playerValues[16]), Int32.Parse(playerValues[17]), Int64.Parse(playerValues[18]), Int32.Parse(playerValues[19]), Int32.Parse(playerValues[20]),
                    Int32.Parse(playerValues[21]), Int32.Parse(playerValues[22]), playerValues[23], Int32.Parse(playerValues[24]), Int32.Parse(playerValues[25]), Int32.Parse(playerValues[26]), Int32.Parse(playerValues[27]), Int64.Parse(playerValues[28]), online, Int32.Parse(playerValues[30]),
                    Int32.Parse(playerValues[31]), Int32.Parse(playerValues[32]), Int32.Parse(playerValues[33]), Int32.Parse(playerValues[34]), Int32.Parse(playerValues[35]), Int32.Parse(playerValues[36]), Int64.Parse(playerValues[37]), Int32.Parse(playerValues[38]), Int64.Parse(playerValues[39]), Int32.Parse(playerValues[40]),
                    Int64.Parse(playerValues[41]), Int32.Parse(playerValues[42]), Int64.Parse(playerValues[43]), Int32.Parse(playerValues[44]), Int64.Parse(playerValues[45]), Int32.Parse(playerValues[46]), Int64.Parse(playerValues[47]));
            }

            return players;
        }

        public Player NewCharacterValues(string[] playerValues)
        {
            Player player;
            string acc_name = FindAccountNameOfId(Int32.Parse(playerValues[3]));
            string acc_pass = FindAccountPasswordOfId(Int32.Parse(playerValues[3]));
            bool online;
            if (playerValues[29] == "True")
                online = true;
            else
                online = false;
            player = new Player(acc_name, acc_pass, Int32.Parse(playerValues[0]), playerValues[1], Int32.Parse(playerValues[2]), Int32.Parse(playerValues[3]), Int32.Parse(playerValues[4]), Int32.Parse(playerValues[5]), Int32.Parse(playerValues[6]), Int32.Parse(playerValues[7]), Int64.Parse(playerValues[8]), Int32.Parse(playerValues[9]), Int32.Parse(playerValues[10]),
                Int32.Parse(playerValues[11]), Int32.Parse(playerValues[12]), Int32.Parse(playerValues[13]), Int32.Parse(playerValues[14]), Int32.Parse(playerValues[15]), Int32.Parse(playerValues[16]), Int32.Parse(playerValues[17]), Int64.Parse(playerValues[18]), Int32.Parse(playerValues[19]), Int32.Parse(playerValues[20]),
                Int32.Parse(playerValues[21]), Int32.Parse(playerValues[22]), playerValues[23], Int32.Parse(playerValues[24]), Int32.Parse(playerValues[25]), Int32.Parse(playerValues[26]), Int32.Parse(playerValues[27]), Int64.Parse(playerValues[28]), online, Int32.Parse(playerValues[30]),
                Int32.Parse(playerValues[31]), Int32.Parse(playerValues[32]), Int32.Parse(playerValues[33]), Int32.Parse(playerValues[34]), Int32.Parse(playerValues[35]), Int32.Parse(playerValues[36]), Int64.Parse(playerValues[37]), Int32.Parse(playerValues[38]), Int64.Parse(playerValues[39]), Int32.Parse(playerValues[40]),
                Int64.Parse(playerValues[41]), Int32.Parse(playerValues[42]), Int64.Parse(playerValues[43]), Int32.Parse(playerValues[44]), Int64.Parse(playerValues[45]), Int32.Parse(playerValues[46]), Int64.Parse(playerValues[47]));
            return player;
        }
    }
}
