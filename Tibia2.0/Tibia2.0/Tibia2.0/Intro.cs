using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tibia2._0
{
    public class Intro
    {

        Texture2D intro_image, input_texture;
        SpriteFont font;
        int text_size = 14;
        public int gender = 0; //0 = male, 1 = female.
        public List<string> characters = new List<string>();
        public Rectangle[] character_boxes;
        Rectangle window_bounds;
        string create = "Create!", login = "Login!", create_account_string = "Create Account", back_string = "back", new_character_string = "New character";
        string male_string = "male", female_string = "female";
        string title_text = "Mingy's Realm"; //Will change!
        string acc_name = "Account Name:", character_name = "Characters name:";
        string acc_pass_1 = "Password:", acc_pass_2 = "Re-enter the password:";
        public string entered_pass_1 = "", entered_pass_2 = "", hidden_pass_1 = "", hidden_pass_2 = "";
        public string entered_name = "", entered_charactername = "";
        string message = "";

        public bool character_succes = false;
        public bool create_succes = false;
        public bool login_succes = false;
        public bool login_character_succes = false;

        public int create_account = 0;
        public int login_account = 0;
        public int create_character = 0;
        public int login_character = 0;

        int start_y = 25, increment_y = 50;

        Rectangle name_rec, pass_rec_1, pass_rec_2;
        Rectangle button_1, button_2, gender_rec;
        enum IntroState
        {
            login,
            create,
            charactermenu,
            charactercreation
        }
        IntroState introState;

        int selected = 0;
        //0 = nothing is selected.
        //1 = name is selected.
        //2 = first pw is selected.
        //3 = second pw is selected.
        //4 = email is selected.
        
        public Intro()
        {
        }

        public void LoadContent(ContentManager Content)
        {
            intro_image = Content.Load<Texture2D>("intro");
            input_texture = Content.Load<Texture2D>("solidblack");
            font = Content.Load<SpriteFont>(@"SpriteFont1");
            introState = IntroState.login;
        }

        public void Update(GameTime gameTime, Rectangle window_bounds, MouseState ms, MouseState oms, KeyboardState ks, KeyboardState oks)
        {
            name_rec = new Rectangle(window_bounds.Width / 2 - 100, start_y + increment_y * 2, 200, 30);
            pass_rec_1 = new Rectangle(window_bounds.Width / 2 - 100, start_y + increment_y * 4, 200, 30);
            gender_rec = pass_rec_1;
            pass_rec_2 = new Rectangle(window_bounds.Width / 2 - 100, start_y + increment_y * 6, 200, 30);
            button_1 = new Rectangle(window_bounds.Width / 2 - 220, start_y + increment_y * 8, 200, 30);
            button_2 = new Rectangle(window_bounds.Width / 2 + 20, start_y + increment_y * 8, 200, 30);
            this.window_bounds = window_bounds;
            hidden_pass_1 = "";
            hidden_pass_2 = "";
            for (int i = 0; i < entered_pass_1.Length; i++)
            {
                hidden_pass_1 += "*";
            }
            for (int i = 0; i < entered_pass_2.Length; i++)
            {
                hidden_pass_2 += "*";
            } 
            switch (introState)
            {
                case IntroState.login:
                    if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released)
                    {
                        if (name_rec.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 1;
                        }
                        else if (pass_rec_1.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 2;
                        } 
                        else 
                        {
                            selected = 0;
                        }
                        if (button_1.Contains(new Point(ms.X, ms.Y)) && login_account == 0)
                        {
                            login_account = 1;
                        }
                        else if (button_2.Contains(new Point(ms.X, ms.Y)))
                        {
                            introState = IntroState.create;
                        }
                    }
                    if (ks.IsKeyDown(Keys.Tab) && oks.IsKeyUp(Keys.Tab))
                    {
                        selected++;
                        if (selected > 2)
                            selected = 1;
                    }

                    if (login_account == 3 && login_succes == false)
                    {
                        message = "Failed!";
                        login_account = 0;
                        login_succes = false;
                    }
                    else if (login_account == 3 && login_succes == true)
                    {
                        message = "Succes!";
                        introState = IntroState.charactermenu;
                        login_account = 0;
                        login_succes = false;
                    }

                    if (selected == 1)
                        entered_name = NameHandler(ks, oks, entered_name);
                    else if (selected == 2)
                        entered_pass_1 = NameHandler(ks, oks, entered_pass_1);
                    break;
                case IntroState.create:
                    if (ks.IsKeyDown(Keys.Tab) && oks.IsKeyUp(Keys.Tab))
                    {
                        selected++;
                        if (selected > 3)
                            selected = 1;
                    }
                    if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released)
                    {
                        if (name_rec.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 1;
                        }
                        else if (pass_rec_1.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 2;
                        }
                        else if (pass_rec_2.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 3;
                        }
                        else
                        {
                            selected = 0;
                        }

                        if (button_1.Contains(new Point(ms.X, ms.Y)) && create_account == 0)
                        {
                            if (entered_pass_1 != entered_pass_2)
                            {
                                message = "Passwords don't match!";
                            }
                            else if (entered_pass_1.Length < 5)
                            {
                                message = "Password is too short!";
                            }
                            else if (entered_name.Length < 5)
                            {
                                message = "Account name is too short!";
                            }
                            else
                            {
                                create_account = 1;
                            }
                        }
                        else if (button_2.Contains(new Point(ms.X, ms.Y)))
                        {
                            introState = IntroState.login;
                            entered_name = "";
                            entered_pass_1 = "";
                            entered_pass_2 = "";
                        }
                    }
                    if (create_account == 3 && create_succes == false)
                    {
                        message = "Failed! Try new Account name!";
                        create_account = 0;
                        create_succes = false;
                    }
                    else if (create_account == 3 && create_succes == true)
                    {
                        message = "Succes!";
                        introState = IntroState.login;
                        create_account = 0;
                        create_succes = false;
                    }
                    if (selected == 1)
                        entered_name = NameHandler(ks, oks, entered_name);
                    else if (selected == 2)
                        entered_pass_1 = NameHandler(ks, oks, entered_pass_1);
                    else if (selected == 3)
                        entered_pass_2 = NameHandler(ks, oks, entered_pass_2);
                    break;
                case IntroState.charactermenu:
                    character_boxes = new Rectangle[characters.Count];
                    for (int i = 0; i < character_boxes.Length; i++)
                    {
                        character_boxes[i] = new Rectangle(window_bounds.Width / 2 - 100, start_y + increment_y * (i+1), 200, 30);
                    }
                    if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released)
                    {
                        for (int i = 0; i < character_boxes.Length; i++)
                        {
                            if (character_boxes[i].Contains(new Point(ms.X, ms.Y)) && login_character == 0)
                            {
                                entered_charactername = characters[i];
                                login_character = 1;
                            }
                        }
                        if (button_1.Contains(new Point(ms.X, ms.Y)))
                        {
                            introState = IntroState.charactercreation;
                        }
                        else if (button_2.Contains(new Point(ms.X, ms.Y)))
                        {
                            introState = IntroState.login;
                            entered_name = "";
                            entered_pass_1 = "";
                            entered_pass_2 = "";
                        }
                    }
                    if (login_character == 3 && login_character_succes == false)
                    {
                        message = "Failed! Couldn't log in!";
                        login_character_succes = false;
                        login_character = 0;
                    }
                    break;
                case IntroState.charactercreation:
                    if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released)
                    {
                        if (name_rec.Contains(new Point(ms.X, ms.Y)))
                        {
                            selected = 1;
                        }
                        else if (gender_rec.Contains(new Point(ms.X, ms.Y)))
                        {
                            if (gender == 0)
                                gender = 1;
                            else
                                gender = 0;
                        }
                        else
                        {
                            selected = 0;
                        }
                        if (ks.IsKeyDown(Keys.Tab) && oks.IsKeyUp(Keys.Tab))
                        {
                            selected = 1;
                        }
                        if (button_1.Contains(new Point(ms.X, ms.Y)))
                        {
                            if (entered_charactername.Length < 5)
                            {
                                message = "Character name is too short!";
                            }
                            else if (characters.Count > 4)
                            {
                                message = "You have to many characters!";
                            }
                            else
                            {
                                create_character = 1;
                            }
                        }
                        else if (button_2.Contains(new Point(ms.X, ms.Y)))
                        {
                            introState = IntroState.charactermenu;
                        }
                    }
                    if (create_character == 3 && character_succes == false)
                    {
                        message = "Failed to create character.";
                        create_character = 0;
                        character_succes = false;
                    }
                    else if (create_character == 3 && character_succes == true)
                    {
                        message = "Succesfully created character " + entered_charactername+"!";
                        characters.Add(entered_charactername);
                        introState = IntroState.charactermenu;
                        create_character = 0;
                        character_succes = false;
                    }

                    if (selected == 1)
                        entered_charactername = NameHandler(ks, oks, entered_charactername);
                    break;
            }
        }

        public void Draw(SpriteBatch sp)
        {
            sp.Draw(intro_image, new Rectangle(0, 0, window_bounds.Width, window_bounds.Height), Color.White);
            sp.DrawString(font, title_text, new Vector2(window_bounds.Width / 2 - (title_text.Length * text_size) / 2, start_y), Color.Black);
            sp.DrawString(font, message, new Vector2(window_bounds.Width / 2 - (message.Length * text_size) / 2, start_y + increment_y * 10), Color.Red);
            switch (introState)
            {
                case IntroState.login:
                    sp.DrawString(font, acc_name, new Vector2(window_bounds.Width / 2 - (acc_name.Length * text_size) / 2, start_y + increment_y * 1), Color.White);
                    sp.Draw(input_texture, name_rec, Color.White);
                    if (selected == 1)
                        sp.DrawString(font, entered_name, new Vector2(window_bounds.Width / 2 - (entered_name.Length * text_size) / 2, start_y + increment_y * 2), Color.White);
                    else
                        sp.DrawString(font, entered_name, new Vector2(window_bounds.Width / 2 - (entered_name.Length * text_size) / 2, start_y + increment_y * 2), Color.Gray);

                    sp.DrawString(font, acc_pass_1, new Vector2(window_bounds.Width / 2 - (acc_pass_1.Length * text_size) / 2, start_y + increment_y * 3), Color.White);
                    sp.Draw(input_texture, pass_rec_1, Color.White);
                    if (selected == 2)
                        sp.DrawString(font, hidden_pass_1, new Vector2(window_bounds.Width / 2 - (hidden_pass_1.Length * text_size) / 2, start_y + increment_y * 4), Color.White);
                    else
                        sp.DrawString(font, hidden_pass_1, new Vector2(window_bounds.Width / 2 - (hidden_pass_1.Length * text_size) / 2, start_y + increment_y * 4), Color.Gray);


                    sp.Draw(input_texture, button_1, Color.White);
                    sp.DrawString(font, login, new Vector2(button_1.X + button_1.Width/2 - (login.Length * text_size) / 2, start_y + increment_y * 8), Color.White);
                    sp.Draw(input_texture, button_2, Color.White);
                    sp.DrawString(font, create_account_string, new Vector2(button_2.X + button_2.Width/2 - (create_account_string.Length * text_size)/ 2, start_y + increment_y * 8), Color.White);
                    break;
                case IntroState.create:
                    sp.DrawString(font, acc_name, new Vector2(window_bounds.Width / 2 - (acc_name.Length * text_size) / 2, start_y + increment_y * 1), Color.White);
                    sp.Draw(input_texture, name_rec, Color.White);
                    if (selected == 1)
                        sp.DrawString(font, entered_name, new Vector2(window_bounds.Width / 2 - (entered_name.Length * text_size) / 2, start_y + increment_y * 2), Color.White);
                    else
                        sp.DrawString(font, entered_name, new Vector2(window_bounds.Width / 2 - (entered_name.Length * text_size) / 2, start_y + increment_y * 2), Color.Gray);


                    sp.DrawString(font, acc_pass_1, new Vector2(window_bounds.Width / 2 - (acc_pass_1.Length * text_size) / 2, start_y + increment_y * 3), Color.White);
                    sp.Draw(input_texture, pass_rec_1, Color.White);
                    if (selected == 2)
                        sp.DrawString(font, hidden_pass_1, new Vector2(window_bounds.Width / 2 - (hidden_pass_1.Length * text_size) / 2, start_y + increment_y * 4), Color.White);
                    else
                        sp.DrawString(font, hidden_pass_1, new Vector2(window_bounds.Width / 2 - (hidden_pass_1.Length * text_size) / 2, start_y + increment_y * 4), Color.Gray);

                    sp.DrawString(font, acc_pass_2, new Vector2(window_bounds.Width / 2 - (acc_pass_2.Length * text_size) / 2, start_y + increment_y * 5), Color.White);
                    sp.Draw(input_texture, pass_rec_2, Color.White);
                    if (selected == 3)
                        sp.DrawString(font, hidden_pass_2, new Vector2(window_bounds.Width / 2 - (hidden_pass_2.Length * text_size) / 2, start_y + increment_y * 6), Color.White);
                    else
                        sp.DrawString(font, hidden_pass_2, new Vector2(window_bounds.Width / 2 - (hidden_pass_2.Length * text_size) / 2, start_y + increment_y * 6), Color.Gray);

                    sp.Draw(input_texture, button_1, Color.White);
                    sp.DrawString(font, create, new Vector2(button_1.X + button_1.Width/2 - (create.Length * text_size) / 2, start_y + increment_y * 8), Color.White);
                    sp.Draw(input_texture, button_2, Color.White);
                    sp.DrawString(font, back_string, new Vector2(button_2.X + button_2.Width/2 - (back_string.Length * text_size)/ 2, start_y + increment_y * 8), Color.White);
                    break;
                case IntroState.charactermenu:
                    for (int i = 0; i < character_boxes.Length; i++)
                    {
                        sp.Draw(input_texture, character_boxes[i], Color.White);
                    }
                    for (int i = 0; i < characters.Count; i++)
			        {
                        sp.DrawString(font, characters[i], new Vector2(window_bounds.Width / 2 - (characters[i].Length * text_size) / 2, start_y + increment_y * (i+1)), Color.Yellow);
			        }
                    sp.Draw(input_texture, button_1, Color.White);
                    sp.DrawString(font, new_character_string, new Vector2(button_1.X + button_1.Width/2 - (new_character_string.Length * text_size) / 2, start_y + increment_y * 8), Color.White);
                    sp.Draw(input_texture, button_2, Color.White);
                    sp.DrawString(font, back_string, new Vector2(button_2.X + button_2.Width/2 - (back_string.Length * text_size)/ 2, start_y + increment_y * 8), Color.White);
                    break;
                case IntroState.charactercreation:
                    sp.DrawString(font, character_name, new Vector2(window_bounds.Width / 2 - (character_name.Length * text_size) / 2, start_y + increment_y * 1), Color.White);
                    sp.Draw(input_texture, name_rec, Color.White);
                    if (selected == 1)
                        sp.DrawString(font, entered_charactername, new Vector2(window_bounds.Width / 2 - (entered_charactername.Length * text_size) / 2, start_y + increment_y * 2), Color.White);
                    else
                        sp.DrawString(font, entered_charactername, new Vector2(window_bounds.Width / 2 - (entered_charactername.Length * text_size) / 2, start_y + increment_y * 2), Color.Gray);

                    sp.Draw(input_texture, gender_rec, Color.White);
                    if (gender == 0)
                        sp.DrawString(font, male_string, new Vector2(gender_rec.X + gender_rec.Width / 2 - (male_string.Length * text_size)/2, start_y + increment_y * 4), Color.White);
                    else
                        sp.DrawString(font, female_string, new Vector2(gender_rec.X + gender_rec.Width / 2 - (female_string.Length * text_size)/2, start_y + increment_y * 4), Color.White);

                    sp.Draw(input_texture, button_1, Color.White);
                    sp.DrawString(font, create, new Vector2(button_1.X + button_1.Width/2 - (create.Length * text_size) / 2, start_y + increment_y * 8), Color.White);
                    sp.Draw(input_texture, button_2, Color.White);
                    sp.DrawString(font, back_string, new Vector2(button_2.X + button_2.Width/2 - (back_string.Length * text_size)/ 2, start_y + increment_y * 8), Color.White);
                    break;
            }
        }

        public string NameHandler(KeyboardState ks, KeyboardState oks, String name)
        {
            foreach (Keys key in ks.GetPressedKeys())
            {
                if (oks.IsKeyUp(key))
                {
                    if (key == Keys.Back && name.Length > 0)
                    {
                        name = name.Remove(name.Length - 1, 1);
                    }
                    else if (key == Keys.Enter)
                    {
                        selected = 0;
                    }
                    else if (name.Length > 11)
                    {
                    }
                    else if (key == Keys.Space)
                    {

                    }
                    else
                    {
                        Char character = (char)key;
                        if (font.Characters.Contains(character))
                        {
                            if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift)) //if shift is held down
                                name += character.ToString().ToUpper(); //convert the Key enum member to uppercase string
                            else
                                name += character.ToString().ToLower(); //convert the Key enum member to lowercase string
                        }
                    }
                }
            }
            return name;

        }
    }
}
