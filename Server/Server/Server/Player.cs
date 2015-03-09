using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Player
    {
        

        public int id;
        public string name;
        public int group_id;
        public int acc_id;
        public int level;
        public int vocation;
        public int health;
        public int health_max;
        public long experience;

        //Outfit related
        public int lookbody;
        public int lookfeet;
        public int lookhead;
        public int looklegs;
        public int looktype;
        public int lookaddons;

        public int maglevel;
        public int mana;
        public int mana_max;
        public long mana_spent;

        public int town_id;
        public Vector3 position;

        public string condition;
        public int cap;
        public int sex;

        //pvp
        public int skull;
        public int skull_time;

        public long balance;

        public bool online;
        public int promotion;

        //Blessings
        public int blessing1;
        public int blessing2;
        public int blessing3;
        public int blessing4;
        public int blessing5;

        //skills
        public int axe;
        public long axe_progress;
        public int sword;
        public long sword_progress;
        public int club;
        public long club_progress;
        public int fishing;
        public long fishing_progress;
        public int mining;
        public long mining_progress;
        public int smithing;
        public long smithing_progress;

        public string acc_name;
        public string acc_pass;

        public Player(string acc_name, string acc_pass, int id, string name, int group_id, int acc_id, int level, int vocation, int health, int health_max, long experience, 
            int lookbody, int lookfeet, int lookhead, int looklegs, int looktype, int lookaddons,
            int maglevel, int mana, int mana_max, long mana_spent, 
            int town_id, int posX, int posY, int posZ,
            string condition, int cap, int sex,
            int skull, int skull_time, 
            long balance, bool online, int promotion,
            int blessing1, int blessing2, int blessing3, int blessing4, int blessing5,
            int axe, long axe_progress, int sword, long sword_progress, int club, long club_progress, int fishing, long fishing_progress, int mining, long mining_progress, int smithing, long smithing_progress
            )
        {
            this.id = id;
            this.group_id = group_id;
            this.acc_id = acc_id;
            this.name = name;
            this.level = level;
            this.vocation = vocation;
            this.health = health;
            this.health_max = health_max;
            this.experience = experience;

            //Outfit
            this.lookbody = lookbody;
            this.lookfeet = lookfeet;
            this.lookhead = lookhead;
            this.looktype = looktype;
            this.lookaddons = lookaddons;

            this.maglevel = maglevel;
            this.mana = mana;
            this.mana_max = mana_max;
            this.mana_spent = mana_spent;

            this.town_id = town_id;
            position = new Vector3(posX, posY, posZ);

            this.condition = condition;
            this.cap = cap;
            this.sex = sex;

            this.skull = skull;
            this.skull_time = skull_time;

            this.balance = balance;
            this.online = online;
            this.promotion = promotion;

            this.blessing1 = blessing1;
            this.blessing2 = blessing2;
            this.blessing3 = blessing3;
            this.blessing4 = blessing4;
            this.blessing5 = blessing5;

            this.axe = axe;
            this.axe_progress = axe_progress;
            this.sword = sword;
            this.sword_progress = sword;
            this.club = club;
            this.club_progress = club_progress;
            this.fishing = fishing;
            this.fishing_progress = fishing_progress;
            this.mining = mining;
            this.mining_progress = mining_progress;
            this.smithing = smithing;
            this.smithing_progress = smithing_progress;

            this.acc_name = acc_name;
            this.acc_pass = acc_pass;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
