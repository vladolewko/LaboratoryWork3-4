using LaboratoryWork2_Oleshko.Models;
using LaboratoryWork2_Oleshko.Repository.Implementation;
using LaboratoryWork2_Oleshko.Utils;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace LaboratoryWork2_Oleshko.Forms
{
    public partial class RegistrationForm : MaterialForm
    {
        public RegistrationForm()
        {
            InitializeComponent();
            MaterialSkinManager.Instance.AddFormToManage(this);
        }

        private void loginLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }

        private void guideLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var form = new GuidePageForm();
            form.Show();
            Hide();
            form.FormClosed += (c, arg) => Show();
        }

        private void seePasswordCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (seePasswordCheckBox.Checked)
            {
                passwordTextField.PasswordChar = '\0';
                repeatPasswordTextField.PasswordChar = '\0';
            }
            else
            {
                passwordTextField.PasswordChar = '*';
                repeatPasswordTextField.PasswordChar = '*';
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            string login = loginTextField.Text;
            string password = passwordTextField.Text;
            string repeatedPassword = repeatPasswordTextField.Text;

            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(repeatedPassword))
            {
                MessageBox.Show("Fill in all data.");
                return;
            }

            if(password != repeatedPassword)
            {
                MessageBox.Show("Passwords don't match.");
                return;
            }

            if(amITeacherCheckBox.Checked)
            {
                HandleTeacherRegistration(login, password);
            }
            else
            {
                HandleApplicantRegistration(login, password);
            }

            Close();
        }

        private void HandleTeacherRegistration(string login, string password)
        {
            var teacher = new Teacher
            {
                Login = login,
                Password = password,
            };

            if (Repository<Teacher>.GetRepo(Filenames.TeachersFileName)
                .GetByFilter(t => t.Login == teacher.Login) != null)
            {
                MessageBox.Show("Teacher with this login already exists.");
                return;
            }

            Repository<Teacher>.GetRepo(Filenames.TeachersFileName).Create(teacher);
            MessageBox.Show("You successfuly registered.");
        }

        private void HandleApplicantRegistration(string login, string password)
        {
            var applicant = new Applicant
            {
                Login = login,
                Password = password
            };

            if (Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName)
                .GetByFilter(a => a.Login == applicant.Login) != null)
            {
                MessageBox.Show("Applicant with this login already exists.");
                return;
            }

            Repository<Applicant>.GetRepo(Filenames.ApplicantsFileName).Create(applicant);
            MessageBox.Show("You successfuly registered.");
        }
    }
}
