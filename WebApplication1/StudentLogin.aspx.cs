﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Text;
using System.Data;


namespace PeerEvaluation
{
    public partial class StudentLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString);
            conn.Open();
            string checkuser = "select count(*) from [Account] where [UserName]='" + TextBoxUserName.Text + "'";
            SqlCommand com = new SqlCommand(checkuser, conn);
            int temp = Convert.ToInt32(com.ExecuteScalar().ToString());
            conn.Close();
            if (temp == 1)
            {
                conn.Open();
                string checkPasswordQuery = "select [Password] from [Account] where [UserName]='" + TextBoxUserName.Text + "'";
                SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
                string password = passComm.ExecuteScalar().ToString().Replace(" ","");
                if(password == TextBoxPassword.Text)
                {
                    Session["New"] = TextBoxUserName.Text;
                    Response.Write("Password is correct");
                }
                else
                {
                    Response.Write("Password is not correct");
                }
            }
            else
            {
                Response.Write("Username is not correct");
            }
        }

        protected void ButtonRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("Registration.aspx");
        }

        protected void SendEmail_Click(object sender, EventArgs e)
        {
            string username = string.Empty;
            string password = string.Empty;
            Session["email"] = txtEmail.Text;
            string constr = ConfigurationManager.ConnectionStrings["RegistrationConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Username, [Password] FROM [Account] WHERE Email = @Email"))
                {
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
                        {
                            username = sdr["Username"].ToString().Trim();
                            password = sdr["Password"].ToString().Trim();
                        }
                    }
                    con.Close();
                }
            }
            if (!string.IsNullOrEmpty(password))
            {
                /*
                MailMessage mm = new MailMessage("acele245@gmail.com", txtEmail.Text.Trim());
                mm.Subject = "Password Recovery";
                mm.Body = string.Format("Hi, <br/><br />Click on below given link to reset your password. <br/><br /> <a href=http://localhost:5766/ResetPassword.aspx?username=" + username + "&email=" + txtEmail.Text + ">Click here to change your password</a> <br />");
                mm.IsBodyHtml = true; */

                StringBuilder sb = new StringBuilder();
                sb.Append("Hi " + username + ", <br /> Click on below given link to Reset Your Password <br />");
                string encryed = Encryption.encrypt(username,2);
                sb.Append("<a href=http://localhost:5766/ResetPasswordPage.aspx?username=" + encryed);
                sb.Append(">Click here to change your password</a> <br/>");
                sb.Append("<br /> Thanks");
                MailMessage mm = new System.Net.Mail.MailMessage("javaemailsender@gmail.com", txtEmail.Text.Trim(), "Password Recovery", sb.ToString());
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = "javaemailsender@gmail.com";
                NetworkCred.Password = "sendtestemail";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                mm.IsBodyHtml = true;
                smtp.Send(mm);
                msgLabel.ForeColor = Color.Green;
                msgLabel.Text = "Password has been sent to your email address.";
            }
            else
            {
                msgLabel.ForeColor = Color.Red;
                msgLabel.Text = "This email address does not match our records.";
            }
        }
    }
}